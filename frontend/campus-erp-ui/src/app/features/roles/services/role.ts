import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Role } from '../../../core/models/role';
import { Lookup } from '../../../core/models/lookup';
import { CreateRoleRequest } from '../models/create-role-request';
import { UpdateRoleRequest } from '../models/update-role-request';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Role[]> {
    return this.http.get<Role[]>(`${environment.apiUrl}/role`);
  }

  getById(id: string): Observable<Role> {
    return this.http.get<Role>(`${environment.apiUrl}/role/${id}`);
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/role/lookup`);
  }

  create(request: CreateRoleRequest): Observable<Role> {
    return this.http.post<Role>(`${environment.apiUrl}/role`, request);
  }

  update(id: string, request: UpdateRoleRequest): Observable<Role> {
    return this.http.put<Role>(`${environment.apiUrl}/role/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/role/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/role/${id}/deactivate`, {});
  }
}
