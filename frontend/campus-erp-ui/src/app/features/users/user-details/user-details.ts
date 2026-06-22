import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from '../services/user';
import { User } from '../../../core/models/user';
import { NotificationService } from '../../../core/services/notification';
import { UserEditDialog } from '../user-edit-dialog/user-edit-dialog';
import { ResetPasswordDialog } from '../reset-password-dialog/reset-password-dialog';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './user-details.html',
  styleUrl: './user-details.scss',
})
export class UserDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly userService = inject(UserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialog = inject(MatDialog);

  readonly user = signal<User | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.load(id);
  }

  load(id: string): void {
    this.userService.getById(id).subscribe({
      next: (user) => {
        this.user.set(user);
      },
      error: () => {
        this.notificationService.error('Failed to load user.');
      },
    });
  }

  editUser(): void {
    const user = this.user();

    if (!user) {
      return;
    }

    const dialogRef = this.dialog.open(UserEditDialog, {
      width: '950px',
      maxWidth: '95vw',
      maxHeight: '90vh',

      data: {
        userId: user.id,
      },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        this.load(user.id);
      }
    });
  }

  resetPassword(): void {
    const user = this.user();

    if (!user) {
      return;
    }

    this.dialog.open(ResetPasswordDialog, {
      width: '500px',
      data: user,
    });
  }

  toggleStatus(): void {
    const user = this.user();

    if (!user) {
      return;
    }

    const request = user.isActive
      ? this.userService.deactivate(user.id)
      : this.userService.activate(user.id);

    request.subscribe({
      next: () => {
        this.notificationService.success(user.isActive ? 'User deactivated.' : 'User activated.');

        this.load(user.id);
      },
      error: () => {
        this.notificationService.error('Failed to update user.');
      },
    });
  }
}
