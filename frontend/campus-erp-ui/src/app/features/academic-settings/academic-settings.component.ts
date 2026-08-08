import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

import { AcademicConfigurationService } from './services/academic-configuration';
import { NotificationService } from '../../core/services/notification';
import { PermissionService } from '../../core/services/permission';

import { AcademicConfiguration } from './models/academic-configuration';
import { UpdateAcademicConfigurationRequest } from './models/update-academic-configuration-request';
import { AcademicTermType } from './models/academic-term-type';
import { Permissions } from '../../core/constants/permissions';

@Component({
  selector: 'app-academic-settings',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,

    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  templateUrl: './academic-settings.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './academic-settings.component.scss',
})
export class AcademicSettingsComponent implements OnInit {
  private readonly service = inject(AcademicConfigurationService);

  private readonly fb = inject(FormBuilder);

  private readonly notification = inject(NotificationService);

  private readonly permissionService = inject(PermissionService);

  readonly configuration = signal<AcademicConfiguration | null>(null);

  readonly loading = signal(true);

  readonly saving = signal(false);

  readonly canEdit = signal(this.permissionService.has(Permissions.AcademicSettingsEdit));

  readonly academicTermTypes = [
    {
      value: AcademicTermType.Annual,
      name: 'Annual',
    },
    {
      value: AcademicTermType.Semester,
      name: 'Semester',
    },
    {
      value: AcademicTermType.Trimester,
      name: 'Trimester',
    },
    {
      value: AcademicTermType.Quarter,
      name: 'Quarter',
    },
    {
      value: AcademicTermType.Custom,
      name: 'Custom',
    },
  ];

  readonly form = this.fb.nonNullable.group({
    // Academic Structure

    academicTermType: [AcademicTermType.Semester, Validators.required],

    academicTermsPerSession: [2, [Validators.required, Validators.min(1)]],

    autoPromoteEnabled: [true],

    // Attendance Rules

    minimumAttendancePercentage: [
      75,
      [Validators.required, Validators.min(0), Validators.max(100)],
    ],

    allowAttendanceEditing: [true],

    attendanceEditWindowDays: [7, [Validators.required, Validators.min(0)]],

    // Attendance Automation

    autoGenerateAttendanceSessions: [true],

    autoGenerateAttendanceRecords: [true],

    // Attendance Lock

    attendanceLockAfterDays: [7, [Validators.required, Validators.min(0)]],

    allowTeacherAttendanceUnlock: [false],

    // Attendance Behaviour

    lateThresholdMinutes: [10, [Validators.required, Validators.min(0)]],

    medicalLeaveCountsAsPresent: [false],

    onDutyCountsAsPresent: [true],

    // Student Requests

    allowStudentAttendanceCorrection: [true],
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);

    this.service
      .get()
      .pipe(
        finalize(() => {
          this.loading.set(false);

          if (!this.canEdit()) {
            this.form.disable();
          }
        }),
      )
      .subscribe({
        next: (configuration) => {
          this.configuration.set(configuration);

          this.form.patchValue({
            // Academic Structure

            academicTermType: configuration.academicTermType,

            academicTermsPerSession: configuration.academicTermsPerSession,

            autoPromoteEnabled: configuration.autoPromoteEnabled,

            // Attendance Rules

            minimumAttendancePercentage: configuration.minimumAttendancePercentage,

            allowAttendanceEditing: configuration.allowAttendanceEditing,

            attendanceEditWindowDays: configuration.attendanceEditWindowDays,

            // Attendance Automation

            autoGenerateAttendanceSessions: configuration.autoGenerateAttendanceSessions,

            autoGenerateAttendanceRecords: configuration.autoGenerateAttendanceRecords,

            // Attendance Lock

            attendanceLockAfterDays: configuration.attendanceLockAfterDays,

            allowTeacherAttendanceUnlock: configuration.allowTeacherAttendanceUnlock,

            // Attendance Behaviour

            lateThresholdMinutes: configuration.lateThresholdMinutes,

            medicalLeaveCountsAsPresent: configuration.medicalLeaveCountsAsPresent,

            onDutyCountsAsPresent: configuration.onDutyCountsAsPresent,

            // Student Requests

            allowStudentAttendanceCorrection: configuration.allowStudentAttendanceCorrection,
          });
        },

        error: () => {
          this.notification.error('Unable to load academic settings.');
        },
      });
  }

  save(): void {
    if (!this.canEdit()) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();

      this.notification.warning('Please correct the validation errors.');

      return;
    }

    const request: UpdateAcademicConfigurationRequest = {
      // Academic Structure

      academicTermType: this.form.controls.academicTermType.value,

      academicTermsPerSession: this.form.controls.academicTermsPerSession.value,

      autoPromoteEnabled: this.form.controls.autoPromoteEnabled.value,

      // Attendance Rules

      minimumAttendancePercentage: this.form.controls.minimumAttendancePercentage.value,

      allowAttendanceEditing: this.form.controls.allowAttendanceEditing.value,

      attendanceEditWindowDays: this.form.controls.attendanceEditWindowDays.value,

      // Attendance Automation

      autoGenerateAttendanceSessions: this.form.controls.autoGenerateAttendanceSessions.value,

      autoGenerateAttendanceRecords: this.form.controls.autoGenerateAttendanceRecords.value,

      // Attendance Lock

      attendanceLockAfterDays: this.form.controls.attendanceLockAfterDays.value,

      allowTeacherAttendanceUnlock: this.form.controls.allowTeacherAttendanceUnlock.value,

      // Attendance Behaviour

      lateThresholdMinutes: this.form.controls.lateThresholdMinutes.value,

      medicalLeaveCountsAsPresent: this.form.controls.medicalLeaveCountsAsPresent.value,

      onDutyCountsAsPresent: this.form.controls.onDutyCountsAsPresent.value,

      // Student Requests

      allowStudentAttendanceCorrection: this.form.controls.allowStudentAttendanceCorrection.value,
    };

    this.saving.set(true);

    this.service
      .update(request)
      .pipe(
        finalize(() => {
          this.saving.set(false);
        }),
      )
      .subscribe({
        next: (configuration) => {
          this.configuration.set(configuration);

          this.notification.success('Academic settings updated successfully.');
        },

        error: () => {
          this.notification.error('Unable to update academic settings.');
        },
      });
  }

  reset(): void {
    const configuration = this.configuration();

    if (!configuration) {
      return;
    }

    this.form.reset({
      // Academic Structure

      academicTermType: configuration.academicTermType,

      academicTermsPerSession: configuration.academicTermsPerSession,

      autoPromoteEnabled: configuration.autoPromoteEnabled,

      // Attendance Rules

      minimumAttendancePercentage: configuration.minimumAttendancePercentage,

      allowAttendanceEditing: configuration.allowAttendanceEditing,

      attendanceEditWindowDays: configuration.attendanceEditWindowDays,

      // Attendance Automation

      autoGenerateAttendanceSessions: configuration.autoGenerateAttendanceSessions,

      autoGenerateAttendanceRecords: configuration.autoGenerateAttendanceRecords,

      // Attendance Lock

      attendanceLockAfterDays: configuration.attendanceLockAfterDays,

      allowTeacherAttendanceUnlock: configuration.allowTeacherAttendanceUnlock,

      // Attendance Behaviour

      lateThresholdMinutes: configuration.lateThresholdMinutes,

      medicalLeaveCountsAsPresent: configuration.medicalLeaveCountsAsPresent,

      onDutyCountsAsPresent: configuration.onDutyCountsAsPresent,

      // Student Requests

      allowStudentAttendanceCorrection: configuration.allowStudentAttendanceCorrection,
    });
  }
}
