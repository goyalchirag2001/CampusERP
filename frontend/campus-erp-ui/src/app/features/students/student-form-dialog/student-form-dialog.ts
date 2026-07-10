import { Component, Inject, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatExpansionModule } from '@angular/material/expansion';

import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

import { StudentService } from '../services/student';
import { DepartmentService } from '../../departments/services/department';
import { CourseService } from '../../courses/services/course';
import { CampusService } from '../../campuses/services/campus';
import { SemesterService } from '../../semesters/services/semester';
import { SectionService } from '../../sections/services/section';
import { AcademicSessionService } from '../../academic-sessions/services/academic-session';

import { NotificationService } from '../../../core/services/notification';
import { CurrentUserService } from '../../../core/services/current-user';

import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';
import { Course } from '../../courses/models/course';

import { Student } from '../models/student';

import { TemporaryPasswordDialog } from '../../users/temporary-password-dialog/temporary-password-dialog';

import { StudentFormDialogData } from '../models/student-form-dialog-data';
import { AcademicSessionLookup } from '../../academic-sessions/models/academic-session-lookup';

@Component({
  selector: 'app-student-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatExpansionModule,
  ],
  templateUrl: './student-form-dialog.html',
  styleUrl: './student-form-dialog.scss',
})
export class StudentFormDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly studentService = inject(StudentService);

  private readonly departmentService = inject(DepartmentService);

  private readonly courseService = inject(CourseService);

  private readonly campusService = inject(CampusService);

  private readonly semesterService = inject(SemesterService);

  private readonly sectionService = inject(SectionService);

  private readonly academicSessionService = inject(AcademicSessionService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<StudentFormDialog>);

  private readonly dialog = inject(MatDialog);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: StudentFormDialogData,
  ) {}

  readonly isEdit = computed(() => this.data.mode === 'edit');

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly courses = signal<Course[]>([]);

  readonly semesters = signal<Lookup[]>([]);

  readonly sections = signal<Lookup[]>([]);

  readonly academicSessions = signal<AcademicSessionLookup[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly saving = signal(false);

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

  form = this.fb.group({
    institutionId: [''],

    campusId: ['', Validators.required],

    departmentId: ['', Validators.required],

    courseId: ['', Validators.required],

    semesterId: ['', Validators.required],

    sectionId: ['', Validators.required],

    academicSessionId: ['', Validators.required],

    enrollmentStatus: [1, Validators.required],

    admissionNumber: ['', Validators.required],

    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', [Validators.required, Validators.email]],

    phoneNumber: [''],

    rollNumber: ['', Validators.required],

    batch: [''],

    admissionDate: [null as Date | null, Validators.required],
  });

  private updateBatch(): void {
    const admissionDate = this.form.getRawValue().admissionDate;

    const courseId = this.form.getRawValue().courseId;

    if (!admissionDate || !courseId) {
      this.form.patchValue(
        {
          batch: '',
        },
        {
          emitEvent: false,
        },
      );

      return;
    }

    const course = this.courses().find((x) => x.id === courseId);

    if (!course) {
      return;
    }

    const startYear =
      admissionDate instanceof Date
        ? admissionDate.getFullYear()
        : new Date(admissionDate).getFullYear();

    this.form.patchValue(
      {
        batch: `${startYear}-${startYear + course.durationYears}`,
      },
      {
        emitEvent: false,
      },
    );
  }

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    this.form.patchValue({
      institutionId: user?.institutionId ?? '',
    });

    this.departmentService.getLookupWithCampus().subscribe((x) => {
      this.departments.set(x);
    });

    this.courseService.getAll().subscribe((x) => {
      this.courses.set(x);
    });

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

    this.form.valueChanges.subscribe(() => {
      this.updateBatch();
    });

    this.form.controls.campusId.valueChanges.subscribe((value) => {
      this.selectedCampusId.set(value ?? '');

      this.form.patchValue(
        {
          departmentId: '',
          courseId: '',
          semesterId: '',
          sectionId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.selectedDepartmentId.set('');

      this.semesters.set([]);

      this.sections.set([]);
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

    if (this.isCampusAdmin()) {
      const campusId = user?.campusId ?? '';

      this.form.patchValue({
        campusId,
      });

      this.selectedCampusId.set(campusId);

      this.form.controls.campusId.disable();
    } else {
      this.campusService.getLookup().subscribe((x) => {
        this.campuses.set(x);
      });
    }

    if (this.isEdit()) {
      this.loadStudent();
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

  private loadStudent(): void {
    const student = this.data.student;

    if (!student) {
      return;
    }

    this.selectedCampusId.set(student.campusId);

    this.selectedDepartmentId.set(student.departmentId);

    this.selectedCourseId.set(student.courseId);

    this.selectedSemesterId.set(student.semesterId);

    this.loadSemesters(student.courseId);

    this.loadSections(student.semesterId);

    this.form.patchValue({
      campusId: student.campusId,

      departmentId: student.departmentId,

      courseId: student.courseId,

      semesterId: student.semesterId,

      sectionId: student.sectionId,

      academicSessionId: student.academicSessionId,

      enrollmentStatus: student.enrollmentStatus,

      admissionNumber: student.admissionNumber,

      firstName: student.firstName,

      lastName: student.lastName,

      email: student.email,

      phoneNumber: student.phoneNumber,

      rollNumber: student.rollNumber,

      batch: student.batch,

      admissionDate: student.admissionDate ? new Date(student.admissionDate) : null,
    });
  }

  private create(): void {
    const user = this.currentUserService.user();

    this.saving.set(true);

    this.studentService
      .create({
        institutionId: user?.institutionId ?? '',

        campusId: this.form.getRawValue().campusId ?? user?.campusId ?? '',

        departmentId: this.form.value.departmentId ?? '',

        courseId: this.form.value.courseId ?? '',

        semesterId: this.form.value.semesterId ?? '',

        sectionId: this.form.value.sectionId ?? '',

        academicSessionId: this.form.value.academicSessionId ?? '',

        enrollmentStatus: this.form.value.enrollmentStatus ?? 1,

        admissionNumber: this.form.value.admissionNumber ?? '',

        firstName: this.form.value.firstName ?? '',

        lastName: this.form.value.lastName ?? '',

        email: this.form.value.email ?? '',

        phoneNumber: this.form.value.phoneNumber ?? '',

        rollNumber: this.form.value.rollNumber ?? '',

        batch: this.form.value.batch ?? '',

        admissionDate: this.form.value.admissionDate ?? '',
      })
      .subscribe({
        next: (student) => {
          this.notificationService.success('Student created successfully.');

          this.dialog.open(TemporaryPasswordDialog, {
            width: '500px',

            data: {
              password: student.temporaryPassword,
            },
          });

          this.dialogRef.close(student);
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(err?.error?.message ?? 'Failed to create student.');
        },
      });
  }

  private update(): void {
    const student = this.data.student;

    if (!student) {
      return;
    }

    this.saving.set(true);

    this.studentService
      .update(student.id, {
        departmentId: this.form.value.departmentId ?? '',

        courseId: this.form.value.courseId ?? '',

        semesterId: this.form.value.semesterId ?? '',

        sectionId: this.form.value.sectionId ?? '',

        academicSessionId: this.form.value.academicSessionId ?? '',

        enrollmentStatus: this.form.value.enrollmentStatus ?? 1,

        admissionNumber: this.form.value.admissionNumber ?? '',

        rollNumber: this.form.value.rollNumber ?? '',

        batch: this.form.value.batch ?? '',

        admissionDate: this.form.value.admissionDate ?? '',

        firstName: this.form.value.firstName ?? '',

        lastName: this.form.value.lastName ?? '',

        email: this.form.value.email ?? '',

        phoneNumber: this.form.value.phoneNumber ?? '',
      })
      .subscribe({
        next: (updated) => {
          this.notificationService.success('Student updated successfully.');

          this.dialogRef.close(updated);
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(err?.error?.message ?? 'Failed to update student.');
        },
      });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    if (this.isEdit()) {
      this.update();
    } else {
      this.create();
    }
  }

  close(): void {
    this.dialogRef.close();
  }
}
