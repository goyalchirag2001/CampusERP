import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DepartmentService } from '../../departments/services/department';
import { CourseService } from '../services/course';
import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';
import { Lookup } from '../../../core/models/lookup';

@Component({
  selector: 'app-course-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './course-create-dialog.html',
  styleUrl: './course-create-dialog.scss',
})
export class CourseCreateDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly courseService = inject(CourseService);

  private readonly departmentService = inject(DepartmentService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<CourseCreateDialog>);

  readonly departments = signal<Lookup[]>([]);

  readonly saving = signal(false);

  form = this.fb.group({
    departmentId: ['', Validators.required],

    name: ['', Validators.required],

    code: ['', Validators.required],

    degreeType: ['', Validators.required],

    durationYears: [4, Validators.required],

    totalSemesters: [8, Validators.required],
  });

  ngOnInit(): void {
    this.departmentService.getLookup().subscribe((data) => {
      this.departments.set(data);
    });
  }

  onNameChange(): void {
    const value = this.form.controls.name.value ?? '';

    const code = value.trim().toUpperCase().replace(/\s+/g, '-');

    this.form.patchValue(
      {
        code,
      },
      {
        emitEvent: false,
      },
    );
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const user = this.currentUserService.user();

    this.saving.set(true);

    this.courseService
      .create({
        institutionId: user?.institutionId ?? '',

        campusId: user?.campusId ?? '',

        departmentId: this.form.value.departmentId ?? '',

        name: this.form.value.name ?? '',

        code: this.form.value.code ?? '',

        degreeType: this.form.value.degreeType ?? '',

        durationYears: this.form.value.durationYears ?? 0,

        totalSemesters: this.form.value.totalSemesters ?? 0,
      })
      .subscribe({
        next: (course) => {
          this.notificationService.success('Course created successfully.');

          this.dialogRef.close(course);
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(err?.error?.message ?? 'Failed to create course.');
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
