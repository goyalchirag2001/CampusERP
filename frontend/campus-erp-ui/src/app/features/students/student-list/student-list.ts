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
import { StudentCreateDialog } from '../student-create-dialog/student-create-dialog';

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

  displayedColumns = ['name', 'rollNumber', 'course', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredStudents = computed(() =>
    this.students().filter(
      (x) =>
        `${x.firstName} ${x.lastName}`.toLowerCase().includes(this.search().toLowerCase()) ||
        x.rollNumber.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedStudents = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    return this.filteredStudents().slice(start, start + this.pageSize());
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
      .open(StudentCreateDialog, {
        width: '800px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
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
}
