import { Injectable, inject } from '@angular/core';

import { CurrentUserService } from './current-user';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private readonly currentUserService = inject(CurrentUserService);

  has(permission: string): boolean {
    return this.currentUserService.user()?.permissions.includes(permission) ?? false;
  }

  hasAny(...permissions: string[]): boolean {
    const userPermissions = this.currentUserService.user()?.permissions ?? [];

    return permissions.some((p) => userPermissions.includes(p));
  }

  hasAll(...permissions: string[]): boolean {
    const userPermissions = this.currentUserService.user()?.permissions ?? [];

    return permissions.every((p) => userPermissions.includes(p));
  }
}
