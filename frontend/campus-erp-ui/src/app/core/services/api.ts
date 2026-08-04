import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';

import { ApiResponse } from '../models/api-response';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly http = inject(HttpClient);

  //#region GET

  get<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(url, {
        params,
        headers,
      })
      .pipe(map((response) => this.extractData(response)));
  }

  //#endregion

  //#region POST

  post<T>(url: string, body?: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http
      .post<ApiResponse<T>>(url, body, {
        headers,
      })
      .pipe(map((response) => this.extractData(response)));
  }

  //#endregion

  //#region PUT

  put<T>(url: string, body?: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http
      .put<ApiResponse<T>>(url, body, {
        headers,
      })
      .pipe(map((response) => this.extractData(response)));
  }

  //#endregion

  //#region PATCH

  patch<T>(url: string, body?: unknown, headers?: HttpHeaders): Observable<T> {
    return this.http
      .patch<ApiResponse<T>>(url, body, {
        headers,
      })
      .pipe(map((response) => this.extractData(response)));
  }

  //#endregion

  //#region DELETE

  delete<T>(url: string, headers?: HttpHeaders): Observable<T> {
    return this.http
      .delete<ApiResponse<T>>(url, {
        headers,
      })
      .pipe(map((response) => this.extractData(response)));
  }

  //#endregion

  //#region Private

  private extractData<T>(response: ApiResponse<T>): T {
    if (!response.success) {
      throw new Error(response.errors.join('\n') || response.message || 'Unexpected server error.');
    }

    return response.data as T;
  }

  //#endregion
}
