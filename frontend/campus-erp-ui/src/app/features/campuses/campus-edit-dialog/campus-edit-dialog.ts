import { Component, Inject, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Campus } from '../models/campus';
import { CampusService } from '../services/campus';

@Component({
  selector: 'app-campus-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './campus-edit-dialog.html',
})
export class CampusEditDialog {
  private readonly fb = inject(FormBuilder);

  private readonly campusService = inject(CampusService);

  private readonly dialogRef = inject(MatDialogRef<CampusEditDialog>);

  form!: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public campus: Campus,
  ) {
    this.form = this.fb.group({
      name: [campus.name, Validators.required],

      code: [campus.code, Validators.required],

      campusHeadName: [campus.campusHeadName ?? ''],

      email: [campus.email ?? ''],

      phone: [campus.phone ?? ''],

      address: [campus.address ?? ''],
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.campusService.update(this.campus.id, this.form.getRawValue()).subscribe((updated) => {
      this.dialogRef.close(updated);
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
