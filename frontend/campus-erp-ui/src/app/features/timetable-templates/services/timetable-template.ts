import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { ApiService } from '../../../core/services/api';

import { ApiEndpoints } from '../../../core/constants/api-endpoints';

import { TimetableTemplate } from '../models/timetable-template';
import { CreateTimetableTemplateRequest } from '../models/create-timetable-template-request';
import { UpdateTimetableTemplateRequest } from '../models/update-timetable-template-request';

@Injectable({
  providedIn: 'root',
})
export class TimetableTemplateService {
  private readonly api = inject(ApiService);

  private static readonly Endpoint = `${environment.apiUrl}/${ApiEndpoints.TimetableTemplates}`;

  //#region Queries

  getAll(): Observable<TimetableTemplate[]> {
    return this.api.get<TimetableTemplate[]>(TimetableTemplateService.Endpoint);
  }

  getById(id: string): Observable<TimetableTemplate> {
    return this.api.get<TimetableTemplate>(`${TimetableTemplateService.Endpoint}/${id}`);
  }

  getByTeacher(teacherId: string): Observable<TimetableTemplate[]> {
    return this.api.get<TimetableTemplate[]>(
      `${TimetableTemplateService.Endpoint}/teacher/${teacherId}`,
    );
  }

  getBySection(sectionId: string): Observable<TimetableTemplate[]> {
    return this.api.get<TimetableTemplate[]>(
      `${TimetableTemplateService.Endpoint}/section/${sectionId}`,
    );
  }

  getByAcademicSession(academicSessionId: string): Observable<TimetableTemplate[]> {
    return this.api.get<TimetableTemplate[]>(
      `${TimetableTemplateService.Endpoint}/academic-session/${academicSessionId}`,
    );
  }

  getWeeklyTimetable(
    sectionId: string,
    academicSessionId: string,
  ): Observable<TimetableTemplate[]> {
    return this.api.get<TimetableTemplate[]>(
      `${TimetableTemplateService.Endpoint}/weekly?sectionId=${sectionId}&academicSessionId=${academicSessionId}`,
    );
  }

  //#endregion

  //#region Commands

  create(request: CreateTimetableTemplateRequest): Observable<TimetableTemplate> {
    return this.api.post<TimetableTemplate>(TimetableTemplateService.Endpoint, request);
  }

  update(id: string, request: UpdateTimetableTemplateRequest): Observable<TimetableTemplate> {
    return this.api.put<TimetableTemplate>(`${TimetableTemplateService.Endpoint}/${id}`, request);
  }

  activate(id: string): Observable<boolean> {
    return this.api.post<boolean>(`${TimetableTemplateService.Endpoint}/${id}/activate`, {});
  }

  deactivate(id: string): Observable<boolean> {
    return this.api.post<boolean>(`${TimetableTemplateService.Endpoint}/${id}/deactivate`, {});
  }

  delete(id: string): Observable<boolean> {
    return this.api.delete<boolean>(`${TimetableTemplateService.Endpoint}/${id}`);
  }

  //#endregion
}
