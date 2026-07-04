import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Teacher } from '../models/teacher';
import { CreateTeacherRequest } from '../models/create-teacher-request';
import { Lookup } from '../../../core/models/lookup';
import { TeacherLookup } from '../../../core/models/teacher-lookup';

@Injectable({
  providedIn: 'root',
})
export class TeacherService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Teacher[]> {
    return this.http.get<Teacher[]>(`${environment.apiUrl}/teacher`);
  }

  getById(id: string): Observable<Teacher> {
    return this.http.get<Teacher>(`${environment.apiUrl}/teacher/${id}`);
  }

  create(request: CreateTeacherRequest): Observable<Teacher> {
    return this.http.post<Teacher>(`${environment.apiUrl}/teacher`, request);
  }

  update(id: string, request: any): Observable<Teacher> {
    return this.http.put<Teacher>(`${environment.apiUrl}/teacher/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/teacher/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/teacher/${id}/deactivate`, {});
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/teacher/lookup`);
  }

  getLookupWithDepartment(): Observable<TeacherLookup[]> {
    return this.http.get<TeacherLookup[]>(`${environment.apiUrl}/teacher/lookup-department`);
  }
}
