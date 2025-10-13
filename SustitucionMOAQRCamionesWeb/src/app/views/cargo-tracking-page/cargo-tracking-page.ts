import { Component, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CargoTrackingService } from '../../infrastructure/services/cargo-tracking.service';
import { ShipmentCardComponent } from '../shipment-card/shipment-card';
import { StageProgressComponent } from '../stage-progress/stage-progress';
import { ProgressStepperComponent } from '../progress-stepper/progress-stepper';
import { ExpandableSectionComponent } from '../expandable-section/expandable-section';
import { ContainerComponent } from '../../shared/container/container';

@Component({
  selector: 'app-cargo-tracking-page',
  standalone: true,
  imports: [
    CommonModule,
    ShipmentCardComponent,
    StageProgressComponent,
    ProgressStepperComponent,
    ExpandableSectionComponent,
    ContainerComponent
  ],
  templateUrl: './cargo-tracking-page.html',
  styleUrls: ['./cargo-tracking-page.scss']
})
export class CargoTrackingPageComponent implements OnInit {
  private cargoTrackingService = inject(CargoTrackingService);
  
  // Get data from service
  cargoData = this.cargoTrackingService.cargoTrackingData;
  
  // Computed values
  currentStage = computed(() => {
    const data = this.cargoData();
    if (!data) return null;
    return {
      name: data.etapas[data.currentStageIndex],
      index: data.currentStageIndex + 1,
      total: data.etapas.length,
      date: data.stageDate,
      estimatedTime: data.estimatedTime
    };
  });

  ngOnInit() {
    // Load mock data on init
    this.cargoTrackingService.loadMockData();
    
    // Or fetch from API:
    // this.cargoTrackingService.getCargoTrackingInfo('098765123456')
    //   .subscribe(data => {
    //     this.cargoTrackingService.cargoTrackingData.set({
    //       ...data,
    //       currentStageIndex: 0,
    //       stageDate: new Date(),
    //       estimatedTime: 'aprox. 10 min'
    //     });
    //   });
  }

  consultarOtraCTG() {
    // Navigate to search or reset
    console.log('Consultar otra CTG');
  }
  
  onStageChange(stageIndex: number) {
    this.cargoTrackingService.updateCurrentStage(stageIndex);
  }
}