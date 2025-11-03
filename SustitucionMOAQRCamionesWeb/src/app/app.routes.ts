import { Routes } from '@angular/router';
import { authGuard } from './infrastructure/services/auth/auth.guard';

// Functional guards
// const canEnter = () => {
//   try {
//     const authService = inject(AuthService);
//     return authService.isAuthenticated();
//   } catch {
//     return false;
//   }
// };

// const adminGuard = () => {
//   try {
//     const authService = inject(AuthService);
//     return authService.isAdmin();
//   } catch {
//     return false;
//   }
// };

export const appRoutes: Routes = [
  {
    path: '',
    redirectTo: 'search',
    pathMatch: 'full'
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
  },
  {
    path: 'redirect',
    loadComponent: () => import('./views/pages/auth-redirect/auth-redirect').then(m => m.AuthRedirectComponent),
    title: 'Redirigiendo'
  },
  {
    path: 'tracking',
    loadComponent: () => import('./views/pages/tracking/tracking').then(m => m.TrackingComponent),
    canActivate: [authGuard],
    title: 'Seguimiento en planta'
  },
  {
    path: 'information',
    loadComponent: () => import('./views/pages/information/information').then(m => m.InformationComponent),
    canActivate: [authGuard],
    title: 'Información detallada'
  },
  {
    path: '**',
    redirectTo: 'search'
  }
];