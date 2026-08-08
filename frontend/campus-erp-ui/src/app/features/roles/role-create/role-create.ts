import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
  ChangeDetectionStrategy,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { RoleService } from '../services/role';
import { PermissionService } from '../services/permission';
import { Permission } from '../../../core/models/permission';
import { NotificationService } from '../../../core/services/notification';
import { CurrentUserService } from '../../../core/services/current-user';

@Component({
  selector: 'app-role-create',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
  ],
  templateUrl: './role-create.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './role-create.scss',
})
export class RoleCreate implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly roleService = inject(RoleService);

  private readonly permissionService = inject(PermissionService);

  private readonly notificationService = inject(NotificationService);

  private readonly currentUserService = inject(CurrentUserService);

  private readonly router = inject(Router);

  readonly permissions = signal<Permission[]>([]);

  readonly selectedPermissionIds = signal<string[]>([]);

  form = this.fb.group({
    name: ['', Validators.required],

    description: [''],
  });

  readonly groupedPermissions = computed(() => {
    const groups: Record<string, Permission[]> = {};

    for (const permission of this.permissions()) {
      if (!groups[permission.module]) {
        groups[permission.module] = [];
      }

      groups[permission.module].push(permission);
    }

    return Object.entries(groups).sort(([a], [b]) => a.localeCompare(b));
  });

  ngOnInit(): void {
    this.loadPermissions();
  }

  private loadPermissions(): void {
    this.permissionService.getAll().subscribe({
      next: (permissions) => {
        this.permissions.set(permissions);
      },
      error: () => {
        this.notificationService.error('Failed to load permissions.');
      },
    });
  }

  togglePermission(permissionId: string, checked: boolean): void {
    const current = [...this.selectedPermissionIds()];

    if (checked) {
      current.push(permissionId);
    } else {
      const index = current.indexOf(permissionId);

      if (index >= 0) {
        current.splice(index, 1);
      }
    }

    this.selectedPermissionIds.set(current);
  }

  isSelected(permissionId: string): boolean {
    return this.selectedPermissionIds().includes(permissionId);
  }

  create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    if (this.selectedPermissionIds().length === 0) {
      this.notificationService.warning('Select at least one permission.');

      return;
    }

    this.roleService
      .create({
        name: this.form.value.name ?? '',

        description: this.form.value.description ?? '',

        permissionIds: this.selectedPermissionIds(),
      })
      .subscribe({
        next: () => {
          this.notificationService.success('Role created successfully.');

          this.navigateBack();
        },
        error: (error) => {
          this.notificationService.error(error?.error?.message ?? 'Failed to create role.');
        },
      });
  }

  cancel(): void {
    this.navigateBack();
  }

  private navigateBack(): void {
    const slug = this.currentUserService.user()?.institutionSlug;

    if (slug) {
      this.router.navigate(['/', slug, 'roles']);

      return;
    }

    this.router.navigate(['/platform/roles']);
  }

  isAllSelected(): boolean {
    return (
      this.permissions().length > 0 &&
      this.permissions().every((x) => this.selectedPermissionIds().includes(x.id))
    );
  }

  toggleAllPermissions(checked: boolean): void {
    if (checked) {
      this.selectedPermissionIds.set(this.permissions().map((x) => x.id));

      return;
    }

    this.selectedPermissionIds.set([]);
  }

  isGroupFullySelected(permissions: Permission[]): boolean {
    return permissions.every((x) => this.selectedPermissionIds().includes(x.id));
  }

  toggleGroup(permissions: Permission[], checked: boolean): void {
    const current = new Set(this.selectedPermissionIds());

    if (checked) {
      permissions.forEach((x) => current.add(x.id));
    } else {
      permissions.forEach((x) => current.delete(x.id));
    }

    this.selectedPermissionIds.set([...current]);
  }

  getModuleDisplayName(moduleName: string): string {
    return moduleName.replace(/([A-Z])/g, ' $1').trim();
  }
}
