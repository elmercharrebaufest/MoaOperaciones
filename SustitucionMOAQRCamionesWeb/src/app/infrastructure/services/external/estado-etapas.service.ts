import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';
import { EstadoEtapasResponse } from '../../../models/estado-etapas-response.model';
import { EstadoEtapas } from '../../../models/estado-etapas.model';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EstadoEtapasService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  
  estadoEtapasData = signal<EstadoEtapas | null>(null);

  getEstadoEtapas(ctg: string, patente: string): Observable<EstadoEtapasResponse> {
    const params = new HttpParams()
      .set('request.ctg', ctg)
      .set('request.patente', patente);

    return this.http
      .get<EstadoEtapasResponse>(`${this.apiUrl}/api/qrcamiones/estadoEtapas`, { params })
      .pipe(
        catchError((error) => {
          console.error('Error fetching estado etapas:', error);
          
          return of({
            resultado: false,
            mensaje: error.error?.mensaje || error.message || 'Error al obtener datos'
          });
        })
      );
  }
  
  updateEstadoEtapas(data: EstadoEtapas): void {
    this.estadoEtapasData.set(data);
  }
  
  clearEstadoEtapas(): void {
    this.estadoEtapasData.set(null);
  }
}