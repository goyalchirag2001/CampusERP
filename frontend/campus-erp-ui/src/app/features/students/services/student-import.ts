import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { StudentImportValidation } from '../models/student-import-validation';
import { StudentImportCredential } from '../models/student-import-credential';

@Injectable({
  providedIn: 'root',
})
export class StudentImportService {
  private readonly http = inject(HttpClient);

  downloadTemplate(): Observable<Blob> {
    return this.http.get(`${environment.apiUrl}/student-import/template`, {
      responseType: 'blob',
    });
  }

  validate(
    institutionId: string,
    campusId: string,
    file: File,
  ): Observable<StudentImportValidation> {
    const formData = new FormData();

    formData.append('institutionId', institutionId);

    formData.append('campusId', campusId);

    formData.append('file', file);

    return this.http.post<StudentImportValidation>(
      `${environment.apiUrl}/student-import/validate`,
      formData,
    );
  }

  import(institutionId: string, campusId: string, file: File): Observable<StudentImportValidation> {
    const formData = new FormData();

    formData.append('institutionId', institutionId);

    formData.append('campusId', campusId);

    formData.append('file', file);

    return this.http.post<StudentImportValidation>(
      `${environment.apiUrl}/student-import/import`,
      formData,
    );
  }

  downloadCredentials(credentials: StudentImportCredential[]): Observable<Blob> {
    return this.http.post(`${environment.apiUrl}/student-import/credentials`, credentials, {
      responseType: 'blob',
    });
  }
}
