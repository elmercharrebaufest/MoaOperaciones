import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';
import { TrackingData } from '../../../models/tracking-data.model';
import { MOCK_CARGO_DATA } from '../../../data/mock-tracking.data';
import { TrackingResponse } from '../../../models/tracking-response.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TrackingService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  
  trackingData = signal<TrackingData | null>(null);

  getTrackingData(ctg: string, patente: string, captcha?: string): Observable<TrackingResponse> {
    const requestBody: any = {
      ctg: ctg,
      patente: patente
    };

    if (environment.production && captcha) {
      requestBody.captcha = captcha;
    }

    return this.http
      .post<TrackingResponse>(`${this.apiUrl}/api/qrcamiones/search`, requestBody)
      .pipe(
        tap((response) => {
          if (response.resultado && response.data) {
            this.trackingData.set(response.data);
          }
        }),
        catchError((error) => {
          console.error('Error fetching tracking data:', error);
          
          return of({
            resultado: false,
            mensaje: error.message || 'Datos no encontrados',
            data: null as any
          });
        })
      );
  }

  loadMockData(): void {
    this.trackingData.set(MOCK_CARGO_DATA);
  }

  updateCurrentStage(stageIndex: number): void {
    const current = this.trackingData();
    if (current) {
      this.trackingData.set({
        ...current,
      });
    }
  }
}