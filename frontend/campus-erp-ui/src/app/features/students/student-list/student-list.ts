import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CurrentUserService } from '../../../core/services/current-user';
import { Student } from '../models/student';
import { StudentService } from '../services/student';
import { MatTooltipModule } from '@angular/material/tooltip';
import { StudentFormDialog } from '../student-form-dialog/student-form-dialog';
import { StudentImportDialog } from '../student-import-dialog/student-import-dialog';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatDialogModule,
    MatTooltipModule,
  ],
  templateUrl: './student-list.html',
  styleUrl: './student-list.scss',
})
export class StudentList implements OnInit {
  private readonly studentService = inject(StudentService);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  private readonly currentUserService = inject(CurrentUserService);

  readonly students = signal<Student[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  readonly sortColumn = signal<string>('rollNumber');

  readonly sortDirection = signal<'asc' | 'desc'>('asc');

  displayedColumns = [
    'name',
    'admissionNumber',
    'rollNumber',
    'course',
    'semester',
    'section',
    'status',
    'actions',
  ];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredStudents = computed(() => {
    const search = this.search().trim().toLowerCase();

    if (!search) {
      return this.students();
    }

    return this.students().filter((student) =>
      [
        student.firstName,
        student.lastName,
        student.email,
        student.admissionNumber,
        student.rollNumber,
        student.courseName,
        student.departmentName,
        student.semesterName,
        student.sectionName,
      ]
        .filter(Boolean)
        .some((value) => value.toLowerCase().includes(search)),
    );
  });

  readonly pagedStudents = computed(() => {
    const students = [...this.filteredStudents()];

    const direction = this.sortDirection() === 'asc' ? 1 : -1;

    students.sort((a, b) => {
      switch (this.sortColumn()) {
        case 'name': {
          const nameA = `${a.firstName} ${a.lastName}`.toLowerCase();

          const nameB = `${b.firstName} ${b.lastName}`.toLowerCase();

          return nameA.localeCompare(nameB) * direction;
        }

        case 'admissionNumber':
          return (
            a.admissionNumber.localeCompare(b.admissionNumber, undefined, {
              numeric: true,
              sensitivity: 'base',
            }) * direction
          );

        case 'rollNumber':
          return (
            a.rollNumber.localeCompare(b.rollNumber, undefined, {
              numeric: true,
              sensitivity: 'base',
            }) * direction
          );

        case 'course':
          return a.courseName.localeCompare(b.courseName) * direction;

        case 'semester':
          return (
            a.semesterName.localeCompare(b.semesterName, undefined, {
              numeric: true,
            }) * direction
          );

        case 'section':
          return a.sectionName.localeCompare(b.sectionName) * direction;

        case 'status':
          return (Number(a.isActive) - Number(b.isActive)) * direction;

        default:
          return 0;
      }
    });

    const start = this.pageIndex() * this.pageSize();

    return students.slice(start, start + this.pageSize());
  });
  
  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.studentService.getAll().subscribe((data) => {
      this.students.set(data);
    });
  }

  createStudent(): void {
    this.dialog
      .open(StudentFormDialog, {
        width: '950px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: {
          mode: 'create',
        },
      })
      .afterClosed()
      .subscribe((saved) => {
        if (saved) {
          this.load();
        }
      });
  }

  importStudents(): void {
    this.dialog
      .open(StudentImportDialog, {
        width: '1200px',
        maxWidth: '95vw',
        disableClose: true,
      })
      .afterClosed()
      .subscribe((imported) => {
        if (imported) {
          this.load();
        }
      });
  }

  openStudent(id: string): void {
    this.router.navigate([this.baseRoute, 'students', id]);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  sort(column: string): void {
    if (this.sortColumn() === column) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortColumn.set(column);

      this.sortDirection.set('asc');
    }
  }
}
