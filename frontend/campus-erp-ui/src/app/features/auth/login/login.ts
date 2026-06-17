import { Component, OnInit, inject, signal } from '@angular/core';

import { ActivatedRoute, Router } from '@angular/router';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';

import { MatFormFieldModule } from '@angular/material/form-field';

import { MatInputModule } from '@angular/material/input';

import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../../core/services/auth';

import { CurrentUserService } from '../../../core/services/current-user';

import { InstitutionBrandingService } from '../../../core/services/institution-branding';

import { InstitutionBranding } from '../../../core/models/institution-branding';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly authService = inject(AuthService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly brandingService = inject(InstitutionBrandingService);

  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  readonly branding = signal<InstitutionBranding | null>(null);

  institutionSlug: string | null = null;

  isPlatformLogin = true;

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],

    password: ['', Validators.required],
  });

  ngOnInit(): void {
    this.institutionSlug = this.route.snapshot.paramMap.get('institutionSlug');

    this.isPlatformLogin = !this.institutionSlug;

    if (this.institutionSlug && this.institutionSlug !== 'platform') {
      this.brandingService.getBySlug(this.institutionSlug).subscribe({
        next: (branding) => {
          this.branding.set(branding);

          document.documentElement.style.setProperty(
            '--institution-primary',
            branding.primaryColor ?? '#0F172A',
          );

          document.documentElement.style.setProperty(
            '--institution-secondary',
            branding.secondaryColor ?? '#3B82F6',
          );
        },
        error: () => {
          this.router.navigate(['/platform/login']);
        },
      });
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.authService
      .login({
        email: this.loginForm.value.email!,
        password: this.loginForm.value.password!,
        institutionSlug: this.isPlatformLogin ? null : this.institutionSlug,
      })
      .subscribe({
        next: (response) => {
          this.authService.saveTokens(response);

          this.authService.getCurrentUser().subscribe({
            next: (user) => {
              this.currentUserService.setUser(user);

              const slug = user.institutionSlug;

              if (slug) {
                this.router.navigate(['/', slug, 'dashboard']);
              } else {
                this.router.navigate(['/platform/dashboard']);
              }
            },
          });
        },
        error: console.error,
      });
  }
}
