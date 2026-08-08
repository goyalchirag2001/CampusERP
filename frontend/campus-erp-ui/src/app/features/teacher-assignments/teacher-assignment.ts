import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { TeacherAssignmentService } from './services/teacher-assignment';
import { TeacherService } from '../teachers/services/teacher';
import { AcademicSessionService } from '../academic-sessions/services/academic-session';
import { SectionService } from '../sections/services/section';
import { SemesterSubjectService } from '../semester-subjects/services/semester-subject';
import { NotificationService } from '../../core/services/notification';
import { Lookup } from '../../core/models/lookup';
import { CreateTeacherAssignmentRequest } from './models/create-teacher-assignment-request';
import { UpdateTeacherAssignmentRequest } from './models/update-teacher-assignment-request';
import { TeacherAssignment } from './models/teacher-assignment';

interface SubjectAssignmentRow {
  semesterSubjectId: string;

  subjectName: string;

  teacherId: string | null;

  teacherName: string | null;

  assignmentId: string | null;

  dirty: boolean;
}

@Component({
  selector: 'app-teacher-assignment',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatTableModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './teacher-assignment.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './teacher-assignment.scss',
})
export class TeacherAssignmentComponent implements OnInit {
  private readonly teacherAssignmentService = inject(TeacherAssignmentService);

  private readonly teacherService = inject(TeacherService);

  private readonly academicSessionService = inject(AcademicSessionService);

  private readonly sectionService = inject(SectionService);

  private readonly semesterSubjectService = inject(SemesterSubjectService);

  private readonly notificationService = inject(NotificationService);

  readonly academicSessions = signal<Lookup[]>([]);

  readonly sections = signal<Lookup[]>([]);

  readonly teachers = signal<Lookup[]>([]);

  readonly semesterSubjects = signal<Lookup[]>([]);

  readonly assignments = signal<TeacherAssignment[]>([]);

  readonly rows = signal<SubjectAssignmentRow[]>([]);

  readonly loading = signal(false);

  readonly saving = signal(false);

  readonly selectedAcademicSession = signal('');

  readonly selectedSection = signal('');

  readonly displayedColumns = ['subject', 'teacher'];

  readonly hasChanges = computed(() => this.rows().some((x) => x.dirty));

  ngOnInit(): void {
    this.loadLookups();
  }

  private loadLookups(): void {
    this.loading.set(true);

    forkJoin({
      academicSessions: this.academicSessionService.getLookup(),

      sections: this.sectionService.getLookup(),

      teachers: this.teacherService.getLookup(),
    }).subscribe({
      next: (result) => {
        this.academicSessions.set(result.academicSessions);

        this.sections.set(result.sections);

        this.teachers.set(result.teachers);

        if (result.academicSessions.length > 0) {
          this.selectedAcademicSession.set(result.academicSessions[0].id);
        }

        if (result.sections.length > 0) {
          this.selectedSection.set(result.sections[0].id);
        }

        if (this.selectedAcademicSession() && this.selectedSection()) {
          this.onSectionChanged();
        }
      },

      error: () => {
        this.notificationService.error('Unable to load required data.');

        this.loading.set(false);
      },

      complete: () => {
        this.loading.set(false);
      },
    });
  }

  onAcademicSessionChanged(): void {
    if (!this.selectedSection()) return;

    this.loadAssignments();
  }

  onSectionChanged(): void {
    if (!this.selectedSection()) return;

    this.loading.set(true);

    this.semesterSubjectService.getLookupBySection(this.selectedSection()).subscribe({
      next: (subjects) => {
        this.semesterSubjects.set(subjects);

        this.loadAssignments();
      },

      error: () => {
        this.loading.set(false);

        this.notificationService.error('Unable to load subjects.');
      },
    });
  }

  private loadAssignments(): void {
    this.teacherAssignmentService.getAll().subscribe({
      next: (assignments) => {
        const filteredAssignments = assignments.filter(
          (x) =>
            x.sectionId === this.selectedSection() &&
            x.academicSessionId === this.selectedAcademicSession(),
        );

        this.assignments.set(filteredAssignments);

        this.buildRows();

        this.loading.set(false);
      },

      error: () => {
        this.loading.set(false);

        this.notificationService.error('Unable to load assignments.');
      },
    });
  }

  private buildRows(): void {
    const assignmentMap = new Map(this.assignments().map((x) => [x.semesterSubjectId, x]));

    const rows: SubjectAssignmentRow[] = [];

    for (const subject of this.semesterSubjects()) {
      const assignment = assignmentMap.get(subject.id);

      rows.push({
        semesterSubjectId: subject.id,

        subjectName: subject.name,

        assignmentId: assignment?.id ?? null,

        teacherId: assignment?.teacherId ?? null,

        teacherName: assignment?.teacherName ?? null,

        dirty: false,
      });
    }

    this.rows.set(rows);
  }

  teacherChanged(row: SubjectAssignmentRow, teacherId: string): void {
    const teacher = this.teachers().find((x) => x.id === teacherId);

    row.teacherId = teacherId;

    row.teacherName = teacher?.name ?? '';

    row.dirty = true;

    this.rows.update((x) => [...x]);
  }

  save(): void {
    const createRequests: CreateTeacherAssignmentRequest[] = [];

    const updateRequests: {
      id: string;
      request: UpdateTeacherAssignmentRequest;
    }[] = [];

    for (const row of this.rows()) {
      if (!row.dirty) continue;

      if (!row.teacherId) continue;

      if (row.assignmentId) {
        updateRequests.push({
          id: row.assignmentId,

          request: {
            teacherId: row.teacherId,

            semesterSubjectId: row.semesterSubjectId,

            sectionId: this.selectedSection(),

            academicSessionId: this.selectedAcademicSession(),
          },
        });
      } else {
        createRequests.push({
          teacherId: row.teacherId,

          semesterSubjectId: row.semesterSubjectId,

          sectionId: this.selectedSection(),

          academicSessionId: this.selectedAcademicSession(),
        });
      }
    }

    if (createRequests.length === 0 && updateRequests.length === 0) {
      this.notificationService.warning('No changes found.');

      return;
    }

    this.saving.set(true);

    const requests = [
      ...createRequests.map((x) => this.teacherAssignmentService.create(x)),

      ...updateRequests.map((x) => this.teacherAssignmentService.update(x.id, x.request)),
    ];

    forkJoin(requests).subscribe({
      next: () => {
        this.notificationService.success('Teacher assignments saved successfully.');

        this.loadAssignments();
      },

      error: () => {
        this.notificationService.error('Unable to save teacher assignments.');

        this.saving.set(false);
      },

      complete: () => {
        this.saving.set(false);
      },
    });
  }

  trackBySubject(index: number, row: SubjectAssignmentRow): string {
    return row.semesterSubjectId;
  }
}
