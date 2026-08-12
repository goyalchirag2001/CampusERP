import { ChangeDetectionStrategy, Component, Inject, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { TeacherAssignmentService } from '../../teacher-assignments/services/teacher-assignment';
import { RoomService } from '../../rooms/services/room';
import { NotificationService } from '../../../core/services/notification';
import { TimetableTemplate } from '../models/timetable-template';
import { CreateTimetableTemplateRequest } from '../models/create-timetable-template-request';
import { UpdateTimetableTemplateRequest } from '../models/update-timetable-template-request';
import { TeacherAssignment } from '../../teacher-assignments/models/teacher-assignment';
import { Lookup } from '../../../core/models/lookup';
import { TimetableTemplateService } from '../services/timetable-template';
import { concatMap, from, toArray } from 'rxjs';

export interface TimetableTemplateDialogData {
  mode: 'create' | 'edit' | 'duplicate';
  timetable?: TimetableTemplate;
}

interface AcademicSessionOption {
  id: string;

  name: string;
}

@Component({
  selector: 'app-timetable-template-form-dialog',

  standalone: true,

  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatExpansionModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],

  templateUrl: './timetable-template-form-dialog.html',

  styleUrl: './timetable-template-form-dialog.scss',

  changeDetection: ChangeDetectionStrategy.Eager,
})
export class TimetableTemplateFormDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly teacherAssignmentService = inject(TeacherAssignmentService);

  private readonly timetableTemplateService = inject(TimetableTemplateService);

  private readonly roomService = inject(RoomService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<TimetableTemplateFormDialog>);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public readonly data: TimetableTemplateDialogData,
  ) {}

  // =========================================================
  // State
  // =========================================================

  readonly teacherAssignments = signal<TeacherAssignment[]>([]);

  readonly rooms = signal<Lookup[]>([]);

  readonly academicSessions = signal<AcademicSessionOption[]>([]);

  readonly loading = signal(true);

  readonly saving = signal(false);

  // =========================================================
  // Options
  // =========================================================

  readonly lectureTypes = [
    { value: 1, label: 'Lecture' },
    { value: 2, label: 'Practical' },
    { value: 3, label: 'Laboratory' },
    { value: 4, label: 'Tutorial' },
    { value: 5, label: 'Seminar' },
    { value: 6, label: 'Workshop' },
    { value: 7, label: 'Project' },
    { value: 8, label: 'Viva' },
    { value: 9, label: 'Extra Class' },
    { value: 10, label: 'Revision' },
    { value: 11, label: 'Guest Lecture' },
  ];

  readonly daysOfWeek = [
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' },
    { value: 7, label: 'Sunday' },
  ];

  // =========================================================
  // Form
  // =========================================================

  readonly form = this.fb.nonNullable.group({
    academicSessionId: ['', Validators.required],

    teacherAssignmentId: ['', Validators.required],

    roomId: [''],

    /*
     * Always keep this as number[].
     *
     * Create:
     *   [1, 2, 3, 4]
     *
     * Edit:
     *   [1]
     */
    dayOfWeek: this.fb.control<number[]>([1], {
      validators: Validators.required,
      nonNullable: true,
    }),

    startTime: ['', Validators.required],

    endTime: ['', Validators.required],

    validFrom: this.fb.control<Date | null>(null, Validators.required),

    validTo: this.fb.control<Date | null>(null, Validators.required),

    lectureType: [1, Validators.required],

    priority: [100, [Validators.required, Validators.min(0)]],

    generateAttendance: [true],

    isOnline: [false],

    meetingLink: [''],

    remarks: [''],

    displayOrder: [1, [Validators.required, Validators.min(1)]],
  });

  // =========================================================
  // Getters
  // =========================================================

  get isEdit(): boolean {
    return this.data.mode === 'edit';
  }

  get isDuplicate(): boolean {
    return this.data.mode === 'duplicate';
  }

  get title(): string {
    if (this.isEdit) {
      return 'Edit Timetable';
    }

    if (this.isDuplicate) {
      return 'Duplicate Timetable';
    }

    return 'Create Timetable';
  }

  get submitLabel(): string {
    if (this.isEdit) {
      return 'Update Timetable';
    }

    if (this.isDuplicate) {
      return 'Create Timetable';
    }

    return 'Create Timetable';
  }

  // =========================================================
  // Lifecycle
  // =========================================================

  ngOnInit(): void {
    this.loadData();
  }

  // =========================================================
  // Loading
  // =========================================================

  private loadData(): void {
    this.loading.set(true);

    this.teacherAssignmentService.getAll().subscribe({
      next: (assignments) => {
        this.teacherAssignments.set(assignments);

        this.buildAcademicSessions(assignments);

        this.roomService.getLookup().subscribe({
          next: (rooms) => {
            this.rooms.set(rooms);

            this.populateEditData();

            this.loading.set(false);
          },

          error: () => {
            this.notificationService.error('Unable to load rooms.');

            this.loading.set(false);
          },
        });
      },

      error: () => {
        this.notificationService.error('Unable to load teacher assignments.');

        this.loading.set(false);
      },
    });
  }

  private buildAcademicSessions(assignments: TeacherAssignment[]): void {
    const map = new Map<string, AcademicSessionOption>();

    for (const assignment of assignments) {
      if (!map.has(assignment.academicSessionId)) {
        map.set(assignment.academicSessionId, {
          id: assignment.academicSessionId,
          name: assignment.academicSessionName,
        });
      }
    }

    this.academicSessions.set(
      Array.from(map.values()).sort((a, b) => a.name.localeCompare(b.name)),
    );
  }

  // =========================================================
  // Edit
  // =========================================================

  private populateEditData(): void {
    const timetable = this.data.timetable;

    /*
     * Create mode.
     */
    if (!timetable) {
      this.form.controls.dayOfWeek.setValue([1]);

      this.updateRoomValidation(this.form.controls.isOnline.value);

      return;
    }

    /*
     * Edit mode.
     *
     * The backend stores one DayOfWeekType value.
     *
     * The frontend control always stores number[] because
     * create mode supports multiple days.
     *
     * Therefore an existing Monday timetable becomes:
     *
     *     [1]
     *
     * Tuesday:
     *
     *     [2]
     */
    this.form.patchValue({
      academicSessionId: timetable.academicSessionId,

      teacherAssignmentId: timetable.teacherAssignmentId,

      roomId:
        timetable.roomId && timetable.roomId !== '00000000-0000-0000-0000-000000000000'
          ? timetable.roomId
          : '',

      dayOfWeek: [Number(timetable.dayOfWeek)],

      startTime: this.toTimeInputValue(timetable.startTime),

      endTime: this.toTimeInputValue(timetable.endTime),

      validFrom: this.toDateObject(timetable.validFrom),

      validTo: this.toDateObject(timetable.validTo),

      lectureType: Number(timetable.lectureType),

      priority: timetable.priority,

      generateAttendance: timetable.generateAttendance,

      isOnline: timetable.isOnline,

      meetingLink: timetable.meetingLink ?? '',

      remarks: timetable.remarks ?? '',

      displayOrder: timetable.displayOrder,
    });

    /*
     * An existing timetable cannot be converted into
     * multiple days by this dialog because the backend
     * UpdateTimetableTemplateRequest represents one
     * DayOfWeekType.
     */
    this.form.controls.dayOfWeek.disable();

    this.updateRoomValidation(timetable.isOnline);
  }

  // =========================================================
  // Teacher Assignment
  // =========================================================

  get selectedAssignment(): TeacherAssignment | null {
    const id = this.form.controls.teacherAssignmentId.value;

    return this.teacherAssignments().find((x) => x.id === id) ?? null;
  }

  get filteredAssignments(): TeacherAssignment[] {
    const sessionId = this.form.controls.academicSessionId.value;

    if (!sessionId) {
      return [];
    }

    return this.teacherAssignments().filter((x) => x.academicSessionId === sessionId);
  }

  onAcademicSessionChange(): void {
    const selectedAssignment = this.selectedAssignment;

    const sessionId = this.form.controls.academicSessionId.value;

    if (selectedAssignment && selectedAssignment.academicSessionId !== sessionId) {
      this.form.controls.teacherAssignmentId.setValue('');
    }
  }

  // =========================================================
  // Online / Offline
  // =========================================================

  onOnlineChange(): void {
    const isOnline = this.form.controls.isOnline.value;

    this.updateRoomValidation(isOnline);

    if (isOnline) {
      this.form.controls.roomId.setValue('');
    }
  }

  private updateRoomValidation(isOnline: boolean): void {
    const control = this.form.controls.roomId;

    if (isOnline) {
      control.clearValidators();
    } else {
      control.setValidators([Validators.required]);
    }

    control.updateValueAndValidity();
  }

  // =========================================================
  // Save
  // =========================================================

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    const value = this.form.getRawValue();

    const selectedDays = value.dayOfWeek;

    if (!selectedDays.length) {
      this.notificationService.error('Please select at least one day.');

      return;
    }

    if (!value.startTime || !value.endTime) {
      this.notificationService.error('Start time and end time are required.');

      return;
    }

    if (value.startTime >= value.endTime) {
      this.notificationService.error('End time must be greater than start time.');

      return;
    }

    if (!value.validFrom || !value.validTo) {
      this.notificationService.error('Valid From and Valid To are required.');

      return;
    }

    if (value.validFrom > value.validTo) {
      this.notificationService.error('Valid From cannot be greater than Valid To.');

      return;
    }

    if (value.isOnline && !value.meetingLink.trim()) {
      this.notificationService.error('Meeting link is required for online lectures.');

      return;
    }

    this.saving.set(true);

    const commonRequest = {
      teacherAssignmentId: value.teacherAssignmentId,

      academicSessionId: value.academicSessionId,

      roomId: value.roomId || null,

      startTime: this.toTimeApiValue(value.startTime),

      endTime: this.toTimeApiValue(value.endTime),

      validFrom: this.toDateApiValue(value.validFrom),

      validTo: this.toDateApiValue(value.validTo),

      lectureType: Number(value.lectureType),

      priority: value.priority,

      generateAttendance: value.generateAttendance,

      isOnline: value.isOnline,

      meetingLink: value.isOnline ? value.meetingLink.trim() || null : null,

      remarks: value.remarks.trim() || null,

      displayOrder: value.displayOrder,
    };

    // =======================================================
    // EDIT
    // =======================================================

    if (this.isEdit && this.data.timetable) {
      /*
       * Update endpoint accepts exactly one day.
       *
       * Since the edit form contains exactly one day,
       * selectedDays[0] is the correct value.
       */
      const request: UpdateTimetableTemplateRequest = {
        ...commonRequest,

        dayOfWeek: selectedDays[0],
      };

      this.update(this.data.timetable.id, request);

      return;
    }

    // =======================================================
    // CREATE
    // =======================================================

    const requests: CreateTimetableTemplateRequest[] = selectedDays.map((day) => ({
      ...commonRequest,

      dayOfWeek: day,
    }));

    this.createMultiple(requests);
  }

  // =========================================================
  // Create Multiple Days
  // =========================================================

  private createMultiple(requests: CreateTimetableTemplateRequest[]): void {
    if (!requests.length) {
      this.saving.set(false);

      return;
    }

    from(requests)
      .pipe(
        concatMap((request) => this.timetableTemplateService.create(request)),

        toArray(),
      )
      .subscribe({
        next: (timetables) => {
          this.notificationService.success(
            timetables.length === 1
              ? 'Timetable created successfully.'
              : `${timetables.length} timetable entries created successfully.`,
          );

          this.dialogRef.close(timetables);
        },

        error: (err) => {
          this.saving.set(false);

          this.notificationService.error(
            err?.error?.message ?? err?.message ?? 'Unable to create timetable.',
          );
        },
      });
  }

  // =========================================================
  // Update
  // =========================================================

  private update(id: string, request: UpdateTimetableTemplateRequest): void {
    this.timetableTemplateService.update(id, request).subscribe({
      next: (timetable) => {
        this.notificationService.success('Timetable updated successfully.');

        this.dialogRef.close(timetable);
      },

      error: (err) => {
        this.saving.set(false);

        this.notificationService.error(
          err?.error?.message ?? err?.message ?? 'Unable to update timetable.',
        );
      },
    });
  }

  // =========================================================
  // Formatting
  // =========================================================

  private toTimeInputValue(value: string): string {
    if (!value) {
      return '';
    }

    return value.substring(0, 5);
  }

  private toTimeApiValue(value: string): string {
    if (!value) {
      return '';
    }

    return value.length === 5 ? `${value}:00` : value;
  }

  private toDateObject(value: string): Date | null {
    if (!value) {
      return null;
    }

    const datePart = value.substring(0, 10);

    const [year, month, day] = datePart.split('-').map(Number);

    if (!year || !month || !day) {
      return null;
    }

    return new Date(year, month - 1, day);
  }

  private toDateApiValue(value: Date): string {
    const year = value.getFullYear();

    const month = String(value.getMonth() + 1).padStart(2, '0');

    const day = String(value.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  // =========================================================
  // Cancel
  // =========================================================

  cancel(): void {
    this.dialogRef.close(false);
  }
}
