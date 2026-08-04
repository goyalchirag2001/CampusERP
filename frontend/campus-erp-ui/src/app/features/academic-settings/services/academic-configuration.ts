import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';

import { ApiService } from '../../../core/services/api';
import { ApiEndpoints } from '../../../core/constants/api-endpoints';

import { AcademicConfiguration } from '../models/academic-configuration';
import { UpdateAcademicConfigurationRequest } from '../models/update-academic-configuration-request';

@Injectable({
  providedIn: 'root',
})
export class AcademicConfigurationService {
  private readonly api = inject(ApiService);

  private static readonly Endpoint = `${environment.apiUrl}/${ApiEndpoints.AcademicConfiguration}`;

  //#region Queries

  get(): Observable<AcademicConfiguration> {
    return this.api.get<AcademicConfiguration>(AcademicConfigurationService.Endpoint);
  }

  //#endregion

  //#region Commands

  update(request: UpdateAcademicConfigurationRequest): Observable<AcademicConfiguration> {
    return this.api.put<AcademicConfiguration>(AcademicConfigurationService.Endpoint, request);
  }

  //#endregion
}
