import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth';

let isRefreshing = false;

const refreshSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  const router = inject(Router);

  const token = authService.getAccessToken();

  let request = req;

  if (token) {
    request = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (
        error.status !== 401 ||
        request.url.includes('/Auth/login') ||
        request.url.includes('/Auth/refresh-token')
      ) {
        return throwError(() => error);
      }

      if (!isRefreshing) {
        isRefreshing = true;

        refreshSubject.next(null);

        return authService.refreshToken().pipe(
          switchMap((response) => {
            isRefreshing = false;

            authService.saveTokens(response);

            refreshSubject.next(response.accessToken);

            return next(
              request.clone({
                setHeaders: {
                  Authorization: `Bearer ${response.accessToken}`,
                },
              }),
            );
          }),

          catchError((refreshError) => {
            isRefreshing = false;

            authService.logout();

            router.navigate(['/']);

            return throwError(() => refreshError);
          }),
        );
      }

      return refreshSubject.pipe(
        filter((token) => token !== null),
        take(1),
        switchMap((token) =>
          next(
            request.clone({
              setHeaders: {
                Authorization: `Bearer ${token!}`,
              },
            }),
          ),
        ),
      );
    }),
  );
};
