import { Injectable, inject } from '@angular/core';
import { CurrentUserService } from './current-user';
import { PermissionService } from './permission';
import { NavigationItem } from '../models/navigation-item';
import { Permissions } from '../constants/permissions';

@Injectable({
  providedIn: 'root',
})
export class NavigationService {
  private readonly currentUserService = inject(CurrentUserService);

  private readonly permissionService = inject(PermissionService);

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
        permission: Permissions.AdminDashboardView,
      },

      {
        label: 'Institutions',
        icon: 'apartment',
        route: '/platform/institutions',
        permission: Permissions.InstitutionView,
        roles: ['SuperAdmin', 'PlatformAdmin'],
      },

      {
        label: 'Campuses',
        icon: 'location_city',
        route: slug ? `/${slug}/campuses` : '/platform/campuses',
        permission: Permissions.CampusView,
        roles: ['SuperAdmin', 'PlatformAdmin', 'InstitutionAdmin'],
      },

      {
        label: 'Users',
        icon: 'group',
        route: slug ? `/${slug}/users` : '/platform/users',
        permission: Permissions.UserView,
        roles: ['SuperAdmin', 'PlatformAdmin', 'InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Roles',
        icon: 'security',
        route: slug ? `/${slug}/roles` : '/platform/roles',
        permission: Permissions.RoleView,
        roles: ['SuperAdmin'],
      },

      {
        label: 'Departments',
        icon: 'account_tree',
        route: slug ? `/${slug}/departments` : '/platform/departments',
        permission: Permissions.DepartmentView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Courses',
        icon: 'menu_book',
        route: slug ? `/${slug}/courses` : '/platform/courses',
        permission: Permissions.CourseView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Subjects',
        icon: 'library_books',
        route: slug ? `/${slug}/subjects` : '/platform/subjects',
        permission: Permissions.SubjectView,
        roles: ['InstitutionAdmin', 'CampusAdmin', 'Teacher'],
      },

      {
        label: 'Teachers',
        icon: 'groups',
        route: slug ? `/${slug}/teachers` : '/platform/teachers',
        permission: Permissions.TeacherView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Students',
        icon: 'school',
        route: slug ? `/${slug}/students` : '/platform/students',
        permission: Permissions.StudentView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },
    ];

    return items.filter((item) => {
      const hasPermission = !item.permission || this.permissionService.has(item.permission);

      const hasRole = !item.roles || item.roles.some((role) => user.roles.includes(role));

      return hasPermission && hasRole;
    });
  }
}
