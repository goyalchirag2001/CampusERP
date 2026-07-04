import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Subject } from '../models/subject';
import { CreateSubjectRequest } from '../models/create-subject-request';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class SubjectService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Subject[]> {
    return this.http.get<Subject[]>(`${environment.apiUrl}/subject`);
  }

  getById(id: string): Observable<Subject> {
    return this.http.get<Subject>(`${environment.apiUrl}/subject/${id}`);
  }

  create(request: CreateSubjectRequest): Observable<Subject> {
    return this.http.post<Subject>(`${environment.apiUrl}/subject`, request);
  }

  update(id: string, request: any): Observable<Subject> {
    return this.http.put<Subject>(`${environment.apiUrl}/subject/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/subject/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/subject/${id}/deactivate`, {});
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/subject/lookup`);
  }
}
