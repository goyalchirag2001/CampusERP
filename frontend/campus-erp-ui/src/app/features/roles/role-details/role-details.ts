import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RoleService } from '../services/role';
import { Role } from '../../../core/models/role';
import { NotificationService } from '../../../core/services/notification';
import { CurrentUserService } from '../../../core/services/current-user';
import { Permission } from '../../../core/models/permission';
import { MatDialog } from '@angular/material/dialog';
import { RoleEditDialog } from '../role-edit-dialog/role-edit-dialog';

@Component({
  selector: 'app-role-details',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, MatIconModule],
  templateUrl: './role-details.html',
  styleUrl: './role-details.scss',
})
export class RoleDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);

  private readonly router = inject(Router);

  private readonly roleService = inject(RoleService);

  private readonly notificationService = inject(NotificationService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly dialog = inject(MatDialog);

  readonly role = signal<Role | null>(null);

  readonly groupedPermissions = computed(() => {
    const role = this.role();

    if (!role) {
      return [];
    }

    const groups: Record<string, Permission[]> = {};

    for (const permission of role.permissions) {
      if (!groups[permission.module]) {
        groups[permission.module] = [];
      }

      groups[permission.module].push(permission);
    }

    return Object.entries(groups).sort(([a], [b]) => a.localeCompare(b));
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      return;
    }

    this.roleService.getById(id).subscribe({
      next: (role) => {
        this.role.set(role);
      },
      error: () => {
        this.notificationService.error('Failed to load role.');
      },
    });
  }

  edit(): void {
    const role = this.role();

    if (!role) {
      return;
    }

    const dialogRef = this.dialog.open(RoleEditDialog, {
      width: '1100px',

      maxWidth: '95vw',

      maxHeight: '90vh',

      data: {
        roleId: role.id,
      },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (!updated) {
        return;
      }

      this.roleService.getById(role.id).subscribe({
        next: (updatedRole) => {
          this.role.set(updatedRole);
        },
      });
    });
  }

  activate(): void {
    const role = this.role();

    if (!role) {
      return;
    }

    this.roleService.activate(role.id).subscribe({
      next: () => {
        this.notificationService.success('Role activated.');

        this.role.update((x) =>
          x
            ? {
                ...x,
                isActive: true,
              }
            : null,
        );
      },
      error: (error) => {
        this.notificationService.error(error?.error?.message ?? 'Failed to activate role.');
      },
    });
  }

  deactivate(): void {
    const role = this.role();

    if (!role) {
      return;
    }

    this.roleService.deactivate(role.id).subscribe({
      next: () => {
        this.notificationService.success('Role deactivated.');

        this.role.update((x) =>
          x
            ? {
                ...x,
                isActive: false,
              }
            : null,
        );
      },
      error: (error) => {
        this.notificationService.error(error?.error?.message ?? 'Failed to deactivate role.');
      },
    });
  }

  getType(): string {
    return this.role()?.isSystemRole ? 'System' : 'Custom';
  }
}
