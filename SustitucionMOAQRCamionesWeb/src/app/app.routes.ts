import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './infrastructure/services/auth.service';
import { TrackingPageComponent } from './views/tracking-page/tracking-page'
import { InformationDetailComponent } from './views/information-detail/information-detail';
import { LoginComponent } from './views/login/login';
import { LoginErrorComponent } from './views/login-error/login-error';

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
    loadComponent: () => import('./views/tracking-page/tracking-page').then(m => m.TrackingPageComponent),
    title: 'QR Camiones'
  },
  {
    path: 'tracking',
    loadComponent: () => import('./views/tracking-page/tracking-page').then(m => m.TrackingPageComponent),
    title: 'QR Camiones'
  },
  {
    path: 'information-detail',
    loadComponent: () => import('./views/information-detail/information-detail').then(m => m.InformationDetailComponent),
    title: 'Información detallada'
  },
  {
    path: 'login',
    loadComponent: () => import('./views/login/login').then(m => m.LoginComponent),
    title: 'Login'
  },
  {
    path: 'login-error',
    loadComponent: () => import('./views/login-error/login-error').then(m => m.LoginErrorComponent),
    title: 'Error al loguearse'
  }
];