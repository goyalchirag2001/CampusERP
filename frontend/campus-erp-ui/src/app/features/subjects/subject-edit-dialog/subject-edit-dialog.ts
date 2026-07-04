import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { SubjectService } from '../services/subject';
import { Subject } from '../models/subject';

@Component({
  selector: 'app-subject-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './subject-edit-dialog.html',
  styleUrl: './subject-edit-dialog.scss',
})
export class SubjectEditDialog {
  private readonly fb = inject(FormBuilder);

  private readonly subjectService = inject(SubjectService);

  private readonly dialogRef = inject(MatDialogRef<SubjectEditDialog>);

  readonly subjectTypes = [
    { value: 1, label: 'Core' },
    { value: 2, label: 'Elective' },
    { value: 3, label: 'Laboratory' },
    { value: 4, label: 'Project' },
  ];

  form;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public subject: Subject,
  ) {
    this.form = this.fb.group({
      code: [subject.code, Validators.required],

      name: [subject.name, Validators.required],

      credits: [subject.credits, Validators.required],

      subjectType: [subject.subjectType, Validators.required],
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.subjectService
      .update(this.subject.id, {
        institutionId: this.subject.institutionId,

        campusId: this.subject.campusId,

        code: this.form.value.code ?? '',

        name: this.form.value.name ?? '',

        credits: Number(this.form.value.credits ?? 0),

        subjectType: Number(this.form.value.subjectType ?? 1),
      })
      .subscribe((updated) => {
        this.dialogRef.close(updated);
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
