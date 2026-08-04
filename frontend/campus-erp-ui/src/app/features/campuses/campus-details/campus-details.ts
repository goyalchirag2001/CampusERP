import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { CampusService } from '../services/campus';
import { Campus } from '../models/campus';
import { CampusEditDialog } from '../campus-edit-dialog/campus-edit-dialog';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-campus-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './campus-details.html',
  styleUrl: './campus-details.scss',
})
export class CampusDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly campusService = inject(CampusService);

  private readonly dialog = inject(MatDialog);

  readonly campus = signal<Campus | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.campusService.getById(id).subscribe((data) => this.campus.set(data));
  }

  editCampus(): void {
    const campus = this.campus();

    if (!campus) {
      return;
    }

    this.dialog
      .open(CampusEditDialog, {
        width: '700px',
        maxWidth: '95vw',
        maxHeight: '90vh',
        data: campus,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.campus.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const campus = this.campus();

    if (!campus) {
      return;
    }

    const request = campus.isActive
      ? this.campusService.deactivate(campus.id)
      : this.campusService.activate(campus.id);

    request.subscribe(() => {
      this.campus.update((current) => {
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
