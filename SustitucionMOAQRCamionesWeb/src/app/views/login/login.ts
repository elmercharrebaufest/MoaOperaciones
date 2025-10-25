import { Component, signal, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { RecaptchaModule, RecaptchaComponent } from "ng-recaptcha-2";
import { environment } from '../../../environments/environment';

interface LoginRequest {
  ctg: string;
  patente: string;
}

interface LoginResponse {
  resultado: boolean;
  mensaje: string;
  data: any;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RecaptchaModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  ctg = signal('');
  patente = signal('');
  isLoading = signal(false);
  captchaOk: any = null;

  recaptchaSiteKey = environment.recaptchaSiteKey;
  isProduction = environment.production;

  @ViewChild('recaptchaComponent')
  protected captcha!: RecaptchaComponent;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  handleCorrectCaptcha(event: any) {
    this.captchaOk = event;
  }

  onSubmit() {
    if (!this.ctg() || !this.patente()) {
      alert('Por favor, complete todos los campos');
      return;
    }

    if (environment.production) {
      if (this.captchaOk == null) {
        alert('Debe completar el Captcha');
        return;
      }
    }

    this.isLoading.set(true);

    const loginData: LoginRequest = {
      ctg: this.ctg(),
      patente: this.patente()
    };

    this.http.post<LoginResponse>(`${environment.apiUrl}/api/qrcamiones/login`, loginData)
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);
          
          if (response.resultado && response.data) {
            this.router.navigate(['/cargo-tracking'], { 
              state: { trackingData: response.data } 
            });
          } else {
            this.router.navigate(['/error-login'], {
              queryParams: { mensaje: response.mensaje }
            });
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          this.router.navigate(['/login-error'], {
            queryParams: { mensaje: error.error?.mensaje || 'Error de conexión' }
          });
        }
      });
  }
}