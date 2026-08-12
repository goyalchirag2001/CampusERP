import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { ActivatedRoute, Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';

import { TimetableCalendarService } from '../services/timetable-calendar';

import { TimetableCalendarEvent } from '../models/timetable-calendar-event';

import { NotificationService } from '../../../core/services/notification';

import { CurrentUserService } from '../../../core/services/current-user';

type CalendarViewMode = 'day' | 'week' | 'month';

type CalendarMode = 'teacher' | 'student';

interface CalendarDay {
  date: Date;

  dateKey: string;

  dayNumber: number;

  dayName: string;

  isToday: boolean;

  isCurrentMonth: boolean;
}

interface PositionedEvent {
  event: TimetableCalendarEvent;

  top: number;

  height: number;

  left: number;

  width: number;
}

@Component({
  selector: 'app-timetable-calendar',

  standalone: true,

  imports: [
    CommonModule,

    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatMenuModule,
  ],

  templateUrl: './timetable-calendar.html',

  styleUrl: './timetable-calendar.scss',

  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TimetableCalendar implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly calendarService = inject(TimetableCalendarService);

  private readonly notificationService = inject(NotificationService);

  private readonly currentUserService = inject(CurrentUserService);

  // =========================================================
  // State
  // =========================================================

  readonly loading = signal(false);

  readonly error = signal<string | null>(null);

  readonly events = signal<TimetableCalendarEvent[]>([]);

  readonly currentDate = signal<Date>(this.startOfDay(new Date()));

  readonly viewMode = signal<CalendarViewMode>('week');

  readonly calendarMode = signal<CalendarMode>('teacher');

  // =========================================================
  // Constants
  // =========================================================

  readonly weekDays = [
    {
      value: 1,
      short: 'Mon',
      full: 'Monday',
    },

    {
      value: 2,
      short: 'Tue',
      full: 'Tuesday',
    },

    {
      value: 3,
      short: 'Wed',
      full: 'Wednesday',
    },

    {
      value: 4,
      short: 'Thu',
      full: 'Thursday',
    },

    {
      value: 5,
      short: 'Fri',
      full: 'Friday',
    },

    {
      value: 6,
      short: 'Sat',
      full: 'Saturday',
    },

    {
      value: 7,
      short: 'Sun',
      full: 'Sunday',
    },
  ];

  readonly calendarStartHour = 6;

  readonly calendarEndHour = 22;

  readonly hourHeight = 80;

  readonly totalCalendarHeight = (this.calendarEndHour - this.calendarStartHour) * this.hourHeight;

  // =========================================================
  // Computed
  // =========================================================

  readonly isToday = computed(() => {
    return this.isSameDate(this.currentDate(), new Date());
  });

  readonly weekDaysForView = computed(() => {
    return this.getWeekDays(this.currentDate());
  });

  readonly monthDays = computed(() => {
    return this.getMonthCalendarDays(this.currentDate());
  });

  readonly dayEvents = computed(() => {
    const dateKey = this.toDateKey(this.currentDate());

    return this.sortEvents(
      this.events().filter((event) => event.date.substring(0, 10) === dateKey),
    );
  });

  readonly monthEvents = computed(() => {
    const map = new Map<string, TimetableCalendarEvent[]>();

    for (const event of this.events()) {
      const dateKey = event.date.substring(0, 10);

      const existing = map.get(dateKey) ?? [];

      existing.push(event);

      map.set(dateKey, this.sortEvents(existing));
    }

    return map;
  });

  readonly title = computed(() => {
    const date = this.currentDate();

    if (this.viewMode() === 'day') {
      return new Intl.DateTimeFormat('en-IN', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
        year: 'numeric',
      }).format(date);
    }

    if (this.viewMode() === 'month') {
      return new Intl.DateTimeFormat('en-IN', {
        month: 'long',
        year: 'numeric',
      }).format(date);
    }

    const week = this.getWeekDays(date);

    const first = week[0].date;

    const last = week[6].date;

    const firstMonth = first.toLocaleDateString('en-IN', {
      month: 'short',
    });

    const lastMonth = last.toLocaleDateString('en-IN', {
      month: 'short',
    });

    if (first.getFullYear() !== last.getFullYear()) {
      return `${firstMonth} ${first.getFullYear()} ` + `– ${lastMonth} ${last.getFullYear()}`;
    }

    if (first.getMonth() !== last.getMonth()) {
      return (
        `${first.getDate()} ${firstMonth} ` +
        `– ${last.getDate()} ${lastMonth} ` +
        `${last.getFullYear()}`
      );
    }

    return (
      `${first.getDate()} – ` + `${last.getDate()} ` + `${lastMonth} ` + `${last.getFullYear()}`
    );
  });

  readonly todayLabel = computed(() => {
    return new Intl.DateTimeFormat('en-IN', {
      weekday: 'short',
      day: 'numeric',
      month: 'short',
    }).format(new Date());
  });

  // =========================================================
  // Lifecycle
  // =========================================================

  ngOnInit(): void {
    const routeMode = this.route.snapshot.data['calendarMode'];

    if (routeMode === 'teacher' || routeMode === 'student') {
      this.calendarMode.set(routeMode);
    }

    this.loadCalendar();
  }

  // =========================================================
  // API
  // =========================================================

  loadCalendar(): void {
    const range = this.getApiRange();

    this.loading.set(true);

    this.error.set(null);

    const request = {
      startDate: this.toDateKey(range.start),

      endDate: this.toDateKey(range.end),
    };

    const observable =
      this.calendarMode() === 'teacher'
        ? this.calendarService.getTeacherCalendar(request)
        : this.calendarService.getStudentCalendar(request);

    observable.subscribe({
      next: (events) => {
        const normalizedEvents = events
          .map((event) => this.normalizeEvent(event))
          .filter((event) => !(event.timetableTemplateId && event.isCancelled));

        this.events.set(normalizedEvents);

        this.loading.set(false);
      },

      error: (err) => {
        this.loading.set(false);

        const message = err?.error?.message ?? err?.message ?? 'Unable to load calendar.';

        this.error.set(message);

        this.notificationService.error(message);
      },
    });
  }

  // =========================================================
  // Navigation
  // =========================================================

  previous(): void {
    const current = this.currentDate();

    if (this.viewMode() === 'day') {
      this.currentDate.set(this.addDays(current, -1));
    } else if (this.viewMode() === 'week') {
      this.currentDate.set(this.addDays(current, -7));
    } else {
      this.currentDate.set(new Date(current.getFullYear(), current.getMonth() - 1, 1));
    }

    this.loadCalendar();
  }

  next(): void {
    const current = this.currentDate();

    if (this.viewMode() === 'day') {
      this.currentDate.set(this.addDays(current, 1));
    } else if (this.viewMode() === 'week') {
      this.currentDate.set(this.addDays(current, 7));
    } else {
      this.currentDate.set(new Date(current.getFullYear(), current.getMonth() + 1, 1));
    }

    this.loadCalendar();
  }

  today(): void {
    this.currentDate.set(this.startOfDay(new Date()));

    this.loadCalendar();
  }

  setView(view: CalendarViewMode): void {
    if (this.viewMode() === view) {
      return;
    }

    this.viewMode.set(view);

    this.loadCalendar();
  }

  // =========================================================
  // API Range
  // =========================================================

  private getApiRange(): {
    start: Date;
    end: Date;
  } {
    const date = this.currentDate();

    if (this.viewMode() === 'day') {
      return {
        start: this.startOfDay(date),

        end: this.startOfDay(date),
      };
    }

    if (this.viewMode() === 'week') {
      const start = this.startOfWeek(date);

      return {
        start,

        end: this.addDays(start, 6),
      };
    }

    const monthStart = new Date(date.getFullYear(), date.getMonth(), 1);

    const monthEnd = new Date(date.getFullYear(), date.getMonth() + 1, 0);

    return {
      start: this.startOfWeek(monthStart),

      end: this.endOfWeek(monthEnd),
    };
  }

  // =========================================================
  // Day View
  // =========================================================

  getDayPosition(event: TimetableCalendarEvent): PositionedEvent {
    return this.positionEvent(event, 0, 1);
  }

  // =========================================================
  // Week View
  // =========================================================

  getEventsForDate(date: Date): TimetableCalendarEvent[] {
    const key = this.toDateKey(date);

    return this.sortEvents(
      this.events().filter((event) => event.date.substring(0, 10) === key),
    );
  }

  getPositionedWeekEvents(date: Date): PositionedEvent[] {
    const events = this.getEventsForDate(date).filter((event) => !event.isFullDay);

    return this.positionOverlappingEvents(events);
  }

  getFullDayEvents(date: Date): TimetableCalendarEvent[] {
    return this.getEventsForDate(date).filter((event) => event.isFullDay);
  }

  // =========================================================
  // Month View
  // =========================================================

  getMonthEvents(date: Date): TimetableCalendarEvent[] {
    return this.monthEvents().get(this.toDateKey(date)) ?? [];
  }

  // =========================================================
  // Event Helpers
  // =========================================================

  isTimetableEvent(event: TimetableCalendarEvent): boolean {
    return !!event.timetableTemplateId;
  }

  isCalendarEvent(event: TimetableCalendarEvent): boolean {
    return !!event.calendarEventId;
  }

  isHoliday(event: TimetableCalendarEvent): boolean {
    return !event.timetableTemplateId && event.isFullDay;
  }

  getEventTitle(event: TimetableCalendarEvent): string {
    if (event.title) {
      return event.title;
    }

    if (event.subjectName) {
      return event.subjectName;
    }

    return 'Calendar Event';
  }

  getEventSubtitle(event: TimetableCalendarEvent): string {
    if (this.isTimetableEvent(event)) {
      if (event.subjectCode) {
        return event.subjectCode;
      }
    }

    return event.description ?? '';
  }

  getEventTime(event: TimetableCalendarEvent): string {
    if (event.isFullDay || !event.startTime) {
      return 'All day';
    }

    const start = this.formatTime(event.startTime);

    if (!event.endTime) {
      return start;
    }

    return `${start} – ` + `${this.formatTime(event.endTime)}`;
  }

  getEventClass(event: TimetableCalendarEvent): string {
    if (event.isCancelled) {
      return 'event-cancelled';
    }

    if (this.isHoliday(event)) {
      return 'event-holiday';
    }

    if (event.calendarEventId) {
      return 'event-calendar';
    }

    if (event.isOnline) {
      return 'event-online';
    }

    return 'event-lecture';
  }

  getLectureType(type: number | null | undefined): string {
    switch (type) {
      case 1:
        return 'Lecture';

      case 2:
        return 'Practical';

      case 3:
        return 'Laboratory';

      case 4:
        return 'Tutorial';

      case 5:
        return 'Seminar';

      case 6:
        return 'Workshop';

      case 7:
        return 'Project';

      case 8:
        return 'Viva';

      case 9:
        return 'Extra Class';

      case 10:
        return 'Revision';

      case 11:
        return 'Guest Lecture';

      default:
        return 'Event';
    }
  }

  // =========================================================
  // Room
  // =========================================================

  hasPhysicalRoom(event: TimetableCalendarEvent): boolean {
    return (
      !event.isOnline &&
      !!(event.roomBuilding || event.roomFloor || event.roomNumber || event.roomName)
    );
  }

  getRoomShortDisplay(event: TimetableCalendarEvent): string {
    if (event.isOnline) {
      return 'Online';
    }

    const parts: string[] = [];

    if (event.roomBuilding) {
      parts.push(event.roomBuilding);
    }

    if (event.roomFloor) {
      parts.push(`Floor ${event.roomFloor}`);
    }

    if (event.roomNumber) {
      parts.push(event.roomNumber);
    }

    if (event.roomName) {
      parts.push(event.roomName);
    }

    return parts.length ? parts.join(' · ') : 'Room not assigned';
  }

  // =========================================================
  // Date Grid
  // =========================================================

  private getWeekDays(date: Date): CalendarDay[] {
    const start = this.startOfWeek(date);

    return Array.from(
      {
        length: 7,
      },
      (_, index) => {
        const current = this.addDays(start, index);

        return this.createCalendarDay(current, true);
      },
    );
  }

  private getMonthCalendarDays(date: Date): CalendarDay[] {
    const monthStart = new Date(date.getFullYear(), date.getMonth(), 1);

    const monthEnd = new Date(date.getFullYear(), date.getMonth() + 1, 0);

    const gridStart = this.startOfWeek(monthStart);

    const gridEnd = this.endOfWeek(monthEnd);

    const result: CalendarDay[] = [];

    let cursor = gridStart;

    while (cursor <= gridEnd) {
      result.push(this.createCalendarDay(cursor, cursor.getMonth() === date.getMonth()));

      cursor = this.addDays(cursor, 1);
    }

    return result;
  }

  private createCalendarDay(date: Date, isCurrentMonth: boolean): CalendarDay {
    return {
      date,

      dateKey: this.toDateKey(date),

      dayNumber: date.getDate(),

      dayName: date.toLocaleDateString('en-IN', {
        weekday: 'short',
      }),

      isToday: this.isSameDate(date, new Date()),

      isCurrentMonth,
    };
  }

  // =========================================================
  // Event Positioning
  // =========================================================

  private positionEvent(
    event: TimetableCalendarEvent,
    column: number,
    columnCount: number,
  ): PositionedEvent {
    const start = this.timeToMinutes(event.startTime);

    const end = this.timeToMinutes(event.endTime);

    const effectiveStart = start ?? this.calendarStartHour * 60;

    const effectiveEnd = end ?? effectiveStart + 60;

    const calendarStart = this.calendarStartHour * 60;

    const top = ((effectiveStart - calendarStart) / 60) * this.hourHeight;

    const duration = Math.max(effectiveEnd - effectiveStart, 30);

    const height = (duration / 60) * this.hourHeight;

    return {
      event,

      top: Math.max(top, 0),

      height: Math.max(height, 40),

      left: (column / columnCount) * 100,

      width: 100 / columnCount,
    };
  }

  private positionOverlappingEvents(events: TimetableCalendarEvent[]): PositionedEvent[] {
    if (!events.length) {
      return [];
    }

    const sorted = [...events].sort(
      (a, b) => this.timeToMinutes(a.startTime) - this.timeToMinutes(b.startTime),
    );

    const columns: TimetableCalendarEvent[][] = [];

    const assignments = new Map<TimetableCalendarEvent, number>();

    for (const event of sorted) {
      const eventStart = this.timeToMinutes(event.startTime);

      let placed = false;

      for (let column = 0; column < columns.length; column++) {
        const last = columns[column][columns[column].length - 1];

        const lastEnd = this.timeToMinutes(last.endTime);

        if (eventStart >= lastEnd) {
          columns[column].push(event);

          assignments.set(event, column);

          placed = true;

          break;
        }
      }

      if (!placed) {
        columns.push([event]);

        assignments.set(event, columns.length - 1);
      }
    }

    const columnCount = Math.max(columns.length, 1);

    return sorted.map((event) =>
      this.positionEvent(event, assignments.get(event) ?? 0, columnCount),
    );
  }

  // =========================================================
  // Formatting
  // =========================================================

  formatTime(time: string | null | undefined): string {
    if (!time) {
      return '';
    }

    const value = time.substring(0, 5);

    const [hourString, minute] = value.split(':');

    const hour = Number(hourString);

    if (Number.isNaN(hour) || !minute) {
      return value;
    }

    const period = hour >= 12 ? 'PM' : 'AM';

    const displayHour = hour % 12 || 12;

    return `${displayHour}:${minute} ${period}`;
  }

  // =========================================================
  // Utility
  // =========================================================

  private normalizeEvent(event: TimetableCalendarEvent): TimetableCalendarEvent {
    return {
      ...event,

      title: event.title ?? event.subjectName ?? 'Calendar Event',

      priority: event.priority ?? 100,

      isFullDay: event.isFullDay ?? false,

      isOverride: event.isOverride ?? false,

      isCancelled: event.isCancelled ?? false,
    };
  }

  private sortEvents(events: TimetableCalendarEvent[]): TimetableCalendarEvent[] {
    return [...events].sort((a, b) => {
      if (a.isFullDay !== b.isFullDay) {
        return a.isFullDay ? -1 : 1;
      }

      const startA = this.timeToMinutes(a.startTime);

      const startB = this.timeToMinutes(b.startTime);

      if (startA !== startB) {
        return startA - startB;
      }

      return (b.priority ?? 0) - (a.priority ?? 0);
    });
  }

  private timeToMinutes(value: string | null | undefined): number {
    if (!value) {
      return 0;
    }

    const parts = value.substring(0, 5).split(':');

    if (parts.length !== 2) {
      return 0;
    }

    const hour = Number(parts[0]);

    const minute = Number(parts[1]);

    if (Number.isNaN(hour) || Number.isNaN(minute)) {
      return 0;
    }

    return hour * 60 + minute;
  }

  private toDateKey(date: Date): string {
    const year = date.getFullYear();

    const month = String(date.getMonth() + 1).padStart(2, '0');

    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  private startOfDay(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  }

  private startOfWeek(date: Date): Date {
    const result = this.startOfDay(date);

    const day = result.getDay();

    const mondayOffset = day === 0 ? -6 : 1 - day;

    result.setDate(result.getDate() + mondayOffset);

    return result;
  }

  private endOfWeek(date: Date): Date {
    return this.addDays(this.startOfWeek(date), 6);
  }

  private addDays(date: Date, days: number): Date {
    const result = new Date(date);

    result.setDate(result.getDate() + days);

    return result;
  }

  private isSameDate(first: Date, second: Date): boolean {
    return (
      first.getFullYear() === second.getFullYear() &&
      first.getMonth() === second.getMonth() &&
      first.getDate() === second.getDate()
    );
  }

  // =========================================================
  // Event Navigation
  // =========================================================

  openEvent(event: TimetableCalendarEvent): void {
    const institutionSlug = this.currentUserService.user()?.institutionSlug;

    const baseRoute = institutionSlug ? `/${institutionSlug}` : '/platform';

    this.router.navigate([baseRoute, 'calendar-event-details'], {
      state: {
        event,
      },
    });
  }
}
