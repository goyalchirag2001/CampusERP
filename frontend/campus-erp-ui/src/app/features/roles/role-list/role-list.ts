import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { RoleService } from '../services/role';
import { Role } from '../../../core/models/role';
import { CurrentUserService } from '../../../core/services/current-user';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatTooltipModule
  ],
  templateUrl: './role-list.html',
  styleUrl: './role-list.scss',
})
export class RoleList implements OnInit {
  private readonly roleService = inject(RoleService);

  private readonly router = inject(Router);

  private readonly currentUserService = inject(CurrentUserService);

  readonly roles = signal<Role[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = ['name', 'permissions', 'type', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredRoles = computed(() =>
    this.roles().filter((x) => x.name.toLowerCase().includes(this.search().toLowerCase())),
  );

  readonly pagedRoles = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredRoles().slice(start, end);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.roleService.getAll().subscribe((data) => this.roles.set(data));
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createRole(): void {
    this.router.navigate([this.baseRoute, 'roles', 'new']);
  }

  openRole(id: string): void {
    this.router.navigate([this.baseRoute, 'roles', id]);
  }
}
