import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { CargoTrackingData } from '../../models/cargo-tracking.model';
import { MOCK_CARGO_DATA } from '../../data/mock-cargo-tracking.data';

@Injectable({
  providedIn: 'root'
})
export class CargoTrackingService {
  private http = inject(HttpClient);
  private apiUrl = 'your-api-url';
  
  // Signal to hold current cargo tracking data
  cargoTrackingData = signal<CargoTrackingData | null>(null);

  // Get tracking info from API
  getCargoTrackingInfo(ctg: string): Observable<CargoTrackingData> {
    return this.http.get<CargoTrackingData>(`${this.apiUrl}/cargo-tracking/${ctg}`);
  }
  
  // Load mock data (for development)
  loadMockData(): void {
    this.cargoTrackingData.set(MOCK_CARGO_DATA);
  }
  
  // Update current stage
  updateCurrentStage(stageIndex: number): void {
    const current = this.cargoTrackingData();
    if (current) {
      this.cargoTrackingData.set({
        ...current,
        // currentStageIndex: stageIndex
      });
    }
  }
}