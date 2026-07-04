import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';

import { CourseService } from '../services/course';
import { SubjectService } from '../../subjects/services/subject';

import { Course } from '../models/course';
import { Subject } from '../../subjects/models/subject';

import { CourseEditDialog } from '../course-edit-dialog/course-edit-dialog';
import { SemesterCard } from '../semester-card/semester-card';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [MatButtonModule, SemesterCard],
  templateUrl: './course-details.html',
  styleUrl: './course-details.scss',
})
export class CourseDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly dialog = inject(MatDialog);

  private readonly courseService = inject(CourseService);

  private readonly subjectService = inject(SubjectService);

  private readonly notificationService = inject(NotificationService);

  readonly course = signal<Course | null>(null);

  readonly subjects = signal<Subject[]>([]);

  readonly loading = signal(true);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.subjectService.getAll().subscribe((subjects) => {
      this.subjects.set(subjects.filter((x) => x.isActive));
    });

    this.courseService.getById(id).subscribe({
      next: (course) => {
        this.course.set(course);

        this.loading.set(false);
      },

      error: () => {
        this.loading.set(false);
      },
    });
  }

  editCourse(): void {
    const course = this.course();

    if (!course) {
      return;
    }

    this.dialog
      .open(CourseEditDialog, {
        width: '750px',
        maxWidth: '95vw',
        data: course,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.course.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const course = this.course();

    if (!course) {
      return;
    }

    const request = course.isActive
      ? this.courseService.deactivate(course.id)
      : this.courseService.activate(course.id);

    request.subscribe({
      next: () => {
        this.course.update((current) => {
          if (!current) {
            return current;
          }

          return {
            ...current,

            isActive: !current.isActive,
          };
        });

        this.notificationService.success(
          course.isActive ? 'Course deactivated.' : 'Course activated.',
        );
      },
    });
  }
}
