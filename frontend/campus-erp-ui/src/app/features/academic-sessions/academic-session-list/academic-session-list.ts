import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CurrentUserService } from '../../../core/services/current-user';

import { AcademicSession } from '../models/academic-session';
import { AcademicSessionService } from '../services/academic-session';
import { AcademicSessionFormDialog } from '../academic-session-form-dialog/academic-session-form-dialog';
import { DatePipe } from '@angular/common';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-academic-session-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatInputModule,
    MatPaginatorModule,
    MatTableModule,
    MatTooltipModule,
    DatePipe,
  ],
  templateUrl: './academic-session-list.html',
  styleUrl: './academic-session-list.scss',
})
export class AcademicSessionList implements OnInit {
  private readonly service = inject(AcademicSessionService);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  private readonly currentUserService = inject(CurrentUserService);

  readonly sessions = signal<AcademicSession[]>([]);

  private readonly notificationService = inject(NotificationService);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = ['name', 'campus', 'duration', 'current', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredSessions = computed(() =>
    this.sessions().filter(
      (x) =>
        x.name.toLowerCase().includes(this.search().toLowerCase()) ||
        x.campusName.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedSessions = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    return this.filteredSessions().slice(start, start + this.pageSize());
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.service.getAll().subscribe((data) => {
      this.sessions.set(data);
    });
  }

  create(): void {
    this.dialog
      .open(AcademicSessionFormDialog, {
        width: '900px',
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

  edit(session: AcademicSession): void {
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
          this.load();
        }
      });
  }

  setCurrent(session: AcademicSession): void {
    this.service.setCurrent(session.id).subscribe({
      next: () => {
        this.notificationService.success(`"${session.name}" is now the current academic session.`);

        this.load();
      },

      error: (err) => {
        this.notificationService.error(err?.error?.message ?? 'Failed to set current session.');
      },
    });
  }

  toggleStatus(session: AcademicSession): void {
    const request = session.isActive
      ? this.service.deactivate(session.id)
      : this.service.activate(session.id);

    request.subscribe({
      next: () => {
        this.notificationService.success(
          session.isActive ? 'Academic session deactivated.' : 'Academic session activated.',
        );

        this.load();
      },

      error: (err) => {
        this.notificationService.error(err?.error?.message ?? 'Operation failed.');
      },
    });
  }

  open(id: string): void {
    this.router.navigate([this.baseRoute, 'academic-sessions', id]);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }
}
