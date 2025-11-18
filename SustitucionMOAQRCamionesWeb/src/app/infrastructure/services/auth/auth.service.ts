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
    if (!environment.production && !environment.isQA) {
      this.captchaVerified.set(true);
    }
  }

  login() {
    const hasData = this.trackingService.trackingData() !== null;
    const needsCaptcha = environment.production || environment.isQA;
    if (hasData && (this.captchaVerified() || !needsCaptcha)) {
      this.isAuthenticated.set(true);
    }
  }

  logout() {
    this.trackingService.clearTrackingData();
    this.isAuthenticated.set(false);

    if (!environment.production && !environment.isQA) {
      this.captchaVerified.set(true);
    } else {
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
    const needsCaptcha = environment.production || environment.isQA;
    const captchaOk = this.captchaVerified() || !needsCaptcha;
    return this.isAuthenticated() && hasData && captchaOk;
  }

  getToken(): string | null {
    return null;
  }
}