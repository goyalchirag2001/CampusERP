import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';

import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

import { CalendarEvent } from '../models/calendar-event';
import { EventType } from '../models/event-type';

import { CalendarEventService } from '../services/calendar-event';

import { CalendarEventFormDialogComponent } from '../calendar-event-form-dialog/calendar-event-form-dialog';
import { ConfirmationDialogComponent } from '../../../shared/dialogs/confirmation-dialog';

@Component({
  selector: 'app-calendar-event-details',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatIconModule, DatePipe],
  templateUrl: './calendar-event-details.html',
  styleUrl: './calendar-event-details.scss',
})
export class CalendarEventDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  private readonly service = inject(CalendarEventService);

  private readonly notification = inject(NotificationService);

  private readonly currentUser = inject(CurrentUserService);

  readonly event = signal<CalendarEvent | null>(null);

  readonly loading = signal(false);

  readonly EventType = EventType;

  readonly refreshing = signal(false);

  private get baseRoute(): string {
    const slug = this.currentUser.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.back();

      return;
    }

    this.refreshing.set(true);

    this.service.getById(id).subscribe({
      next: (event) => {
        this.event.set(event);

        this.refreshing.set(false);
      },
      error: () => {
        this.refreshing.set(false);

        this.notification.error('Unable to load calendar event.');

        this.back();
      },
    });
  }

  edit(): void {
    const event = this.event();

    if (!event) {
      return;
    }

    this.dialog
      .open(CalendarEventFormDialogComponent, {
        width: '950px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        disableClose: true,
        autoFocus: false,
        restoreFocus: false,
        data: {
          mode: 'edit',
          event,
        },
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (!saved) {
          return;
        }

        this.load();
      });
  }

  activate(): void {
    const event = this.event();

    if (!event) {
      return;
    }

    this.dialog
      .open(ConfirmationDialogComponent, {
        width: '430px',
        data: {
          title: 'Activate Calendar Event',
          message: 'Are you sure you want to activate this calendar event?',
          confirmText: 'Activate',
        },
      })
      .afterClosed()
      .subscribe((confirmed: boolean) => {
        if (!confirmed) {
          return;
        }

        this.service.activate(event.id).subscribe(() => {
          this.notification.success('Calendar event activated successfully.');

          this.load();
        });
      });
  }

  deactivate(): void {
    const event = this.event();

    if (!event) {
      return;
    }

    this.dialog
      .open(ConfirmationDialogComponent, {
        maxWidth: '500px',
        width: 'fit-content',
        data: {
          title: 'Deactivate Calendar Event',
          message: 'This event will no longer be considered during timetable processing. Continue?',
          confirmText: 'Deactivate',
          danger: true,
        },
      })
      .afterClosed()
      .subscribe((confirmed: boolean) => {
        if (!confirmed) {
          return;
        }

        this.service.deactivate(event.id).subscribe(() => {
          this.notification.success('Calendar event deactivated successfully.');

          this.load();
        });
      });
  }

  back(): void {
    this.router.navigate([this.baseRoute, 'calendar-events']);
  }

  getEventTypeName(type: EventType): string {
    return EventType[type];
  }

  getEventIcon(type: EventType): string {
    switch (type) {
      case EventType.Holiday:
        return 'beach_access';

      case EventType.Examination:
        return 'assignment';

      case EventType.Workshop:
        return 'build';

      case EventType.Seminar:
        return 'campaign';

      case EventType.SportsDay:
        return 'sports_soccer';

      case EventType.CulturalEvent:
        return 'celebration';

      case EventType.GuestLecture:
        return 'record_voice_over';

      case EventType.FacultyMeeting:
        return 'groups';

      case EventType.ParentTeacherMeeting:
        return 'diversity_3';

      case EventType.ExtraClass:
        return 'school';

      case EventType.Maintenance:
        return 'construction';

      case EventType.PlacementDrive:
        return 'work';

      case EventType.Convocation:
        return 'workspace_premium';

      case EventType.Orientation:
        return 'explore';

      default:
        return 'event';
    }
  }

  getScope(value: string | null | undefined, entity: string): string {
    return value ?? `All ${entity}`;
  }

  formatTime(value: string | null | undefined): string {
    return value || 'N/A';
  }

  formatRecurrence(value: string | null | undefined): string {
    return value || 'Not Applicable';
  }

  getDuration(): string {
    const event = this.event();

    if (!event) {
      return '';
    }

    const start = new Date(event.startDate);

    const end = new Date(event.endDate);

    const milliseconds = end.getTime() - start.getTime();

    const days = Math.floor(milliseconds / (1000 * 60 * 60 * 24)) + 1;

    if (days === 1) {
      return '1 Day';
    }

    return `${days} Days`;
  }
}
