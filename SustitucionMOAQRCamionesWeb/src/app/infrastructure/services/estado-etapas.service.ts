import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';
import { EstadoEtapasResponse } from '../../models/estado-etapas-response.model';
import { EstadoEtapas } from '../../models/estado-etapas.model';

@Injectable({
  providedIn: 'root'
})
export class EstadoEtapasService {
  private http = inject(HttpClient);
  private apiUrl = 'your-api-url';
  
  estadoEtapasData = signal<EstadoEtapas | null>(null);

  getEstadoEtapas(ctg: string, patente: string): Observable<EstadoEtapasResponse> {
    const requestBody = {
      ctg: ctg,
      patente: patente
    };

    return this.http
      .post<EstadoEtapasResponse>(`${this.apiUrl}/estadoEtapas`, requestBody)
      .pipe(
        catchError((error) => {
          console.error('Error fetching estado etapas:', error);
          
          return of({
            resultado: false,
            mensaje: error.message || 'Datos no encontrados'
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

  loadMockData(): void {
    // MOCK    
  }
}