import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { TeacherService } from '../services/teacher';
import { Teacher } from '../models/teacher';
import { TeacherEditDialog } from '../teacher-edit-dialog/teacher-edit-dialog';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-teacher-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './teacher-details.html',
  styleUrl: './teacher-details.scss',
})
export class TeacherDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly teacherService = inject(TeacherService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly teacher = signal<Teacher | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.teacherService.getById(id).subscribe((data) => {
      this.teacher.set(data);
    });
  }

  editTeacher(): void {
    const teacher = this.teacher();

    if (!teacher) {
      return;
    }

    this.dialog
      .open(TeacherEditDialog, {
        width: '700px',
        maxWidth: '95vw',
        data: teacher,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.teacher.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const teacher = this.teacher();

    if (!teacher) {
      return;
    }

    const request = teacher.isActive
      ? this.teacherService.deactivate(teacher.id)
      : this.teacherService.activate(teacher.id);

    request.subscribe({
      next: () => {
        this.teacher.update((current) => {
          if (!current) {
            return current;
          }

          return {
            ...current,
            isActive: !current.isActive,
          };
        });

        this.notificationService.success(
          teacher.isActive ? 'Teacher deactivated.' : 'Teacher activated.',
        );
      },
    });
  }
}
