import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { AcademicSession } from '../models/academic-session';
import { AcademicSessionLookup } from '../models/academic-session-lookup';

@Injectable({
  providedIn: 'root',
})
export class AcademicSessionService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<AcademicSession[]> {
    return this.http.get<AcademicSession[]>(`${environment.apiUrl}/AcademicSession`);
  }

  getById(id: string): Observable<AcademicSession> {
    return this.http.get<AcademicSession>(`${environment.apiUrl}/AcademicSession/${id}`);
  }

  create(request: unknown): Observable<AcademicSession> {
    return this.http.post<AcademicSession>(`${environment.apiUrl}/AcademicSession`, request);
  }

  update(id: string, request: unknown): Observable<AcademicSession> {
    return this.http.put<AcademicSession>(`${environment.apiUrl}/AcademicSession/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/AcademicSession/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/AcademicSession/${id}/deactivate`, {});
  }

  setCurrent(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/AcademicSession/${id}/set-current`, {});
  }

  getCurrent(): Observable<AcademicSession> {
    return this.http.get<AcademicSession>(`${environment.apiUrl}/AcademicSession/current`);
  }

  getLookup(): Observable<AcademicSessionLookup[]> {
    return this.http.get<AcademicSessionLookup[]>(`${environment.apiUrl}/AcademicSession/lookup`);
  }

  getLookupByCampus(campusId: string): Observable<AcademicSessionLookup[]> {
    return this.http.get<AcademicSessionLookup[]>(
      `${environment.apiUrl}/AcademicSession/lookup/campus/${campusId}`,
    );
  }
}
