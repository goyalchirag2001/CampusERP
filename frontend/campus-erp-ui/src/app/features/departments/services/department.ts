import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Department } from '../models/department';
import { CreateDepartmentRequest } from '../models/create-department-request';
import { UpdateDepartmentRequest } from '../models/update-department-request';
import { Lookup } from '../../../core/models/lookup';
import { DepartmentLookup } from '../../../core/models/department-lookup';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Department[]> {
    return this.http.get<Department[]>(`${environment.apiUrl}/department`);
  }

  getById(id: string): Observable<Department> {
    return this.http.get<Department>(`${environment.apiUrl}/department/${id}`);
  }

  create(request: CreateDepartmentRequest): Observable<Department> {
    return this.http.post<Department>(`${environment.apiUrl}/department`, request);
  }

  update(id: string, request: UpdateDepartmentRequest): Observable<Department> {
    return this.http.put<Department>(`${environment.apiUrl}/department/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/department/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/department/${id}/deactivate`, {});
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/department/lookup`);
  }

  getLookupWithCampus(): Observable<DepartmentLookup[]> {
    return this.http.get<DepartmentLookup[]>(`${environment.apiUrl}/department/lookup-campus`);
  }
}
