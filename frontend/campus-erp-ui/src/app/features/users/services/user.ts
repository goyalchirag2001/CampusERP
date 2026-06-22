import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { User } from '../../../core/models/user';
import { CreateUserRequest } from '../models/create-user-request';
import { UpdateUserRequest } from '../models/update-user-request';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/user`);
  }

  getById(id: string): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/user/${id}`);
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.http.post<User>(`${environment.apiUrl}/user`, request);
  }

  update(id: string, request: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${environment.apiUrl}/user/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/user/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/user/${id}/deactivate`, {});
  }

  resetPassword(id: string, newPassword: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/user/${id}/reset-password`, {
      newPassword,
    });
  }
}
