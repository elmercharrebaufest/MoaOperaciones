import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './infrastructure/services/auth.service';
import { CargoTrackingPageComponent } from './views/cargo-tracking-page/cargo-tracking-page'
import { InformationDetailComponent } from './views/information-detail/information-detail';

// Functional guards
const canEnter = () => {
  try {
    const authService = inject(AuthService);
    return authService.isAuthenticated();
  } catch {
    return false;
  }
};

const adminGuard = () => {
  try {
    const authService = inject(AuthService);
    return authService.isAdmin();
  } catch {
    return false;
  }
};

export const appRoutes: Routes = [
  // {
  //   path: '',
  //   loadComponent: () => import('./views/home/home').then(m => m.Home),
  //   title: 'Home - Angular 20 Demo'
  // },
  // {
  //   path: 'auth',
  //   loadChildren: () => import('./views/auth/auth.routes').then(m => m.authRoutes),
  //   title: 'Authentication'
  // },
  // {
  //   path: 'posts',
  //   loadChildren: () => import('./views/posts/posts.routes').then(m => m.postsRoutes),
  //   title: 'Posts Management'
  // },
  // {
  //   path: 'users',
  //   loadChildren: () => import('./views/users/users.routes').then(m => m.usersRoutes),
  //   title: 'Users Management'
  // },
  // {
  //   path: 'todos',
  //   loadComponent: () => import('./views/todos/todos').then(m => m.TodosComponent),
  //   title: 'Todos Management'
  // },
  // {
  //   path: 'admin',
  //   loadChildren: () => import('./views/admin/admin.routes').then(m => m.adminRoutes),
  //   title: 'Admin Panel'
  // },
  // {
  //   path: 'about',
  //   loadComponent: () => import('./views/about/about').then(m => m.About),
  //   title: 'About'
  // },
  // { 
  //   path: '**', 
  //   redirectTo: '',
  //   title: 'Page Not Found'
  // },
  {
    path: '',
    loadComponent: () => import('./views/cargo-tracking-page/cargo-tracking-page').then(m => m.CargoTrackingPageComponent),
    title: 'QR Camiones'
  },
  {
    path: 'cargo-tracking',
    loadComponent: () => import('./views/cargo-tracking-page/cargo-tracking-page').then(m => m.CargoTrackingPageComponent),
    title: 'QR Camiones'
  },
  {
    path: 'information-detail',
    loadComponent: () => import('./views/information-detail/information-detail').then(m => m.InformationDetailComponent),
    title: 'Información detallada'
  }
];