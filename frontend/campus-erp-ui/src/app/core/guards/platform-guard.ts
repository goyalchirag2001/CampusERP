import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { CurrentUserService } from '../services/current-user';

export const platformGuard: CanActivateFn = () => {
  const currentUserService = inject(CurrentUserService);
  const router = inject(Router);

  const user = currentUserService.user();

  if (!user) {
    return router.parseUrl('/platform/login');
  }

  const isPlatformUser = user.roles.some(
    (role) => role === 'SuperAdmin' || role === 'PlatformAdmin',
  );

  return isPlatformUser ? true : router.parseUrl('/access-denied');
};
