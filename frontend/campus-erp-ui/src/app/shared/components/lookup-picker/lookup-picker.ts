import { CommonModule } from '@angular/common';
import {
  Component,
  Inject,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { LookupPickerData, LookupPickerItem } from './lookup-picker.model';

@Component({
  selector: 'app-lookup-picker',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './lookup-picker.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './lookup-picker.scss',
})
export class LookupPickerComponent {
  private readonly dialogRef = inject(MatDialogRef<LookupPickerComponent>);

  readonly search = new FormControl('');

  readonly searchText = signal('');

  readonly selected = signal<LookupPickerItem | null>(null);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: LookupPickerData,
  ) {
    this.search.valueChanges.subscribe((value) => {
      this.searchText.set((value ?? '').toLowerCase());
    });
  }

  readonly filtered = computed(() => {
    const search = this.searchText();

    if (!search) {
      return this.data.items;
    }

    return this.data.items.filter(
      (x) =>
        x.title.toLowerCase().includes(search) ||
        (x.subtitle ?? '').toLowerCase().includes(search) ||
        (x.tag ?? '').toLowerCase().includes(search),
    );
  });

  select(item: LookupPickerItem): void {
    if (item.disabled) {
      return;
    }

    this.selected.set(item);
  }

  doubleClick(item: LookupPickerItem): void {
    this.dialogRef.close(item);
  }

  confirm(): void {
    this.dialogRef.close(this.selected());
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
