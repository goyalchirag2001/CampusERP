import { Component, EventEmitter, Output, inject } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { CommonModule } from '@angular/common';

import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

import { StudentService } from '../../../core/services/student';

@Component({
  selector: 'app-student-drawer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatInputModule, MatFormFieldModule],
  templateUrl: './student-drawer.html',
  styleUrl: './student-drawer.scss',
})
export class StudentDrawer {
  @Output()
  closed = new EventEmitter<void>();

  @Output()
  saved = new EventEmitter<void>();

  private readonly fb = inject(FormBuilder);

  private readonly studentService = inject(StudentService);

  form = this.fb.group({
    institutionId: [''],
    campusId: [''],
    departmentId: [''],
    courseId: [''],

    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', Validators.required],

    phoneNumber: [''],

    password: ['Student@123'],

    rollNumber: ['', Validators.required],

    batch: ['', Validators.required],

    admissionDate: ['', Validators.required],
  });

  close(): void {
    this.closed.emit();
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.studentService.create(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.saved.emit();

        this.close();
      },
    });
  }
}
