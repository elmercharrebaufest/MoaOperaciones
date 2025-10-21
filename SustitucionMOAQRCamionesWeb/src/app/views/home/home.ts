import { ChangeDetectionStrategy, Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import { AuthService } from '../../infrastructure/services/auth.service';
import { I18nService, SupportedLocale } from '../../infrastructure/services/i18n.service';
import { DataService } from '../../infrastructure/services/data.service';
import { LoadingService } from '../../infrastructure/services/loading.service';
import { MATERIAL } from '../../shared/material';


interface FeatureDemo {
  title: string;
  description: string;
  icon: string;
  route?: string;
  action?: () => void;
}

@Component({
  standalone: true,
  selector: 'app-home',
  imports: [
    CommonModule,
    RouterLink,
    ...MATERIAL
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './home.html',
  styleUrls: ['./home.scss']
})
export class Home implements OnInit {
  // Signal-based counter demo
  public readonly counter = signal(0);
  readonly doubleCounter = computed(() => this.counter() * 2);
  readonly isEven = computed(() => this.counter() % 2 === 0);

  // Features list
  readonly features = computed<FeatureDemo[]>(() => [
    {
      title: this.i18n.t('home.features.signals'),
      description: 'See how Signals provide reactive state management without Zone.js dependency.',
      icon: 'insights',
      action: () => this.loadSampleData()
    },
    {
      title: this.i18n.t('home.features.lazyLoading'),
      description: 'Explore advanced lazy loading with route-level code splitting.',
      icon: 'dynamic_feed',
      route: this.authService.isAuthenticated() ? '/posts' : '/auth/login'
    },
    {
      title: this.i18n.t('home.features.i18n'),
      description: 'Built-in internationalization with reactive language switching.',
      icon: 'language',
      action: () => this.toggleLanguage()
    },
    {
      title: this.i18n.t('home.features.ssr'),
      description: 'Server-side rendering with hydration and event replay.',
      icon: 'cloud',
      route: this.authService.isAuthenticated() ? '/todos' : '/auth/login'
    },
    {
      title: 'HTTP Interceptors',
      description: 'Modern functional interceptors for authentication and loading states.',
      icon: 'http',
      route: this.authService.isAuthenticated() ? '/posts' : '/auth/login'
    },
    {
      title: 'State Management',
      description: 'Signal-based global state management without external libraries.',
      icon: 'hub',
      route: this.authService.isAuthenticated() ? '/todos' : '/auth/login'
    }
  ]);

  constructor(
    protected readonly authService: AuthService,
    protected readonly i18n: I18nService,
    protected readonly dataService: DataService,
    protected readonly loadingService: LoadingService
  ) {}

  ngOnInit(): void {
    // Initialize data if user is authenticated
    if (this.authService.isAuthenticated()) {
      this.loadSampleData();
    }
  }

  // Counter methods
  increment(): void {
    this.counter.update(value => value + 1);
  }

  decrement(): void {
    this.counter.update(value => value - 1);
  }

  reset(): void {
    this.counter.set(0);
  }

  // Language methods
  changeLanguage(locale: SupportedLocale): void {
    this.i18n.setLocale(locale);
  }

  toggleLanguage(): void {
    const currentLocale = this.i18n.currentLocale();
    const newLocale = currentLocale === 'en' ? 'es' : 'en';
    this.i18n.setLocale(newLocale);
  }

  // Data methods
  loadSampleData(): void {
    if (this.authService.isAuthenticated()) {
      this.dataService.getPosts().subscribe();
      this.dataService.getUsers().subscribe();
      this.dataService.getTodos().subscribe();
    }
  }

  // Auth methods
  logout(): void {
    this.authService.logout();
    this.dataService.resetData();
  }
}
