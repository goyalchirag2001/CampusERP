import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';

import { NotificationService } from '../../../core/services/notification';

import { Section } from '../models/section';
import { SectionService } from '../services/section';
import { SectionFormDialog } from '../section-form-dialog/section-form-dialog';

@Component({
  selector: 'app-section-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './section-details.html',
  styleUrl: './section-details.scss',
})
export class SectionDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly service = inject(SectionService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly section = signal<Section | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.service.getById(id).subscribe((data) => {
      this.section.set(data);
    });
  }

  edit(): void {
    const section = this.section();

    if (!section) {
      return;
    }

    this.dialog
      .open(SectionFormDialog, {
        width: '900px',
        maxWidth: '95vw',
        data: {
          isEdit: true,
          section,
        },
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.section.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const section = this.section();

    if (!section) {
      return;
    }

    const request = section.isActive
      ? this.service.deactivate(section.id)
      : this.service.activate(section.id);

    request.subscribe({
      next: () => {
        this.section.update((current) => {
          if (!current) {
            return current;
          }

          return {
            ...current,
            isActive: !current.isActive,
          };
        });

        this.notificationService.success(
          section.isActive ? 'Section deactivated.' : 'Section activated.',
        );
      },

      error: (err) => {
        this.notificationService.error(err?.error?.message ?? 'Operation failed.');
      },
    });
  }
}
