import { Component, OnInit, Inject, inject, signal, ChangeDetectionStrategy } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatSelectModule } from '@angular/material/select';

import { CourseService } from '../services/course';

import { DepartmentService } from '../../departments/services/department';

import { Course } from '../models/course';

import { Lookup } from '../../../core/models/lookup';

@Component({
  selector: 'app-course-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './course-edit-dialog.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './course-edit-dialog.scss',
})
export class CourseEditDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly courseService = inject(CourseService);

  private readonly departmentService = inject(DepartmentService);

  private readonly dialogRef = inject(MatDialogRef<CourseEditDialog>);

  readonly departments = signal<Lookup[]>([]);

  form;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public course: Course,
  ) {
    this.form = this.fb.group({
      departmentId: [course.departmentId, Validators.required],

      name: [course.name, Validators.required],

      code: [course.code, Validators.required],

      degreeType: [course.degreeType, Validators.required],

      durationYears: [course.durationYears, Validators.required],

      totalSemesters: [course.totalSemesters, Validators.required],
    });
  }

  ngOnInit(): void {
    this.departmentService.getLookup().subscribe((data) => {
      this.departments.set(data);
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.courseService
      .update(this.course.id, {
        institutionId: this.course.institutionId,

        campusId: this.course.campusId,

        departmentId: this.form.value.departmentId ?? '',

        name: this.form.value.name ?? '',

        code: this.form.value.code ?? '',

        degreeType: this.form.value.degreeType ?? '',

        durationYears: this.form.value.durationYears ?? 0,

        totalSemesters: this.form.value.totalSemesters ?? 0,
      })
      .subscribe((updated) => {
        this.dialogRef.close(updated);
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
