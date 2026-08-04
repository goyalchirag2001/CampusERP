import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';

import { CurrentUserService } from '../../../core/services/current-user';

import { CalendarEventService } from '../services/calendar-event';
import { CalendarEvent } from '../models/calendar-event';
import { MatDialog } from '@angular/material/dialog';
import { CalendarEventFormDialogComponent } from '../calendar-event-form-dialog/calendar-event-form-dialog';

import { EventType } from '../models/event-type';
import { DatePipe, NgClass } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-calendar-event-list',
  standalone: true,
  imports: [
    FormsModule,
    NgClass,
    DatePipe,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatPaginatorModule,
    MatTableModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './calendar-event-list.html',
  styleUrl: './calendar-event-list.scss',
})
export class CalendarEventListComponent implements OnInit {
  private readonly service = inject(CalendarEventService);

  private readonly router = inject(Router);

  private readonly currentUser = inject(CurrentUserService);

  readonly events = signal<CalendarEvent[]>([]);

  private readonly dialog = inject(MatDialog);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  readonly sortColumn = signal('startDate');

  readonly sortDirection = signal<'asc' | 'desc'>('desc');

  readonly EventType = EventType;

  readonly loading = signal(false);

  displayedColumns = [
    'title',
    'eventType',
    'startDate',
    'endDate',
    'academicSession',
    'status',
    'actions',
  ];

  private get baseRoute(): string {
    const slug = this.currentUser.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredEvents = computed(() => {
    const keyword = this.search().trim().toLowerCase();

    if (!keyword) {
      return this.events();
    }

    return this.events().filter((event) =>
      [
        event.title,
        event.description,
        event.campusName,
        event.departmentName,
        event.courseName,
        event.sectionName,
        event.teacherName,
        event.roomName,
        event.academicSessionName,
      ]
        .filter(Boolean)
        .some((value) => value!.toLowerCase().includes(keyword)),
    );
  });

  readonly pagedEvents = computed(() => {
    const events = [...this.filteredEvents()];

    const direction = this.sortDirection() === 'asc' ? 1 : -1;

    events.sort((a, b) => {
      switch (this.sortColumn()) {
        case 'title':
          return a.title.localeCompare(b.title) * direction;

        case 'eventType':
          return (a.eventType - b.eventType) * direction;

        case 'startDate':
          return (this.toDate(a.startDate) - this.toDate(b.startDate)) * direction;

        case 'endDate':
          return (this.toDate(a.endDate) - this.toDate(b.endDate)) * direction;

        case 'academicSession':
          return a.academicSessionName.localeCompare(b.academicSessionName) * direction;

        case 'status':
          return (Number(a.isActive) - Number(b.isActive)) * direction;

        default:
          return 0;
      }
    });

    const start = this.pageIndex() * this.pageSize();

    return events.slice(start, start + this.pageSize());
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);

    this.service.getAll().subscribe({
      next: (events) => {
        this.events.set(events);
      },
      complete: () => {
        this.loading.set(false);
      },
    });
  }

  create(): void {
    this.dialog
      .open(CalendarEventFormDialogComponent, {
        width: '950px',
        maxHeight: '90vh',
        maxWidth: '95vw',

        data: {
          mode: 'create',
        },
      })
      .afterClosed()
      .subscribe((saved) => {
        if (saved) {
          this.load();
        }
      });
  }

  open(id: string): void {
    this.router.navigate([this.baseRoute, 'calendar-events', id]);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  sort(column: string): void {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);

      this.sortDirection.set('asc');
    }
  }

  getEventTypeName(eventType: EventType): string {
    return EventType[eventType];
  }

  private toDate(value: string): number {
    return new Date(`${value}T00:00:00`).getTime();
  }

  private readonly eventTypeClasses: Record<EventType, string> = {
    [EventType.Holiday]: 'holiday',
    [EventType.Examination]: 'exam',
    [EventType.Workshop]: 'workshop',
    [EventType.Seminar]: 'seminar',
    [EventType.SportsDay]: 'sports',
    [EventType.CulturalEvent]: 'cultural',
    [EventType.GuestLecture]: 'guest',
    [EventType.FacultyMeeting]: 'faculty',
    [EventType.ParentTeacherMeeting]: 'ptm',
    [EventType.ExtraClass]: 'extra',
    [EventType.Maintenance]: 'maintenance',
    [EventType.PlacementDrive]: 'placement',
    [EventType.Convocation]: 'convocation',
    [EventType.Orientation]: 'orientation',
    [EventType.Custom]: 'custom',
  };

  getEventTypeClass(type: EventType): string {
    return this.eventTypeClasses[type];
  }
}
