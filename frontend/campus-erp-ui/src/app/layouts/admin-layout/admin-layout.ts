import { BreakpointObserver } from '@angular/cdk/layout';
import {
  Component,
  ViewChild,
  computed,
  inject,
  signal,
  OnInit,
  DestroyRef,
  ChangeDetectionStrategy,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterModule } from '@angular/router';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { InstitutionBrandingService } from '../../core/services/institution-branding';
import { ThemeService } from '../../core/services/theme';
import { CurrentUserService } from '../../core/services/current-user';
import { NavigationService } from '../../core/services/navigation';
import { AuthService } from '../../core/services/auth';
import { InstitutionBranding } from '../../core/models/institution-branding';
import { UserMenuComponent } from '../../core/layout/user-menu/user-menu';
import { UserContextService } from '../../core/services/user-context';

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
    UserMenuComponent,
  ],
  templateUrl: './admin-layout.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './admin-layout.scss',
})
export class AdminLayout implements OnInit {
  @ViewChild('drawer')
  drawer!: MatSidenav;

  private readonly destroyRef = inject(DestroyRef);

  private readonly breakpointObserver = inject(BreakpointObserver);

  private readonly themeService = inject(ThemeService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly navigationService = inject(NavigationService);

  private readonly authService = inject(AuthService);

  private readonly router = inject(Router);

  private readonly institutionBrandingService = inject(InstitutionBrandingService);

  private readonly userContext = inject(UserContextService);

  readonly user = this.currentUserService.user;

  readonly menuItems = computed(() => this.navigationService.getMenuItems());

  readonly branding = signal<InstitutionBranding | null>(null);

  readonly isMobile = signal(false);

  ngOnInit(): void {
    this.breakpointObserver
      .observe('(max-width: 768px)')
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((result) => {
        this.isMobile.set(result.matches);

        if (!this.drawer) {
          return;
        }

        if (result.matches) {
          this.drawer.close();
        } else {
          this.drawer.open();
        }
      });

    const slug = this.user()?.institutionSlug;

    if (slug) {
      this.institutionBrandingService.getBySlug(slug).subscribe({
        next: (branding) => this.branding.set(branding),
      });
    }

    this.userContext.refresh();
  }

  toggleMobileMenu(): void {
    this.drawer.toggle();
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
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
