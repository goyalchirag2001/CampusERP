import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { Institution } from '../../../core/models/institution';

import { CreateInstitutionRequest } from '../models/create-institution-request';

@Injectable({
  providedIn: 'root',
})
export class InstitutionService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Institution[]> {
    return this.http.get<Institution[]>(`${environment.apiUrl}/institution`);
  }

  getById(id: string): Observable<Institution> {
    return this.http.get<Institution>(`${environment.apiUrl}/institution/${id}`);
  }

  create(request: CreateInstitutionRequest): Observable<Institution> {
    return this.http.post<Institution>(`${environment.apiUrl}/institution`, request);
  }

  update(id: string, request: any): Observable<Institution> {
    return this.http.put<Institution>(`${environment.apiUrl}/institution/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/institution/${id}`);
  }

  activate(id: string) {
    return this.http.put(`${environment.apiUrl}/institution/${id}/activate`, {});
  }

  deactivate(id: string) {
    return this.http.put(`${environment.apiUrl}/institution/${id}/deactivate`, {});
  }
}
