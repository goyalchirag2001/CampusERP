import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { Room } from '../models/room';
import { CreateRoomRequest } from '../models/create-room-request';
import { UpdateRoomRequest } from '../models/update-room-request';

import { RoomService } from '../services/room';

import { NotificationService } from '../../../core/services/notification';
import { CampusService } from '../../campuses/services/campus';
import { CurrentUserService } from '../../../core/services/current-user';
import { Lookup } from '../../../core/models/lookup';

interface RoomFormDialogData {
  room?: Room;
}

@Component({
  selector: 'app-room-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
  ],
  templateUrl: './room-form-dialog.html',
  styleUrl: './room-form-dialog.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
})
export class RoomFormDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly roomService = inject(RoomService);

  private readonly notificationService = inject(NotificationService);

  private readonly campusService = inject(CampusService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly dialogRef = inject(MatDialogRef<RoomFormDialog>);

  private readonly data = inject<RoomFormDialogData>(MAT_DIALOG_DATA);

  readonly campuses = signal<Lookup[]>([]);

  readonly isCampusAdmin = signal(false);

  readonly saving = signal(false);

  readonly room = this.data?.room;

  readonly isEditMode = !!this.room;

  readonly roomTypes = [
    'Classroom',
    'ComputerLab',
    'ScienceLab',
    'ElectronicsLab',
    'MechanicalLab',
    'CivilLab',
    'LanguageLab',
    'SeminarHall',
    'Auditorium',
    'ConferenceRoom',
    'StaffRoom',
    'Library',
    'ExaminationHall',
    'SportsGround',
    'Other',
  ];

  readonly form = this.fb.nonNullable.group({
    campusId: [this.room?.campusId ?? '', Validators.required],

    building: [this.room?.building ?? '', [Validators.required, Validators.maxLength(100)]],

    floor: [this.room?.floor ?? '', [Validators.required, Validators.maxLength(50)]],

    roomNumber: [this.room?.roomNumber ?? '', [Validators.required, Validators.maxLength(50)]],

    roomName: [this.room?.roomName ?? '', [Validators.required, Validators.maxLength(150)]],

    roomType: [this.room?.roomType ?? 'Classroom', Validators.required],

    capacity: [
      this.room?.capacity ?? 0,
      [Validators.required, Validators.min(1), Validators.max(10000)],
    ],

    hasProjector: [this.room?.hasProjector ?? false],

    hasSmartBoard: [this.room?.hasSmartBoard ?? false],

    hasAirConditioning: [this.room?.hasAirConditioning ?? false],

    hasComputers: [this.room?.hasComputers ?? false],

    hasInternet: [this.room?.hasInternet ?? false],

    description: [this.room?.description ?? '', Validators.maxLength(1000)],

    locationCode: [this.room?.locationCode ?? '', Validators.maxLength(50)],

    displayOrder: [
      this.room?.displayOrder ?? 0,
      [Validators.required, Validators.min(0), Validators.max(100000)],
    ],

    isAccessible: [this.room?.isAccessible ?? true],
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    if (this.isCampusAdmin()) {
      this.form.patchValue({
        campusId: user?.campusId ?? '',
      });

      this.form.controls.campusId.disable();

      return;
    }

    this.campusService.getLookup().subscribe({
      next: (data) => {
        this.campuses.set(data);
      },

      error: (err) => {
        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to load campuses.',
        );
      },
    });
  }

  get title(): string {
    return this.isEditMode ? 'Edit Room' : 'Create Room';
  }

  get submitLabel(): string {
    return this.isEditMode ? 'Update Room' : 'Create Room';
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const rawValue = this.form.getRawValue();

    if (this.isEditMode && this.room) {
      const request: UpdateRoomRequest = {
        building: rawValue.building.trim(),

        floor: rawValue.floor.trim(),

        roomNumber: rawValue.roomNumber.trim(),

        roomName: rawValue.roomName.trim(),

        roomType: rawValue.roomType,

        capacity: rawValue.capacity,

        hasProjector: rawValue.hasProjector,

        hasSmartBoard: rawValue.hasSmartBoard,

        hasAirConditioning: rawValue.hasAirConditioning,

        hasComputers: rawValue.hasComputers,

        hasInternet: rawValue.hasInternet,

        description: rawValue.description.trim() || null,

        locationCode: rawValue.locationCode.trim() || null,

        displayOrder: rawValue.displayOrder,

        isAccessible: rawValue.isAccessible,
      };

      this.update(this.room.id, request);

      return;
    }

    const request: CreateRoomRequest = {
      campusId: rawValue.campusId,
      
      building: rawValue.building.trim(),

      floor: rawValue.floor.trim(),

      roomNumber: rawValue.roomNumber.trim(),

      roomName: rawValue.roomName.trim(),

      roomType: rawValue.roomType,

      capacity: rawValue.capacity,

      hasProjector: rawValue.hasProjector,

      hasSmartBoard: rawValue.hasSmartBoard,

      hasAirConditioning: rawValue.hasAirConditioning,

      hasComputers: rawValue.hasComputers,

      hasInternet: rawValue.hasInternet,

      description: rawValue.description.trim() || null,

      locationCode: rawValue.locationCode.trim() || null,

      displayOrder: rawValue.displayOrder,

      isAccessible: rawValue.isAccessible,
    };

    this.create(request);
  }

  private create(request: CreateRoomRequest): void {
    this.roomService.create(request).subscribe({
      next: (room) => {
        this.notificationService.success('Room created successfully.');

        this.dialogRef.close(room);
      },

      error: (err) => {
        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to create room.',
        );
      },
    });
  }

  private update(id: string, request: UpdateRoomRequest): void {
    this.roomService.update(id, request).subscribe({
      next: (room) => {
        this.notificationService.success('Room updated successfully.');

        this.dialogRef.close(room);
      },

      error: (err) => {
        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to update room.',
        );
      },
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
