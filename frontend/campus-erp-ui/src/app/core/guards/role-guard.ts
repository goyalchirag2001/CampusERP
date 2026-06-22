import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { CurrentUserService } from '../services/current-user';

export const roleGuard = (p0: RouterStateSnapshot | ActivatedRouteSnapshot, roles: string[]) => {
  return () => {
    const currentUser = inject(CurrentUserService);

    const router = inject(Router);

    const user = currentUser.user();

    if (!user) {
      router.navigate(['/platform/login']);
      return false;
    }

    const allowed = roles.some((role) => user.roles.includes(role));

    if (!allowed) {
      router.navigate(['/']);
      return false;
    }

    return true;
  };
};
