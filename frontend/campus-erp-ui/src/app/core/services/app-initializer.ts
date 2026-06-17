import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { AuthService } from './auth';
import { CurrentUserService } from './current-user';

@Injectable({
  providedIn: 'root',
})
export class AppInitializerService {
  private readonly authService = inject(AuthService);

  private readonly currentUserService = inject(CurrentUserService);

  async initialize(): Promise<void> {
    const token = this.authService.getAccessToken();

    if (!token) {
      return;
    }

    try {
      const user = await firstValueFrom(this.authService.getCurrentUser());

      this.currentUserService.setUser(user);
    } catch {
      this.authService.logout();
    }
  }
}
