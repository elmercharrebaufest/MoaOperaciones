import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
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
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  ctg = signal('');
  patente = signal('');
  isLoading = signal(false);
  recaptchaVerified = signal(false);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadRecaptchaScript();
  }

  loadRecaptchaScript() {
    const script = document.createElement('script');
    script.src = 'https://www.google.com/recaptcha/api.js';
    script.async = true;
    script.defer = true;
    document.head.appendChild(script);

    // Setup callback for reCAPTCHA
    (window as any).onRecaptchaSuccess = () => {
      this.recaptchaVerified.set(true);
    };
  }

  onSubmit() {
    if (!this.ctg() || !this.patente()) {
      alert('Por favor, complete todos los campos');
      return;
    }

    if (!this.recaptchaVerified()) {
      alert('Por favor, complete el reCAPTCHA');
      return;
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
            // Success with data - navigate to cargo tracking page
            this.router.navigate(['/cargo-tracking'], { 
              state: { trackingData: response.data } 
            });
          } else {
            // Success but no data or error - navigate to error page
            this.router.navigate(['/error-login'], {
              queryParams: { mensaje: response.mensaje }
            });
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          this.router.navigate(['/error-login'], {
            queryParams: { mensaje: error.error?.mensaje || 'Error de conexión' }
          });
        }
      });
  }
}