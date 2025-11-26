import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class LoggingService {
  constructor(public apiService: ApiService) {}

  logError(error: any, context?: string): void {
    let errorMessage = '';
    let errorInfo: any = error;

    if (error instanceof Error) {
      errorInfo = {
        message: error. message,
        stack: error. stack,
        name: error. name
      };
      errorMessage = `${error.name}: ${error.message}`;
    } else if (typeof error === 'string') {
      errorMessage = error;
    } else if (error?. error?.mensaje) {
      errorMessage = error. error.mensaje;
    } else {
      errorMessage = JSON.stringify(error);
    }

    const fullMessage = context 
      ? `[${context}] ${errorMessage}` 
      : errorMessage;

    console.error('Error:', errorInfo, 'Contexto:', context);

    this.apiService. logToBackend(fullMessage, true).subscribe({
      next: () => {
        console.log('Error logged to backend successfully');
      },
      error: (err) => {
        console.error('Failed to send error log to backend:', err);
      },
    });
  }

  logInfo(message: string, context?: string): void {
    const fullMessage = context 
      ? `[${context}] ${message}` 
      : message;

    console.info('Info:', fullMessage);

    this.apiService.logToBackend(fullMessage, false).subscribe({
      next: () => {
        console.log('Info logged to backend successfully');
      },
      error: (err) => {
        console.error('Failed to send info log to backend:', err);
      },
    });
  }
}