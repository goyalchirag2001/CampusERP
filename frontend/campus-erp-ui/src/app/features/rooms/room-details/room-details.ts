import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

import { RoomService } from '../services/room';
import { Room } from '../models/room';
import { RoomFormDialog } from '../room-form-dialog/room-form-dialog';

@Component({
  selector: 'app-room-details',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatTooltipModule, MatDialogModule],
  templateUrl: './room-details.html',
  styleUrl: './room-details.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoomDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly roomService = inject(RoomService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly room = signal<Room | null>(null);

  readonly loading = signal(true);

  readonly error = signal<string | null>(null);

  readonly actionLoading = signal(false);

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error.set('Room ID was not provided.');
      this.loading.set(false);

      return;
    }

    this.load(id);
  }

  private load(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.roomService.getById(id).subscribe({
      next: (room) => {
        this.room.set(room);
        this.loading.set(false);
      },

      error: (err) => {
        this.error.set(err?.error?.message ?? err?.message ?? 'Unable to load room details.');

        this.loading.set(false);
      },
    });
  }

  goBack(): void {
    this.router.navigate([this.baseRoute, 'rooms']);
  }

  editRoom(): void {
    const room = this.room();

    if (!room) {
      return;
    }

    const dialogRef = this.dialog.open(RoomFormDialog, {
      width: '850px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      autoFocus: false,
      data: {
        room,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (!result) {
        return;
      }

      this.load(room.id);
    });
  }

  activateRoom(): void {
    const id = this.room()?.id;

    if (!id || this.actionLoading()) {
      return;
    }

    this.actionLoading.set(true);

    this.roomService.activate(id).subscribe({
      next: () => {
        this.room.update((room) =>
          room
            ? {
                ...room,
                isActive: true,
              }
            : room,
        );

        this.notificationService.success('Room activated successfully.');

        this.actionLoading.set(false);
      },

      error: (err) => {
        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to activate room.',
        );

        this.actionLoading.set(false);
      },
    });
  }

  deactivateRoom(): void {
    const id = this.room()?.id;

    if (!id || this.actionLoading()) {
      return;
    }

    this.actionLoading.set(true);

    this.roomService.deactivate(id).subscribe({
      next: () => {
        this.room.update((room) =>
          room
            ? {
                ...room,
                isActive: false,
              }
            : room,
        );

        this.notificationService.success('Room deactivated successfully.');

        this.actionLoading.set(false);
      },

      error: (err) => {
        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to deactivate room.',
        );

        this.actionLoading.set(false);
      },
    });
  }

  getAmenityClass(value: boolean): string {
    return value ? 'available' : 'unavailable';
  }
}
