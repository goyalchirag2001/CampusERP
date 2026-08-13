import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard/dashboard';

import { AdminLayout } from './layouts/admin-layout/admin-layout';

import { authGuard } from './core/guards/auth-guard';
import { permissionGuard } from './core/guards/permission-guard';

import { Permissions } from './core/constants/permissions';

import { ForbiddenComponent } from './shared/pages/forbidden/forbidden';
import { NotFoundComponent } from './shared/pages/not-found/not-found';
import { platformGuard } from './core/guards/platform-guard';

export const routes: Routes = [
  /* =========================================================
     Authentication
     ========================================================= */

  {
    path: 'platform/login',

    component: Login,
  },

  {
    path: 'access-denied',

    component: ForbiddenComponent,
  },

  {
    path: ':institutionSlug/login',

    component: Login,
  },

  /* =========================================================
     Platform Administration
     ========================================================= */

  {
    path: 'platform',

    component: AdminLayout,

    canActivate: [authGuard, platformGuard],

    children: [
      {
        path: 'dashboard',

        loadComponent: () =>
          import('./features/dashboard/dashboard/dashboard').then((m) => m.Dashboard),

        canActivate: [permissionGuard(Permissions.AdminDashboardView)],
      },

      /* -------------------------------------------------------
         Institutions
         ------------------------------------------------------- */

      {
        path: 'institutions',

        loadComponent: () =>
          import('./features/institutions/institution-list/institution-list').then(
            (m) => m.InstitutionList,
          ),

        canActivate: [permissionGuard(Permissions.InstitutionView)],
      },

      {
        path: 'institutions/new',

        loadComponent: () =>
          import('./features/institutions/institution-create/institution-create').then(
            (m) => m.InstitutionCreate,
          ),

        canActivate: [permissionGuard(Permissions.InstitutionCreate)],
      },

      {
        path: 'institutions/:id',

        loadComponent: () =>
          import('./features/institutions/institution-details/institution-details').then(
            (m) => m.InstitutionDetails,
          ),

        canActivate: [permissionGuard(Permissions.InstitutionView)],
      },

      /* -------------------------------------------------------
         Default Platform Route
         ------------------------------------------------------- */

      {
        path: '',

        redirectTo: 'dashboard',

        pathMatch: 'full',
      },

      /* -------------------------------------------------------
         Campuses
         ------------------------------------------------------- */

      {
        path: 'campuses',

        loadComponent: () =>
          import('./features/campuses/campus-list/campus-list').then((m) => m.CampusList),

        canActivate: [permissionGuard(Permissions.CampusView)],
      },

      {
        path: 'campuses/new',

        loadComponent: () =>
          import('./features/campuses/campus-create/campus-create').then((m) => m.CampusCreate),

        canActivate: [permissionGuard(Permissions.CampusCreate)],
      },

      {
        path: 'campuses/:id',

        loadComponent: () =>
          import('./features/campuses/campus-details/campus-details').then((m) => m.CampusDetails),

        canActivate: [permissionGuard(Permissions.CampusView)],
      },

      /* -------------------------------------------------------
         Users
         ------------------------------------------------------- */

      {
        path: 'users',

        loadComponent: () => import('./features/users/user-list/user-list').then((m) => m.UserList),

        canActivate: [permissionGuard(Permissions.UserView)],
      },

      {
        path: 'users/new',

        loadComponent: () =>
          import('./features/users/user-create/user-create').then((m) => m.UserCreate),

        canActivate: [permissionGuard(Permissions.UserCreate)],
      },

      {
        path: 'users/:id',

        loadComponent: () =>
          import('./features/users/user-details/user-details').then((m) => m.UserDetails),

        canActivate: [permissionGuard(Permissions.UserView)],
      },

      /* -------------------------------------------------------
         Roles
         ------------------------------------------------------- */

      {
        path: 'roles',

        loadComponent: () => import('./features/roles/role-list/role-list').then((m) => m.RoleList),

        canActivate: [permissionGuard(Permissions.RoleView)],
      },

      {
        path: 'roles/new',

        loadComponent: () =>
          import('./features/roles/role-create/role-create').then((m) => m.RoleCreate),

        canActivate: [permissionGuard(Permissions.RoleCreate)],
      },

      {
        path: 'roles/:id',

        loadComponent: () =>
          import('./features/roles/role-details/role-details').then((m) => m.RoleDetails),

        canActivate: [permissionGuard(Permissions.RoleView)],
      },

      /* -------------------------------------------------------
         Platform Profile
         ------------------------------------------------------- */

      {
        path: 'profile',

        loadComponent: () =>
          import('./features/profile/profile/profile').then((m) => m.ProfileComponent),
      },
    ],
  },

  /* =========================================================
     Institution Administration
     ========================================================= */

  {
    path: ':institutionSlug',

    component: AdminLayout,

    canActivate: [authGuard],

    children: [
      /* -------------------------------------------------------
         Dashboard
         ------------------------------------------------------- */

      {
        path: 'dashboard',

        loadComponent: () =>
          import('./features/dashboard/dashboard/dashboard').then((m) => m.Dashboard),

        canActivate: [permissionGuard(Permissions.AdminDashboardView)],
      },

      /* -------------------------------------------------------
         Students
         ------------------------------------------------------- */

      {
        path: 'students',

        loadComponent: () =>
          import('./features/students/student-list/student-list').then((m) => m.StudentList),

        canActivate: [permissionGuard(Permissions.StudentView)],
      },

      {
        path: 'students/:id',

        loadComponent: () =>
          import('./features/students/student-details/student-details').then(
            (m) => m.StudentDetails,
          ),

        canActivate: [permissionGuard(Permissions.StudentView)],
      },

      /* -------------------------------------------------------
         Teachers
         ------------------------------------------------------- */

      {
        path: 'teachers',

        loadComponent: () =>
          import('./features/teachers/teacher-list/teacher-list').then((m) => m.TeacherList),

        canActivate: [permissionGuard(Permissions.TeacherView)],
      },

      {
        path: 'teachers/:id',

        loadComponent: () =>
          import('./features/teachers/teacher-details/teacher-details').then(
            (m) => m.TeacherDetails,
          ),

        canActivate: [permissionGuard(Permissions.TeacherView)],
      },

      /* -------------------------------------------------------
         Subjects
         ------------------------------------------------------- */

      {
        path: 'subjects',

        loadComponent: () =>
          import('./features/subjects/subject-list/subject-list').then((m) => m.SubjectList),

        canActivate: [permissionGuard(Permissions.SubjectView)],
      },

      {
        path: 'subjects/:id',

        loadComponent: () =>
          import('./features/subjects/subject-details/subject-details').then(
            (m) => m.SubjectDetails,
          ),

        canActivate: [permissionGuard],

        data: {
          permission: Permissions.SubjectView,
        },
      },

      /* -------------------------------------------------------
         Default Institution Route
         ------------------------------------------------------- */

      {
        path: '',

        redirectTo: 'dashboard',

        pathMatch: 'full',
      },

      /* -------------------------------------------------------
         Campuses
         ------------------------------------------------------- */

      {
        path: 'campuses',

        loadComponent: () =>
          import('./features/campuses/campus-list/campus-list').then((m) => m.CampusList),

        canActivate: [permissionGuard(Permissions.CampusView)],
      },

      {
        path: 'campuses/new',

        loadComponent: () =>
          import('./features/campuses/campus-create/campus-create').then((m) => m.CampusCreate),

        canActivate: [permissionGuard(Permissions.CampusCreate)],
      },

      {
        path: 'campuses/:id',

        loadComponent: () =>
          import('./features/campuses/campus-details/campus-details').then((m) => m.CampusDetails),

        canActivate: [permissionGuard(Permissions.CampusView)],
      },

      /* -------------------------------------------------------
         Users
         ------------------------------------------------------- */

      {
        path: 'users',

        loadComponent: () => import('./features/users/user-list/user-list').then((m) => m.UserList),

        canActivate: [permissionGuard(Permissions.UserView)],
      },

      {
        path: 'users/new',

        loadComponent: () =>
          import('./features/users/user-create/user-create').then((m) => m.UserCreate),

        canActivate: [permissionGuard(Permissions.UserCreate)],
      },

      {
        path: 'users/:id',

        loadComponent: () =>
          import('./features/users/user-details/user-details').then((m) => m.UserDetails),

        canActivate: [permissionGuard(Permissions.UserView)],
      },

      /* -------------------------------------------------------
         Departments
         ------------------------------------------------------- */

      {
        path: 'departments',

        loadComponent: () =>
          import('./features/departments/department-list/department-list').then(
            (m) => m.DepartmentList,
          ),

        canActivate: [permissionGuard(Permissions.DepartmentView)],
      },

      {
        path: 'departments/:id',

        loadComponent: () =>
          import('./features/departments/department-details/department-details').then(
            (m) => m.DepartmentDetails,
          ),

        canActivate: [permissionGuard(Permissions.DepartmentView)],
      },

      /* -------------------------------------------------------
         Courses
         ------------------------------------------------------- */

      {
        path: 'courses',

        loadComponent: () =>
          import('./features/courses/course-list/course-list').then((m) => m.CourseList),

        canActivate: [permissionGuard(Permissions.CourseView)],
      },

      {
        path: 'courses/:id',

        loadComponent: () =>
          import('./features/courses/course-details/course-details').then((m) => m.CourseDetails),

        canActivate: [permissionGuard(Permissions.CourseView)],
      },

      /* -------------------------------------------------------
         Academic Sessions
         ------------------------------------------------------- */

      {
        path: 'academic-sessions',

        loadComponent: () =>
          import('./features/academic-sessions/academic-session-list/academic-session-list').then(
            (m) => m.AcademicSessionList,
          ),

        canActivate: [permissionGuard(Permissions.AcademicSessionView)],
      },

      {
        path: 'academic-sessions/:id',

        loadComponent: () =>
          import('./features/academic-sessions/academic-session-details/academic-session-details').then(
            (m) => m.AcademicSessionDetails,
          ),

        canActivate: [permissionGuard(Permissions.AcademicSessionView)],
      },

      /* -------------------------------------------------------
         Academic Settings
         ------------------------------------------------------- */

      {
        path: 'academic-settings',

        loadComponent: () =>
          import('./features/academic-settings/academic-settings.component').then(
            (m) => m.AcademicSettingsComponent,
          ),

        canActivate: [permissionGuard(Permissions.AcademicSettingsView)],
      },

      /* -------------------------------------------------------
         Sections
         ------------------------------------------------------- */

      {
        path: 'sections',

        loadComponent: () =>
          import('./features/sections/section-list/section-list').then((m) => m.SectionList),

        canActivate: [permissionGuard(Permissions.SectionView)],
      },

      {
        path: 'sections/:id',

        loadComponent: () =>
          import('./features/sections/section-details/section-details').then(
            (m) => m.SectionDetails,
          ),

        canActivate: [permissionGuard(Permissions.SectionView)],
      },

      /* -------------------------------------------------------
         Teacher Assignments
         ------------------------------------------------------- */

      {
        path: 'teacher-assignments',

        loadComponent: () =>
          import('./features/teacher-assignments/teacher-assignment').then(
            (m) => m.TeacherAssignmentComponent,
          ),

        canActivate: [permissionGuard(Permissions.TeacherAssignmentView)],
      },

      /* -------------------------------------------------------
         Calendar Events
         ------------------------------------------------------- */

      {
        path: 'calendar-events',

        loadComponent: () =>
          import('./features/calendar-events/calendar-event-list/calendar-event-list').then(
            (m) => m.CalendarEventListComponent,
          ),

        canActivate: [permissionGuard(Permissions.CalendarView)],
      },

      {
        path: 'calendar-events/:id',

        loadComponent: () =>
          import('./features/calendar-events/calendar-event-details/calendar-event-details').then(
            (m) => m.CalendarEventDetails,
          ),

        canActivate: [permissionGuard(Permissions.CalendarView)],
      },

      /* -------------------------------------------------------
         Timetable Templates
         ------------------------------------------------------- */

      {
        path: 'timetable-templates',

        loadComponent: () =>
          import('./features/timetable-templates/timetable-template-list/timetable-template-list').then(
            (m) => m.TimetableTemplateList,
          ),

        canActivate: [permissionGuard(Permissions.TimetableTemplateView)],
      },

      {
        path: 'timetable-templates/:id',

        loadComponent: () =>
          import('./features/timetable-templates/timetable-template-details/timetable-template-details').then(
            (m) => m.TimetableTemplateDetails,
          ),

        canActivate: [permissionGuard(Permissions.TimetableTemplateView)],
      },

      /* -------------------------------------------------------
         Timetable Calendar
         ------------------------------------------------------- */

      {
        path: 'teacher-calendar',

        loadComponent: () =>
          import('./features/timetable-calendar/timetable-calendar/timetable-calendar').then(
            (m) => m.TimetableCalendar,
          ),

        data: {
          calendarMode: 'teacher',
        },

        canActivate: [permissionGuard(Permissions.TeacherCalendarView)],
      },

      {
        path: 'student-calendar',

        loadComponent: () =>
          import('./features/timetable-calendar/timetable-calendar/timetable-calendar').then(
            (m) => m.TimetableCalendar,
          ),

        data: {
          calendarMode: 'student',
        },

        canActivate: [permissionGuard(Permissions.StudentCalendarView)],
      },

      {
        path: 'calendar-event-details',

        loadComponent: () =>
          import('./features/timetable-calendar/timetable-calendar-details/timetable-calendar-details').then(
            (m) => m.TimetableCalendarDetails,
          ),
      },

      /* -------------------------------------------------------
   Attendance
   ------------------------------------------------------- */

      {
        path: 'attendance',

        loadComponent: () =>
          import('./features/attendance/teacher-attendance/teacher-attendance').then(
            (m) => m.TeacherAttendance,
          ),

        canActivate: [permissionGuard(Permissions.AttendanceView)],
      },

      {
        path: 'attendance/sessions/:id',

        loadComponent: () =>
          import('./features/attendance/attendance-session/attendance-session').then(
            (m) => m.AttendanceSession,
          ),

        canActivate: [permissionGuard(Permissions.AttendanceView)],
      },

      {
        path: 'attendance/sessions/:id/qr',

        loadComponent: () =>
          import('./features/attendance/teacher-qr/teacher-qr').then((m) => m.TeacherQr),

        canActivate: [permissionGuard(Permissions.AttendanceManage)],
      },

      {
        path: 'attendance/qr/scan',

        loadComponent: () =>
          import('./features/attendance/student-qr-scanner/student-qr-scanner').then(
            (m) => m.StudentQrScanner,
          ),

        canActivate: [permissionGuard(Permissions.AttendanceStudentMark)],
      },

      /* -------------------------------------------------------
         Institution Profile
         ------------------------------------------------------- */

      {
        path: 'profile',

        loadComponent: () =>
          import('./features/profile/profile/profile').then((m) => m.ProfileComponent),
      },

      /* -------------------------------------------------------
   Rooms
   ------------------------------------------------------- */

      {
        path: 'rooms',

        loadComponent: () => import('./features/rooms/room-list/room-list').then((m) => m.RoomList),

        canActivate: [permissionGuard(Permissions.RoomView)],
      },

      {
        path: 'rooms/:id',

        loadComponent: () =>
          import('./features/rooms/room-details/room-details').then((m) => m.RoomDetails),

        canActivate: [permissionGuard(Permissions.RoomView)],
      },
    ],
  },

  /* =========================================================
     Fallback
     ========================================================= */

  {
    path: '**',

    component: NotFoundComponent,
  },
];
