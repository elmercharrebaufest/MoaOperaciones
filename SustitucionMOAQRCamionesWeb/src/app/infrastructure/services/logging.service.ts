import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class LoggingService {
  constructor(public apiService: ApiService) {}

  logError(error: any, context?: string): void {
  let errorInfo = error;
  if (error instanceof Error) {
    errorInfo = {
      message: error.message,
      stack: error.stack,
      name: error.name
    };
  }
  console.error('Error:', errorInfo, 'Contexto:', context);
  this.apiService.logError(context ?? '', errorInfo).subscribe({
    next: () => {},
    error: (err) => console.error('Error enviando log al backend:', err),
  });
}
}
