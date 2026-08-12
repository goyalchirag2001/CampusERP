import { Injectable, inject } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { ApiService } from '../../../core/services/api';
import { ApiEndpoints } from '../../../core/constants/api-endpoints';

import { TimetableCalendarEvent } from '../models/timetable-calendar-event';
import { TimetableCalendarRequest } from '../models/timetable-calendar-request';

@Injectable({
  providedIn: 'root',
})
export class TimetableCalendarService {
  private readonly api = inject(ApiService);

  private static readonly Endpoint = `${environment.apiUrl}/${ApiEndpoints.TimetableCalendar}`;

  // =========================================================
  // Teacher Calendar
  // =========================================================

  getTeacherCalendar(request: TimetableCalendarRequest): Observable<TimetableCalendarEvent[]> {
    return this.api.get<TimetableCalendarEvent[]>(
      `${TimetableCalendarService.Endpoint}/teacher`,
      this.buildParams(request),
    );
  }

  // =========================================================
  // Student Calendar
  // =========================================================

  getStudentCalendar(request: TimetableCalendarRequest): Observable<TimetableCalendarEvent[]> {
    return this.api.get<TimetableCalendarEvent[]>(
      `${TimetableCalendarService.Endpoint}/student`,
      this.buildParams(request),
    );
  }

  // =========================================================
  // Parameters
  // =========================================================

  private buildParams(request: TimetableCalendarRequest): HttpParams {
    let params = new HttpParams()
      .set('startDate', request.startDate)
      .set('endDate', request.endDate);

    if (request.academicSessionId) {
      params = params.set('academicSessionId', request.academicSessionId);
    }

    return params;
  }
}
