import { Component, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RecaptchaModule, RecaptchaComponent } from "ng-recaptcha-2";
import { environment } from '../../../../environments/environment';
import { TrackingService } from '../../../infrastructure/services/external/tracking.service';
import { AuthService } from '../../../infrastructure/services/auth/auth.service';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule, RecaptchaModule],
  templateUrl: './search.html',
  styleUrls: ['./search.scss']
})
export class SearchComponent {
  ctg = signal('');
  patente = signal('');
  isLoading = signal(false);
  captchaOk: string | null = null;

  recaptchaSiteKey = environment.recaptchaSiteKey;
  isProduction = environment.production;

  @ViewChild('recaptchaComponent')
  protected captcha!: RecaptchaComponent;

  constructor(
    private router: Router,
    private trackingService: TrackingService,
    private authService: AuthService
  ) {
    if (environment.production) {
      this.authService.logout();
    }
  }

  handleCorrectCaptcha(event: string | null) {
    this.captchaOk = event;
  }

  onSubmit() {
    if (!this.ctg() || !this.patente()) {
      alert('Por favor, complete todos los campos');
      return;
    }

    if (environment.production && !this.captchaOk) {
      alert('Debe completar el Captcha');
      return;
    }

    this.isLoading.set(true);

    this.trackingService.getTrackingData(this.ctg(), this.patente(), this.captchaOk || undefined)
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);
          
          if (response.resultado && response.data) {
            this.authService.login();
            this.router.navigate(['/tracking']);
          } else {
            this.router.navigate(['/search-error'], {
              queryParams: { mensaje: response.mensaje }
            });
          }

          if (environment.production && this.captcha) {
            this.captcha.reset();
            this.captchaOk = null;
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          this.router.navigate(['/search-error'], {
            queryParams: { mensaje: error.error?.mensaje || 'Error de conexión' }
          });

          if (environment.production && this.captcha) {
            this.captcha.reset();
            this.captchaOk = null;
          }
        }
      });
  }
}