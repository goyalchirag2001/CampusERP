import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { UserContextService } from '../../services/user-context';
import { AuthService } from '../../services/auth';
import { MatDivider } from '@angular/material/divider';
import { CurrentUserService } from '../../services/current-user';
import { MatDialog } from '@angular/material/dialog';
import { NotificationService } from '../../services/notification';
import { ChangePasswordDialogComponent } from '../../../features/account/change-password-dialog/change-password-dialog';

@Component({
  selector: 'app-user-menu',

  standalone: true,

  imports: [CommonModule, MatButtonModule, MatMenuModule, MatIconModule, MatDivider],

  templateUrl: './user-menu.html',

  styleUrl: './user-menu.scss',
})
export class UserMenuComponent {
  readonly userContext = inject(UserContextService);

  private readonly router = inject(Router);

  private readonly route = inject(ActivatedRoute);

  private readonly authService = inject(AuthService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly user = this.currentUserService.user;

  openProfile(): void {
    const slug = this.user()?.institutionSlug;

    if (slug) {
      this.router.navigate(['/', slug, 'profile']);

      return;
    }

    this.router.navigate(['/platform/profile']);
  }

  changePassword(): void {
    const dialogRef = this.dialog.open(ChangePasswordDialogComponent, {
      width: '500px',
      
      maxWidth: '95vw',

      disableClose: true,

      autoFocus: false,

      restoreFocus: false,
    });

    dialogRef.afterClosed().subscribe((changed) => {
      if (!changed) {
        return;
      }

      this.logoutAfterPasswordChange();
    });
  }

  private logoutAfterPasswordChange(): void {
    const slug = this.user()?.institutionSlug;

    this.authService.logout();

    this.currentUserService.clear();

    this.userContext.clear();

    this.notificationService.success('Password changed successfully. Please login again.');

    if (slug) {
      this.router.navigate(['/', slug, 'login']);

      return;
    }

    this.router.navigate(['/platform/login']);
  }

  logout(): void {
    const slug = this.user()?.institutionSlug;

    this.authService.logout();

    this.currentUserService.clear();

    this.userContext.clear();

    if (slug) {
      this.router.navigate(['/', slug, 'login']);

      return;
    }

    this.router.navigate(['/platform/login']);
  }
}
