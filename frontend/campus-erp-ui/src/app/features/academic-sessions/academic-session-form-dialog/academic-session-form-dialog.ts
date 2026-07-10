import { Component, Inject, OnInit, computed, inject, signal } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { CampusService } from '../../campuses/services/campus';

import { AcademicSessionService } from '../services/academic-session';
import { AcademicSession } from '../models/academic-session';

import { Lookup } from '../../../core/models/lookup';

import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

export interface AcademicSessionFormDialogData {
  isEdit: boolean;

  academicSession?: AcademicSession;
}

@Component({
  selector: 'app-academic-session-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './academic-session-form-dialog.html',
  styleUrl: './academic-session-form-dialog.scss',
})
export class AcademicSessionFormDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly service = inject(AcademicSessionService);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<AcademicSessionFormDialog>);

  readonly campuses = signal<Lookup[]>([]);

  readonly saving = signal(false);

  readonly isCampusAdmin = signal(false);

  readonly isEdit = computed(() => this.data?.isEdit ?? false);

  form = this.fb.group({
    institutionId: [''],

    campusId: ['', Validators.required],

    name: ['', Validators.required],

    startDate: [null as Date | null, Validators.required],

    endDate: [null as Date | null, Validators.required],

    isCurrent: [false],
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: AcademicSessionFormDialogData | null,
  ) {}

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
      this.campusService.getLookup().subscribe((x) => {
        this.campuses.set(x);
      });
    }

    if (!this.isEdit()) {
      return;
    }

    const session = this.data?.academicSession;

    if (!session) {
      return;
    }

    this.form.patchValue({
      campusId: session.campusId,

      name: session.name,

      startDate: new Date(session.startDate),

      endDate: new Date(session.endDate),

      isCurrent: session.isCurrent,
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.saving.set(true);

    const user = this.currentUserService.user();

    const request = {
      institutionId: user?.institutionId ?? '',

      campusId: this.form.getRawValue().campusId ?? user?.campusId ?? '',

      name: this.form.value.name ?? '',

      startDate: this.toDateOnly(this.form.value.startDate ?? ''),

      endDate: this.toDateOnly(this.form.value.endDate ?? ''),

      isCurrent: this.form.value.isCurrent ?? false,
    };

    const operation =
      this.isEdit() && this.data?.academicSession
        ? this.service.update(this.data.academicSession.id, request)
        : this.service.create(request);

    operation.subscribe({
      next: (session) => {
        this.notificationService.success(
          this.isEdit()
            ? 'Academic session updated successfully.'
            : 'Academic session created successfully.',
        );

        this.dialogRef.close(session);
      },

      error: (err) => {
        this.saving.set(false);

        this.notificationService.error(err?.error?.message ?? 'Operation failed.');
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  private toDateOnly(date: Date | string | null): string {
    if (!date) {
      return '';
    }

    const d = new Date(date);

    const year = d.getFullYear();

    const month = String(d.getMonth() + 1).padStart(2, '0');

    const day = String(d.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}
