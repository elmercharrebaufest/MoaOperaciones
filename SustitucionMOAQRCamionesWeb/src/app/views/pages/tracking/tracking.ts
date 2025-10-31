import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Etapa } from '../../../models/estado-etapas.model';
import { environment } from '../../../../environments/environment';
// Components
import { ShipmentCardComponent } from '../../components/shipment-card/shipment-card';
import { ProgressStepperComponent } from '../../components/progress-stepper/progress-stepper';
import { StagesListComponent } from '../../components/stages-list/stages-list';
import { InformationButtonComponent } from '../../components/information-button/information-button';
import { ContainerComponent } from '../../../shared/container/container';
import { SectionWrapperComponent } from '../../components/section-wrapper/section-wrapper';
// Helpers
import { formatDate } from '../../../shared/helpers/date.helper';
import { returnStatusUppercase, returnStatusClass } from '../../../shared/helpers/status.helper'
// Services
import { StageStateService } from '../../../infrastructure/services/internal/stage-state.service';
import { TrackingService } from '../../../infrastructure/services/external/tracking.service'

@Component({
  selector: 'app-tracking-page',
  standalone: true,
  imports: [
    CommonModule,
    ShipmentCardComponent,
    ProgressStepperComponent,
    StagesListComponent,
    InformationButtonComponent,
    ContainerComponent,
    SectionWrapperComponent
  ],
  templateUrl: './tracking.html',
  styleUrls: ['./tracking.scss']
})
export class TrackingComponent {
  cargoData = computed(() => this.trackingService.trackingData());
  isExpanded = signal(false);

  selectedIndex = computed(() => this.stageStateService.getSelectedIndex()());

  currentStage = computed(() => {
    const data = this.cargoData();
    const stage = this.stageStateService.currentStage();
    const index = this.selectedIndex();
    
    if (data && stage) {
      return {
        index,
        total: data.etapas.length,
        name: stage.nombre,
        date: formatDate(stage.fecha),
        estimatedTime: stage.tiempoEstimado,
        status: this.getStatusUppercase(stage.estado),
        rawStatus: stage.estado
      };
    }
    return null;
  });

  constructor(
    private router: Router,
    public stageStateService: StageStateService,
    private trackingService: TrackingService
  ) {
    if (!this.cargoData() && environment.production) {
      this.router.navigate(['/search']);
      return;
    }
    this.initializeData();
  }

  private initializeData() {
    const data = this.cargoData();
    if (data) {
      this.stageStateService.updateStages(data.etapas, data.rechazado);
    }
  }

  onStageChange(index: number) {
    this.stageStateService.setSelectedIndex(index);
  }

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  onActualizar() {
    this.initializeData();
  }

  consultarOtraCTG() {
    this.router.navigate(['/search']);
  }

  getStatusUppercase(status: Etapa['estado']): string {
    const data = this.cargoData();
    return data ? returnStatusUppercase(status, data.rechazado) : '';
  }

  getStatusClass(status: Etapa['estado']): string {
    const data = this.cargoData();
    return data ? returnStatusClass(status, data.rechazado) : '';
  }
}