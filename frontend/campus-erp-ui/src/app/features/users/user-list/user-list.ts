import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { UserService } from '../services/user';
import { User } from '../../../core/models/user';
import { CurrentUserService } from '../../../core/services/current-user';
import { NotificationService } from '../../../core/services/notification';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
  ],
  templateUrl: './user-list.html',
  styleUrl: './user-list.scss',
})
export class UserList implements OnInit {
  private readonly userService = inject(UserService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly notificationService = inject(NotificationService);

  private readonly router = inject(Router);

  readonly users = signal<User[]>([]);

  readonly search = signal('');

  readonly pageSize = signal(10);

  readonly pageIndex = signal(0);

  displayedColumns = ['name', 'email', 'campus', 'roles', 'status', 'actions'];

  private get baseRoute(): string {
    const slug = this.currentUserService.user()?.institutionSlug;

    return slug ? `/${slug}` : '/platform';
  }

  readonly filteredUsers = computed(() => {
    const term = this.search().toLowerCase();

    return this.users().filter(
      (x) =>
        x.firstName.toLowerCase().includes(term) ||
        x.lastName.toLowerCase().includes(term) ||
        x.email.toLowerCase().includes(term) ||
        x.campusName.toLowerCase().includes(term),
    );
  });

  readonly pagedUsers = computed(() => {
    const start = this.pageIndex() * this.pageSize();

    const end = start + this.pageSize();

    return this.filteredUsers().slice(start, end);
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.userService.getAll().subscribe({
      next: (data) => {
        this.users.set(data);
      },
      error: () => {
        this.notificationService.error('Failed to load users.');
      },
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);

    this.pageSize.set(event.pageSize);
  }

  createUser(): void {
    this.router.navigate([this.baseRoute, 'users', 'new']);
  }

  openUser(id: string): void {
    this.router.navigate([this.baseRoute, 'users', id]);
  }
}
