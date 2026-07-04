import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { TeacherService } from '../services/teacher';
import { DepartmentService } from '../../departments/services/department';
import { CampusService } from '../../campuses/services/campus';

import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';

import { TemporaryPasswordDialog } from '../../users/temporary-password-dialog/temporary-password-dialog';

@Component({
  selector: 'app-teacher-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './teacher-create-dialog.html',
  styleUrl: './teacher-create-dialog.scss',
})
export class TeacherCreateDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly teacherService = inject(TeacherService);

  private readonly departmentService = inject(DepartmentService);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<TeacherCreateDialog>);

  private readonly dialog = inject(MatDialog);

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly saving = signal(false);

  readonly selectedCampusId = signal('');

  readonly filteredDepartments = computed(() =>
    this.departments().filter((x) => x.campusId === this.selectedCampusId()),
  );

  form = this.fb.group({
    institutionId: [''],

    campusId: ['', Validators.required],

    departmentId: ['', Validators.required],

    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', [Validators.required, Validators.email]],

    phoneNumber: [''],

    employeeCode: ['', Validators.required],

    designation: ['', Validators.required],
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.form.patchValue({
      institutionId: user?.institutionId ?? '',
    });

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    this.departmentService.getLookupWithCampus().subscribe((data) => {
      this.departments.set(data);
    });

    this.form.controls.campusId.valueChanges.subscribe((value) => {
      this.selectedCampusId.set(value ?? '');

      this.form.patchValue(
        {
          departmentId: '',
        },
        {
          emitEvent: false,
        },
      );
    });

    if (this.isCampusAdmin()) {
      this.form.patchValue({
        campusId: user?.campusId ?? '',
      });

      this.selectedCampusId.set(user?.campusId ?? '');

      this.form.controls.campusId.disable();
    } else {
      this.campusService.getLookup().subscribe((data) => {
        this.campuses.set(data);
      });
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const user = this.currentUserService.user();

    this.saving.set(true);

    this.teacherService
      .create({
        institutionId: user?.institutionId ?? '',

        campusId: this.form.getRawValue().campusId ?? user?.campusId ?? '',

        departmentId: this.form.value.departmentId ?? '',

        firstName: this.form.value.firstName ?? '',

        lastName: this.form.value.lastName ?? '',

        email: this.form.value.email ?? '',

        phoneNumber: this.form.value.phoneNumber ?? '',

        employeeCode: this.form.value.employeeCode ?? '',

        designation: this.form.value.designation ?? '',
      })
      .subscribe({
        next: (teacher) => {
          this.notificationService.success('Teacher created successfully.');

          this.dialog.open(TemporaryPasswordDialog, {
            width: '500px',
            data: {
              password: teacher.temporaryPassword,
            },
          });

          this.dialogRef.close(teacher);
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(err?.error?.message ?? 'Failed to create teacher.');
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
