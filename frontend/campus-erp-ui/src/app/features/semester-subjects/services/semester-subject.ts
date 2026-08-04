import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SemesterSubject } from '../models/semester-subject';
import { AssignSemesterSubjectRequest } from '../models/assign-semester-subject-request';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class SemesterSubjectService {
  private readonly http = inject(HttpClient);

  assign(request: AssignSemesterSubjectRequest): Observable<SemesterSubject> {
    return this.http.post<SemesterSubject>(`${environment.apiUrl}/SemesterSubject`, request);
  }

  getBySemester(semesterId: string): Observable<SemesterSubject[]> {
    return this.http.get<SemesterSubject[]>(
      `${environment.apiUrl}/SemesterSubject/semester/${semesterId}`,
    );
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/SemesterSubject/${id}`);
  }

  moveUp(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/SemesterSubject/${id}/move-up`, {});
  }

  moveDown(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/SemesterSubject/${id}/move-down`, {});
  }

  getLookupBySection(sectionId: string): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(
      `${environment.apiUrl}/SemesterSubject/lookup/section/${sectionId}`,
    );
  }
}
