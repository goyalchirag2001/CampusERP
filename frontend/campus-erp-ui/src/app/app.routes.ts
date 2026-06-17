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
      },

      {
        path: 'institutions',
        component: InstitutionList,
      },

      {
        path: 'institutions/new',
        component: InstitutionCreate,
      },

      {
        path: 'institutions/:id',
        component: InstitutionDetails,
      },

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
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
      },

      {
        path: 'students',
        component: StudentList,
      },

      {
        path: 'teachers',
        component: TeacherList,
      },

      {
        path: 'subjects',
        component: SubjectList,
      },

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
    ],
  },

  {
    path: '**',
    redirectTo: 'platform/login',
  },
];
