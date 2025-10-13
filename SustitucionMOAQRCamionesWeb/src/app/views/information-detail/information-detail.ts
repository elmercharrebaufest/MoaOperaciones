import { Component, OnInit, inject, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { SectionWrapperComponent } from '../section-wrapper/section-wrapper';
import { CargoTrackingService } from '../../infrastructure/services/cargo-tracking.service';
import { ContainerComponent } from '../../shared/container/container';

@Component({
  selector: 'app-information-detail',
  standalone: true,
  imports: [CommonModule, SectionWrapperComponent, ContainerComponent],
  templateUrl: './information-detail.html',
  styleUrls: ['./information-detail.scss']
})
export class InformationDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private location = inject(Location);
  private cargoService = inject(CargoTrackingService);

  detailType: 'carga' | 'planta' = 'carga';
  
  // Get data from service
  cargoData = computed(() => this.cargoService.cargoTrackingData());

  ngOnInit() {
    // Load mock data if not already loaded
    if (!this.cargoService.cargoTrackingData()) {
      this.cargoService.loadMockData();
    }

    this.route.queryParams.subscribe(params => {
      this.detailType = params['type'] || 'carga';
    });
  }

  goBack() {
    this.location.back();
  }

  getTitle(): string {
    return this.detailType === 'carga' ? 'Información de la carga' : 'Información de la planta';
  }
}