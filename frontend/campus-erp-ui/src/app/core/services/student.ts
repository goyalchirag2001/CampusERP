import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import { StudentResponse } from '../models/student-response';

import { CreateStudentRequest } from '../models/create-student-request';

@Injectable({
  providedIn: 'root'
})
export class StudentService {

  private readonly http = inject(HttpClient);

  getAll(): Observable<StudentResponse[]> {

    return this.http.get<StudentResponse[]>(
      `${environment.apiUrl}/Student`
    );
  }

  create(
    request: CreateStudentRequest
  ): Observable<StudentResponse> {

    return this.http.post<StudentResponse>(
      `${environment.apiUrl}/Student`,
      request
    );
  }
}