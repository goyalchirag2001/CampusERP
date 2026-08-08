import {
  Component,
  Inject,
  OnInit,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { CampusService } from '../../campuses/services/campus';
import { DepartmentService } from '../../departments/services/department';
import { CourseService } from '../../courses/services/course';
import { SemesterService } from '../../semesters/services/semester';
import { SectionService } from '../services/section';

import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';

import { Course } from '../../courses/models/course';
import { Section } from '../models/section';

export interface SectionFormDialogData {
  isEdit: boolean;

  section?: Section;
}

@Component({
  selector: 'app-section-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './section-form-dialog.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './section-form-dialog.scss',
})
export class SectionFormDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly sectionService = inject(SectionService);

  private readonly campusService = inject(CampusService);

  private readonly departmentService = inject(DepartmentService);

  private readonly courseService = inject(CourseService);

  private readonly semesterService = inject(SemesterService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<SectionFormDialog>);

  readonly campuses = signal<Lookup[]>([]);

  readonly departments = signal<DepartmentLookup[]>([]);

  readonly courses = signal<Course[]>([]);

  readonly semesters = signal<Lookup[]>([]);

  readonly saving = signal(false);

  readonly isCampusAdmin = signal(false);

  readonly selectedCampusId = signal('');

  readonly selectedDepartmentId = signal('');

  readonly selectedCourseId = signal('');

  readonly isEdit = computed(() => this.data?.isEdit ?? false);

  readonly filteredDepartments = computed(() =>
    this.departments().filter((x) => x.campusId === this.selectedCampusId()),
  );

  readonly filteredCourses = computed(() =>
    this.courses().filter((x) => x.departmentId === this.selectedDepartmentId()),
  );

  form = this.fb.group({
    campusId: ['', Validators.required],

    departmentId: ['', Validators.required],

    courseId: ['', Validators.required],

    semesterId: ['', Validators.required],

    name: ['', Validators.required],

    capacity: [60, [Validators.required, Validators.min(1), Validators.max(500)]],
  });

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: SectionFormDialogData,
  ) {}

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.isCampusAdmin.set(user?.roles.includes('CampusAdmin') ?? false);

    this.departmentService.getLookupWithCampus().subscribe((x) => {
      this.departments.set(x);
    });

    this.courseService.getAll().subscribe((x) => {
      this.courses.set(x);
    });

    if (this.isCampusAdmin()) {
      const campusId = user?.campusId ?? '';

      this.form.patchValue({
        campusId,
      });

      this.selectedCampusId.set(campusId);

      this.form.controls.campusId.disable();
    } else {
      this.campusService.getLookup().subscribe((x) => {
        this.campuses.set(x);
      });
    }

    this.form.controls.campusId.valueChanges.subscribe((value) => {
      this.selectedCampusId.set(value ?? '');

      this.form.patchValue(
        {
          departmentId: '',
          courseId: '',
          semesterId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.selectedDepartmentId.set('');

      this.selectedCourseId.set('');

      this.semesters.set([]);
    });

    this.form.controls.departmentId.valueChanges.subscribe((value) => {
      this.selectedDepartmentId.set(value ?? '');

      this.form.patchValue(
        {
          courseId: '',
          semesterId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.selectedCourseId.set('');

      this.semesters.set([]);
    });

    this.form.controls.courseId.valueChanges.subscribe((value) => {
      this.selectedCourseId.set(value ?? '');

      this.form.patchValue(
        {
          semesterId: '',
        },
        {
          emitEvent: false,
        },
      );

      this.semesters.set([]);

      if (!value) {
        return;
      }

      this.semesterService.getLookupByCourse(value).subscribe((x) => {
        this.semesters.set(x);
      });
    });

    if (!this.isEdit()) {
      return;
    }

    const section = this.data.section!;

    this.selectedCampusId.set(section.campusId);

    this.selectedDepartmentId.set(section.departmentId);

    this.selectedCourseId.set(section.courseId);

    this.semesterService.getLookupByCourse(section.courseId).subscribe((x) => {
      this.semesters.set(x);

      this.form.patchValue({
        campusId: section.campusId,
        departmentId: section.departmentId,
        courseId: section.courseId,
        semesterId: section.semesterId,
        name: section.name,
        capacity: section.capacity,
      });
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.saving.set(true);

    const request = this.isEdit()
      ? this.sectionService.update(this.data.section!.id, {
          name: this.form.value.name ?? '',

          capacity: this.form.value.capacity ?? 0,
        })
      : this.sectionService.create({
          semesterId: this.form.value.semesterId ?? '',

          name: this.form.value.name ?? '',

          capacity: this.form.value.capacity ?? 0,
        });

    request.subscribe({
      next: (section) => {
        this.notificationService.success(
          this.isEdit() ? 'Section updated successfully.' : 'Section created successfully.',
        );

        this.dialogRef.close(section);
      },

      error: (err) => {
        this.saving.set(false);

        this.notificationService.error(err?.error?.message ?? 'Operation failed.');
      },
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}
