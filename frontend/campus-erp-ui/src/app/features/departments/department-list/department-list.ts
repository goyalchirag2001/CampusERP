import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { CurrentUserService } from '../../../core/services/current-user';
import { DepartmentService } from '../services/department';
import { Department } from '../models/department';
import { DepartmentCreateDialog } from '../department-create/department-create-dialog';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-department-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatDialogModule,
  ],
  templateUrl: './department-list.html',
  styleUrl: './department-list.scss',
})
export class DepartmentList implements OnInit {
  private readonly departmentService = inject(DepartmentService);

  private readonly router = inject(Router);

  private readonly dialog = inject(MatDialog);

  private readonly currentUserService = inject(CurrentUserService);

  readonly departments = signal<Department[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = ['name', 'code', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredDepartments = computed(() =>
    this.departments().filter(
      (x) =>
        x.name.toLowerCase().includes(this.search().toLowerCase()) ||
        x.code.toLowerCase().includes(this.search().toLowerCase()),
    ),
  );

  readonly pagedDepartments = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredDepartments().slice(start, end);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.departmentService.getAll().subscribe((data) => {
      this.departments.set(data);
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createDepartment(): void {
    this.dialog
      .open(DepartmentCreateDialog, {
        width: '600px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.load();
        }
      });
  }

  openDepartment(id: string): void {
    this.router.navigate([this.baseRoute, 'departments', id]);
  }
}
