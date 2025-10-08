import { describe, it, expect, beforeEach, vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpResponse, HttpHeaders } from '@angular/common/http';
import { signal } from '@angular/core';
import { of, throwError, firstValueFrom } from 'rxjs';
import { catchError } from 'rxjs/operators';

// Test constants
const TEST_ENDPOINTS = {
  USERS: '/api/users',
  LOGIN: '/api/auth/login',
  REGISTER: '/api/auth/register',
  PUBLIC_POSTS: '/api/public/posts',
  PROFILE: '/api/profile',
  PROTECTED: '/api/protected'
} as const;

const TEST_TOKENS = {
  SIMPLE: 'test-jwt-token',
  JWT: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c',
  SIMPLE_TOKEN: 'abc123token',
  EXPIRED: 'expired-token',
  VALID: 'valid-token',
  SERVICE: 'service-token'
} as const;

const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  UNAUTHORIZED: 401,
  INTERNAL_SERVER_ERROR: 500
} as const;

// Mock AuthService
class MockAuthService {
  private readonly tokenSignal = signal<string | null>(null);
  private readonly isAuthenticatedSignal = signal<boolean>(false);

  readonly token = this.tokenSignal.asReadonly();
  readonly isAuthenticated = this.isAuthenticatedSignal.asReadonly();

  setToken(token: string | null): void {
    this.tokenSignal.set(token);
    this.isAuthenticatedSignal.set(!!token);
  }

  logout(): void {
    this.tokenSignal.set(null);
    this.isAuthenticatedSignal.set(false);
  }
}

// Auth Interceptor implementation for testing
function createAuthInterceptor(authService: MockAuthService): HttpInterceptorFn {
  return (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
    const token = authService.token();
    
    // Skip auth for login and public endpoints
    const skipAuth = req.url.includes('/login') || 
                     req.url.includes('/register') || 
                     req.url.includes('/public');
    
    if (skipAuth || !token) {
      return next(req);
    }

    // Clone request and add authorization header
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(authReq);
  };
}

// Refresh Token Interceptor for handling token refresh
function createRefreshTokenInterceptor(authService: MockAuthService): HttpInterceptorFn {
  return (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
    return next(req).pipe(
      catchError((error) => {
        if (error.status === 401 && authService.isAuthenticated()) {
          // Token expired, logout user
          authService.logout();
          // In a real app, you might redirect to login page
          return throwError(() => new Error('Token expired. Please login again.'));
        }
        return throwError(() => error);
      })
    );
  };
}

describe('AuthInterceptor', () => {
  let authService: MockAuthService;
  let authInterceptor: HttpInterceptorFn;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    
    authService = new MockAuthService();
    authInterceptor = createAuthInterceptor(authService);
  });

  describe('Auth Token Injection', () => {
    it('should add Authorization header when token exists', () => {
      const token = TEST_TOKENS.SIMPLE;
      authService.setToken(token);
      
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: HTTP_STATUS.OK }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest).toBeDefined();
      expect(capturedRequest!.headers.get('Authorization')).toBe(`Bearer ${token}`);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should not add Authorization header when no token exists', () => {
      authService.setToken(null);
      
      const request = new HttpRequest('GET', '/api/users');
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest).toBeDefined();
      expect(capturedRequest!.headers.has('Authorization')).toBe(false);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should skip auth for login endpoint', () => {
      const token = 'test-jwt-token';
      authService.setToken(token);
      
      const request = new HttpRequest('POST', '/api/auth/login', {});
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest).toBeDefined();
      expect(capturedRequest!.headers.has('Authorization')).toBe(false);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should skip auth for register endpoint', () => {
      const token = 'test-jwt-token';
      authService.setToken(token);
      
      const request = new HttpRequest('POST', '/api/auth/register', {});
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 201 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest).toBeDefined();
      expect(capturedRequest!.headers.has('Authorization')).toBe(false);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should skip auth for public endpoints', () => {
      const token = 'test-jwt-token';
      authService.setToken(token);
      
      const request = new HttpRequest('GET', '/api/public/posts');
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest).toBeDefined();
      expect(capturedRequest!.headers.has('Authorization')).toBe(false);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });
  });

  describe('Token Formats', () => {
    it('should handle JWT token format', () => {
      const jwtToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
      authService.setToken(jwtToken);
      
      const request = new HttpRequest('GET', '/api/profile');
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest!.headers.get('Authorization')).toBe(`Bearer ${jwtToken}`);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle simple token format', () => {
      const simpleToken = 'abc123token';
      authService.setToken(simpleToken);
      
      const request = new HttpRequest('GET', '/api/profile');
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest!.headers.get('Authorization')).toBe(`Bearer ${simpleToken}`);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle empty string token as no token', () => {
      authService.setToken('');
      
      const request = new HttpRequest('GET', '/api/profile');
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest!.headers.has('Authorization')).toBe(false);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });
  });

  describe('Header Preservation', () => {
    it('should preserve existing headers', () => {
      authService.setToken('test-token');
      
      const headers = new HttpHeaders({
        'Content-Type': 'application/json',
        'X-Custom-Header': 'custom-value'
      });
      
      const request = new HttpRequest('POST', '/api/users', {}, { headers });
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest!.headers.get('Content-Type')).toBe('application/json');
      expect(capturedRequest!.headers.get('X-Custom-Header')).toBe('custom-value');
      expect(capturedRequest!.headers.get('Authorization')).toBe('Bearer test-token');
    });

    it('should override existing Authorization header', () => {
      authService.setToken('service-token');
      
      const headers = new HttpHeaders({
        'Authorization': 'Bearer existing-token'
      });
      
      const request = new HttpRequest('GET', '/api/users', null, { headers });
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(request, mockNext);
      
      expect(capturedRequest!.headers.get('Authorization')).toBe('Bearer service-token');
    });
  });

  describe('Request Cloning', () => {
    it('should create a new request instance', () => {
      authService.setToken('test-token');
      
      const originalRequest = new HttpRequest('GET', '/api/users');
      let capturedRequest: HttpRequest<unknown> | undefined;
      
      const mockNext = vi.fn().mockImplementation((req: HttpRequest<unknown>) => {
        capturedRequest = req;
        return of(new HttpResponse({ status: 200 }));
      });
      
      authInterceptor(originalRequest, mockNext);
      
      expect(capturedRequest).not.toBe(originalRequest);
      expect(capturedRequest!.url).toBe(originalRequest.url);
      expect(capturedRequest!.method).toBe(originalRequest.method);
    });
  });
});

describe('RefreshTokenInterceptor', () => {
  let authService: MockAuthService;
  let refreshInterceptor: HttpInterceptorFn;

  beforeEach(() => {
    authService = new MockAuthService();
    refreshInterceptor = createRefreshTokenInterceptor(authService);
  });

  it('should logout user on 401 error when authenticated', async () => {
    authService.setToken('expired-token');
    
    const request = new HttpRequest('GET', '/api/protected');
    
    const mockNext = vi.fn().mockReturnValue(
      throwError(() => ({ status: 401, message: 'Unauthorized' }))
    );
    
    try {
      await firstValueFrom(refreshInterceptor(request, mockNext));
    } catch (error: any) {
      expect(authService.isAuthenticated()).toBe(false);
      expect(authService.token()).toBe(null);
      expect(error.message).toBe('Token expired. Please login again.');
    }
  });

  it('should not logout on other errors', async () => {
    authService.setToken('valid-token');
    
    const request = new HttpRequest('GET', '/api/protected');
    const serverError = { status: 500, message: 'Internal Server Error' };
    
    const mockNext = vi.fn().mockReturnValue(
      throwError(() => serverError)
    );
    
    try {
      await firstValueFrom(refreshInterceptor(request, mockNext));
    } catch (error: any) {
      expect(authService.isAuthenticated()).toBe(true);
      expect(authService.token()).toBe('valid-token');
      expect(error).toBe(serverError);
    }
  });

  it('should pass through 401 errors when not authenticated', async () => {
    authService.setToken(null);
    
    const request = new HttpRequest('GET', '/api/protected');
    const unauthorizedError = { status: 401, message: 'Unauthorized' };
    
    const mockNext = vi.fn().mockReturnValue(
      throwError(() => unauthorizedError)
    );
    
    try {
      await firstValueFrom(refreshInterceptor(request, mockNext));
    } catch (error: any) {
      expect(authService.isAuthenticated()).toBe(false);
      expect(error).toBe(unauthorizedError);
    }
  });

  it('should pass through successful requests', async () => {
    authService.setToken('valid-token');
    
    const request = new HttpRequest('GET', '/api/protected');
    const successResponse = new HttpResponse({ status: 200, body: { data: 'success' } });
    
    const mockNext = vi.fn().mockReturnValue(of(successResponse));
    
    const response = await firstValueFrom(refreshInterceptor(request, mockNext));
    expect(response).toBe(successResponse);
    expect(authService.isAuthenticated()).toBe(true);
  });
});
