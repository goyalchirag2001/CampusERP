import { Component, OnInit, computed, inject, signal, effect } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { TimetableTemplate } from '../models/timetable-template';
import { TimetableTemplateService } from '../services/timetable-template';
import { NotificationService } from '../../../core/services/notification';
import { CurrentUserService } from '../../../core/services/current-user';
import {
  TimetableTemplateDialogData,
  TimetableTemplateFormDialog,
} from '../timetable-template-form-dialog/timetable-template-form-dialog';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-timetable-template-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatChipsModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatMenuModule,
    MatDividerModule,
    MatPaginatorModule,
    MatSortModule,
  ],
  templateUrl: './timetable-template-list.html',
  styleUrl: './timetable-template-list.scss',
})
export class TimetableTemplateList implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator)
  private paginator!: MatPaginator;

  @ViewChild(MatSort)
  private sort!: MatSort;

  private readonly service = inject(TimetableTemplateService);

  private readonly notification = inject(NotificationService);

  private readonly dialog = inject(MatDialog);

  private readonly router = inject(Router);

  private readonly currentUser = inject(CurrentUserService);

  readonly loading = signal(false);

  readonly templates = signal<TimetableTemplate[]>([]);

  readonly dataSource = new MatTableDataSource<TimetableTemplate>();

  readonly search = signal('');

  readonly selectedTeacher = signal<string | null>(null);

  readonly selectedSection = signal<string | null>(null);

  readonly selectedDay = signal<number | null>(null);

  readonly showInactive = signal(false);

  readonly selectedAcademicSession = signal<string | null>(null);

  readonly selectedLectureType = signal<number | null>(null);

  constructor() {
    effect(() => {
      this.dataSource.data = this.filteredTemplates();

      this.paginator?.firstPage();
    });
  }

  readonly teachers = computed(() => {
    return [...new Map(this.templates().map((x) => [x.teacherId, x])).values()].sort((a, b) =>
      a.teacherName.localeCompare(b.teacherName),
    );
  });

  readonly sections = computed(() => {
    return [...new Map(this.templates().map((x) => [x.sectionId, x])).values()].sort((a, b) =>
      a.sectionName.localeCompare(b.sectionName),
    );
  });

  readonly academicSessions = computed(() => {
    return [...new Map(this.templates().map((x) => [x.academicSessionId, x])).values()].sort(
      (a, b) => a.academicSessionName.localeCompare(b.academicSessionName),
    );
  });

  readonly filteredTemplates = computed(() => {
    const keyword = this.search().trim().toLowerCase();

    return this.templates().filter((x) => {
      if (
        this.selectedAcademicSession() &&
        x.academicSessionId !== this.selectedAcademicSession()
      ) {
        return false;
      }

      if (this.selectedLectureType() && x.lectureType !== this.selectedLectureType()) {
        return false;
      }

      if (!this.showInactive() && !x.isActive) {
        return false;
      }

      if (this.selectedTeacher() && x.teacherId !== this.selectedTeacher()) {
        return false;
      }

      if (this.selectedSection() && x.sectionId !== this.selectedSection()) {
        return false;
      }

      if (this.selectedDay() && x.dayOfWeek !== this.selectedDay()) {
        return false;
      }

      if (!keyword) {
        return true;
      }

      return (
        x.academicSessionName.toLowerCase().includes(keyword) ||
        x.subjectName.toLowerCase().includes(keyword) ||
        x.subjectCode.toLowerCase().includes(keyword) ||
        x.teacherName.toLowerCase().includes(keyword) ||
        x.sectionName.toLowerCase().includes(keyword) ||
        (x.roomName ?? '').toLowerCase().includes(keyword) ||
        this.getDayName(x.dayOfWeek).toLowerCase().includes(keyword)
      );
    });
  });

  private get baseRoute(): string {
    const slug = this.currentUser.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  ngOnInit(): void {
    this.load();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;

    this.dataSource.sort = this.sort;

    this.dataSource.sortingDataAccessor = (item, property) => {
      switch (property) {
        case 'day':
          return item.dayOfWeek;

        case 'teacher':
          return item.teacherName;

        case 'subject':
          return item.subjectName;

        case 'section':
          return item.sectionName;

        case 'lectureType':
          return item.lectureType;

        default:
          return (item as unknown as Record<string, unknown>)[property] as string | number;
      }
    };
  }

  load(): void {
    this.loading.set(true);

    this.service.getAll().subscribe({
      next: (response) => {
        this.templates.set(response);

        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);

        this.notification.error('Unable to load timetable templates.');
      },
    });
  }

  create(): void {
    this.dialog
      .open(TimetableTemplateFormDialog, {
        width: '1100px',
        maxWidth: '95vw',
        disableClose: true,
        autoFocus: false,
        restoreFocus: false,
        data: {
          mode: 'create',
        } satisfies TimetableTemplateDialogData,
      })
      .afterClosed()
      .subscribe((saved) => {
        if (saved) {
          this.load();
        }
      });
  }

  details(id: string): void {
    this.router.navigate([this.baseRoute, 'timetable-templates', id]);
  }

  refresh(): void {
    this.load();
  }

  updateSearch(value: string): void {
    this.search.set(value);
  }

  formatTime(time: string): string {
    return time.substring(0, 5);
  }

  readonly displayedColumns: string[] = [
    'day',

    'time',

    'subject',

    'teacher',

    'section',

    'room',

    'lectureType',

    'attendance',

    'status',

    'actions',
  ];

  readonly totalTemplates = computed(() => this.templates().length);

  readonly activeTemplates = computed(() => this.templates().filter((x) => x.isActive).length);

  readonly onlineLectures = computed(() => this.templates().filter((x) => x.isOnline).length);

  readonly attendanceEnabled = computed(
    () => this.templates().filter((x) => x.generateAttendance).length,
  );

  private readonly dayNames = [
    '',
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday',
    'Sunday',
  ];

  getDayName(day: number): string {
    return this.dayNames[day] ?? '';
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

  readonly lectureTypes = [
    { value: 1, name: 'Lecture' },
    { value: 2, name: 'Practical' },
    { value: 3, name: 'Laboratory' },
    { value: 4, name: 'Tutorial' },
    { value: 5, name: 'Seminar' },
    { value: 6, name: 'Workshop' },
    { value: 7, name: 'Project' },
    { value: 8, name: 'Viva' },
    { value: 9, name: 'Extra Class' },
    { value: 10, name: 'Revision' },
    { value: 11, name: 'Guest Lecture' },
  ];

  readonly days = [
    { value: 1, name: 'Monday' },
    { value: 2, name: 'Tuesday' },
    { value: 3, name: 'Wednesday' },
    { value: 4, name: 'Thursday' },
    { value: 5, name: 'Friday' },
    { value: 6, name: 'Saturday' },
    { value: 7, name: 'Sunday' },
  ];

  edit(template: TimetableTemplate): void {
    this.dialog
      .open(TimetableTemplateFormDialog, {
        width: '1100px',
        maxWidth: '95vw',
        disableClose: true,
        autoFocus: false,
        restoreFocus: false,
        data: {
          mode: 'edit',
          timetable: template,
        } satisfies TimetableTemplateDialogData,
      })
      .afterClosed()
      .subscribe((saved: boolean) => {
        if (saved) {
          this.load();
        }
      });
  }

  activate(template: TimetableTemplate): void {
    this.service.activate(template.id).subscribe(() => {
      this.notification.success('Timetable activated successfully.');

      this.load();
    });
  }

  deactivate(template: TimetableTemplate): void {
    this.service.deactivate(template.id).subscribe(() => {
      this.notification.success('Timetable deactivated successfully.');

      this.load();
    });
  }

  duplicate(template: TimetableTemplate): void {
    this.notification.info(
      `Duplicate "${template.subjectCode}" will be available in a future update.`,
    );
  }

  delete(template: TimetableTemplate): void {
    this.notification.info(
      'Delete functionality will be enabled once the backend endpoint is available.',
    );
  }
}
