import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);

  const token = localStorage.getItem('accessToken');

  if (!token) {
    router.navigate(['/platform/login']);
    return false;
  }

  return true;
};
