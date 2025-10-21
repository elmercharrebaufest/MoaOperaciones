import { Routes } from '@angular/router';

export const usersRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./users-list/users-list').then(m => m.UsersListComponent),
    title: 'Users List'
  }
];
