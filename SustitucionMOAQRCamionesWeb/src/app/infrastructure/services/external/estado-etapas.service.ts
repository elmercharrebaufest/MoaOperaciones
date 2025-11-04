import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';
import { EstadoEtapasResponse } from '../../../models/estado-etapas-response.model';
import { EstadoEtapas } from '../../../models/estado-etapas.model';
import { environment } from '../../../../environments/environment';
import { 
  MOCK_ESTADO_ETAPAS_1,
  MOCK_ESTADO_ETAPAS_2,
  MOCK_ESTADO_ETAPAS_3,
  MOCK_ESTADO_ETAPAS_4,
  MOCK_ESTADO_ETAPAS_5,
  MOCK_ESTADO_ETAPAS_6
} from '../../../data/estado-etapas.data';

@Injectable({
  providedIn: 'root'
})
export class EstadoEtapasService {
  private http = inject(HttpClient);
  private apiUrl = 'your-api-url';
  
  estadoEtapasData = signal<EstadoEtapas | null>(null);
  private currentMockIndex = signal<number>(0);
  
  private mockDataArray = [
    MOCK_ESTADO_ETAPAS_1,
    MOCK_ESTADO_ETAPAS_2,
    MOCK_ESTADO_ETAPAS_3,
    MOCK_ESTADO_ETAPAS_4,
    MOCK_ESTADO_ETAPAS_5,
    MOCK_ESTADO_ETAPAS_6
  ];

  getEstadoEtapas(ctg: string, patente: string): Observable<EstadoEtapasResponse> {
    if (!environment.production) {
      return this.getMockEstadoEtapas();
    }

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
  
  private getMockEstadoEtapas(): Observable<EstadoEtapasResponse> {
    const currentIndex = this.currentMockIndex();
    const mockData = this.mockDataArray[currentIndex];
    
    this.estadoEtapasData.set(mockData);
    
    const nextIndex = (currentIndex + 1) % this.mockDataArray.length;
    this.currentMockIndex.set(nextIndex);
    
    return of({
      resultado: true,
      mensaje: 'Datos encontrados',
      data: mockData
    });
  }
  
  updateEstadoEtapas(data: EstadoEtapas): void {
    this.estadoEtapasData.set(data);
  }
  
  clearEstadoEtapas(): void {
    this.estadoEtapasData.set(null);
    this.currentMockIndex.set(0);
  }

  resetMockCycle(): void {
    this.currentMockIndex.set(0);
  }

  loadMockData(): void {
    const mockData = this.mockDataArray[0];
    this.estadoEtapasData.set(mockData);
    this.currentMockIndex.set(1);
  }
}