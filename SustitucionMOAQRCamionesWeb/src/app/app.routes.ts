import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './infrastructure/services/auth.service';
import { TrackingComponent } from './views/pages/tracking/tracking'
import { InformationComponent } from './views/pages/information/information';
import { SearchComponent } from './views/pages/search/search';
import { SearchErrorComponent } from './views/pages/search-error/search-error';

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
    loadComponent: () => import('./views/pages/tracking/tracking').then(m => m.TrackingComponent),
    title: 'Seguimiento en planta'
  },
  {
    path: 'tracking',
    loadComponent: () => import('./views/pages/tracking/tracking').then(m => m.TrackingComponent),
    title: 'Seguimiento en planta'
  },
  {
    path: 'information',
    loadComponent: () => import('./views/pages/information/information').then(m => m.InformationComponent),
    title: 'Información detallada'
  },
  {
    path: 'search',
    loadComponent: () => import('./views/pages/search/search').then(m => m.SearchComponent),
    title: 'Busqueda por CTG y Patente'
  },
  {
    path: 'search-error',
    loadComponent: () => import('./views/pages/search-error/search-error').then(m => m.SearchErrorComponent),
    title: 'Error al buscar'
  }
];