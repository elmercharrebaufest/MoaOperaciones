import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { catchError, Observable, of, tap } from 'rxjs';
import { TrackingData } from '../../../models/tracking-data.model';
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

  getTrackingData(ctg: string, patente: string, captcha?: string): Observable<TrackingResponse> {
    let params = new HttpParams()
      .set('request.ctg', ctg)
      .set('request.patente', patente)
      .set('request.tipoWorkflow', "Ingreso por Compra de Granos - Calada Externa");

    if (captcha) {
      params = params.set('request.captcha', captcha);
    }

    const headers = new HttpHeaders({
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    });

    return this.http
      .get<TrackingResponse>(`${this.apiUrl}/api/qrcamiones/search`, { params, headers })
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
            mensaje: error.error?.mensaje || error.message || 'Datos no encontrados',
            data: null as any
          });
        })
      );
  }

  updateFromEstadoEtapas(estadoEtapas: EstadoEtapas): void {
    const current = this.trackingData();
    if (current) {
      this.trackingData.set({
        ...current,
        etapas: estadoEtapas.etapas,
        datosAdicionales: estadoEtapas.datosAdicionales
      });
    }
  }

  clearTrackingData(): void {
    this.trackingData.set(null);
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