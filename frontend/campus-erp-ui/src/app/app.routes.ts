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

export const routes: Routes = [
  {
    path: 'platform/login',
    component: Login,
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
        path: 'teachers',
        component: TeacherList,
        canActivate: [permissionGuard(Permissions.TeacherView)],
      },

      {
        path: 'subjects',
        component: SubjectList,
        canActivate: [permissionGuard(Permissions.SubjectView)],
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
      },

      {
        path: 'departments/:id',
        component: DepartmentDetails,
      },
    ],
  },

  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
