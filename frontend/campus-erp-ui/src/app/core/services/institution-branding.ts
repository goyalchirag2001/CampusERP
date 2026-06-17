import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface InstitutionBranding {
  id: string;

  name: string;

  loginSlug: string;

  logoUrl?: string;

  primaryColor?: string;

  secondaryColor?: string;
}

@Injectable({
  providedIn: 'root',
})
export class InstitutionBrandingService {
  private readonly http = inject(HttpClient);

  getBySlug(slug: string): Observable<InstitutionBranding> {
    return this.http.get<InstitutionBranding>(`${environment.apiUrl}/institution-discovery/slug/${slug}`);
  }
}
