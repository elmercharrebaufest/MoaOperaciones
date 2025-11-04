import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';
import { TrackingData } from '../../../models/tracking-data.model';
import { MOCK_CARGO_DATA } from '../../../data/mock-tracking.data';
import { TrackingResponse } from '../../../models/tracking-response.model';
import { EstadoEtapas } from '../../../models/estado-etapas.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TrackingService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  
  trackingData = signal<TrackingData | null>(null);
  private isUsingInitialMock = signal<boolean>(true);

  getTrackingData(ctg: string, patente: string, captcha?: string): Observable<TrackingResponse> {
    if (!environment.production) {
      this.loadMockData();
      return of({
        resultado: true,
        mensaje: 'Datos encontrados',
        data: MOCK_CARGO_DATA
      });
    }

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
            this.isUsingInitialMock.set(false);
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
    this.isUsingInitialMock.set(true);
  }

  updateFromEstadoEtapas(estadoEtapas: EstadoEtapas): void {
    const current = this.trackingData();
    if (current) {
      this.trackingData.set({
        ...current,
        etapas: estadoEtapas.etapas,
        datosAdicionales: estadoEtapas.datosAdicionales
      });
      this.isUsingInitialMock.set(false);
    }
  }

  resetToInitialMock(): void {
    this.loadMockData();
  }

  getIsUsingInitialMock(): boolean {
    return this.isUsingInitialMock();
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