import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';

import { CommonModule } from '@angular/common';

import { ActivatedRoute, Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { TimetableCalendarEvent } from '../models/timetable-calendar-event';

import { NotificationService } from '../../../core/services/notification';

import { CurrentUserService } from '../../../core/services/current-user';

@Component({
  selector: 'app-timetable-calendar-details',

  standalone: true,

  imports: [
    CommonModule,

    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatCardModule,
    MatChipsModule,
    MatDividerModule,
    MatProgressSpinnerModule,
  ],

  templateUrl: './timetable-calendar-details.html',

  styleUrl: './timetable-calendar-details.scss',

  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TimetableCalendarDetails implements OnInit {
  // =========================================================
  // Dependencies
  // =========================================================

  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  // =========================================================
  // State
  // =========================================================

  readonly event = signal<TimetableCalendarEvent | null>(null);

  readonly loading = signal(true);

  readonly error = signal<string | null>(null);

  // =========================================================
  // Base Route
  // =========================================================

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  // =========================================================
  // Lifecycle
  // =========================================================

  ngOnInit(): void {
    this.loadEvent();
  }

  // =========================================================
  // Load Event
  // =========================================================

  private loadEvent(): void {
    this.loading.set(true);

    this.error.set(null);

    /*
     * The calendar passes the complete event
     * through router navigation state.
     */
    const navigationState = history.state as {
      event?: TimetableCalendarEvent;
    };

    const event = navigationState?.event;

    if (!event) {
      this.error.set(
        'Calendar event details are not available. Please open the event again from the timetable calendar.',
      );

      this.loading.set(false);

      return;
    }

    this.event.set(this.normalizeEvent(event));

    this.loading.set(false);
  }

  // =========================================================
  // Refresh
  // =========================================================

  refresh(): void {
    this.loadEvent();
  }

  // =========================================================
  // Navigation
  // =========================================================

  goBack(): void {
    /*
     * Preserve the exact calendar view/date
     * by going back through browser history.
     */
    if (window.history.length > 1) {
      window.history.back();

      return;
    }

    this.router.navigate([this.baseRoute, 'timetables']);
  }

  // =========================================================
  // Event Type
  // =========================================================

  isTimetableEvent(event: TimetableCalendarEvent | null = this.event()): boolean {
    return !!event?.timetableTemplateId;
  }

  isCalendarEvent(event: TimetableCalendarEvent | null = this.event()): boolean {
    return !!event?.calendarEventId;
  }

  isHoliday(event: TimetableCalendarEvent | null = this.event()): boolean {
    if (!event) {
      return false;
    }

    return !event.timetableTemplateId && !!event.isFullDay;
  }

  isOverride(event: TimetableCalendarEvent | null = this.event()): boolean {
    return !!event?.isOverride;
  }

  isCancelled(event: TimetableCalendarEvent | null = this.event()): boolean {
    return !!event?.isCancelled;
  }

  // =========================================================
  // Event Category
  // =========================================================

  getEventCategory(): string {
    const event = this.event();

    if (!event) {
      return 'Calendar Event';
    }

    if (event.isCancelled) {
      return 'Cancelled Lecture';
    }

    if (event.isOverride) {
      return 'Lecture Override';
    }

    if (this.isHoliday(event)) {
      return 'Holiday';
    }

    if (this.isTimetableEvent(event)) {
      return 'Timetable Lecture';
    }

    if (this.isCalendarEvent(event)) {
      return 'Calendar Event';
    }

    return 'Calendar Event';
  }

  getEventIcon(): string {
    const event = this.event();

    if (!event) {
      return 'event';
    }

    if (event.isCancelled) {
      return 'event_busy';
    }

    if (event.isOverride) {
      return 'edit_calendar';
    }

    if (this.isHoliday(event)) {
      return 'beach_access';
    }

    if (this.isTimetableEvent(event)) {
      return 'school';
    }

    return 'event';
  }

  // =========================================================
  // Title
  // =========================================================

  getEventTitle(): string {
    const event = this.event();

    if (!event) {
      return 'Calendar Event';
    }

    return event.title || event.subjectName || 'Calendar Event';
  }

  // =========================================================
  // Description
  // =========================================================

  getDescription(): string {
    return this.event()?.description?.trim() || '';
  }

  // =========================================================
  // Date
  // =========================================================

  formatDate(date: string | null | undefined): string {
    if (!date) {
      return '—';
    }

    const value = date.substring(0, 10);

    const parts = value.split('-');

    if (parts.length !== 3) {
      return date;
    }

    const [year, month, day] = parts;

    if (!year || !month || !day) {
      return date;
    }

    const parsed = new Date(Number(year), Number(month) - 1, Number(day));

    if (Number.isNaN(parsed.getTime())) {
      return date;
    }

    return new Intl.DateTimeFormat('en-IN', {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    }).format(parsed);
  }

  // =========================================================
  // Time
  // =========================================================

  formatTime(time: string | null | undefined): string {
    if (!time) {
      return '—';
    }

    const value = time.substring(0, 5);

    const parts = value.split(':');

    if (parts.length !== 2) {
      return time;
    }

    const hour = Number(parts[0]);

    const minute = parts[1];

    if (Number.isNaN(hour) || hour < 0 || hour > 23) {
      return time;
    }

    const period = hour >= 12 ? 'PM' : 'AM';

    const displayHour = hour % 12 || 12;

    return `${displayHour}:${minute} ${period}`;
  }

  getTimeRange(): string {
    const event = this.event();

    if (!event) {
      return '—';
    }

    if (event.isFullDay) {
      return 'All day';
    }

    if (!event.startTime) {
      return '—';
    }

    const start = this.formatTime(event.startTime);

    if (!event.endTime) {
      return start;
    }

    return `${start} – ` + `${this.formatTime(event.endTime)}`;
  }

  // =========================================================
  // Duration
  // =========================================================

  getDuration(): string {
    const event = this.event();

    if (!event || event.isFullDay || !event.startTime || !event.endTime) {
      return '—';
    }

    const start = this.timeToMinutes(event.startTime);

    const end = this.timeToMinutes(event.endTime);

    if (start === null || end === null || end <= start) {
      return '—';
    }

    const duration = end - start;

    const hours = Math.floor(duration / 60);

    const minutes = duration % 60;

    if (hours > 0 && minutes > 0) {
      return `${hours}h ${minutes}m`;
    }

    if (hours > 0) {
      return `${hours}h`;
    }

    return `${minutes}m`;
  }

  // =========================================================
  // Academic Information
  // =========================================================

  hasSubject(): boolean {
    const event = this.event();

    return !!(event?.subjectName || event?.subjectCode);
  }

  hasTeacher(): boolean {
    return !!this.event()?.teacherName;
  }

  hasSection(): boolean {
    return !!this.event()?.sectionName;
  }

  // =========================================================
  // Subject
  // =========================================================

  getSubjectDisplay(): string {
    const event = this.event();

    if (!event) {
      return '—';
    }

    if (event.subjectCode && event.subjectName) {
      return `${event.subjectCode} · ` + `${event.subjectName}`;
    }

    return event.subjectName ?? event.subjectCode ?? '—';
  }

  // =========================================================
  // Teacher
  // =========================================================

  getTeacherDisplay(): string {
    return this.event()?.teacherName?.trim() || 'Not assigned';
  }

  // =========================================================
  // Section
  // =========================================================

  getSectionDisplay(): string {
    return this.event()?.sectionName?.trim() || 'Not assigned';
  }

  // =========================================================
  // Room
  // =========================================================

  hasRoom(): boolean {
    const event = this.event();

    return !!(
      event?.isOnline ||
      event?.roomBuilding ||
      event?.roomFloor ||
      event?.roomNumber ||
      event?.roomName
    );
  }

  hasPhysicalRoom(): boolean {
    const event = this.event();

    return !!(
      event &&
      !event.isOnline &&
      (event.roomBuilding || event.roomFloor || event.roomNumber || event.roomName)
    );
  }

  getRoomBuilding(): string {
    return this.event()?.roomBuilding?.trim() || '—';
  }

  getRoomFloor(): string {
    return this.event()?.roomFloor?.trim() || '—';
  }

  getRoomNumber(): string {
    return this.event()?.roomNumber?.trim() || '—';
  }

  getRoomName(): string {
    return this.event()?.roomName?.trim() || '—';
  }

  getRoomDisplay(): string {
    const event = this.event();

    if (!event) {
      return '—';
    }

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

    return parts.length ? parts.join(' · ') : 'No room assigned';
  }

  // =========================================================
  // Lecture Type
  // =========================================================

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
  // Status
  // =========================================================

  getStatus(): string {
    const event = this.event();

    if (!event) {
      return 'Unknown';
    }

    if (event.isCancelled) {
      return 'Cancelled';
    }

    if (event.isOverride) {
      return 'Modified';
    }

    if (event.isFullDay) {
      return 'All Day';
    }

    return 'Scheduled';
  }

  getStatusIcon(): string {
    const event = this.event();

    if (!event) {
      return 'help';
    }

    if (event.isCancelled) {
      return 'cancel';
    }

    if (event.isOverride) {
      return 'edit';
    }

    if (event.isFullDay) {
      return 'event';
    }

    return 'check_circle';
  }

  // =========================================================
  // Attendance
  // =========================================================

  hasAttendance(): boolean {
    return this.isTimetableEvent() && !!this.event()?.generateAttendance;
  }

  getAttendanceLabel(): string {
    const event = this.event();

    if (!event?.timetableTemplateId) {
      return 'Not applicable';
    }

    return event.generateAttendance ? 'Enabled' : 'Disabled';
  }

  // =========================================================
  // Online Meeting
  // =========================================================

  hasMeetingLink(): boolean {
    return !!this.event()?.meetingLink;
  }

  openMeetingLink(): void {
    const link = this.event()?.meetingLink;

    if (!link) {
      return;
    }

    window.open(link, '_blank', 'noopener,noreferrer');
  }

  // =========================================================
  // Override
  // =========================================================

  getOverrideReason(): string {
    return (
      this.event()?.overrideReason?.trim() ||
      'This lecture has been modified from its original schedule.'
    );
  }

  // =========================================================
  // Remarks
  // =========================================================

  getRemarks(): string {
    return this.event()?.description?.trim() || '';
  }

  // =========================================================
  // Error
  // =========================================================

  retry(): void {
    this.loadEvent();
  }

  // =========================================================
  // Utility
  // =========================================================

  private normalizeEvent(event: TimetableCalendarEvent): TimetableCalendarEvent {
    return {
      ...event,

      title: event.title || event.subjectName || 'Calendar Event',

      priority: event.priority ?? 100,

      isFullDay: event.isFullDay ?? false,

      isOverride: event.isOverride ?? false,

      isCancelled: event.isCancelled ?? false,

      isOnline: event.isOnline ?? false,
    };
  }

  private timeToMinutes(value: string | null | undefined): number | null {
    if (!value) {
      return null;
    }

    const parts = value.substring(0, 5).split(':');

    if (parts.length !== 2) {
      return null;
    }

    const hour = Number(parts[0]);

    const minute = Number(parts[1]);

    if (
      Number.isNaN(hour) ||
      Number.isNaN(minute) ||
      hour < 0 ||
      hour > 23 ||
      minute < 0 ||
      minute > 59
    ) {
      return null;
    }

    return hour * 60 + minute;
  }
}
