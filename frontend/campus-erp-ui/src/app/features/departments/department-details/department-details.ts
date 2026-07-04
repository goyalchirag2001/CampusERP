import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentService } from '../services/department';
import { Department } from '../models/department';
import { DepartmentEditDialog } from '../department-edit-dialog/department-edit-dialog';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-department-details',
  standalone: true,
  imports: [MatButtonModule],
  templateUrl: './department-details.html',
  styleUrl: './department-details.scss',
})
export class DepartmentDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly departmentService = inject(DepartmentService);

  private readonly dialog = inject(MatDialog);

  private readonly notificationService = inject(NotificationService);

  readonly department = signal<Department | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.departmentService.getById(id).subscribe((data) => this.department.set(data));
  }

  editDepartment(): void {
    const department = this.department();

    if (!department) {
      return;
    }

    this.dialog
      .open(DepartmentEditDialog, {
        width: '600px',
        maxWidth: '95vw',
        data: department,
      })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.department.set(updated);
        }
      });
  }

  toggleStatus(): void {
    const department = this.department();

    if (!department) {
      return;
    }

    const request = department.isActive
      ? this.departmentService.deactivate(department.id)
      : this.departmentService.activate(department.id);

    request.subscribe({
      next: () => {
        this.department.update((current) => {
          if (!current) {
            return current;
          }

          return {
            ...current,
            isActive: !current.isActive,
          };
        });

        this.notificationService.success(
          department.isActive ? 'Department deactivated.' : 'Department activated.',
        );
      },
    });
  }
}
