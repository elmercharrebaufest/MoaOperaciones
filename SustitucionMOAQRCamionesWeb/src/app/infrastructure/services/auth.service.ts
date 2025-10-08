import { Injectable, signal, computed, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Observable } from 'rxjs';

export interface User {
  id: number;
  email: string;
  name: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
  expiresIn: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = 'https://jsonplaceholder.typicode.com';
  
  // Signals para el estado de autenticación
  private readonly _isAuthenticated = signal(false);
  private readonly _currentUser = signal<User | null>(null);
  private readonly _token = signal<string | null>(null);

  // Computed signals
  readonly isAuthenticated = this._isAuthenticated.asReadonly();
  readonly currentUser = this._currentUser.asReadonly();
  readonly userRole = computed(() => this._currentUser()?.role || 'guest');
  readonly isAdmin = computed(() => this.userRole() === 'admin');

  constructor(@Inject(PLATFORM_ID) private readonly platformId: Object) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    // Solo acceder a localStorage en el navegador
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem('auth_token');
      const user = localStorage.getItem('auth_user');
      
      if (token && user) {
        this._token.set(token);
        this._currentUser.set(JSON.parse(user));
        this._isAuthenticated.set(true);
      }
    }
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    // Simulando login con JSONPlaceholder
    return new Observable(observer => {
      setTimeout(() => {
        const mockResponse: AuthResponse = {
          token: 'mock_jwt_token_' + Date.now(),
          user: {
            id: 1,
            email: credentials.email,
            name: 'Usuario Demo',
            role: credentials.email.includes('admin') ? 'admin' : 'user'
          },
          expiresIn: 3600
        };

        // Guardar en localStorage solo en el navegador
        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('auth_token', mockResponse.token);
          localStorage.setItem('auth_user', JSON.stringify(mockResponse.user));
        }

        // Actualizar signals
        this._token.set(mockResponse.token);
        this._currentUser.set(mockResponse.user);
        this._isAuthenticated.set(true);

        observer.next(mockResponse);
        observer.complete();
      }, 1000); // Simular delay de red
    });
  }

  logout(): void {
    // Remover del localStorage solo en el navegador
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('auth_user');
    }
    
    this._token.set(null);
    this._currentUser.set(null);
    this._isAuthenticated.set(false);
  }

  getToken(): string | null {
    return this._token();
  }

  refreshToken(): Observable<AuthResponse> {
    // Implementar lógica de refresh token
    return new Observable(observer => {
      // Simulación de refresh
      observer.next({
        token: 'refreshed_token_' + Date.now(),
        user: this._currentUser()!,
        expiresIn: 3600
      });
    });
  }
}
