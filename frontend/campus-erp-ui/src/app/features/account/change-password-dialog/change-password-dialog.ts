import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';

import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { NotificationService } from '../../../core/services/notification';
import { AccountService } from '../services/account';

@Component({
  selector: 'app-change-password-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './change-password-dialog.html',
  styleUrl: './change-password-dialog.scss',
})
export class ChangePasswordDialogComponent {
  private readonly fb = inject(FormBuilder);

  private readonly accountService = inject(AccountService);

  private readonly notification = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<ChangePasswordDialogComponent>);

  readonly saving = signal(false);

  readonly hideCurrent = signal(true);

  readonly hideNew = signal(true);

  readonly hideConfirm = signal(true);

  readonly form = this.fb.group(
    {
      currentPassword: ['', Validators.required],

      newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(16)]],

      confirmPassword: ['', Validators.required],
    },
    {
      validators: this.passwordMatchValidator,
    },
  );

  readonly password = computed(() => this.form.controls.newPassword.value ?? '');

  readonly hasMinLength = computed(() => this.password().length >= 8);

  readonly hasMaxLength = computed(() => this.password().length <= 16);

  readonly hasUpper = computed(() => /[A-Z]/.test(this.password()));

  readonly hasLower = computed(() => /[a-z]/.test(this.password()));

  readonly hasDigit = computed(() => /\d/.test(this.password()));

  readonly hasSpecial = computed(() => /[^A-Za-z0-9]/.test(this.password()));

  readonly strengthScore = computed(() => {
    let score = 0;

    if (this.hasMinLength()) score++;

    if (this.hasUpper()) score++;

    if (this.hasLower()) score++;

    if (this.hasDigit()) score++;

    if (this.hasSpecial()) score++;

    return score;
  });

  readonly strengthLabel = computed(() => {
    switch (this.strengthScore()) {
      case 0:
      case 1:
        return 'Very Weak';

      case 2:
        return 'Weak';

      case 3:
        return 'Fair';

      case 4:
        return 'Strong';

      case 5:
        return 'Excellent';

      default:
        return '';
    }
  });

  readonly strengthPercentage = computed(() => {
    return (this.strengthScore() / 5) * 100;
  });

  readonly strengthClass = computed(() => {
    switch (this.strengthScore()) {
      case 0:
      case 1:
        return 'strength-danger';

      case 2:
        return 'strength-warning';

      case 3:
        return 'strength-info';

      case 4:
      case 5:
        return 'strength-success';

      default:
        return '';
    }
  });

  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('newPassword')?.value;

    const confirm = control.get('confirmPassword')?.value;

    if (!password || !confirm) {
      return null;
    }

    return password === confirm
      ? null
      : {
          passwordMismatch: true,
        };
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.saving.set(true);

    this.accountService
      .changePassword({
        currentPassword: this.form.controls.currentPassword.value ?? '',

        newPassword: this.form.controls.newPassword.value ?? '',
      })
      .subscribe({
        next: () => {
          this.notification.success('Password changed successfully. Please login again.');

          this.dialogRef.close(true);
        },

        error: (err) => {
          this.saving.set(false);

          this.notification.error(err.error?.message ?? 'Unable to change password.');
        },
      });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  toggleCurrent(): void {
    this.hideCurrent.update((x) => !x);
  }

  toggleNew(): void {
    this.hideNew.update((x) => !x);
  }

  toggleConfirm(): void {
    this.hideConfirm.update((x) => !x);
  }
}
