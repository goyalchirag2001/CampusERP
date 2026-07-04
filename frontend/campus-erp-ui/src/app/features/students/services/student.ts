import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Student } from '../models/student';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Student[]> {
    return this.http.get<Student[]>(`${environment.apiUrl}/Student`);
  }

  getById(id: string): Observable<Student> {
    return this.http.get<Student>(`${environment.apiUrl}/Student/${id}`);
  }

  create(request: unknown): Observable<Student> {
    return this.http.post<Student>(`${environment.apiUrl}/Student`, request);
  }

  update(id: string, request: unknown): Observable<Student> {
    return this.http.put<Student>(`${environment.apiUrl}/Student/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/Student/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/Student/${id}/deactivate`, {});
  }
}
