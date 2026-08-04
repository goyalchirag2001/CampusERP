import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { TeacherAssignment } from '../models/teacher-assignment';
import { TeacherAssignmentResponse } from '../models/teacher-assignment-response';
import { CreateTeacherAssignmentRequest } from '../models/create-teacher-assignment-request';
import { UpdateTeacherAssignmentRequest } from '../models/update-teacher-assignment-request';

@Injectable({
  providedIn: 'root',
})
export class TeacherAssignmentService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiUrl}/TeacherAssignments`;

  getAll(): Observable<TeacherAssignment[]> {
    return this.http.get<TeacherAssignment[]>(this.baseUrl);
  }

  getById(id: string): Observable<TeacherAssignmentResponse> {
    return this.http.get<TeacherAssignmentResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateTeacherAssignmentRequest): Observable<TeacherAssignmentResponse> {
    return this.http.post<TeacherAssignmentResponse>(this.baseUrl, request);
  }

  update(
    id: string,
    request: UpdateTeacherAssignmentRequest,
  ): Observable<TeacherAssignmentResponse> {
    return this.http.put<TeacherAssignmentResponse>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
