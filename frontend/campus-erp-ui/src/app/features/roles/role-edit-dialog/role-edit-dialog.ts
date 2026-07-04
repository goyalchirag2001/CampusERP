import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PermissionService } from '../services/permission';
import { RoleService } from '../services/role';
import { Permission } from '../../../core/models/permission';
import { NotificationService } from '../../../core/services/notification';
import { computed } from '@angular/core';

@Component({
  selector: 'app-role-edit-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './role-edit-dialog.html',
  styleUrl: './role-edit-dialog.scss',
})
export class RoleEditDialog implements OnInit {
  private readonly fb = inject(FormBuilder);

  private readonly roleService = inject(RoleService);

  private readonly permissionService = inject(PermissionService);

  private readonly notificationService = inject(NotificationService);

  private readonly dialogRef = inject(MatDialogRef<RoleEditDialog>);

  private readonly data = inject(MAT_DIALOG_DATA) as {
    roleId: string;
  };

  readonly permissions = signal<Permission[]>([]);

  readonly groupedPermissions = computed(() => {
    const groups: Record<string, Permission[]> = {};

    for (const permission of this.permissions()) {
      if (!groups[permission.module]) {
        groups[permission.module] = [];
      }

      groups[permission.module].push(permission);
    }

    return Object.entries(groups)
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([module, permissions]) => ({
        module,
        permissions,
      }));
  });

  readonly selectedPermissionIds = signal<string[]>([]);

  private roleId = '';

  form = this.fb.group({
    name: ['', Validators.required],

    description: [''],
  });

  ngOnInit(): void {
    this.roleId = this.data.roleId;

    this.load();
  }

  private load(): void {
    this.roleService.getById(this.roleId).subscribe({
      next: (role) => {
        this.form.patchValue({
          name: role.name,

          description: role.description ?? '',
        });

        this.selectedPermissionIds.set(role.permissionIds);

        this.loadPermissions();
      },

      error: () => {
        this.notificationService.error('Failed to load role.');
      },
    });
  }

  private loadPermissions(): void {
    this.permissionService.getAll().subscribe({
      next: (permissions) => {
        this.permissions.set(permissions);
      },
    });
  }

  togglePermission(permissionId: string, checked: boolean): void {
    const current = [...this.selectedPermissionIds()];

    if (checked) {
      if (!current.includes(permissionId)) {
        current.push(permissionId);
      }
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

  save(): void {
    this.roleService
      .update(this.roleId, {
        name: this.form.value.name ?? '',

        description: this.form.value.description ?? '',

        permissionIds: this.selectedPermissionIds(),
      })
      .subscribe({
        next: () => {
          this.notificationService.success('Role updated successfully.');

          this.dialogRef.close(true);
        },

        error: (error) => {
          this.notificationService.error(error?.error?.message ?? 'Failed to update role.');
        },
      });
  }

  cancel(): void {
    this.dialogRef.close();
  }

  selectAllPermissions(): void {
    this.selectedPermissionIds.set(this.permissions().map((x) => x.id));
  }

  clearAllPermissions(): void {
    this.selectedPermissionIds.set([]);
  }

  toggleModulePermissions(module: string, checked: boolean): void {
    const modulePermissions = this.permissions()
      .filter((x) => x.module === module)
      .map((x) => x.id);

    const selected = [...this.selectedPermissionIds()];

    if (checked) {
      modulePermissions.forEach((id) => {
        if (!selected.includes(id)) {
          selected.push(id);
        }
      });
    } else {
      modulePermissions.forEach((id) => {
        const index = selected.indexOf(id);

        if (index >= 0) {
          selected.splice(index, 1);
        }
      });
    }

    this.selectedPermissionIds.set(selected);
  }

  isModuleSelected(module: string): boolean {
    const modulePermissions = this.permissions().filter((x) => x.module === module);

    return (
      modulePermissions.length > 0 &&
      modulePermissions.every((x) => this.selectedPermissionIds().includes(x.id))
    );
  }
}
