import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { TeacherService } from '../services/teacher';
import { Teacher } from '../models/teacher';
import { TeacherCreateDialog } from '../teacher-create-dialog/teacher-create-dialog';
import { CurrentUserService } from '../../../core/services/current-user';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-teacher-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatDialogModule,
    MatTooltipModule
  ],
  templateUrl: './teacher-list.html',
  styleUrl: './teacher-list.scss',
})
export class TeacherList implements OnInit {
  private readonly teacherService = inject(TeacherService);

  private readonly dialog = inject(MatDialog);

  private readonly router = inject(Router);

  private readonly currentUserService = inject(CurrentUserService);

  readonly teachers = signal<Teacher[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  readonly isInstitutionAdmin = signal(false);

  displayedColumns: string[] = [];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredTeachers = computed(() =>
    this.teachers().filter(
      (x) =>
        x.firstName.toLowerCase().includes(this.search().toLowerCase()) ||
        x.lastName.toLowerCase().includes(this.search().toLowerCase()) ||
        x.employeeCode.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedTeachers = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    return this.filteredTeachers().slice(start, start + this.pageSize());
  });

  ngOnInit(): void {
    const user = this.currentUserService.user();

    this.isInstitutionAdmin.set(user?.roles.includes('InstitutionAdmin') ?? false);

    this.displayedColumns = this.isInstitutionAdmin()
      ? ['employeeCode', 'name', 'campus', 'department', 'designation', 'status', 'actions']
      : ['employeeCode', 'name', 'department', 'designation', 'status', 'actions'];

    this.load();
  }

  load(): void {
    this.teacherService.getAll().subscribe((data) => {
      this.teachers.set(data);
    });
  }

  createTeacher(): void {
    this.dialog
      .open(TeacherCreateDialog, {
        width: '700px',
        maxWidth: '95vw',
        maxHeight: '90vh',
      })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.load();
        }
      });
  }

  openTeacher(id: string): void {
    this.router.navigate([this.baseRoute, 'teachers', id]);
  }

  onPageChange(event: PageEvent): void {
    this.pageSize.set(event.pageSize);

    this.pageIndex.set(event.pageIndex);
  }
}
