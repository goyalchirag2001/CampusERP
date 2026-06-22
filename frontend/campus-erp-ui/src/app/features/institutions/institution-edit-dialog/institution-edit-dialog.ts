import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { Institution } from '../../../core/models/institution';
import { InstitutionService } from '../services/institution';

@Component({
  selector: 'app-institution-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './institution-edit-dialog.html',
})
export class InstitutionEditDialog {
  private readonly fb = inject(FormBuilder);

  private readonly institutionService = inject(InstitutionService);

  private readonly dialogRef = inject(MatDialogRef<InstitutionEditDialog>);

  form;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public institution: Institution,
  ) {
    this.form = this.fb.group({
      name: [institution.name, Validators.required],

      code: [institution.code, Validators.required],

      loginSlug: [institution.loginSlug, Validators.required],

      email: [institution.email ?? ''],

      phone: [institution.phone ?? ''],

      website: [institution.website ?? ''],

      address: [institution.address ?? ''],

      logoUrl: [institution.logoUrl ?? ''],

      primaryColor: [institution.primaryColor ?? '#0F172A'],

      secondaryColor: [institution.secondaryColor ?? '#3B82F6'],

      adminFirstName: [institution.adminFirstName ?? ''],

      adminLastName: [institution.adminLastName ?? ''],

      adminEmail: [institution.adminEmail ?? ''],
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.institutionService
      .update(this.institution.id, this.form.getRawValue())
      .subscribe((updated) => {
        this.dialogRef.close(updated);
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
