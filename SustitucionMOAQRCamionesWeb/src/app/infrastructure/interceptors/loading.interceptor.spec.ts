import { describe, it, expect, beforeEach, vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpResponse, HttpHeaders } from '@angular/common/http';
import { signal } from '@angular/core';
import { of, throwError, delay, Observable, firstValueFrom } from 'rxjs';
import { finalize } from 'rxjs/operators';

// Test constants
const TEST_ENDPOINTS = {
  USERS: '/api/users',
  POSTS: '/api/posts',
  TODOS: '/api/todos',
  PING: '/api/ping',
  HEALTH: '/api/health'
} as const;

const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  NOT_FOUND: 404,
  INTERNAL_SERVER_ERROR: 500
} as const;

const DELAYS = {
  SHORT: 100,
  MEDIUM: 200,
  LONG: 1000
} as const;

// Mock LoadingService
class MockLoadingService {
  private readonly loadingSignal = signal<boolean>(false);
  private readonly loadingCountSignal = signal<number>(0);

  readonly isLoading = this.loadingSignal.asReadonly();
  readonly loadingCount = this.loadingCountSignal.asReadonly();

  increment(): void {
    const currentCount = this.loadingCountSignal();
    this.loadingCountSignal.set(currentCount + 1);
    this.loadingSignal.set(true);
  }

  decrement(): void {
    const currentCount = this.loadingCountSignal();
    const newCount = Math.max(0, currentCount - 1);
    this.loadingCountSignal.set(newCount);
    this.loadingSignal.set(newCount > 0);
  }

  reset(): void {
    this.loadingCountSignal.set(0);
    this.loadingSignal.set(false);
  }

  // Legacy methods for backward compatibility
  show(): void {
    this.increment();
  }

  hide(): void {
    this.decrement();
  }
}

// Loading Interceptor implementation for testing
function createLoadingInterceptor(loadingService: MockLoadingService): HttpInterceptorFn {
  return (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
    // Skip loading for certain endpoints
    const skipLoading = req.url.includes('/ping') || 
                       req.url.includes('/health') ||
                       req.headers.has('X-Skip-Loading');
    
    if (skipLoading) {
      return next(req);
    }

    // Show loading
    loadingService.increment();

    // Handle the request and hide loading when done
    return next(req).pipe(
      finalize(() => {
        loadingService.decrement();
      })
    );
  };
}

// Helper functions to avoid deep nesting
function createControlledSuccessObservable(onComplete: (completeFn: () => void) => void): Observable<HttpResponse<any>> {
  return new Observable(subscriber => {
    const complete = () => {
      subscriber.next(new HttpResponse({ status: HTTP_STATUS.OK }));
      subscriber.complete();
    };
    onComplete(complete);
  });
}

function createControlledErrorObservable(onError: (errorFn: () => void) => void): Observable<never> {
  return new Observable(subscriber => {
    const error = () => {
      subscriber.error(new Error('Second request failed'));
    };
    onError(error);
  });
}

function createControlledObservable(onSetup: (completeFn: () => void) => void): Observable<HttpResponse<any>> {
  return new Observable(subscriber => {
    const complete = () => {
      subscriber.next(new HttpResponse({ status: HTTP_STATUS.OK }));
      subscriber.complete();
    };
    onSetup(complete);
  });
}

describe('LoadingInterceptor', () => {
  let loadingService: MockLoadingService;
  let loadingInterceptor: HttpInterceptorFn;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    
    loadingService = new MockLoadingService();
    loadingInterceptor = createLoadingInterceptor(loadingService);
  });

  describe('Basic Loading Behavior', () => {
    it('should show loading when request starts', () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK }))
      );
      
      expect(loadingService.isLoading()).toBe(false);
      
      loadingInterceptor(request, mockNext);
      
      expect(loadingService.isLoading()).toBe(true);
      expect(loadingService.loadingCount()).toBe(1);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should hide loading when request completes successfully', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK }))
      );
      
      const response = await firstValueFrom(loadingInterceptor(request, mockNext)) as HttpResponse<any>;
      
      expect(response.status).toBe(HTTP_STATUS.OK);
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should hide loading when request fails', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        throwError(() => new Error('Request failed'))
      );
      
      try {
        await firstValueFrom(loadingInterceptor(request, mockNext));
      } catch (error: any) {
        expect(error.message).toBe('Request failed');
      }
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });
  });

  describe('Multiple Concurrent Requests', () => {
    it('should handle multiple concurrent requests', async () => {
      const request1 = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      const request2 = new HttpRequest('GET', TEST_ENDPOINTS.POSTS);
      
      const mockNext1 = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK })).pipe(delay(DELAYS.SHORT))
      );
      
      const mockNext2 = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK })).pipe(delay(DELAYS.MEDIUM))
      );
      
      // Start both requests
      const promise1 = firstValueFrom(loadingInterceptor(request1, mockNext1));
      expect(loadingService.loadingCount()).toBe(1);
      expect(loadingService.isLoading()).toBe(true);
      
      const promise2 = firstValueFrom(loadingInterceptor(request2, mockNext2));
      expect(loadingService.loadingCount()).toBe(2);
      expect(loadingService.isLoading()).toBe(true);
      
      // Wait for first request to complete
      await promise1;
      expect(loadingService.loadingCount()).toBe(1);
      expect(loadingService.isLoading()).toBe(true); // Still loading because of second request
      
      // Wait for second request to complete
      await promise2;
      expect(loadingService.loadingCount()).toBe(0);
      expect(loadingService.isLoading()).toBe(false);
    });

    it('should handle mixed success and failure requests', () => {
      const request1 = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      const request2 = new HttpRequest('GET', TEST_ENDPOINTS.POSTS);
      
      // Create observables that we can control manually
      let complete1: () => void = () => {};
      let error2: () => void = () => {};
      
      const mockNext1 = vi.fn().mockReturnValue(
        createControlledSuccessObservable((completeFn) => { complete1 = completeFn; })
      );
      
      const mockNext2 = vi.fn().mockReturnValue(
        createControlledErrorObservable((errorFn) => { error2 = errorFn; })
      );
      
      // Start both requests
      const observable1 = loadingInterceptor(request1, mockNext1);
      const observable2 = loadingInterceptor(request2, mockNext2);
      
      // Subscribe to both
      observable1.subscribe();
      observable2.subscribe({
        error: () => {} // Handle the error
      });
      
      // At this point both should be loading
      expect(loadingService.loadingCount()).toBe(2);
      expect(loadingService.isLoading()).toBe(true);
      
      // Complete both requests
      complete1();
      error2();
      
      // Now both should be finished
      expect(loadingService.loadingCount()).toBe(0);
      expect(loadingService.isLoading()).toBe(false);
    });
  });

  describe('Skip Loading Scenarios', () => {
    it('should skip loading for ping endpoint', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.PING);
      
      const mockNext = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK }))
      );
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should skip loading for health endpoint', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.HEALTH);
      
      const mockNext = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK }))
      );
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should skip loading when X-Skip-Loading header is present', async () => {
      const headers = new HttpHeaders({ 'X-Skip-Loading': 'true' });
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS, null, { headers });
      
      const mockNext = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK }))
      );
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should not skip loading for regular endpoints', () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      let completeRequest: () => void = () => {};
      
      const mockNext = vi.fn().mockReturnValue(
        createControlledObservable((completeFn) => { completeRequest = completeFn; })
      );
      
      const observable = loadingInterceptor(request, mockNext);
      
      // Subscribe to trigger the interceptor logic
      observable.subscribe();
      
      // Should show loading during request (before completion)
      expect(loadingService.isLoading()).toBe(true);
      expect(loadingService.loadingCount()).toBe(1);
      expect(mockNext).toHaveBeenCalledTimes(1);
      
      // Complete the request
      completeRequest();
      
      // Should hide loading after completion
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
    });
  });

  describe('Loading Service Integration', () => {
    it('should correctly increment loading count', () => {
      const requests = [
        new HttpRequest('GET', TEST_ENDPOINTS.USERS),
        new HttpRequest('GET', TEST_ENDPOINTS.POSTS),
        new HttpRequest('GET', TEST_ENDPOINTS.TODOS)
      ];
      
      const mockNexts = requests.map(() => 
        vi.fn().mockReturnValue(of(new HttpResponse({ status: HTTP_STATUS.OK })).pipe(delay(DELAYS.LONG)))
      );
      
      // Start all requests
      requests.forEach((request, index) => {
        loadingInterceptor(request, mockNexts[index]);
        expect(loadingService.loadingCount()).toBe(index + 1);
        expect(loadingService.isLoading()).toBe(true);
      });
    });

    it('should handle service reset during active requests', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        of(new HttpResponse({ status: HTTP_STATUS.OK })).pipe(delay(DELAYS.SHORT))
      );
      
      const promise = firstValueFrom(loadingInterceptor(request, mockNext));
      
      expect(loadingService.isLoading()).toBe(true);
      expect(loadingService.loadingCount()).toBe(1);
      
      // Reset service while request is active
      loadingService.reset();
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      
      // Request should still complete normally
      await promise;
      
      // After completion, service should remain at 0 (won't go negative)
      expect(loadingService.loadingCount()).toBe(0);
      expect(loadingService.isLoading()).toBe(false);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });
  });

  describe('Error Handling', () => {
    it('should handle network errors properly', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        throwError(() => new Error('Network error'))
      );
      
      try {
        await firstValueFrom(loadingInterceptor(request, mockNext));
      } catch (error: any) {
        expect(error.message).toBe('Network error');
      }
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle HTTP error responses', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        throwError(() => ({ status: HTTP_STATUS.NOT_FOUND, statusText: 'Not Found' }))
      );
      
      try {
        await firstValueFrom(loadingInterceptor(request, mockNext));
      } catch (error: any) {
        expect(error.status).toBe(HTTP_STATUS.NOT_FOUND);
      }
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle timeout errors', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      
      const mockNext = vi.fn().mockReturnValue(
        throwError(() => new Error('Timeout'))
      );
      
      try {
        await firstValueFrom(loadingInterceptor(request, mockNext));
      } catch (error: any) {
        expect(error.message).toBe('Timeout');
      }
      
      expect(loadingService.isLoading()).toBe(false);
      expect(loadingService.loadingCount()).toBe(0);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });
  });

  describe('Request Types', () => {
    it('should handle GET requests', async () => {
      const request = new HttpRequest('GET', TEST_ENDPOINTS.USERS);
      const mockNext = vi.fn().mockReturnValue(of(new HttpResponse({ status: HTTP_STATUS.OK })));
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      expect(mockNext).toHaveBeenCalledWith(request);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle POST requests', async () => {
      const request = new HttpRequest('POST', TEST_ENDPOINTS.USERS, { name: 'John' });
      const mockNext = vi.fn().mockReturnValue(of(new HttpResponse({ status: HTTP_STATUS.CREATED })));
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      expect(mockNext).toHaveBeenCalledWith(request);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle PUT requests', async () => {
      const request = new HttpRequest('PUT', `${TEST_ENDPOINTS.USERS}/1`, { name: 'John Updated' });
      const mockNext = vi.fn().mockReturnValue(of(new HttpResponse({ status: HTTP_STATUS.OK })));
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      expect(mockNext).toHaveBeenCalledWith(request);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });

    it('should handle DELETE requests', async () => {
      const request = new HttpRequest('DELETE', `${TEST_ENDPOINTS.USERS}/1`);
      const mockNext = vi.fn().mockReturnValue(of(new HttpResponse({ status: HTTP_STATUS.NO_CONTENT })));
      
      await firstValueFrom(loadingInterceptor(request, mockNext));
      expect(mockNext).toHaveBeenCalledWith(request);
      expect(mockNext).toHaveBeenCalledTimes(1);
    });
  });
});
