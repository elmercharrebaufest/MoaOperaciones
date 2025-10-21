import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthService } from '../../../infrastructure/services/auth.service';
import { I18nService } from '../../../infrastructure/services/i18n.service';
import { LoadingService } from '../../../infrastructure/services/loading.service';
import { MATERIAL } from '../../../shared/material';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  public readonly isLoading = signal(false);
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  protected readonly i18n = inject(I18nService);
  private readonly loadingService = inject(LoadingService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);


  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      this.loadingService.showLoading(this.i18n.t('auth.login'));

      const credentials = this.loginForm.value;
      
      this.authService.login(credentials).subscribe({
        next: (response) => {
          this.isLoading.set(false);
          this.loadingService.hideLoading();
          this.snackBar.open(this.i18n.t('auth.loginSuccess'), 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.router.navigate(['/']);
        },
        error: (error) => {
          this.isLoading.set(false);
          this.loadingService.hideLoading();
          this.snackBar.open(this.i18n.t('auth.loginError'), 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
          console.error('Login error:', error);
        }
      });
    }
  }
}
