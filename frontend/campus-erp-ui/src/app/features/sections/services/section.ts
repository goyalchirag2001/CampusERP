import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Section } from '../models/section';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class SectionService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiUrl}/Section`;

  getAll(): Observable<Section[]> {
    return this.http.get<Section[]>(this.baseUrl);
  }

  getById(id: string): Observable<Section> {
    return this.http.get<Section>(`${this.baseUrl}/${id}`);
  }

  create(request: { semesterId: string; name: string; capacity: number }): Observable<Section> {
    return this.http.post<Section>(this.baseUrl, request);
  }

  update(
    id: string,
    request: {
      name: string;
      capacity: number;
    },
  ): Observable<Section> {
    return this.http.put<Section>(`${this.baseUrl}/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/deactivate`, {});
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${this.baseUrl}/lookup`);
  }

  getLookupBySemester(semesterId: string): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${this.baseUrl}/lookup/semester/${semesterId}`);
  }
}
