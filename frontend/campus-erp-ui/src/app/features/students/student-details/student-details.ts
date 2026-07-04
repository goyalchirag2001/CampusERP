import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { StudentService } from '../services/student';
import { Student } from '../models/student';
import { StudentEditDialog } from '../student-edit-dialog/student-edit-dialog';
import { NotificationService } from '../../../core/services/notification';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-student-details',
  standalone: true,
  imports: [MatButtonModule, DatePipe],
  templateUrl: './student-details.html',
  styleUrl: './student-details.scss',
})
export class StudentDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly studentService = inject(StudentService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly student = signal<Student | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.studentService.getById(id).subscribe((data) => {
      this.student.set(data);
    });
  }

  editStudent(): void {
    const student = this.student();

    if (!student) {
      return;
    }

    this.dialog
      .open(StudentEditDialog, {
        width: '700px',
        maxWidth: '95vw',
        data: student,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.student.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const student = this.student();

    if (!student) {
      return;
    }

    const request = student.isActive
      ? this.studentService.deactivate(student.id)
      : this.studentService.activate(student.id);

    request.subscribe({
      next: () => {
        this.student.update((current) => {
          if (!current) {
            return current;
          }

          return {
            ...current,
            isActive: !current.isActive,
          };
        });

        this.notificationService.success(
          student.isActive ? 'Student deactivated.' : 'Student activated.',
        );
      },
    });
  }
}
