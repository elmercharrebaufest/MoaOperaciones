import { Injectable, signal, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { TrackingService } from '../external/tracking.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private trackingService = inject(TrackingService);
  private isAuthenticated = signal(false);
  private captchaVerified = signal(false);

  constructor() {
    if (!environment.production) {
      this.trackingService.loadMockData();
      this.isAuthenticated.set(true);
      this.captchaVerified.set(true);
    }
  }

  login() {
    const hasData = this.trackingService.trackingData() !== null;
    if (hasData && (this.captchaVerified() || !environment.production)) {
      this.isAuthenticated.set(true);
    }
  }

  logout() {
    if (environment.production) {
      this.trackingService.trackingData.set(null);
      this.isAuthenticated.set(false);
      this.captchaVerified.set(false);
    }
  }

  setCaptchaVerified(verified: boolean) {
    this.captchaVerified.set(verified);
  }

  getIsAuthenticated() {
    return this.isAuthenticated;
  }

  getCaptchaVerified() {
    return this.captchaVerified;
  }

  isFullyAuthenticated(): boolean {
    const hasData = this.trackingService.trackingData() !== null;
    const captchaOk = this.captchaVerified() || !environment.production;
    return this.isAuthenticated() && hasData && captchaOk;
  }

  getToken(): string | null {
    return null;
  }
}