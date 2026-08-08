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
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';

import { NotificationService } from '../../../core/services/notification';
import { CurrentUserService } from '../../../core/services/current-user';

import { Section } from '../models/section';

import { SectionService } from '../services/section';

import {
  SectionFormDialog,
  SectionFormDialogData,
} from '../section-form-dialog/section-form-dialog';

@Component({
  selector: 'app-section-list',
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
  ],
  templateUrl: './section-list.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './section-list.scss',
})
export class SectionList implements OnInit {
  private readonly sectionService = inject(SectionService);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  readonly sections = signal<Section[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = ['name', 'semester', 'course', 'department', 'capacity', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredSections = computed(() => {
    const keyword = this.search().toLowerCase();

    return this.sections().filter(
      (x) =>
        x.name.toLowerCase().includes(keyword) ||
        x.departmentName.toLowerCase().includes(keyword) ||
        x.courseName.toLowerCase().includes(keyword) ||
        x.semesterName.toLowerCase().includes(keyword),
    );
  });

  readonly pagedSections = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    return this.filteredSections().slice(start, start + this.pageSize());
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.sectionService.getAll().subscribe((data) => {
      this.sections.set(data);
    });
  }

  create(): void {
    this.dialog
      .open(SectionFormDialog, {
        width: '900px',
        maxWidth: '95vw',
        data: {
          isEdit: false,
        } satisfies SectionFormDialogData,
      })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.load();
        }
      });
  }

  edit(section: Section): void {
    this.dialog
      .open(SectionFormDialog, {
        width: '900px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: {
          isEdit: true,
          section,
        } satisfies SectionFormDialogData,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.load();
        }
      });
  }

  toggleStatus(section: Section): void {
    const request = section.isActive
      ? this.sectionService.deactivate(section.id)
      : this.sectionService.activate(section.id);

    request.subscribe({
      next: () => {
        this.notificationService.success(
          section.isActive
            ? 'Section deactivated successfully.'
            : 'Section activated successfully.',
        );

        this.load();
      },

      error: (err) => {
        this.notificationService.error(err?.error?.message ?? 'Operation failed.');
      },
    });
  }

  open(id: string): void {
    this.router.navigate([this.baseRoute, 'sections', id]);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }
}
