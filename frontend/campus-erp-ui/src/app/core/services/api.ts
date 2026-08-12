import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { Observable, map } from 'rxjs';

import { ApiResponse } from '../models/api-response';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly http = inject(HttpClient);

  // =========================================================
  // GET
  // =========================================================

  get<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .get<unknown>(url, {
        params,
        headers,
      })
      .pipe(map((response) => this.extractData<T>(response)));
  }

  // =========================================================
  // POST
  // =========================================================

  post<T>(url: string, body?: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http
      .post<unknown>(url, body, {
        headers,
      })
      .pipe(map((response) => this.extractData<T>(response)));
  }

  // =========================================================
  // PUT
  // =========================================================

  put<T>(url: string, body?: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http
      .put<unknown>(url, body, {
        headers,
      })
      .pipe(map((response) => this.extractData<T>(response)));
  }

  // =========================================================
  // PATCH
  // =========================================================

  patch<T>(url: string, body?: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http
      .patch<unknown>(url, body, {
        headers,
      })
      .pipe(map((response) => this.extractData<T>(response)));
  }

  // =========================================================
  // DELETE
  // =========================================================

  delete<T>(url: string, headers?: HttpHeaders): Observable<T> {
    return this.http
      .delete<unknown>(url, {
        headers,
      })
      .pipe(map((response) => this.extractData<T>(response)));
  }

  // =========================================================
  // Private
  // =========================================================

  private extractData<T>(response: unknown): T {
    if (response === null || response === undefined) {
      return response as T;
    }

    if (this.isApiResponse<T>(response)) {
      if (!response.success) {
        const message =
          response.errors?.join('\n') || response.message || 'Unexpected server error.';

        throw new Error(message);
      }

      return response.data as T;
    }

    return response as T;
  }

  private isApiResponse<T>(response: unknown): response is ApiResponse<T> {
    if (typeof response !== 'object' || response === null) {
      return false;
    }

    return 'success' in response;
  }
}
