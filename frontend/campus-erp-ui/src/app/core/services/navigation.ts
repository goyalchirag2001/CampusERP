import { Injectable, inject } from '@angular/core';

import { CurrentUserService } from './current-user';

import { NavigationItem } from '../models/navigation-item';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private readonly currentUserService = inject(CurrentUserService);

  getMenuItems(): NavigationItem[] {
    const user = this.currentUserService.user();
    const slug = user?.institutionSlug ?? null;

    if (!user) {
      return [];
    }

    const items: NavigationItem[] = [
      {
        label: 'Dashboard',
        icon: 'dashboard',
        route: slug ? `/${slug}/dashboard` : '/platform/dashboard',
        roles: ['PlatformAdmin', 'InstitutionAdmin', 'Teacher', 'Student'],
      },

      {
        label: 'Institutions',
        icon: 'apartment',
        route: '/platform/institutions',
        roles: ['PlatformAdmin'],
      },

      {
        label: 'Students',
        icon: 'school',
        route: `/${slug}/students`,
        roles: ['InstitutionAdmin'],
      },

      {
        label: 'Teachers',
        icon: 'groups',
        route: `/${slug}/teachers`,
        roles: ['InstitutionAdmin'],
      },

      {
        label: 'Subjects',
        icon: 'menu_book',
        route: `/${slug}/subjects`,
        roles: ['InstitutionAdmin', 'Teacher'],
      },
    ];

    return items.filter((item) => item.roles.some((role) => user.roles.includes(role)));
  }
}
