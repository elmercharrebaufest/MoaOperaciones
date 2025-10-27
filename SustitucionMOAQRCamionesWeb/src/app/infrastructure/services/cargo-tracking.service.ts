import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { TrackingData } from '../../models/tracking-data.model';
import { MOCK_CARGO_DATA } from '../../data/mock-cargo-tracking.data';

@Injectable({
  providedIn: 'root'
})
export class CargoTrackingService {
  private http = inject(HttpClient);
  private apiUrl = 'your-api-url';
  
  // Signal to hold current cargo tracking data
  trackingData = signal<TrackingData | null>(null);

  // Get tracking info from API
  getCargoTrackingInfo(ctg: string): Observable<TrackingData> {
    return this.http.get<TrackingData>(`${this.apiUrl}/cargo-tracking/${ctg}`);
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