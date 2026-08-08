import { Component, Inject, inject, ChangeDetectionStrategy } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { DepartmentService } from '../services/department';

import { Department } from '../models/department';

@Component({
  selector: 'app-department-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './department-edit-dialog.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './department-edit-dialog.scss',
})
export class DepartmentEditDialog {
  private readonly fb = inject(FormBuilder);

  private readonly departmentService = inject(DepartmentService);

  private readonly dialogRef = inject(MatDialogRef<DepartmentEditDialog>);

  form;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public department: Department,
  ) {
    this.form = this.fb.group({
      name: [department.name, Validators.required],

      code: [department.code, Validators.required],
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.departmentService
      .update(this.department.id, {
        institutionId: this.department.institutionId,

        campusId: this.department.campusId,

        name: this.form.value.name ?? '',

        code: this.form.value.code ?? '',
      })
      .subscribe((updated) => {
        this.dialogRef.close(updated);
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
