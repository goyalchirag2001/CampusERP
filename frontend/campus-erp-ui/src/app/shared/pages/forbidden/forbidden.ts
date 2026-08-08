import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CurrentUserService } from '../../../core/services/current-user';

@Component({
  selector: 'app-forbidden',

  standalone: true,

  imports: [CommonModule, MatButtonModule, MatIconModule],

  templateUrl: './forbidden.html',

  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './forbidden.scss',
})
export class ForbiddenComponent {
  private readonly router = inject(Router);

  private readonly currentUser = inject(CurrentUserService);

  goHome(): void {
    const user = this.currentUser.user();

    if (!user) {
      this.router.navigate(['/platform/login']);

      return;
    }

    if (!user.institutionSlug) {
      this.router.navigate(['/platform/dashboard']);

      return;
    }

    if (this.currentUser.isStudent() || this.currentUser.isTeacher()) {
      this.router.navigate(['/', user.institutionSlug, 'profile']);

      return;
    }

    this.router.navigate(['/', user.institutionSlug, 'dashboard']);
  }
}
