import { Component, Inject, inject } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { NotificationService } from '../../../core/services/notification';

import { UserService } from '../services/user';

@Component({
  selector: 'app-reset-password-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './reset-password-dialog.html',
  styleUrl: './reset-password-dialog.scss',
})
export class ResetPasswordDialog {
  private readonly fb = inject(FormBuilder);

  private readonly userService = inject(UserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<ResetPasswordDialog>);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      id: string;
      firstName: string;
      lastName: string;
    },
  ) {}

  form = this.fb.group({
    password: ['', Validators.required],

    confirmPassword: ['', Validators.required],
  });

  reset(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const password = this.form.value.password ?? '';

    const confirmPassword = this.form.value.confirmPassword ?? '';

    if (password !== confirmPassword) {
      this.notificationService.error('Passwords do not match.');

      return;
    }

    this.userService.resetPassword(this.data.id, password).subscribe({
      next: () => {
        this.notificationService.success('Password reset successfully.');

        this.dialogRef.close(true);
      },
      error: (err) => {
        this.notificationService.error(err?.error?.message ?? 'Failed to reset password.');
      },
    });
  }
}
