import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { CurrentUserService } from '../../../core/services/current-user';

@Component({
  selector: 'app-not-found',

  standalone: true,

  imports: [CommonModule, MatButtonModule, MatIconModule],

  templateUrl: './not-found.html',

  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './not-found.scss',
})
export class NotFoundComponent {
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
