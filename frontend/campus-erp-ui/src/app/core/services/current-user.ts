import { Injectable, signal } from '@angular/core';
import { CurrentUser } from '../models/current-user';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  readonly user = signal<CurrentUser | null>(null);

  readonly loading = signal(false);

  setUser(user: CurrentUser): void {
    this.user.set(user);
  }

  clear(): void {
    this.user.set(null);
  }

  isPlatformAdmin(): boolean {
    return this.user()?.roles.includes('PlatformAdmin') ?? false;
  }

  isInstitutionAdmin(): boolean {
    return this.user()?.roles.includes('InstitutionAdmin') ?? false;
  }

  isTeacher(): boolean {
    return this.user()?.roles.includes('Teacher') ?? false;
  }

  isStudent(): boolean {
    return this.user()?.roles.includes('Student') ?? false;
  }
}
