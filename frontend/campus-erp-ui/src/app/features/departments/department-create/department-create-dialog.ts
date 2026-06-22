import { Component, OnInit, inject, signal } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatSelectModule } from '@angular/material/select';

import { DepartmentService } from '../services/department';

import { CampusService } from '../../campuses/services/campus';

import { CurrentUserService } from '../../../core/services/current-user';

import { NotificationService } from '../../../core/services/notification';

import { Lookup } from '../../../core/models/lookup';

@Component({
  selector: 'app-department-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './department-create-dialog.html',
  styleUrl: './department-create-dialog.scss',
})
export class DepartmentCreateDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly departmentService = inject(DepartmentService);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<DepartmentCreateDialog>);

  readonly campuses = signal<Lookup[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly saving = signal(false);

  form = this.fb.group({
    institutionId: [''],

    campusId: ['', Validators.required],

    name: ['', Validators.required],

    code: ['', Validators.required],
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.form.patchValue({
      institutionId: user?.institutionId ?? '',
    });

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    if (this.isCampusAdmin()) {
      this.form.patchValue({
        campusId: user?.campusId ?? '',
      });

      this.form.controls.campusId.disable();
    } else {
      this.campusService.getLookup().subscribe((data) => {
        this.campuses.set(data);
      });
    }
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

    this.departmentService
      .create({
        institutionId: user?.institutionId ?? '',

        campusId: this.form.getRawValue().campusId ?? user?.campusId ?? '',

        name: this.form.value.name ?? '',

        code: this.form.value.code ?? '',
      })
      .subscribe({
        next: (department) => {
          this.notificationService.success('Department created successfully.');

          this.dialogRef.close(department);
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(err?.error?.message ?? 'Failed to create department.');
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
