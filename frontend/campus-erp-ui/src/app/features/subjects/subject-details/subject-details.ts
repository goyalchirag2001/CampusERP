import { Component, OnInit, inject, signal } from '@angular/core';

import { ActivatedRoute } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';

import { MatDialog } from '@angular/material/dialog';

import { SubjectService } from '../services/subject';

import { Subject } from '../models/subject';

import { SubjectEditDialog } from '../subject-edit-dialog/subject-edit-dialog';

import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-subject-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './subject-details.html',
  styleUrl: './subject-details.scss',
})
export class SubjectDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly subjectService = inject(SubjectService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly subject = signal<Subject | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.subjectService.getById(id).subscribe((data) => {
      this.subject.set(data);
    });
  }

  getTypeName(type: number): string {
    switch (type) {
      case 1:
        return 'Core';
      case 2:
        return 'Elective';
      case 3:
        return 'Laboratory';
      case 4:
        return 'Project';
      default:
        return '-';
    }
  }

  editSubject(): void {
    const subject = this.subject();

    if (!subject) {
      return;
    }

    this.dialog
      .open(SubjectEditDialog, {
        width: '650px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: subject,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.subject.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const subject = this.subject();

    if (!subject) {
      return;
    }

    const request = subject.isActive
      ? this.subjectService.deactivate(subject.id)
      : this.subjectService.activate(subject.id);

    request.subscribe({
      next: () => {
        this.subject.update((current) => {
          if (!current) {
            return current;
          }

          return {
            ...current,
            isActive: !current.isActive,
          };
        });

        this.notificationService.success(
          subject.isActive ? 'Subject deactivated.' : 'Subject activated.',
        );
      },
    });
  }
}
