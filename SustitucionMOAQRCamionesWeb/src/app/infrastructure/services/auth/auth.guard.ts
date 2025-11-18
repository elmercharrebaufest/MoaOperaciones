import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { TrackingService } from '../external/tracking.service';
import { environment } from '../../../../environments/environment';

export const authGuard = () => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const trackingService = inject(TrackingService);

  if (!environment.production && !environment.isQA) {
    return true;
  }

  const isAuthenticated = authService.getIsAuthenticated()();
  const hasData = trackingService.trackingData() !== null;

  if (isAuthenticated && hasData) {
    return true;
  }

  router.navigate(['/auth-redirect']);
  return false;
};