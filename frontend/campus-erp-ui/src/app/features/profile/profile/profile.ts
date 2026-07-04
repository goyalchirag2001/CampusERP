import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { Profile } from '../models/profile';
import { ProfileService } from '../services/profile';
import { NotificationService } from '../../../core/services/notification';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordDialogComponent } from '../../account/change-password-dialog/change-password-dialog';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { CurrentUserService } from '../../../core/services/current-user';
import { UserContextService } from '../../../core/services/user-context';

@Component({
  selector: 'app-profile',

  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
  ],

  templateUrl: './profile.html',

  styleUrl: './profile.scss',
})
export class ProfileComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly profileService = inject(ProfileService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialog = inject(MatDialog);

  private readonly router = inject(Router);

  private readonly authService = inject(AuthService);

  private readonly currentUserService = inject(CurrentUserService);

  readonly userContext = inject(UserContextService);

  readonly user = this.currentUserService.user;

  readonly profile = signal<Profile | null>(null);

  readonly loading = signal(true);

  readonly saving = signal(false);

  readonly editing = signal(false);

  readonly form = this.fb.group({
    phoneNumber: this.fb.control('', [Validators.required, Validators.maxLength(20)]),
  });

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.loading.set(true);

    this.profileService.getMyProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);

        this.form.patchValue({
          phoneNumber: profile.phoneNumber,
        });

        this.loading.set(false);
      },

      error: (err) => {
        this.loading.set(false);

        this.notificationService.error(err.error?.message ?? 'Unable to load profile.');
      },
    });
  }

  startEditing(): void {
    this.editing.set(true);
  }

  cancelEditing(): void {
    const profile = this.profile();

    if (!profile) {
      return;
    }

    this.form.patchValue({
      phoneNumber: profile.phoneNumber,
    });

    this.editing.set(false);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.saving.set(true);

    this.profileService
      .update({
        phoneNumber: this.form.controls.phoneNumber.value ?? '',
      })
      .subscribe({
        next: (profile) => {
          this.profile.set(profile);

          this.editing.set(false);

          this.saving.set(false);

          this.notificationService.success('Profile updated successfully.');
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(err.error?.message ?? 'Unable to update profile.');
        },
      });
  }

  getStatusClass(status?: number): string {
    switch (status) {
      case 1:
        return 'status-active';

      case 2:
        return 'status-detained';

      case 3:
        return 'status-graduated';

      case 4:
        return 'status-completed';

      default:
        return 'status-default';
    }
  }

  changePassword(): void {
    const dialogRef = this.dialog.open(ChangePasswordDialogComponent, {
      width: '500px',

      disableClose: true,

      autoFocus: false,
    });

    dialogRef.afterClosed().subscribe((changed) => {
      if (!changed) {
        return;
      }

      this.logoutAfterPasswordChange();
    });
  }

  private logoutAfterPasswordChange(): void {
    const slug = this.user()?.institutionSlug;

    this.authService.logout();

    this.currentUserService.clear();

    this.userContext.clear();

    this.notificationService.success('Password changed successfully. Please login again.');

    if (slug) {
      this.router.navigate(['/', slug, 'login']);

      return;
    }

    this.router.navigate(['/platform/login']);
  }

  hasProfilePhoto(): boolean {
    return !!this.profile()?.profilePhotoUrl;
  }

  profilePhoto(): string {
    return this.profile()?.profilePhotoUrl ?? '';
  }

  avatarInitials(): string {
    return this.profile()?.avatarInitials ?? '';
  }

  isStudent(): boolean {
    return this.profile()?.role === 'Student';
  }

  isTeacher(): boolean {
    return this.profile()?.role === 'Teacher';
  }

  isAdmin(): boolean {
    return !this.isStudent() && !this.isTeacher();
  }
}
