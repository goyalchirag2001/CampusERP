import {
  ChangeDetectionStrategy,
  Component,
  OnDestroy,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AttendanceService } from '../services/attendance';

import { AttendanceSession } from '../models/attendance-session';

import { TimetableCalendarService } from '../../timetable-calendar/services/timetable-calendar';

import { TimetableCalendarEvent } from '../../timetable-calendar/models/timetable-calendar-event';

import { NotificationService } from '../../../core/services/notification';

import { CurrentUserService } from '../../../core/services/current-user';

interface TeacherLectureRow {
  timetableTemplateId: string;

  date: string;

  startTime?: string | null;

  endTime?: string | null;

  title: string;

  subjectCode?: string | null;

  subjectName?: string | null;

  sectionName?: string | null;

  roomName?: string | null;

  isOnline?: boolean;

  isCancelled: boolean;

  isOverride: boolean;

  lectureType?: number | null;

  attendanceSessionId?: string | null;

  attendanceStatus?: number | null;

  isAttendanceCreated: boolean;

  isAttendanceCompleted: boolean;

  isAttendanceLocked: boolean;
}

@Component({
  selector: 'app-teacher-attendance',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './teacher-attendance.html',
  styleUrl: './teacher-attendance.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TeacherAttendance implements OnInit, OnDestroy {
  private readonly router = inject(Router);

  private readonly attendanceService = inject(AttendanceService);

  private readonly calendarService = inject(TimetableCalendarService);

  private readonly notificationService = inject(NotificationService);

  private readonly currentUser = inject(CurrentUserService);

  readonly loading = signal(false);

  readonly refreshing = signal(false);

  readonly selectedDate = signal(new Date());

  readonly lectures = signal<TeacherLectureRow[]>([]);

  private refreshTimer: ReturnType<typeof setInterval> | null = null;

  readonly presentCount = computed(
    () => this.lectures().filter((x) => x.isAttendanceCreated && x.attendanceStatus === 3).length,
  );

  readonly sessionsCreated = computed(
    () => this.lectures().filter((x) => x.isAttendanceCreated).length,
  );

  ngOnInit(): void {
    this.load();

    this.refreshTimer = setInterval(() => {
      this.load(true);
    }, 30000);
  }

  ngOnDestroy(): void {
    if (this.refreshTimer) {
      clearInterval(this.refreshTimer);

      this.refreshTimer = null;
    }
  }

  // =========================================================
  // Load
  // =========================================================

  load(silent = false): void {
    if (silent) {
      this.refreshing.set(true);
    } else {
      this.loading.set(true);
    }

    const date = this.toDateApiValue(this.selectedDate());

    forkJoin({
      calendar: this.calendarService.getTeacherCalendar({
        startDate: date,
        endDate: date,
      }),

      sessions: this.attendanceService.getTeacherSessions(date, date),
    })
      .pipe(
        catchError(() => {
          return of({
            calendar: [],
            sessions: [],
          });
        }),
      )
      .subscribe({
        next: ({ calendar, sessions }) => {
          this.lectures.set(this.buildRows(calendar, sessions));

          this.loading.set(false);

          this.refreshing.set(false);
        },

        error: () => {
          this.loading.set(false);

          this.refreshing.set(false);

          this.notificationService.error('Unable to load attendance.');
        },
      });
  }

  // =========================================================
  // Date
  // =========================================================

  onDateChange(date: Date | null): void {
    if (!date) {
      return;
    }

    this.selectedDate.set(date);

    this.load();
  }

  previousDay(): void {
    const date = new Date(this.selectedDate());

    date.setDate(date.getDate() - 1);

    this.selectedDate.set(date);

    this.load();
  }

  nextDay(): void {
    const date = new Date(this.selectedDate());

    date.setDate(date.getDate() + 1);

    this.selectedDate.set(date);

    this.load();
  }

  today(): void {
    this.selectedDate.set(new Date());

    this.load();
  }

  // =========================================================
  // Session creation
  // =========================================================

  createSession(row: TeacherLectureRow): void {
    if (row.isAttendanceCreated || row.isCancelled) {
      return;
    }

    this.loading.set(true);

    this.attendanceService
      .createSession({
        timetableTemplateId: row.timetableTemplateId,

        attendanceDate: row.date,

        remarks: null,
      })
      .subscribe({
        next: (session) => {
          this.loading.set(false);

          this.notificationService.success('Attendance session created.');

          this.openSession(session.id);
        },

        error: (err) => {
          this.loading.set(false);

          this.notificationService.error(
            err?.error?.message ?? err?.message ?? 'Unable to create attendance session.',
          );
        },
      });
  }

  // =========================================================
  // Navigation
  // =========================================================

  openSession(id: string): void {
    this.router.navigate([this.baseRoute, 'attendance', 'sessions', id]);
  }

  openQr(id: string): void {
    this.router.navigate([this.baseRoute, 'attendance', 'sessions', id, 'qr']);
  }

  // =========================================================
  // Helpers
  // =========================================================

  formatDate(value: Date): string {
    return value.toLocaleDateString([], {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    });
  }

  formatTime(value?: string | null): string {
    if (!value) {
      return '—';
    }

    const parts = value.substring(0, 5).split(':');

    if (parts.length !== 2) {
      return value;
    }

    const hour = Number(parts[0]);

    const minute = parts[1];

    const displayHour = hour % 12 || 12;

    const period = hour >= 12 ? 'PM' : 'AM';

    return `${displayHour}:${minute} ${period}`;
  }

  getStatusLabel(row: TeacherLectureRow): string {
    if (row.isCancelled) {
      return 'Cancelled';
    }

    if (!row.isAttendanceCreated) {
      return 'Not Taken';
    }

    if (row.isAttendanceLocked) {
      return 'Locked';
    }

    if (row.isAttendanceCompleted) {
      return 'Completed';
    }

    return 'Open';
  }

  private buildRows(
    events: TimetableCalendarEvent[],
    sessions: AttendanceSession[],
  ): TeacherLectureRow[] {
    const sessionMap = new Map<string, AttendanceSession>();

    for (const session of sessions) {
      if (!session.timetableTemplateId) {
        continue;
      }

      const key = `${session.timetableTemplateId}|${session.attendanceDate}`;

      sessionMap.set(key, session);
    }

    return events
      .filter((event) => !!event.timetableTemplateId)
      .filter((event) => !event.isFullDay)
      .map((event) => {
        const templateId = event.timetableTemplateId!;

        const key = `${templateId}|${event.date}`;

        const session = sessionMap.get(key);

        return {
          timetableTemplateId: templateId,

          date: event.date,

          startTime: event.startTime,

          endTime: event.endTime,

          title: event.title,

          subjectCode: event.subjectCode,

          subjectName: event.subjectName,

          sectionName: event.sectionName,

          roomName: event.roomName,

          isOnline: event.isOnline,

          isCancelled: event.isCancelled,

          isOverride: event.isOverride,

          lectureType: event.lectureType,

          attendanceSessionId: session?.id ?? null,

          attendanceStatus: session?.status ?? null,

          isAttendanceCreated: !!session,

          isAttendanceCompleted: session?.status === 3 || session?.status === 4,

          isAttendanceLocked: session?.status === 4 || session?.isLocked === true,
        };
      })
      .sort((a, b) => (a.startTime ?? '').localeCompare(b.startTime ?? ''));
  }

  private toDateApiValue(value: Date): string {
    const year = value.getFullYear();

    const month = String(value.getMonth() + 1).padStart(2, '0');

    const day = String(value.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  private get baseRoute(): string {
    const slug = this.currentUser.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }
}
