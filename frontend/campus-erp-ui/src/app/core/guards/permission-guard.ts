import { inject } from '@angular/core';

import { CanActivateFn } from '@angular/router';

import { Router } from '@angular/router';

import { PermissionService } from '../services/permission';

export const permissionGuard =
  (permission: string): CanActivateFn =>
  () => {
    const permissionService = inject(PermissionService);

    const router = inject(Router);

    if (permissionService.has(permission)) {
      return true;
    }

    router.navigate(['/']);

    return false;
  };
