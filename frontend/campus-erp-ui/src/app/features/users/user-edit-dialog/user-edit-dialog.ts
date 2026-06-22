import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { forkJoin } from 'rxjs';

import { UserService } from '../services/user';
import { RoleService } from '../../roles/services/role';
import { CampusService } from '../../campuses/services/campus';

import { Lookup } from '../../../core/models/lookup';

import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-user-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './user-edit-dialog.html',
  styleUrl: './user-edit-dialog.scss',
})
export class UserEditDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly userService = inject(UserService);

  private readonly roleService = inject(RoleService);

  private readonly campusService = inject(CampusService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<UserEditDialog>);

  private readonly data = inject(MAT_DIALOG_DATA) as {
    userId: string;
  };

  readonly roles = signal<Lookup[]>([]);

  readonly campuses = signal<Lookup[]>([]);

  readonly selectedRoleIds = signal<string[]>([]);

  private userId = '';

  form = this.fb.group({
    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', [Validators.required, Validators.email]],

    phoneNumber: [''],

    campusId: ['', Validators.required],
  });

  ngOnInit(): void {
    if (!this.data?.userId) {
      this.notificationService.error('User id not found.');

      this.dialogRef.close();

      return;
    }

    this.userId = this.data.userId;

    this.loadData();
  }

  private loadData(): void {
    forkJoin({
      user: this.userService.getById(this.userId),

      roles: this.roleService.getLookup(),

      campuses: this.campusService.getLookup(),
    }).subscribe({
      next: ({ user, roles, campuses }) => {
        this.roles.set(roles);

        this.campuses.set(campuses);

        this.selectedRoleIds.set((user as any).roleIds ?? []);

        this.form.patchValue({
          firstName: user.firstName,

          lastName: user.lastName,

          email: user.email,

          phoneNumber: user.phoneNumber ?? '',

          campusId: user.campusId,
        });
      },

      error: () => {
        this.notificationService.error('Failed to load user.');
      },
    });
  }

  toggleRole(roleId: string, checked: boolean): void {
    const current = [...this.selectedRoleIds()];

    if (checked) {
      if (!current.includes(roleId)) {
        current.push(roleId);
      }
    } else {
      const index = current.indexOf(roleId);

      if (index >= 0) {
        current.splice(index, 1);
      }
    }

    this.selectedRoleIds.set(current);
  }

  isRoleSelected(roleId: string): boolean {
    return this.selectedRoleIds().includes(roleId);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    if (this.selectedRoleIds().length === 0) {
      this.notificationService.warning('Select at least one role.');

      return;
    }

    this.userService
      .update(this.userId, {
        firstName: this.form.value.firstName ?? '',

        lastName: this.form.value.lastName ?? '',

        email: this.form.value.email ?? '',

        phoneNumber: this.form.value.phoneNumber ?? '',

        campusId: this.form.value.campusId ?? '',

        roleIds: this.selectedRoleIds(),
      })
      .subscribe({
        next: () => {
          this.notificationService.success('User updated successfully.');

          this.dialogRef.close(true);
        },

        error: (error) => {
          this.notificationService.error(error?.error?.message ?? 'Failed to update user.');
        },
      });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
