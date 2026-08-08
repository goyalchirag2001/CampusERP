import { Component, OnInit, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';

import { InstitutionService } from '../services/institution';
import { MatDialog } from '@angular/material/dialog';
import { Institution } from '../../../core/models/institution';
import { InstitutionEditDialog } from '../institution-edit-dialog/institution-edit-dialog';

@Component({
  selector: 'app-institution-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './institution-details.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './institution-details.scss',
})
export class InstitutionDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  private readonly institutionService = inject(InstitutionService);

  readonly institution = signal<Institution | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.institutionService.getById(id).subscribe((data) => this.institution.set(data));
  }

  get loginUrl(): string {
    const slug = this.institution()?.loginSlug;

    if (!slug) {
      return '';
    }

    return `http://localhost:4200/${slug}/login`;
  }

  editInstitution(): void {
    const institution = this.institution();

    if (!institution) {
      return;
    }

    const dialogRef = this.dialog.open(InstitutionEditDialog, {
      width: '800px',
      data: institution,
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        this.institution.set(updated);
      }
    });
  }

  copyLoginUrl(): void {
    navigator.clipboard.writeText(this.loginUrl);

    alert('Portal URL copied');
  }

  toggleStatus(): void {
    const institution = this.institution();

    if (!institution) {
      return;
    }

    const request = institution.isActive
      ? this.institutionService.deactivate(institution.id)
      : this.institutionService.activate(institution.id);

    request.subscribe(() => {
      this.institution.update((current) => {
        if (!current) {
          return current;
        }

        return {
          ...current,
          isActive: !current.isActive,
        };
      });
    });
  }
}
