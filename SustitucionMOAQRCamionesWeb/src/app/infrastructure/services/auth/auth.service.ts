import { Injectable, signal, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { TrackingService } from '../external/tracking.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private trackingService = inject(TrackingService);
  private isAuthenticated = signal(false);

  constructor() {
    if (!environment.production) {
      this.trackingService.loadMockData();
      this.isAuthenticated.set(true);
    }
  }

  login() {
    this.isAuthenticated.set(true);
  }

  logout() {
    if (environment.production) {
      this.trackingService.trackingData.set(null);
      this.isAuthenticated.set(false);
    }
  }

  getIsAuthenticated() {
    return this.isAuthenticated;
  }

  getToken(): string | null {
    return null;
  }
}