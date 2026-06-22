import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';

import { inject } from '@angular/core';

import { catchError, throwError } from 'rxjs';

import { NotificationService } from '../services/notification';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 400) {
        notification.error(error.error?.message ?? 'Invalid request.');
      } else if (error.status === 401) {
        notification.error('Unauthorized access.');
      } else if (error.status === 403) {
        notification.error('You do not have permission.');
      } else if (error.status === 404) {
        notification.error(error.error?.message ?? 'Record not found.');
      } else if (error.status >= 500) {
        notification.error('Server error occurred.');
      }

      return throwError(() => error);
    }),
  );
};
