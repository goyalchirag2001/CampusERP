import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class SemesterService {
  private readonly http = inject(HttpClient);

  getLookupByCourse(courseId: string): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/Semester/lookup/${courseId}`);
  }
}
