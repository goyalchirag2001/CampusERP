import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';

import { DatePipe } from '@angular/common';

import { AcademicSessionService } from '../services/academic-session';
import { AcademicSession } from '../models/academic-session';
import { AcademicSessionFormDialog } from '../academic-session-form-dialog/academic-session-form-dialog';

import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-academic-session-details',
  standalone: true,
  imports: [MatButtonModule, DatePipe],
  templateUrl: './academic-session-details.html',
  styleUrl: './academic-session-details.scss',
})
export class AcademicSessionDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly service = inject(AcademicSessionService);

  private readonly dialog = inject(MatDialog);

  private readonly notification = inject(NotificationService);

  readonly session = signal<AcademicSession | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.service.getById(id).subscribe((x) => {
      this.session.set(x);
    });
  }

  edit(): void {
    const session = this.session();

    if (!session) {
      return;
    }

    this.dialog
      .open(AcademicSessionFormDialog, {
        width: '900px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: {
          isEdit: true,
          academicSession: session,
        },
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.session.set(updated);
        }
      });
  }

  activate(): void {
    const session = this.session();

    if (!session) {
      return;
    }

    this.service.activate(session.id).subscribe(() => {
      this.session.update((x) => (x ? { ...x, isActive: true } : x));

      this.notification.success('Academic session activated.');
    });
  }

  deactivate(): void {
    const session = this.session();

    if (!session) {
      return;
    }

    this.service.deactivate(session.id).subscribe(() => {
      this.session.update((x) => (x ? { ...x, isActive: false } : x));

      this.notification.success('Academic session deactivated.');
    });
  }

  makeCurrent(): void {
    const session = this.session();

    if (!session) {
      return;
    }

    this.service.setCurrent(session.id).subscribe(() => {
      this.session.update((x) => (x ? { ...x, isCurrent: true } : x));

      this.notification.success('Current academic session updated.');
    });
  }
}
