import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CurrentUserService } from '../../../core/services/current-user';
import { CourseService } from '../services/course';
import { Course } from '../models/course';
import { CourseCreateDialog } from '../course-create-dialog/course-create-dialog';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-course-list',
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
  templateUrl: './course-list.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './course-list.scss',
})
export class CourseList implements OnInit {
  private readonly courseService = inject(CourseService);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  private readonly currentUserService = inject(CurrentUserService);

  readonly courses = signal<Course[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = [
    'name',
    'department',
    'degreeType',
    'duration',
    'semesters',
    'status',
    'actions',
  ];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredCourses = computed(() =>
    this.courses().filter(
      (x) =>
        x.name.toLowerCase().includes(this.search().toLowerCase()) ||
        x.code.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedCourses = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredCourses().slice(start, end);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.courseService.getAll().subscribe((data) => {
      this.courses.set(data);
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createCourse(): void {
    this.dialog
      .open(CourseCreateDialog, {
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

  openCourse(id: string): void {
    this.router.navigate([this.baseRoute, 'courses', id]);
  }
}
