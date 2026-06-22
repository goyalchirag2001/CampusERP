import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-temporary-password-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './temporary-password-dialog.html',
  styleUrl: './temporary-password-dialog.scss',
})
export class TemporaryPasswordDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      password: string;
    },
  ) {}

  copyPassword(): void {
    navigator.clipboard.writeText(this.data.password);
  }
}
