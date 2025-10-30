import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';
import { TrackingData } from '../../models/tracking-data.model';
import { MOCK_CARGO_DATA } from '../../data/mock-tracking.data';
import { TrackingResponse } from '../../models/tracking-response.model';

@Injectable({
  providedIn: 'root'
})
export class TrackingService {
  private http = inject(HttpClient);
  private apiUrl = 'your-api-url';
  
  // Signal to hold current cargo tracking data
  trackingData = signal<TrackingData | null>(null);

  // Get tracking info from API
  getTrackingData(ctg: string, patente: string): Observable<TrackingResponse> {
    const requestBody = {
        ctg: ctg,
        patente: patente
    };

    return this.http
        .post<TrackingResponse>(`${this.apiUrl}/search`, requestBody)
        .pipe(
            catchError((error) => {
                console.error('Error fetching tracking data:', error);
                
                return of({
                    resultado: false,
                    mensaje: error.message || 'Datos no encontrados'
                });
            })
        );
}
  
  // Load mock data (for development)
  loadMockData(): void {
    this.trackingData.set(MOCK_CARGO_DATA);
  }
  
  // Update current stage
  updateCurrentStage(stageIndex: number): void {
    const current = this.trackingData();
    if (current) {
      this.trackingData.set({
        ...current,
        // currentStageIndex: stageIndex
      });
    }
  }
}