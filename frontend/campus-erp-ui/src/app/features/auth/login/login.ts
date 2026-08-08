import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
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
import { NotificationService } from '../../../core/services/notification';
import { UserContextService } from '../../../core/services/user-context';

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
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './login.scss',
})
export class Login implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly authService = inject(AuthService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly brandingService = inject(InstitutionBrandingService);

  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly notificationService = inject(NotificationService);

  private readonly userContextService = inject(UserContextService);

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

              // Prevent previous user's profile from appearing
              this.userContextService.clear();

              // Load current user's profile
              this.userContextService.refresh();

              this.navigateAfterLogin();
            },

            error: (err) => {
              console.error('ME API FAILED', err);

              this.notificationService.error(err?.error?.message ?? 'Failed to load user profile.');
            },
          });
        },
        error: (err) => {
          if (err.status === 401) {
            this.notificationService.error('Invalid email or password.');

            return;
          }

          this.notificationService.error(
            err.error?.message ?? 'Unable to login. Please try again.',
          );
        },
      });
  }

  private navigateAfterLogin(): void {
    const user = this.currentUserService.user();

    if (!user) {
      this.router.navigate(['/platform/login']);

      return;
    }

    // Platform Admin

    if (!user.institutionSlug) {
      this.router.navigate(['/platform/dashboard']);

      return;
    }

    // Student

    if (this.currentUserService.isStudent()) {
      this.router.navigate(['/', user.institutionSlug, 'profile']);

      return;
    }

    // Teacher

    if (this.currentUserService.isTeacher()) {
      this.router.navigate(['/', user.institutionSlug, 'profile']);

      return;
    }

    // Institution Admin / Campus Admin

    this.router.navigate(['/', user.institutionSlug, 'dashboard']);
  }
}
