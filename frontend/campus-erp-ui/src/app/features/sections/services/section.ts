import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class SectionService {
  private readonly http = inject(HttpClient);

  getLookupBySemester(semesterId: string): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/Section/lookup/semester/${semesterId}`);
  }
}
