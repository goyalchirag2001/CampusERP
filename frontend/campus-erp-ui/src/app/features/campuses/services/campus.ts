import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Campus } from '../models/campus';
import { environment } from '../../../../environments/environment';
import { CreateCampusRequest } from '../models/create-campus-request';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class CampusService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Campus[]> {
    return this.http.get<Campus[]>(`${environment.apiUrl}/campus`);
  }

  getById(id: string): Observable<Campus> {
    return this.http.get<Campus>(`${environment.apiUrl}/campus/${id}`);
  }

  create(request: CreateCampusRequest): Observable<Campus> {
    return this.http.post<Campus>(`${environment.apiUrl}/campus`, request);
  }

  update(id: string, request: any): Observable<Campus> {
    return this.http.put<Campus>(`${environment.apiUrl}/campus/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/campus/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/campus/${id}/deactivate`, {});
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/campus/lookup`);
  }
}
