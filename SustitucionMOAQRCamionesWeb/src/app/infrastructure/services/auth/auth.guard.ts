import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { environment } from '../../../../environments/environment';

export const authGuard = () => {
  const router = inject(Router);
  const authService = inject(AuthService);

  if (!environment.production) {
    return true;
  }

  if (authService.getIsAuthenticated()()) {
    return true;
  }

  router.navigate(['/search']);
  return false;
};