import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent),
    title: 'Admin Dashboard'
  }
];
