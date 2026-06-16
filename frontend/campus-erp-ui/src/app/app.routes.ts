import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';

import { Dashboard } from './features/dashboard/dashboard/dashboard';

import { StudentList } from './features/students/student-list/student-list';

import { TeacherList } from './features/teachers/teacher-list/teacher-list';

import { SubjectList } from './features/subjects/subject-list/subject-list';

import { AdminLayout } from './layouts/admin-layout/admin-layout';

import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },
  {
    path: '',
    component: AdminLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: Dashboard
      },
      {
        path: 'students',
        component: StudentList
      },
      {
        path: 'teachers',
        component: TeacherList
      },
      {
        path: 'subjects',
        component: SubjectList
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  }
];