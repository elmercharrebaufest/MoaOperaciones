import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../services/loading.service';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);
  
  // Skip loading for certain endpoints or if header is present
  const shouldSkipLoading = 
    req.url.includes('/ping') ||
    req.url.includes('/health') ||
    req.headers.has('X-Skip-Loading');
  
  if (!shouldSkipLoading) {
    // Start loading
    loadingService.increment();
  }

  return next(req).pipe(
    finalize(() => {
      if (!shouldSkipLoading) {
        // Stop loading when request completes (success or error)
        loadingService.decrement();
      }
    })
  );
};
