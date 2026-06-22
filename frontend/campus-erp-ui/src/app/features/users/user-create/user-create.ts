import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { UserService } from '../services/user';
import { CampusService } from '../../campuses/services/campus';
import { InstitutionService } from '../../institutions/services/institution';
import { RoleService } from '../../roles/services/role';
import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

import { Institution } from '../../../core/models/institution';
import { Lookup } from '../../../core/models/lookup';

import { TemporaryPasswordDialog } from '../temporary-password-dialog/temporary-password-dialog';

@Component({
  selector: 'app-user-create',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDialogModule,
  ],
  templateUrl: './user-create.html',
  styleUrl: './user-create.scss',
})
export class UserCreate implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly userService = inject(UserService);

  private readonly campusService = inject(CampusService);

  private readonly institutionService = inject(InstitutionService);

  private readonly roleService = inject(RoleService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialog = inject(MatDialog);

  private readonly router = inject(Router);

  readonly institutions = signal<Institution[]>([]);

  readonly campuses = signal<Lookup[]>([]);

  readonly roles = signal<Lookup[]>([]);

  readonly isPlatformAdmin = signal(false);

  readonly isInstitutionAdmin = signal(false);

  readonly isCampusAdmin = signal(false);

  form = this.fb.group({
    institutionId: [''],

    campusId: ['', Validators.required],

    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', [Validators.required, Validators.email]],

    phoneNumber: [''],

    roleIds: this.fb.control<string[]>([], Validators.required),
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.isPlatformAdmin.set(
      user?.roles.includes('SuperAdmin') || user?.roles.includes('PlatformAdmin') || false,
    );

    this.isInstitutionAdmin.set(user?.roles.includes('InstitutionAdmin') || false);

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') || false);

    // Platform Admin / Super Admin
    if (this.isPlatformAdmin()) {
      this.form.controls.institutionId.addValidators(Validators.required);

      this.institutionService.getAll().subscribe((data) => this.institutions.set(data));
    }
    // Institution Admin / Campus Admin
    else {
      this.form.patchValue({
        institutionId: user?.institutionId ?? '',
      });
    }

    // Campus Admin
    if (this.isCampusAdmin()) {
      this.form.patchValue({
        campusId: user?.campusId ?? '',
      });

      this.form.controls.campusId.disable();
    } else {
      this.campusService.getLookup().subscribe((data) => this.campuses.set(data));
    }

    this.roleService.getLookup().subscribe((data) => this.roles.set(data));
  }

  create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const formValue = this.form.getRawValue();

    this.userService
      .create({
        institutionId: formValue.institutionId ?? '',

        campusId: formValue.campusId ?? '',

        firstName: formValue.firstName ?? '',

        lastName: formValue.lastName ?? '',

        email: formValue.email ?? '',

        phoneNumber: formValue.phoneNumber ?? '',

        roleIds: formValue.roleIds ?? [],
      })
      .subscribe({
        next: (user) => {
          this.dialog.open(TemporaryPasswordDialog, {
            width: '500px',
            data: {
              password: user.temporaryPassword,
            },
          });

          this.notificationService.success('User created successfully.');
        },
        error: (err) => {
          this.notificationService.error(err?.error?.message ?? 'Failed to create user.');
        },
      });
  }

  cancel(): void {
    const slug = this.currentUserService.user()?.institutionSlug;

    if (slug) {
      this.router.navigate(['/', slug, 'users']);

      return;
    }

    this.router.navigate(['/platform/users']);
  }
}
