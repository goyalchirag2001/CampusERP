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
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AttendanceService } from '../services/attendance';

import {
  AttendanceRecord,
  AttendanceSession as AttendanceSessionModel,
} from '../models/attendance-session';

import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-attendance-session',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatSelectModule,
    MatFormFieldModule,
    MatInputModule,
    MatDividerModule,
    MatTooltipModule,
  ],
  templateUrl: './attendance-session.html',
  styleUrl: './attendance-session.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AttendanceSession implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly service = inject(AttendanceService);

  private readonly notification = inject(NotificationService);

  readonly loading = signal(false);

  readonly saving = signal(false);

  readonly completing = signal(false);

  readonly session = signal<AttendanceSessionModel | null>(null);

  readonly search = signal('');

  readonly localRecords = signal<AttendanceRecord[]>([]);

  /**
   * Contains only the status explicitly selected by the teacher
   * or a status already persisted as marked by the backend.
   *
   * Important:
   * A newly generated record normally has:
   *
   *   IsMarked = false
   *   Status   = Absent
   *
   * Therefore an unmarked record must not appear visually selected
   * as Absent.
   */
  readonly selectedStatuses = signal<Map<string, number>>(new Map());

  readonly displayedColumns = ['student', 'status', 'remarks'];

  readonly filteredRecords = computed(() => {
    const keyword = this.search().trim().toLowerCase();

    return this.localRecords().filter((record) => {
      if (!keyword) {
        return true;
      }

      return (
        record.studentName.toLowerCase().includes(keyword) ||
        (record.rollNumber ?? '').toLowerCase().includes(keyword)
      );
    });
  });

  /**
   * Number of students for whom attendance is either:
   *
   * - already persisted as marked, OR
   * - selected locally by the teacher.
   *
   * This makes the counter update immediately when a teacher clicks
   * a status, even before Save Attendance is pressed.
   */
  readonly markedCount = computed(
    () =>
      this.localRecords().filter((record) => {
        return record.isMarked || this.selectedStatuses().has(record.id);
      }).length,
  );

  readonly totalCount = computed(() => this.localRecords().length);

  readonly remainingCount = computed(() => Math.max(0, this.totalCount() - this.markedCount()));

  readonly hasPendingChanges = computed(() =>
    this.localRecords().some((record) => {
      const selected = this.selectedStatuses().get(record.id);

      if (selected === undefined) {
        return false;
      }

      return !record.isMarked || record.status !== selected;
    }),
  );

  readonly isLocked = computed(() => {
    const currentSession = this.session();

    return !!currentSession && (currentSession.isLocked || currentSession.status === 4);
  });

  readonly isCompleted = computed(() => {
    const currentSession = this.session();

    return !!currentSession && currentSession.status === 3;
  });

  ngOnInit(): void {
    this.load();
  }

  // =========================================================
  // Load
  // =========================================================

  load(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.notification.error('Attendance session ID was not provided.');

      return;
    }

    this.loading.set(true);

    this.service.getSession(id).subscribe({
      next: (session) => {
        this.session.set(session);

        this.localRecords.set(
          session.records.map((record) => ({
            ...record,
          })),
        );

        /*
         * Only persisted/actually-marked records should be placed
         * into the selection map.
         */
        this.selectedStatuses.set(
          new Map(
            session.records
              .filter((record) => record.isMarked)
              .map((record) => [record.id, record.status]),
          ),
        );

        this.loading.set(false);
      },

      error: (err) => {
        this.loading.set(false);

        this.notification.error(
          err?.error?.message ?? err?.message ?? 'Unable to load attendance session.',
        );
      },
    });
  }

  // =========================================================
  // Marking
  // =========================================================

  setStatus(recordId: string, status: number): void {
    if (this.isLocked() || this.isCompleted()) {
      return;
    }

    const next = new Map(this.selectedStatuses());

    next.set(recordId, status);

    this.selectedStatuses.set(next);
  }

  setAllStatus(status: number): void {
    if (this.isLocked() || this.isCompleted()) {
      return;
    }

    const next = new Map(this.selectedStatuses());

    for (const record of this.localRecords()) {
      next.set(record.id, status);
    }

    this.selectedStatuses.set(next);
  }

  /**
   * Returns the status that should currently be displayed
   * for a student.
   *
   * Explicit local selection always wins.
   *
   * Otherwise, only an already-marked backend status is shown.
   *
   * An unmarked backend record returns null even if its default
   * Status property happens to be Absent.
   */
  getSelectedStatus(record: AttendanceRecord): number | null {
    if (this.selectedStatuses().has(record.id)) {
      return this.selectedStatuses().get(record.id)!;
    }

    return record.isMarked ? record.status : null;
  }

  isStatusSelected(record: AttendanceRecord, status: number): boolean {
    return this.getSelectedStatus(record) === status;
  }

  /**
   * Gives the whole row a visual state based on the selected
   * attendance status.
   */
  getRowSelectionClass(record: AttendanceRecord): string {
    switch (this.getSelectedStatus(record)) {
      case 1:
        return 'row-present';

      case 2:
        return 'row-absent';

      case 3:
        return 'row-late';

      case 4:
        return 'row-medical';

      case 5:
        return 'row-duty';

      default:
        return '';
    }
  }

  // =========================================================
  // Search
  // =========================================================

  updateSearch(value: string): void {
    this.search.set(value);
  }

  clearSearch(input: HTMLInputElement): void {
    input.value = '';

    this.search.set('');
  }

  // =========================================================
  // Save
  // =========================================================

  save(): void {
    const currentSession = this.session();

    if (!currentSession || this.isLocked() || this.isCompleted()) {
      return;
    }

    const changes = this.localRecords()
      .filter((record) => this.selectedStatuses().has(record.id))
      .map((record) => ({
        attendanceRecordId: record.id,

        status: this.selectedStatuses().get(record.id)!,

        remarks: record.remarks ?? null,
      }));

    if (changes.length === 0) {
      this.notification.info('No attendance changes were made.');

      return;
    }

    this.saving.set(true);

    this.service
      .markAttendanceBulk({
        attendanceSessionId: currentSession.id,

        records: changes,
      })
      .subscribe({
        next: (updated) => {
          this.saving.set(false);

          this.session.set(updated);

          this.localRecords.set(
            updated.records.map((record) => ({
              ...record,
            })),
          );

          /*
           * After successful persistence, rebuild the selection
           * state from the server response.
           */
          this.selectedStatuses.set(
            new Map(
              updated.records
                .filter((record) => record.isMarked)
                .map((record) => [record.id, record.status]),
            ),
          );

          this.notification.success('Attendance saved successfully.');
        },

        error: (err) => {
          this.saving.set(false);

          this.notification.error(
            err?.error?.message ?? err?.message ?? 'Unable to save attendance.',
          );
        },
      });
  }

  // =========================================================
  // Complete
  // =========================================================

  complete(): void {
    const currentSession = this.session();

    if (!currentSession || this.isLocked() || this.isCompleted()) {
      return;
    }

    const unmarked = this.localRecords().filter(
      (record) => !this.selectedStatuses().has(record.id) && !record.isMarked,
    );

    if (unmarked.length > 0) {
      this.notification.error(`${unmarked.length} student(s) are still unmarked.`);

      return;
    }

    const confirmed = window.confirm(
      'Complete attendance? No further manual changes will be allowed after completion.',
    );

    if (!confirmed) {
      return;
    }

    this.completing.set(true);

    this.service
      .completeSession({
        attendanceSessionId: currentSession.id,

        remarks: null,
      })
      .subscribe({
        next: (updated) => {
          this.completing.set(false);

          this.session.set(updated);

          this.localRecords.set(
            updated.records.map((record) => ({
              ...record,
            })),
          );

          this.selectedStatuses.set(
            new Map(
              updated.records
                .filter((record) => record.isMarked)
                .map((record) => [record.id, record.status]),
            ),
          );

          this.notification.success('Attendance completed successfully.');
        },

        error: (err) => {
          this.completing.set(false);

          this.notification.error(
            err?.error?.message ?? err?.message ?? 'Unable to complete attendance.',
          );
        },
      });
  }

  // =========================================================
  // QR
  // =========================================================

  openQr(): void {
    const currentSession = this.session();

    if (!currentSession) {
      return;
    }

    this.router.navigate([this.baseRoute, 'attendance', 'sessions', currentSession.id, 'qr']);
  }

  // =========================================================
  // UI Helpers
  // =========================================================

  getStatusLabel(status: number): string {
    switch (status) {
      case 1:
        return 'Present';

      case 2:
        return 'Absent';

      case 3:
        return 'Late';

      case 4:
        return 'Medical Leave';

      case 5:
        return 'On Duty';

      default:
        return 'Unknown';
    }
  }

  getStatusIcon(status: number): string {
    switch (status) {
      case 1:
        return 'check_circle';

      case 2:
        return 'cancel';

      case 3:
        return 'schedule';

      case 4:
        return 'medical_services';

      case 5:
        return 'badge';

      default:
        return 'help';
    }
  }

  getStudentInitial(name: string): string {
    if (!name) {
      return '?';
    }

    return name.trim().charAt(0).toUpperCase();
  }

  formatTime(value: string): string {
    if (!value) {
      return '—';
    }

    const parts = value.substring(0, 5).split(':');

    if (parts.length !== 2) {
      return value;
    }

    const hour = Number(parts[0]);

    const minute = parts[1];

    if (Number.isNaN(hour)) {
      return value;
    }

    const displayHour = hour % 12 || 12;

    const period = hour >= 12 ? 'PM' : 'AM';

    return `${displayHour}:${minute} ${period}`;
  }

  formatDate(value: string): string {
    if (!value) {
      return '—';
    }

    const date = new Date(`${value}T00:00:00`);

    if (Number.isNaN(date.getTime())) {
      return value;
    }

    return date.toLocaleDateString([], {
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    });
  }

  private get baseRoute(): string {
    const slug = this.route.snapshot.parent?.parent?.paramMap.get('institutionSlug');

    return slug ? `/${slug}` : '/platform';
  }
}
