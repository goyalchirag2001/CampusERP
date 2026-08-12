import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';

import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatMenuModule } from '@angular/material/menu';

import { TimetableTemplate } from '../models/timetable-template';
import { TimetableTemplateService } from '../services/timetable-template';

import {
  TimetableTemplateDialogData,
  TimetableTemplateFormDialog,
} from '../timetable-template-form-dialog/timetable-template-form-dialog';

import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-timetable-template-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatDialogModule,
    MatCardModule,
    MatChipsModule,
    MatDividerModule,
    MatMenuModule,
  ],
  templateUrl: './timetable-template-details.html',
  styleUrl: './timetable-template-details.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TimetableTemplateDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly timetableService = inject(TimetableTemplateService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialog = inject(MatDialog);

  // =========================================================
  // State
  // =========================================================

  readonly timetable = signal<TimetableTemplate | null>(null);

  readonly loading = signal(true);

  readonly error = signal<string | null>(null);

  readonly actionLoading = signal(false);

  // =========================================================
  // Route
  // =========================================================

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  // =========================================================
  // Lifecycle
  // =========================================================

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error.set('Timetable template ID was not provided.');
      this.loading.set(false);

      return;
    }

    this.load(id);
  }

  // =========================================================
  // Loading
  // =========================================================

  private load(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.timetableService.getById(id).subscribe({
      next: (timetable) => {
        this.timetable.set(timetable);
        this.loading.set(false);
      },

      error: () => {
        this.loading.set(false);
        this.error.set('Unable to load timetable details.');
      },
    });
  }

  refresh(): void {
    const id = this.timetable()?.id ?? this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.load(id);
  }

  // =========================================================
  // Navigation
  // =========================================================

  goBack(): void {
    this.router.navigate([this.baseRoute, 'timetable-templates']);
  }

  // =========================================================
  // Edit
  // =========================================================

  edit(): void {
    const timetable = this.timetable();

    if (!timetable) {
      return;
    }

    this.dialog
      .open(TimetableTemplateFormDialog, {
        width: '1100px',
        maxWidth: '95vw',
        maxHeight: '92vh',
        disableClose: true,
        autoFocus: false,
        restoreFocus: false,

        data: {
          mode: 'edit',
          timetable,
        } satisfies TimetableTemplateDialogData,
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.refresh();
        }
      });
  }

  // =========================================================
  // Duplicate
  // =========================================================

  duplicate(): void {
    const timetable = this.timetable();

    if (!timetable) {
      return;
    }

    this.dialog
      .open(TimetableTemplateFormDialog, {
        width: '1100px',
        maxWidth: '95vw',
        maxHeight: '92vh',
        disableClose: true,
        autoFocus: false,
        restoreFocus: false,

        data: {
          mode: 'duplicate',
          timetable,
        } satisfies TimetableTemplateDialogData,
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.refresh();
        }
      });
  }

  // =========================================================
  // Activate
  // =========================================================

  activate(): void {
    const timetable = this.timetable();

    if (!timetable || timetable.isActive) {
      return;
    }

    this.actionLoading.set(true);

    this.timetableService.activate(timetable.id).subscribe({
      next: () => {
        this.actionLoading.set(false);

        this.timetable.update((current) =>
          current
            ? {
                ...current,
                isActive: true,
              }
            : current,
        );

        this.notificationService.success('Timetable activated successfully.');
      },

      error: (err) => {
        this.actionLoading.set(false);

        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to activate timetable.',
        );
      },
    });
  }

  // =========================================================
  // Deactivate
  // =========================================================

  deactivate(): void {
    const timetable = this.timetable();

    if (!timetable || !timetable.isActive) {
      return;
    }

    this.actionLoading.set(true);

    this.timetableService.deactivate(timetable.id).subscribe({
      next: () => {
        this.actionLoading.set(false);

        this.timetable.update((current) =>
          current
            ? {
                ...current,
                isActive: false,
              }
            : current,
        );

        this.notificationService.success('Timetable deactivated successfully.');
      },

      error: (err) => {
        this.actionLoading.set(false);

        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to deactivate timetable.',
        );
      },
    });
  }

  // =========================================================
  // Delete
  // =========================================================

  delete(): void {
    const timetable = this.timetable();

    if (!timetable) {
      return;
    }

    const confirmed = window.confirm(
      `Are you sure you want to delete the timetable for ${timetable.subjectName} on ${this.getDayName(
        timetable.dayOfWeek,
      )}?`,
    );

    if (!confirmed) {
      return;
    }

    this.actionLoading.set(true);

    this.timetableService.delete(timetable.id).subscribe({
      next: () => {
        this.actionLoading.set(false);

        this.notificationService.success('Timetable deleted successfully.');

        this.goBack();
      },

      error: (err) => {
        this.actionLoading.set(false);

        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to delete timetable.',
        );
      },
    });
  }

  // =========================================================
  // Formatting
  // =========================================================

  getDayName(day: number): string {
    const days = ['', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

    return days[day] ?? 'Unknown';
  }

  getLectureType(type: number): string {
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
        return 'Unknown';
    }
  }

  formatTime(time: string): string {
    if (!time) {
      return '—';
    }

    const parts = time.substring(0, 5).split(':');

    if (parts.length !== 2) {
      return time;
    }

    const hour = Number(parts[0]);

    const minute = parts[1];

    if (Number.isNaN(hour)) {
      return time;
    }

    const period = hour >= 12 ? 'PM' : 'AM';

    const displayHour = hour % 12 || 12;

    return `${displayHour}:${minute} ${period}`;
  }

  formatDate(date: string): string {
    if (!date) {
      return '—';
    }

    const value = date.substring(0, 10);

    const [year, month, day] = value.split('-');

    if (!year || !month || !day) {
      return date;
    }

    return `${day}/${month}/${year}`;
  }

  getDuration(): string {
    const timetable = this.timetable();

    if (!timetable) {
      return '—';
    }

    const start = this.timeToMinutes(timetable.startTime);

    const end = this.timeToMinutes(timetable.endTime);

    if (start === null || end === null || end <= start) {
      return '—';
    }

    const duration = end - start;

    const hours = Math.floor(duration / 60);

    const minutes = duration % 60;

    if (hours && minutes) {
      return `${hours}h ${minutes}m`;
    }

    if (hours) {
      return `${hours}h`;
    }

    return `${minutes}m`;
  }

  private timeToMinutes(value: string): number | null {
    if (!value) {
      return null;
    }

    const parts = value.substring(0, 5).split(':');

    if (parts.length !== 2) {
      return null;
    }

    const hour = Number(parts[0]);

    const minute = Number(parts[1]);

    if (Number.isNaN(hour) || Number.isNaN(minute)) {
      return null;
    }

    return hour * 60 + minute;
  }
}
