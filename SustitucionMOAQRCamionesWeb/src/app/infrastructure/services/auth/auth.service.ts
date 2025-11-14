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

  constructor() { }

  login() {
    const hasData = this.trackingService.trackingData() !== null;
    if (hasData && (this.captchaVerified() || !environment.production)) {
      this.isAuthenticated.set(true);
    }
  }

  logout() {
    this.trackingService.clearTrackingData();
    this.isAuthenticated.set(false);
    this.captchaVerified.set(false);
  }

  setCaptchaVerified(verified: boolean) {
    // this.captchaVerified.set(verified);
    this.captchaVerified.set(true);
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