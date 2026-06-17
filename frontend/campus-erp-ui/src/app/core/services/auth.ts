import { Injectable, inject } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { LoginRequest } from '../models/login-request';

import { AuthResponse } from '../models/auth-response';

import { CurrentUser } from '../models/current-user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/Auth/login`, request);
  }

  getCurrentUser(): Observable<CurrentUser> {
    return this.http.get<CurrentUser>(`${environment.apiUrl}/Auth/me`);
  }

  saveTokens(response: AuthResponse): void {
    localStorage.setItem('accessToken', response.accessToken);

    localStorage.setItem('refreshToken', response.refreshToken);
  }

  logout(): void {
    localStorage.removeItem('accessToken');

    localStorage.removeItem('refreshToken');
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  isLoggedIn(): boolean {
    return this.getAccessToken() !== null;
  }
}
