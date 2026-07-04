import { Component, Inject, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TeacherService } from '../services/teacher';
import { DepartmentService } from '../../departments/services/department';
import { CampusService } from '../../campuses/services/campus';
import { Teacher } from '../models/teacher';
import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-teacher-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './teacher-edit-dialog.html',
  styleUrl: './teacher-edit-dialog.scss',
})
export class TeacherEditDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly teacherService = inject(TeacherService);

  private readonly departmentService = inject(DepartmentService);

  private readonly campusService = inject(CampusService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<TeacherEditDialog>);

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly selectedCampusId = signal('');

  readonly filteredDepartments = computed(() =>
    this.departments().filter((x) => x.campusId === this.selectedCampusId()),
  );

  form = this.fb.group({
    campusId: ['', Validators.required],

    departmentId: ['', Validators.required],

    firstName: ['', Validators.required],

    lastName: ['', Validators.required],

    email: ['', [Validators.required, Validators.email]],

    phoneNumber: [''],

    employeeCode: ['', Validators.required],

    designation: ['', Validators.required],
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public teacher: Teacher,
  ) {}

  ngOnInit(): void {
    this.campusService.getLookup().subscribe((data) => {
      this.campuses.set(data);
    });

    this.departmentService.getLookupWithCampus().subscribe((data) => {
      this.departments.set(data);
    });

    this.form.patchValue({
      campusId: this.teacher.campusId,

      departmentId: this.teacher.departmentId,

      firstName: this.teacher.firstName,

      lastName: this.teacher.lastName,

      email: this.teacher.email,

      phoneNumber: this.teacher.phoneNumber ?? '',

      employeeCode: this.teacher.employeeCode,

      designation: this.teacher.designation,
    });

    this.selectedCampusId.set(this.teacher.campusId);

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
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.teacherService
      .update(this.teacher.id, {
        institutionId: this.teacher.institutionId,

        campusId: this.form.value.campusId ?? '',

        departmentId: this.form.value.departmentId ?? '',

        employeeCode: this.form.value.employeeCode ?? '',

        designation: this.form.value.designation ?? '',

        firstName: this.form.value.firstName ?? '',

        lastName: this.form.value.lastName ?? '',

        email: this.form.value.email ?? '',

        phoneNumber: this.form.value.phoneNumber ?? '',
      })
      .subscribe({
        next: (teacher) => {
          this.notificationService.success('Teacher updated successfully.');

          this.dialogRef.close(teacher);
        },

        error: (err) => {
          this.notificationService.error(err?.error?.message ?? 'Failed to update teacher.');
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
