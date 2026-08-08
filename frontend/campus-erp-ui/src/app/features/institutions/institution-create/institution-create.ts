import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

import { InstitutionService } from '../services/institution';

@Component({
  selector: 'app-institution-create',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './institution-create.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './institution-create.scss',
})
export class InstitutionCreate {
  private readonly fb = inject(FormBuilder);

  private readonly institutionService = inject(InstitutionService);

  private readonly router = inject(Router);

  form = this.fb.group({
    name: ['', Validators.required],

    code: ['', Validators.required],

    loginSlug: ['', Validators.required],

    email: [''],

    phone: [''],

    website: [''],

    address: [''],

    logoUrl: [''],

    primaryColor: ['#0F172A'],

    secondaryColor: ['#3B82F6'],

    adminFirstName: ['', Validators.required],

    adminLastName: ['', Validators.required],

    adminEmail: ['', [Validators.required, Validators.email]],
  });

  onInstitutionNameChange(): void {
    const name = this.form.controls.name.value ?? '';

    if (!name.trim()) {
      return;
    }

    const cleanName = name
      .replace(/University/gi, '')
      .replace(/College/gi, '')
      .trim();

    const code = cleanName.toUpperCase().replace(/\s+/g, '');

    const slug = cleanName.toLowerCase().replace(/\s+/g, '-');

    this.form.patchValue(
      {
        code,
        loginSlug: slug,
      },
      {
        emitEvent: false,
      },
    );
  }

  create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.institutionService
      .create({
        name: this.form.value.name ?? '',
        code: this.form.value.code ?? '',
        loginSlug: this.form.value.loginSlug ?? '',

        email: this.form.value.email ?? '',
        phone: this.form.value.phone ?? '',
        website: this.form.value.website ?? '',
        address: this.form.value.address ?? '',

        logoUrl: this.form.value.logoUrl ?? '',
        primaryColor: this.form.value.primaryColor ?? '',
        secondaryColor: this.form.value.secondaryColor ?? '',

        adminFirstName: this.form.value.adminFirstName ?? '',

        adminLastName: this.form.value.adminLastName ?? '',

        adminEmail: this.form.value.adminEmail ?? '',
      })
      .subscribe({
        next: () => {
          this.router.navigate(['/institutions']);
        },
        error: console.error,
      });
  }
}
