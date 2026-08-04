import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { ApiService } from '../../../core/services/api';
import { ApiEndpoints } from '../../../core/constants/api-endpoints';

import { CalendarEvent } from '../models/calendar-event';
import { CreateCalendarEventRequest } from '../models/create-calendar-event-request';
import { UpdateCalendarEventRequest } from '../models/update-calendar-event-request';

@Injectable({
  providedIn: 'root',
})
export class CalendarEventService {
  private readonly api = inject(ApiService);

  private static readonly Endpoint = `${environment.apiUrl}/${ApiEndpoints.CalendarEvents}`;

  //#region Queries

  getAll(): Observable<CalendarEvent[]> {
    return this.api.get<CalendarEvent[]>(CalendarEventService.Endpoint);
  }

  getById(id: string): Observable<CalendarEvent> {
    return this.api.get<CalendarEvent>(`${CalendarEventService.Endpoint}/${id}`);
  }

  //#endregion

  //#region Commands

  create(request: CreateCalendarEventRequest): Observable<CalendarEvent> {
    return this.api.post<CalendarEvent>(CalendarEventService.Endpoint, request);
  }

  update(id: string, request: UpdateCalendarEventRequest): Observable<CalendarEvent> {
    return this.api.put<CalendarEvent>(`${CalendarEventService.Endpoint}/${id}`, request);
  }

  activate(id: string): Observable<boolean> {
    return this.api.post<boolean>(`${CalendarEventService.Endpoint}/${id}/activate`, {});
  }

  deactivate(id: string): Observable<boolean> {
    return this.api.post<boolean>(`${CalendarEventService.Endpoint}/${id}/deactivate`, {});
  }

  //#endregion
}
