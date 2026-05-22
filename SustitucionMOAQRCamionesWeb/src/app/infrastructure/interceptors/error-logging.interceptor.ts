import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoggingService } from '../services/logging.service';
import { catchError, throwError } from 'rxjs';

export const errorLoggingInterceptor: HttpInterceptorFn = (req, next) => {
  const loggingService = inject(LoggingService);

  return next(req).pipe(
    catchError((error) => {
      if (! req.url.includes('/api/qrcamiones/log')) {
        loggingService.logError(error, `HTTP error en ${req.url}`);
      } else {
        console.error('Logging endpoint failed:', error);
      }
      
      return throwError(() => error);
    })
  );
};
