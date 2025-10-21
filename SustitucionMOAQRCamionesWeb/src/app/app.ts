import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

import { AuthService } from './infrastructure/services/auth.service';
import { DataService } from './infrastructure/services/data.service';
import { I18nService } from './infrastructure/services/i18n.service';
import { LoadingService } from './infrastructure/services/loading.service';
import { MATERIAL } from './shared/material';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, 
    RouterLink, 
    CommonModule,
    ...MATERIAL
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class App {
  protected readonly title = signal('Angular 20 Demo');
  public readonly authService = inject(AuthService);
  public readonly dataService = inject(DataService);
  public readonly i18n = inject(I18nService);
  public readonly loadingService = inject(LoadingService);
  private readonly router = inject(Router);

  constructor() {}

  changeLanguage(locale: 'en' | 'es'): void {
    this.i18n.setLocale(locale);
  }

  logout(): void {
    // Resetear autenticación
    this.authService.logout();
    
    // Resetear todos los datos de la aplicación
    this.dataService.resetData();
    
    // Resetear estado de loading
    this.loadingService.hideLoading();
    
    // Resetear idioma a inglés (estado inicial)
    this.i18n.setLocale('en');
    
    // Navegar a la página principal
    this.router.navigate(['/']);
  }
}
