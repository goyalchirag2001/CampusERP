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

    if (!user) {
      return [];
    }

    const slug = user.institutionSlug ?? null;

    const items: NavigationItem[] = [
      /* =========================================================
         Dashboard
         ========================================================= */

      {
        label: 'Dashboard',
        icon: 'dashboard',
        route: slug ? `/${slug}/dashboard` : '/platform/dashboard',
        permission: Permissions.AdminDashboardView,
      },

      /* =========================================================
         Platform Administration
         ========================================================= */

      {
        label: 'Institutions',
        icon: 'apartment',
        route: '/platform/institutions',
        permission: Permissions.InstitutionView,
        roles: ['SuperAdmin', 'PlatformAdmin'],
      },

      /* =========================================================
         Institution / Campus Administration
         ========================================================= */

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
        label: 'Rooms',
        icon: 'meeting_room',
        route: slug ? `/${slug}/rooms` : '/platform/rooms',
        permission: Permissions.RoomView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Courses',
        icon: 'menu_book',
        route: slug ? `/${slug}/courses` : '/platform/courses',
        permission: Permissions.CourseView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      /* =========================================================
         Academic Configuration
         ========================================================= */

      {
        label: 'Academic Sessions',
        icon: 'calendar_month',
        route: slug ? `/${slug}/academic-sessions` : '/platform/academic-sessions',
        permission: Permissions.AcademicSessionView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Academic Settings',
        icon: 'tune',
        route: slug ? `/${slug}/academic-settings` : '/platform/academic-settings',
        permission: Permissions.AcademicSettingsView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Academic Calendar',
        icon: 'event',
        route: slug ? `/${slug}/calendar-events` : '/platform/calendar-events',
        permission: Permissions.CalendarView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      /* =========================================================
         Attendance
         ========================================================= */

      {
        label: 'Attendance Corrections',
        icon: 'fact_check',
        route: slug ? `/${slug}/attendance-corrections` : '/platform/attendance-corrections',
        permission: Permissions.AttendanceCorrectionView,
        roles: ['InstitutionAdmin', 'CampusAdmin', 'Teacher'],
      },

      /* =========================================================
         Timetable Configuration
         ========================================================= */

      {
        label: 'Timetables',
        icon: 'schedule',
        route: slug ? `/${slug}/timetable-templates` : '/platform/timetable-templates',
        permission: Permissions.TimetableTemplateView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Sections',
        icon: 'view_module',
        route: slug ? `/${slug}/sections` : '/platform/sections',
        permission: Permissions.SectionView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Teacher Assignments',
        icon: 'assignment_ind',
        route: slug ? `/${slug}/teacher-assignments` : '/platform/teacher-assignments',
        permission: Permissions.TeacherAssignmentView,
        roles: ['InstitutionAdmin', 'CampusAdmin'],
      },

      {
        label: 'Subjects',
        icon: 'library_books',
        route: slug ? `/${slug}/subjects` : '/platform/subjects',
        permission: Permissions.SubjectView,
        roles: ['InstitutionAdmin', 'CampusAdmin', 'Teacher'],
      },

      /* =========================================================
         People
         ========================================================= */

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

      /* =========================================================
         My Calendar
         ========================================================= */

      {
        label: 'Timetable',
        icon: 'event_note',
        route: slug ? `/${slug}/teacher-calendar` : '/platform/teacher-calendar',
        permission: Permissions.TeacherCalendarView,
        roles: ['Teacher'],
      },

      {
        label: 'Timetable',
        icon: 'calendar_view_week',
        route: slug ? `/${slug}/student-calendar` : '/platform/student-calendar',
        permission: Permissions.StudentCalendarView,
        roles: ['Student'],
      },
    ];

    return items.filter((item) => {
      const hasPermission = !item.permission || this.permissionService.has(item.permission);

      const hasRole = !item.roles || item.roles.some((role) => user.roles.includes(role));

      return hasPermission && hasRole;
    });
  }
}
