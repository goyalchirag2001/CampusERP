import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  computed,
  inject,
  signal,
} from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CurrentUserService } from '../../../core/services/current-user';

import { RoomService } from '../services/room';

import { Room } from '../models/room';
import { RoomType } from '../models/room-type';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RoomFormDialog } from '../room-form-dialog/room-form-dialog';

@Component({
  selector: 'app-room-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatTooltipModule,
    MatCardModule,
    MatDialogModule,
  ],
  templateUrl: './room-list.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './room-list.scss',
})
export class RoomList implements OnInit {
  private readonly roomService = inject(RoomService);

  private readonly router = inject(Router);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly dialog = inject(MatDialog);

  readonly rooms = signal<Room[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly displayedColumns = [
    'room',
    'building',
    'floor',
    'roomType',
    'capacity',
    'facilities',
    'accessibility',
    'status',
    'actions',
  ];

  readonly filteredRooms = computed(() => {
    const searchTerm = this.search().trim().toLowerCase();

    if (!searchTerm) {
      return this.rooms();
    }

    return this.rooms().filter(
      (room) =>
        room.roomName.toLowerCase().includes(searchTerm) ||
        room.roomNumber.toLowerCase().includes(searchTerm) ||
        room.building.toLowerCase().includes(searchTerm) ||
        room.floor.toLowerCase().includes(searchTerm) ||
        room.roomType.toLowerCase().includes(searchTerm) ||
        room.campusName.toLowerCase().includes(searchTerm),
    );
  });

  readonly pagedRooms = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredRooms().slice(start, end);
  });

  readonly totalRooms = computed(() => this.rooms().length);

  readonly activeRooms = computed(() => this.rooms().filter((room) => room.isActive).length);

  readonly inactiveRooms = computed(() => this.rooms().filter((room) => !room.isActive).length);

  readonly accessibleRooms = computed(
    () => this.rooms().filter((room) => room.isAccessible).length,
  );

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.roomService.getAll().subscribe((data) => {
      this.rooms.set(data);

      this.pageIndex.set(0);
    });
  }

  refresh(): void {
    this.load();
  }

  onSearchChange(value: string): void {
    this.search.set(value);

    this.pageIndex.set(0);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createRoom(): void {
    const dialogRef = this.dialog.open(RoomFormDialog, {
      width: '850px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      autoFocus: false,
      data: {},
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.load();
      }
    });
  }

  openRoom(id: string): void {
    this.router.navigate([this.baseRoute, 'rooms', id]);
  }

  getRoomTypeName(roomType: string): string {
    const numericType = Number(roomType);

    if (!Number.isNaN(numericType)) {
      return RoomType[numericType] ?? roomType;
    }

    return roomType
      .replace(/([a-z])([A-Z])/g, '$1 $2')
      .replace(/^./, (value) => value.toUpperCase());
  }

  getFacilities(room: Room): string {
    const facilities: string[] = [];

    if (room.hasProjector) {
      facilities.push('Projector');
    }

    if (room.hasSmartBoard) {
      facilities.push('Smart Board');
    }

    if (room.hasAirConditioning) {
      facilities.push('AC');
    }

    if (room.hasComputers) {
      facilities.push('Computers');
    }

    if (room.hasInternet) {
      facilities.push('Internet');
    }

    return facilities.length ? facilities.join(', ') : 'None';
  }
}
