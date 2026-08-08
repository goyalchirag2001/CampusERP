import { CommonModule } from '@angular/common';
import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';

import { TimetableTemplate } from '../models/timetable-template';

export interface TimetableTemplateDialogData {
  mode: 'create' | 'edit';

  timetable?: TimetableTemplate;
}

@Component({
  selector: 'app-timetable-template-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatIconModule,
  ],
  templateUrl: './timetable-template-form-dialog.html',
  styleUrl: './timetable-template-form-dialog.scss',
})
export class TimetableTemplateFormDialog {
  private readonly fb = inject(FormBuilder);

  constructor(
    private readonly dialogRef: MatDialogRef<TimetableTemplateFormDialog>,

    @Inject(MAT_DIALOG_DATA)
    public readonly data: TimetableTemplateDialogData,
  ) {}

  readonly form = this.fb.nonNullable.group({
    teacherAssignmentId: ['', Validators.required],

    academicSessionId: ['', Validators.required],

    teacherId: ['', Validators.required],

    sectionId: ['', Validators.required],

    semesterSubjectId: ['', Validators.required],

    roomId: [''],

    dayOfWeek: [1, Validators.required],

    startTime: ['', Validators.required],

    endTime: ['', Validators.required],

    validFrom: ['', Validators.required],

    validTo: ['', Validators.required],

    lectureType: [1, Validators.required],

    priority: [100],

    generateAttendance: [true],

    isOnline: [false],

    meetingLink: [''],

    remarks: [''],

    displayOrder: [1],
  });

  get isEdit(): boolean {
    return this.data.mode === 'edit';
  }

  save(): void {}

  cancel(): void {
    this.dialogRef.close(false);
  }
}
