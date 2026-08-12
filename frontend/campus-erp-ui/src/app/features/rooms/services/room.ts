import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { ApiService } from '../../../core/services/api';

import { ApiEndpoints } from '../../../core/constants/api-endpoints';

import { Lookup } from '../../../core/models/lookup';

import { Room } from '../models/room';
import { CreateRoomRequest } from '../models/create-room-request';
import { UpdateRoomRequest } from '../models/update-room-request';

@Injectable({
  providedIn: 'root',
})
export class RoomService {
  private readonly api = inject(ApiService);

  private static readonly Endpoint = `${environment.apiUrl}/${ApiEndpoints.Rooms}`;

  //#region Queries

  getAll(): Observable<Room[]> {
    return this.api.get<Room[]>(RoomService.Endpoint);
  }

  getById(id: string): Observable<Room> {
    return this.api.get<Room>(`${RoomService.Endpoint}/${id}`);
  }

  getLookup(): Observable<Lookup[]> {
    return this.api.get<Lookup[]>(`${RoomService.Endpoint}/lookup`);
  }

  //#endregion

  //#region Commands

  create(request: CreateRoomRequest): Observable<Room> {
    return this.api.post<Room>(RoomService.Endpoint, request);
  }

  update(id: string, request: UpdateRoomRequest): Observable<Room> {
    return this.api.put<Room>(`${RoomService.Endpoint}/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.api.put<void>(`${RoomService.Endpoint}/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.api.put<void>(`${RoomService.Endpoint}/${id}/deactivate`, {});
  }

  //#endregion
}
