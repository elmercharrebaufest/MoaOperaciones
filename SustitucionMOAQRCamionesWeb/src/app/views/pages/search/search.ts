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
  
  ctgError = signal(false);
  patenteError = signal(false);

  recaptchaSiteKey = environment.recaptchaSiteKey;
  isProduction = environment.production;

  @ViewChild('recaptchaComponent')
  protected captcha!: RecaptchaComponent;

  constructor(
    private router: Router,
    private trackingService: TrackingService,
    private authService: AuthService
  ) {
    this.authService.logout();
  }

  handleCorrectCaptcha(event: string | null) {
    this.captchaOk = event;
    if (event) {
      this.authService.setCaptchaVerified(true);
    }
  }

  onCtgChange(value: string) {
    const hasInvalidChars = /[^0-9]/.test(value);
    this.ctgError.set(hasInvalidChars);
    
    const sanitized = value.replace(/[^0-9]/g, '');
    this.ctg.set(sanitized);
  }

  onPatenteChange(value: string) {
    const hasInvalidChars = /[^a-zA-Z0-9]/.test(value);
    this.patenteError.set(hasInvalidChars);
    
    const sanitized = value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    this.patente.set(sanitized);
  }

  onSubmit() {
    if (this.isLoading()) {
      return;
    }

    if (!this.ctg() || !this.patente()) {
      alert('Por favor, complete todos los campos');
      return;
    }

    // if (environment.production && !this.captchaOk) {
    //   alert('Debe completar el Captcha');
    //   return;
    // }

    this.isLoading.set(true);

    this.trackingService.getTrackingData(this.ctg(), this.patente())
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);
          
          if (response.resultado && response.data) {
            this.authService.login();
            this.router.navigate(['/tracking']);
          } else {
            this.authService.logout();
            this.router.navigate(['/search-error'], {
              queryParams: { mensaje: response.mensaje || 'No se encontraron datos' }
            });
          }

          if (environment.production && this.captcha) {
            this.captcha.reset();
            this.captchaOk = null;
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          this.authService.logout();
          
          const errorMessage = error.error?.mensaje || error.message || 'Error de conexión';
          console.error('Error fetching tracking data:', error);
          
          this.router.navigate(['/search-error'], {
            queryParams: { mensaje: errorMessage }
          });

          if (environment.production && this.captcha) {
            this.captcha.reset();
            this.captchaOk = null;
          }
        }
      });
  }
}