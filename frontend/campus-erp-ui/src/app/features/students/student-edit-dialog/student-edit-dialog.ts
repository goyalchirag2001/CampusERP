import { Component, Inject, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { StudentService } from '../services/student';
import { DepartmentService } from '../../departments/services/department';
import { CourseService } from '../../courses/services/course';
import { CampusService } from '../../campuses/services/campus';

import { Student } from '../models/student';
import { Course } from '../../courses/models/course';
import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';

import { NotificationService } from '../../../core/services/notification';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-student-edit-dialog',
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
  templateUrl: './student-edit-dialog.html',
  styleUrl: './student-edit-dialog.scss',
})
export class StudentEditDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly studentService = inject(StudentService);

  private readonly departmentService = inject(DepartmentService);

  private readonly courseService = inject(CourseService);

  private readonly campusService = inject(CampusService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<StudentEditDialog>);

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly courses = signal<Course[]>([]);

  readonly selectedCampusId = signal('');

  readonly selectedDepartmentId = signal('');

  readonly filteredDepartments = computed(() =>
    this.departments().filter((x) => x.campusId === this.selectedCampusId()),
  );

  readonly filteredCourses = computed(() =>
    this.courses().filter((x) => x.departmentId === this.selectedDepartmentId()),
  );

  form = this.fb.group({
    campusId: ['', Validators.required],

    departmentId: ['', Validators.required],

    courseId: ['', Validators.required],

    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', [Validators.required, Validators.email]],

    phoneNumber: [''],

    rollNumber: ['', Validators.required],

    batch: ['', Validators.required],

    admissionDate: ['', Validators.required],
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public student: Student,
  ) {}

  ngOnInit(): void {
    this.campusService.getLookup().subscribe((data) => {
      this.campuses.set(data);
    });

    this.departmentService.getLookupWithCampus().subscribe((data) => {
      this.departments.set(data);
    });

    this.courseService.getAll().subscribe((data) => {
      this.courses.set(data);
    });

    this.form.patchValue({
      campusId: this.student.campusId,
      departmentId: this.student.departmentId,
      courseId: this.student.courseId,
      firstName: this.student.firstName,
      lastName: this.student.lastName,
      email: this.student.email,
      phoneNumber: this.student.phoneNumber ?? '',
      rollNumber: this.student.rollNumber,
      batch: this.student.batch,
      admissionDate: this.student.admissionDate,
    });

    this.selectedCampusId.set(this.student.campusId);

    this.selectedDepartmentId.set(this.student.departmentId);

    this.form.controls.campusId.valueChanges.subscribe((value) => {
      this.selectedCampusId.set(value ?? '');

      this.form.patchValue(
        {
          departmentId: '',
          courseId: '',
        },
        { emitEvent: false },
      );
    });

    this.form.controls.departmentId.valueChanges.subscribe((value) => {
      this.selectedDepartmentId.set(value ?? '');

      this.form.patchValue(
        {
          courseId: '',
        },
        { emitEvent: false },
      );
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.studentService
      .update(this.student.id, {
        departmentId: this.form.value.departmentId ?? '',

        courseId: this.form.value.courseId ?? '',

        rollNumber: this.form.value.rollNumber ?? '',

        batch: this.form.value.batch ?? '',

        admissionDate: this.form.value.admissionDate ?? '',

        firstName: this.form.value.firstName ?? '',

        lastName: this.form.value.lastName ?? '',

        email: this.form.value.email ?? '',

        phoneNumber: this.form.value.phoneNumber ?? '',
      })
      .subscribe({
        next: (student) => {
          this.notificationService.success('Student updated successfully.');

          this.dialogRef.close(student);
        },

        error: (err) => {
          this.notificationService.error(err?.error?.message ?? 'Failed to update student.');
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
