import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Permission } from '../../../core/models/permission';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Permission[]> {
    return this.http.get<Permission[]>(`${environment.apiUrl}/permission`);
  }
}
