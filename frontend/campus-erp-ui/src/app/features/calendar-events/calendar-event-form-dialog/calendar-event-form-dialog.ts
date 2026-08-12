import {
  Component,
  Inject,
  OnInit,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { CampusService } from '../../campuses/services/campus';
import { DepartmentService } from '../../departments/services/department';
import { CourseService } from '../../courses/services/course';
import { SemesterService } from '../../semesters/services/semester';
import { SectionService } from '../../sections/services/section';
import { TeacherService } from '../../teachers/services/teacher';
import { AcademicSessionService } from '../../academic-sessions/services/academic-session';
import { CalendarEventService } from '../services/calendar-event';

import { NotificationService } from '../../../core/services/notification';
import { CurrentUserService } from '../../../core/services/current-user';

import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';
import { TeacherLookup } from '../../../core/models/teacher-lookup';

import { Course } from '../../courses/models/course';
import { AcademicSessionLookup } from '../../academic-sessions/models/academic-session-lookup';

import { CalendarEventFormDialogData } from '../models/calendar-event-form-dialog-data';
import { EventType } from '../models/event-type';
import { CreateCalendarEventRequest } from '../models/create-calendar-event-request';
import { UpdateCalendarEventRequest } from '../models/update-calendar-event-request';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-calendar-event-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatExpansionModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSlideToggleModule,
    MatIconModule,
  ],
  templateUrl: './calendar-event-form-dialog.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './calendar-event-form-dialog.scss',
})
export class CalendarEventFormDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly campusService = inject(CampusService);

  private readonly departmentService = inject(DepartmentService);

  private readonly courseService = inject(CourseService);

  private readonly semesterService = inject(SemesterService);

  private readonly sectionService = inject(SectionService);

  private readonly teacherService = inject(TeacherService);

  private readonly academicSessionService = inject(AcademicSessionService);

  private readonly calendarEventService = inject(CalendarEventService);

  private readonly notificationService = inject(NotificationService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly dialogRef = inject(MatDialogRef<CalendarEventFormDialogComponent>);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: CalendarEventFormDialogData,
  ) {}

  readonly EventType = EventType;

  readonly isEdit = computed(() => this.data.mode === 'edit');

  readonly saving = signal(false);

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly courses = signal<Course[]>([]);

  readonly semesters = signal<Lookup[]>([]);

  readonly sections = signal<Lookup[]>([]);

  readonly teachers = signal<TeacherLookup[]>([]);

  readonly academicSessions = signal<AcademicSessionLookup[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly selectedCampusId = signal('');

  readonly selectedDepartmentId = signal('');

  readonly selectedCourseId = signal('');

  readonly selectedSemesterId = signal('');

  readonly filteredDepartments = computed(() =>
    this.departments().filter((x) => x.campusId === this.selectedCampusId()),
  );

  readonly filteredCourses = computed(() =>
    this.courses().filter((x) => x.departmentId === this.selectedDepartmentId()),
  );

  readonly filteredTeachers = computed(() =>
    this.teachers().filter(
      (x) => !this.selectedDepartmentId() || x.departmentId === this.selectedDepartmentId(),
    ),
  );

  form = this.fb.group({
    campusId: ['', Validators.required],

    departmentId: [''],

    courseId: [''],

    semesterId: [''],

    sectionId: [''],

    teacherId: [''],

    academicSessionId: ['', Validators.required],

    title: ['', [Validators.required, Validators.maxLength(200)]],

    description: ['', Validators.maxLength(2000)],

    eventType: [EventType.Holiday, Validators.required],

    startDate: [null as Date | null, Validators.required],

    endDate: [null as Date | null, Validators.required],

    startTime: [''],

    endTime: [''],

    isFullDay: [true],

    isRecurring: [false],

    recurrenceRule: [''],

    priority: [300],

    affectsTimetable: [true],
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    this.departmentService.getLookupWithCampus().subscribe((x) => {
      this.departments.set(x);
    });

    this.courseService.getAll().subscribe((x) => {
      this.courses.set(x);
    });

    this.teacherService.getLookupWithDepartment().subscribe((x) => {
      this.teachers.set(x);
    });

    if (this.isCampusAdmin()) {
      const campusId = user?.campusId ?? '';

      this.form.patchValue({
        campusId,
      });

      this.selectedCampusId.set(campusId);

      this.form.controls.campusId.disable();

      this.academicSessionService.getLookupByCampus(campusId).subscribe((sessions) => {
        this.academicSessions.set(sessions);
      });
    } else {
      this.campusService.getLookup().subscribe((x) => {
        this.campuses.set(x);
      });
    }

    this.form.controls.campusId.valueChanges.subscribe((value) => {
      this.selectedCampusId.set(value ?? '');

      this.form.patchValue(
        {
          departmentId: '',
          courseId: '',
          semesterId: '',
          sectionId: '',
          academicSessionId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.selectedDepartmentId.set('');

      this.selectedCourseId.set('');

      this.selectedSemesterId.set('');

      this.semesters.set([]);

      this.sections.set([]);

      this.academicSessions.set([]);

      if (!value) {
        return;
      }

      this.academicSessionService.getLookupByCampus(value).subscribe((sessions) => {
        this.academicSessions.set(sessions);
      });
    });

    this.form.controls.departmentId.valueChanges.subscribe((value) => {
      this.selectedDepartmentId.set(value ?? '');

      this.form.patchValue(
        {
          courseId: '',
          semesterId: '',
          sectionId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.selectedCourseId.set('');

      this.selectedSemesterId.set('');

      this.semesters.set([]);

      this.sections.set([]);
    });

    this.form.controls.courseId.valueChanges.subscribe((value) => {
      this.selectedCourseId.set(value ?? '');

      this.form.patchValue(
        {
          semesterId: '',
          sectionId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.selectedSemesterId.set('');

      this.sections.set([]);

      this.loadSemesters(value ?? '');
    });

    this.form.controls.semesterId.valueChanges.subscribe((value) => {
      this.selectedSemesterId.set(value ?? '');

      this.form.patchValue(
        {
          sectionId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.loadSections(value ?? '');
    });

    this.form.controls.isFullDay.valueChanges.subscribe((fullDay) => {
      if (fullDay) {
        this.form.patchValue(
          {
            startTime: '',
            endTime: '',
          },
          {
            emitEvent: false,
          },
        );
      }
    });

    if (this.isEdit()) {
      this.loadEvent();
    }
  }

  private loadSemesters(courseId: string): void {
    if (!courseId) {
      this.semesters.set([]);

      return;
    }

    this.semesterService.getLookupByCourse(courseId).subscribe((x) => {
      this.semesters.set(x);
    });
  }

  private loadSections(semesterId: string): void {
    if (!semesterId) {
      this.sections.set([]);

      return;
    }

    this.sectionService.getLookupBySemester(semesterId).subscribe((x) => {
      this.sections.set(x);
    });
  }

  private loadEvent(): void {
    const event = this.data.event;

    if (!event) {
      return;
    }

    this.selectedCampusId.set(event.campusId);

    if (event.departmentId) {
      this.selectedDepartmentId.set(event.departmentId);
    }

    if (event.courseId) {
      this.selectedCourseId.set(event.courseId);
    }

    this.campusService.getLookup().subscribe((x) => {
      this.campuses.set(x);
    });

    this.loadSemesters(event.courseId ?? '');

    this.loadSections(event.semesterId ?? '');

    this.form.patchValue({
      campusId: event.campusId,
      departmentId: event.departmentId ?? '',
      courseId: event.courseId ?? '',
      semesterId: event.semesterId ?? '',
      sectionId: event.sectionId ?? '',
      teacherId: event.teacherId ?? '',
      academicSessionId: event.academicSessionId,

      title: event.title,
      description: event.description,

      eventType: event.eventType,

      startDate: new Date(event.startDate),
      endDate: new Date(event.endDate),

      startTime: event.startTime ?? '',
      endTime: event.endTime ?? '',

      isFullDay: event.isFullDay,

      isRecurring: event.isRecurring,

      recurrenceRule: event.recurrenceRule ?? '',

      priority: event.priority,

      affectsTimetable: event.affectsTimetable,
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const value = this.form.getRawValue();

    if (value.startDate && value.endDate) {
      if (value.endDate < value.startDate) {
        this.notificationService.error('End date cannot be before start date.');

        return;
      }
    }

    if (!value.isFullDay && value.startTime && value.endTime && value.startTime >= value.endTime) {
      this.notificationService.error('End time must be greater than start time.');

      return;
    }

    if (value.isRecurring && !value.recurrenceRule?.trim()) {
      this.notificationService.error('Recurrence rule is required.');

      return;
    }

    this.saving.set(true);

    if (this.isEdit()) {
      this.update();
    } else {
      this.create();
    }
  }

  private create(): void {
    const request = this.buildCreateRequest();

    this.calendarEventService.create(request).subscribe({
      next: () => {
        this.notificationService.success('Calendar event created successfully.');

        this.saving.set(false);

        this.dialogRef.close(true);
      },

      error: () => {
        this.saving.set(false);
      },
    });
  }

  private update(): void {
    const request = this.buildUpdateRequest();

    this.calendarEventService.update(this.data.event!.id, request).subscribe({
      next: () => {
        this.notificationService.success('Calendar event updated successfully.');

        this.saving.set(false);

        this.dialogRef.close(true);
      },

      error: () => {
        this.saving.set(false);
      },
    });
  }

  close(): void {
    this.dialogRef.close(false);
  }

  private buildCreateRequest(): CreateCalendarEventRequest {
    const value = this.form.getRawValue();

    return {
      campusId: value.campusId!,
      departmentId: value.departmentId || null,
      courseId: value.courseId || null,
      semesterId: value.semesterId || null,
      sectionId: value.sectionId || null,
      teacherId: value.teacherId || null,
      roomId: null,

      academicSessionId: value.academicSessionId!,

      title: value.title!.trim(),

      description: value.description?.trim() || null,

      eventType: value.eventType!,

      startDate: this.toDateOnly(value.startDate!),

      endDate: this.toDateOnly(value.endDate!),

      startTime: value.isFullDay || !value.startTime ? null : value.startTime,

      endTime: value.isFullDay || !value.endTime ? null : value.endTime,

      isFullDay: value.isFullDay!,

      isRecurring: value.isRecurring!,

      recurrenceRule: value.isRecurring ? value.recurrenceRule?.trim() || null : null,

      priority: value.priority!,

      affectsTimetable: value.affectsTimetable!,
    };
  }

  private buildUpdateRequest(): UpdateCalendarEventRequest {
    return {
      ...this.buildCreateRequest(),
    };
  }

  private toDateOnly(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  readonly eventTypes = [
    { value: EventType.Holiday, name: 'Holiday', icon: 'beach_access' },
    { value: EventType.Examination, name: 'Examination', icon: 'assignment' },
    { value: EventType.Workshop, name: 'Workshop', icon: 'build' },
    { value: EventType.Seminar, name: 'Seminar', icon: 'school' },
    { value: EventType.SportsDay, name: 'Sports Day', icon: 'sports_soccer' },
    { value: EventType.CulturalEvent, name: 'Cultural', icon: 'celebration' },
    { value: EventType.GuestLecture, name: 'Guest Lecture', icon: 'record_voice_over' },
    { value: EventType.FacultyMeeting, name: 'Faculty Meeting', icon: 'groups' },
    { value: EventType.ParentTeacherMeeting, name: 'Parent Meeting', icon: 'people' },
    { value: EventType.ExtraClass, name: 'Extra Class', icon: 'menu_book' },
    { value: EventType.Maintenance, name: 'Maintenance', icon: 'construction' },
    { value: EventType.PlacementDrive, name: 'Placement', icon: 'work' },
    { value: EventType.Convocation, name: 'Convocation', icon: 'workspace_premium' },
    { value: EventType.Orientation, name: 'Orientation', icon: 'explore' },
    { value: EventType.Custom, name: 'Custom', icon: 'category' },
  ];

  selectEventType(type: EventType): void {
    this.form.controls.eventType.setValue(type);

    this.form.controls.priority.setValue(this.getPriorityByEventType(type));

    switch (type) {
      case EventType.Holiday:
        this.form.patchValue({
          isFullDay: true,
          affectsTimetable: true,
        });
        break;

      case EventType.Examination:
        this.form.patchValue({
          affectsTimetable: true,
        });
        break;

      default:
        break;
    }
  }

  private getPriorityByEventType(type: EventType): number {
    switch (type) {
      case EventType.Holiday:
        return 300;

      case EventType.Examination:
        return 500;

      case EventType.Workshop:
        return 130;

      case EventType.Seminar:
        return 120;

      case EventType.SportsDay:
        return 200;

      case EventType.CulturalEvent:
        return 200;

      case EventType.GuestLecture:
        return 140;

      case EventType.FacultyMeeting:
        return 110;

      case EventType.ParentTeacherMeeting:
        return 110;

      case EventType.ExtraClass:
        return 100;

      case EventType.Maintenance:
        return 250;

      case EventType.PlacementDrive:
        return 200;

      case EventType.Convocation:
        return 300;

      case EventType.Orientation:
        return 150;

      case EventType.Custom:
        return 100;

      default:
        return 100;
    }
  }
}
