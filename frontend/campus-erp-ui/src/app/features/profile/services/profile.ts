import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Profile } from '../models/profile';
import { UpdateProfileRequest } from '../models/update-profile-request';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private readonly http = inject(HttpClient);

  getMyProfile(): Observable<Profile> {
    return this.http.get<Profile>(`${environment.apiUrl}/Profile`);
  }

  update(request: UpdateProfileRequest): Observable<Profile> {
    return this.http.put<Profile>(`${environment.apiUrl}/Profile`, request);
  }
}
