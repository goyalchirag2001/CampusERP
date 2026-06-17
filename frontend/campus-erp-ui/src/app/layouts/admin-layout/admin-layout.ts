import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { OnInit } from '@angular/core';

import { InstitutionBrandingService } from '../../core/services/institution-branding';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { ThemeService } from '../../core/services/theme';
import { CurrentUserService } from '../../core/services/current-user';
import { NavigationService } from '../../core/services/navigation';
import { AuthService } from '../../core/services/auth';
import { InstitutionBranding } from '../../core/models/institution-branding';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [
    RouterModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.scss',
})
export class AdminLayout implements OnInit {
  private readonly themeService = inject(ThemeService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly navigationService = inject(NavigationService);

  private readonly authService = inject(AuthService);

  private readonly router = inject(Router);

  private readonly institutionBrandingService = inject(InstitutionBrandingService);

  ngOnInit(): void {
    const slug = this.user()?.institutionSlug;

    if (!slug) {
      return;
    }

    this.institutionBrandingService.getBySlug(slug).subscribe({
      next: (data) => {
        this.branding.set(data);
      },
    });
  }

  readonly user = this.currentUserService.user;

  readonly menuItems = computed(() => this.navigationService.getMenuItems());

  readonly branding = signal<InstitutionBranding | null>(null);

  isMobileMenuOpen = false;

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  logout(): void {
    const slug = this.user()?.institutionSlug;

    this.authService.logout();

    this.currentUserService.clear();

    if (slug) {
      this.router.navigate(['/', slug, 'login']);

      return;
    }

    this.router.navigate(['/platform/login']);
  }
}
