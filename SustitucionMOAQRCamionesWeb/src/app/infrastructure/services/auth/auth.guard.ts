import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { ApiService } from '../external/api.service';
import { CookieService } from '../internal/cookie.service';
import { environment } from '../../../../environments/environment';

export const authGuard = () => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const apiService = inject(ApiService);
  const cookieService = inject(CookieService);

  if (!environment.production && !environment.isQA) {
    return true;
  }

  const isAuthenticated = authService.getIsAuthenticated()();
  const hasData = apiService.trackingData() !== null;

  if (isAuthenticated && hasData) {
    return true;
  }

  const ctg = cookieService.getCookie('ctg');
  const patente = cookieService.getCookie('patente');
  
  if (ctg && patente) {
    return true;
  }

  router.navigate(['/redirect']);
  return false;
};