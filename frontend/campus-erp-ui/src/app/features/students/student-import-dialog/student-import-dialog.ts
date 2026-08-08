import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { StudentImportService } from '../services/student-import';
import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

import { StudentImportValidation } from '../models/student-import-validation';

@Component({
  selector: 'app-student-import-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './student-import-dialog.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './student-import-dialog.scss',
})
export class StudentImportDialog implements OnInit {
  private readonly service = inject(StudentImportService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef);

  readonly selectedFile = signal<File | null>(null);

  readonly validation = signal<StudentImportValidation | null>(null);

  readonly loading = signal(false);

  readonly validating = signal(false);

  readonly importing = signal(false);

  readonly institutionId = signal('');

  readonly campusId = signal('');

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.institutionId.set(user?.institutionId ?? '');

    this.campusId.set(user?.campusId ?? '');
  }

  chooseFile(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files?.length) {
      return;
    }

    const file = input.files[0];

    if (!file.name.toLowerCase().endsWith('.xlsx')) {
      this.notificationService.error('Please select a valid Excel (.xlsx) file.');

      input.value = '';

      return;
    }

    this.selectedFile.set(file);

    this.validation.set(null);
  }

  downloadTemplate(): void {
    this.loading.set(true);

    this.service.downloadTemplate().subscribe({
      next: (blob) => {
        this.downloadBlob(blob, 'StudentImportTemplate.xlsx');

        this.loading.set(false);
      },

      error: () => {
        this.loading.set(false);

        this.notificationService.error('Unable to download template.');
      },
    });
  }

  validate(): void {
    const file = this.selectedFile();

    if (!file) {
      this.notificationService.warning('Please choose an Excel file.');

      return;
    }

    this.validating.set(true);

    this.service.validate(this.institutionId(), this.campusId(), file).subscribe({
      next: (result) => {
        this.validation.set(result);

        this.validating.set(false);

        if (result.canImport) {
          this.notificationService.success(`${result.validRows} students are ready to import.`);
        } else {
          this.notificationService.warning(`${result.invalidRows} invalid rows found.`);
        }
      },

      error: (err) => {
        this.validating.set(false);

        this.notificationService.error(err?.error?.message ?? 'Validation failed.');
      },
    });
  }

  import(): void {
    const file = this.selectedFile();

    if (!file) {
      return;
    }

    if (!this.validation()?.canImport) {
      this.notificationService.warning('Please fix validation errors before importing.');

      return;
    }

    this.importing.set(true);

    this.service.import(this.institutionId(), this.campusId(), file).subscribe({
      next: (result) => {
        this.service.downloadCredentials(result.credentials).subscribe({
          next: (blob) => {
            this.downloadBlob(blob, 'StudentCredentials.xlsx');
          },

          error: () => {
            this.notificationService.warning(
              'Students were imported, but credentials could not be downloaded.',
            );
          },
        });

        this.notificationService.success(`${result.validRows} students imported successfully.`);

        this.importing.set(false);

        this.dialogRef.close(true);
      },

      error: (err) => {
        this.importing.set(false);

        this.notificationService.error(err?.error?.message ?? 'Import failed.');
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  private downloadBlob(blob: Blob, fileName: string): void {
    const url = URL.createObjectURL(blob);

    const anchor = document.createElement('a');

    anchor.href = url;

    anchor.download = fileName;

    anchor.style.display = 'none';

    document.body.appendChild(anchor);

    anchor.click();

    document.body.removeChild(anchor);

    URL.revokeObjectURL(url);
  }
}
