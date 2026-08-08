import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { CampusService } from '../services/campus';

import { CurrentUserService } from '../../../core/services/current-user';

import { Institution } from '../../../core/models/institution';

import { InstitutionService } from '../../institutions/services/institution';

@Component({
  selector: 'app-campus-create',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './campus-create.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './campus-create.scss',
})
export class CampusCreate implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly institutionService = inject(InstitutionService);

  private readonly router = inject(Router);

  readonly institutions = signal<Institution[]>([]);

  readonly isSuperAdmin = signal(false);

  readonly isPlatformAdmin = signal(false);

  readonly isSaving = signal(false);

  form = this.fb.group({
    institutionId: ['', Validators.required],

    name: ['', Validators.required],

    code: ['', Validators.required],

    campusHeadName: [''],

    email: [''],

    phone: [''],

    address: [''],
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    const isSuperAdminDash = user?.roles.includes('SuperAdmin') ?? false;

    const isPlatform = user?.roles.includes('PlatformAdmin') ?? false;

    this.isSuperAdmin.set(isSuperAdminDash);

    this.isPlatformAdmin.set(isPlatform);

    if (isPlatform || isSuperAdminDash) {
      this.institutionService.getAll().subscribe({
        next: (data) => this.institutions.set(data),
        error: (err) => {
          console.error(err);

          alert('Failed to load institutions.');
        },
      });
    } else {
      this.form.controls.institutionId.clearValidators();

      this.form.controls.institutionId.updateValueAndValidity();
    }
  }

  onCampusNameChange(): void {
    const name = this.form.controls.name.value ?? '';

    if (!name.trim()) {
      return;
    }

    const code = name
      .replace(/Campus/gi, '')
      .trim()
      .toUpperCase()
      .replace(/\s+/g, '-');

    this.form.patchValue(
      {
        code,
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

    this.isSaving.set(true);

    this.campusService
      .create({
        institutionId: this.form.value.institutionId ?? '',

        name: this.form.value.name ?? '',

        code: this.form.value.code ?? '',

        campusHeadName: this.form.value.campusHeadName ?? '',

        email: this.form.value.email ?? '',

        phone: this.form.value.phone ?? '',

        address: this.form.value.address ?? '',
      })
      .subscribe({
        next: () => {
          this.isSaving.set(false);

          alert('Campus created successfully.');

          const slug = this.currentUserService.user()?.institutionSlug;

          if (slug) {
            this.router.navigate(['/', slug, 'campuses']);

            return;
          }

          this.router.navigate(['/platform/campuses']);
        },

        error: (err) => {
          this.isSaving.set(false);

          const message = err?.error?.message ?? 'Failed to create campus.';

          alert(message);

          console.error(err);
        },
      });
  }
}
