import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { StudentList } from './features/students/student-list/student-list';
import { TeacherList } from './features/teachers/teacher-list/teacher-list';
import { SubjectList } from './features/subjects/subject-list/subject-list';
import { InstitutionList } from './features/institutions/institution-list/institution-list';
import { InstitutionCreate } from './features/institutions/institution-create/institution-create';
import { InstitutionDetails } from './features/institutions/institution-details/institution-details';
import { AdminLayout } from './layouts/admin-layout/admin-layout';
import { authGuard } from './core/guards/auth-guard';
import { CampusCreate } from './features/campuses/campus-create/campus-create';
import { CampusDetails } from './features/campuses/campus-details/campus-details';
import { CampusList } from './features/campuses/campus-list/campus-list';
import { UserList } from './features/users/user-list/user-list';
import { UserCreate } from './features/users/user-create/user-create';
import { UserDetails } from './features/users/user-details/user-details';
import { RoleList } from './features/roles/role-list/role-list';
import { RoleCreate } from './features/roles/role-create/role-create';
import { RoleDetails } from './features/roles/role-details/role-details';
import { permissionGuard } from './core/guards/permission-guard';
import { Permissions } from './core/constants/permissions';
import { DepartmentList } from './features/departments/department-list/department-list';
import { DepartmentDetails } from './features/departments/department-details/department-details';
import { CourseList } from './features/courses/course-list/course-list';
import { CourseDetails } from './features/courses/course-details/course-details';
import { SubjectDetails } from './features/subjects/subject-details/subject-details';
import { TeacherDetails } from './features/teachers/teacher-details/teacher-details';
import { StudentDetails } from './features/students/student-details/student-details';
import { ProfileComponent } from './features/profile/profile/profile';
import { ForbiddenComponent } from './shared/pages/forbidden/forbidden';
import { NotFoundComponent } from './shared/pages/not-found/not-found';
import { AcademicSessionList } from './features/academic-sessions/academic-session-list/academic-session-list';
import { AcademicSessionDetails } from './features/academic-sessions/academic-session-details/academic-session-details';
import { SectionList } from './features/sections/section-list/section-list';
import { SectionDetails } from './features/sections/section-details/section-details';

export const routes: Routes = [
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

  {
    path: 'platform',
    component: AdminLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
        canActivate: [permissionGuard(Permissions.AdminDashboardView)],
      },

      {
        path: 'institutions',
        component: InstitutionList,
        canActivate: [permissionGuard(Permissions.InstitutionView)],
      },
      {
        path: 'institutions/new',
        component: InstitutionCreate,
        canActivate: [permissionGuard(Permissions.InstitutionCreate)],
      },
      {
        path: 'institutions/:id',
        component: InstitutionDetails,
        canActivate: [permissionGuard(Permissions.InstitutionView)],
      },

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },

      {
        path: 'campuses',
        component: CampusList,
        canActivate: [permissionGuard(Permissions.CampusView)],
      },
      {
        path: 'campuses/new',
        component: CampusCreate,
        canActivate: [permissionGuard(Permissions.CampusCreate)],
      },
      {
        path: 'campuses/:id',
        component: CampusDetails,
        canActivate: [permissionGuard(Permissions.CampusView)],
      },
      {
        path: 'users',
        component: UserList,
        canActivate: [permissionGuard(Permissions.UserView)],
      },
      {
        path: 'users/new',
        component: UserCreate,
        canActivate: [permissionGuard(Permissions.UserCreate)],
      },
      {
        path: 'users/:id',
        component: UserDetails,
        canActivate: [permissionGuard(Permissions.UserView)],
      },
      {
        path: 'roles',
        component: RoleList,
        canActivate: [permissionGuard(Permissions.RoleView)],
      },
      {
        path: 'roles/new',
        component: RoleCreate,
        canActivate: [permissionGuard(Permissions.RoleCreate)],
      },
      {
        path: 'roles/:id',
        component: RoleDetails,
        canActivate: [permissionGuard(Permissions.RoleView)],
      },

      {
        path: 'profile',
        component: ProfileComponent,
      },
    ],
  },

  {
    path: ':institutionSlug',
    component: AdminLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
        canActivate: [permissionGuard(Permissions.AdminDashboardView)],
      },

      {
        path: 'students',
        component: StudentList,
        canActivate: [permissionGuard(Permissions.StudentView)],
      },

      {
        path: 'students/:id',
        component: StudentDetails,
        canActivate: [permissionGuard(Permissions.StudentView)],
      },

      {
        path: 'teachers',
        component: TeacherList,
        canActivate: [permissionGuard(Permissions.TeacherView)],
      },

      {
        path: 'teachers/:id',
        component: TeacherDetails,
        canActivate: [permissionGuard(Permissions.TeacherView)],
      },

      {
        path: 'subjects',
        component: SubjectList,
        canActivate: [permissionGuard(Permissions.SubjectView)],
      },

      {
        path: 'subjects/:id',
        component: SubjectDetails,
        canActivate: [permissionGuard],
        data: {
          permission: Permissions.SubjectView,
        },
      },

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },

      {
        path: 'campuses',
        component: CampusList,
        canActivate: [permissionGuard(Permissions.CampusView)],
      },
      {
        path: 'campuses/new',
        component: CampusCreate,
        canActivate: [permissionGuard(Permissions.CampusCreate)],
      },
      {
        path: 'campuses/:id',
        component: CampusDetails,
        canActivate: [permissionGuard(Permissions.CampusView)],
      },
      {
        path: 'users',
        component: UserList,
        canActivate: [permissionGuard(Permissions.UserView)],
      },
      {
        path: 'users/new',
        component: UserCreate,
        canActivate: [permissionGuard(Permissions.UserCreate)],
      },
      {
        path: 'users/:id',
        component: UserDetails,
        canActivate: [permissionGuard(Permissions.UserView)],
      },

      {
        path: 'departments',
        component: DepartmentList,
        canActivate: [permissionGuard(Permissions.DepartmentView)],
      },

      {
        path: 'departments/:id',
        component: DepartmentDetails,
        canActivate: [permissionGuard(Permissions.DepartmentView)],
      },

      {
        path: 'courses',
        component: CourseList,
        canActivate: [permissionGuard(Permissions.CourseView)],
      },

      {
        path: 'courses/:id',
        component: CourseDetails,
        canActivate: [permissionGuard(Permissions.CourseView)],
      },

      {
        path: 'academic-sessions',
        component: AcademicSessionList,
        canActivate: [permissionGuard(Permissions.AcademicSessionView)],
      },

      {
        path: 'academic-sessions/:id',
        component: AcademicSessionDetails,
        canActivate: [permissionGuard(Permissions.AcademicSessionView)],
      },

      {
        path: 'sections',
        component: SectionList,
        canActivate: [permissionGuard(Permissions.SectionView)],
      },

      {
        path: 'sections/:id',
        component: SectionDetails,
        canActivate: [permissionGuard(Permissions.SectionView)],
      },

      {
        path: 'profile',
        component: ProfileComponent,
      },
    ],
  },

  {
    path: '**',
    component: NotFoundComponent,
  },
];
