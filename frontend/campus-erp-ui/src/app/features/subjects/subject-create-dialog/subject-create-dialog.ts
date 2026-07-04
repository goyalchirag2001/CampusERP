import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { SubjectService } from '../services/subject';
import { CampusService } from '../../campuses/services/campus';
import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';
import { Lookup } from '../../../core/models/lookup';

@Component({
  selector: 'app-subject-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './subject-create-dialog.html',
  styleUrl: './subject-create-dialog.scss',
})
export class SubjectCreateDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly subjectService = inject(SubjectService);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<SubjectCreateDialog>);

  readonly campuses = signal<Lookup[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly saving = signal(false);

  readonly subjectTypes = [
    { value: 1, label: 'Core' },
    { value: 2, label: 'Elective' },
    { value: 3, label: 'Laboratory' },
    { value: 4, label: 'Project' },
  ];

  form = this.fb.group({
    campusId: ['', Validators.required],

    code: ['', Validators.required],

    name: ['', Validators.required],

    credits: [4, Validators.required],

    subjectType: [1, Validators.required],
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

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

  save(): void {
    const user = this.currentUserService.user();

    this.subjectService
      .create({
        institutionId: user?.institutionId ?? '',
        campusId: this.form.getRawValue().campusId ?? '',
        code: this.form.value.code ?? '',
        name: this.form.value.name ?? '',
        credits: Number(this.form.value.credits ?? 0),
        subjectType: Number(this.form.value.subjectType ?? 1),
      })
      .subscribe({
        next: (subject) => {
          this.notificationService.success('Subject created successfully.');

          this.dialogRef.close(subject);
        },

        error: (err) => {
          this.notificationService.error(err?.error?.message ?? 'Failed to create subject.');
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
