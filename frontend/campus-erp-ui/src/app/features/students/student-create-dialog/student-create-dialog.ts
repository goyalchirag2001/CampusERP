import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { StudentService } from '../services/student';
import { DepartmentService } from '../../departments/services/department';
import { CourseService } from '../../courses/services/course';
import { CampusService } from '../../campuses/services/campus';
import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';
import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';
import { Course } from '../../courses/models/course';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { TemporaryPasswordDialog } from '../../users/temporary-password-dialog/temporary-password-dialog';
import { SemesterService } from '../../semesters/services/semester';
import { SectionService } from '../../sections/services/section';

@Component({
  selector: 'app-student-create-dialog',
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
  ],
  templateUrl: './student-create-dialog.html',
  styleUrl: './student-create-dialog.scss',
})
export class StudentCreateDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly studentService = inject(StudentService);

  private readonly departmentService = inject(DepartmentService);

  private readonly courseService = inject(CourseService);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<StudentCreateDialog>);

  private readonly dialog = inject(MatDialog);

  private readonly semesterService = inject(SemesterService);

  private readonly sectionService = inject(SectionService);

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly courses = signal<Course[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly saving = signal(false);

  readonly selectedCampusId = signal('');

  readonly selectedDepartmentId = signal('');

  readonly semesters = signal<Lookup[]>([]);

  readonly sections = signal<Lookup[]>([]);

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

    const endYear = startYear + course.durationYears;

    this.form.patchValue(
      {
        batch: `${startYear}-${endYear}`,
      },
      {
        emitEvent: false,
      },
    );
  }

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.form.patchValue({
      institutionId: user?.institutionId ?? '',
    });

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    this.departmentService.getLookupWithCampus().subscribe((data) => {
      this.departments.set(data);
    });

    this.courseService.getAll().subscribe((data) => {
      this.courses.set(data);
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

      this.sections.set([]);

      if (!value) {
        this.semesters.set([]);
        return;
      }

      this.semesterService.getLookupByCourse(value).subscribe((data) => {
        this.semesters.set(data);
      });
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

      if (!value) {
        this.sections.set([]);
        return;
      }

      this.sectionService.getLookupBySemester(value).subscribe((data) => {
        this.sections.set(data);
      });
    });

    if (this.isCampusAdmin()) {
      const campusId = user?.campusId ?? '';

      this.form.patchValue({
        campusId,
      });

      this.selectedCampusId.set(campusId);

      this.form.controls.campusId.disable();
    } else {
      this.campusService.getLookup().subscribe((data) => {
        this.campuses.set(data);
      });
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

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

  close(): void {
    this.dialogRef.close();
  }
}
