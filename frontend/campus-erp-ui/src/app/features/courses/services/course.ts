import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Course } from '../models/course';
import { CreateCourseRequest } from '../models/create-course-request';
import { UpdateCourseRequest } from '../models/update-course-request';
import { Lookup } from '../../../core/models/lookup';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Course[]> {
    return this.http.get<Course[]>(`${environment.apiUrl}/course`);
  }

  getById(id: string): Observable<Course> {
    return this.http.get<Course>(`${environment.apiUrl}/course/${id}`);
  }

  create(request: CreateCourseRequest): Observable<Course> {
    return this.http.post<Course>(`${environment.apiUrl}/course`, request);
  }

  update(id: string, request: UpdateCourseRequest): Observable<Course> {
    return this.http.put<Course>(`${environment.apiUrl}/course/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/course/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/course/${id}/deactivate`, {});
  }

  getLookup(): Observable<Lookup[]> {
    return this.http.get<Lookup[]>(`${environment.apiUrl}/course/lookup`);
  }
}
