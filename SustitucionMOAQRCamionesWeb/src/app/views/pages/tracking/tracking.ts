import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
// Components
import { ShipmentCardComponent } from '../../components/shipment-card/shipment-card';
import { ProgressStepperComponent } from '../../components/progress-stepper/progress-stepper';
import { StagesListComponent } from '../../components/stages-list/stages-list';
import { ExpandableSectionComponent } from '../../components/expandable-section/expandable-section';
import { ContainerComponent } from '../../../shared/container/container';
import { SectionWrapperComponent } from '../../components/section-wrapper/section-wrapper';
// Helpers
import { formatDate } from '../../../shared/helpers/date.helper';
import { returnStatusUppercase, returnStatusClass } from '../../../shared/helpers/status.helper'
// MOCK
import { MOCK_CARGO_DATA } from '../../../data/mock-tracking.data';

@Component({
  selector: 'app-tracking-page',
  standalone: true,
  imports: [
    CommonModule,
    ShipmentCardComponent,
    ProgressStepperComponent,
    StagesListComponent,
    ExpandableSectionComponent,
    ContainerComponent,
    SectionWrapperComponent
  ],
  templateUrl: './tracking.html',
  styleUrls: ['./tracking.scss']
})
export class TrackingComponent {
  cargoData = signal(MOCK_CARGO_DATA);
  currentStageIndex = signal(0);
  isExpanded = signal(false);

  currentStage = computed(() => {
    const data = this.cargoData();
    const index = this.currentStageIndex();
    if (data && data.etapas && data.etapas[index]) {
      const estadoSinAlterar = data.etapas[index].estado
      return {
        index: index,
        total: data.etapas.length,
        name: data.etapas[index].nombre,
        date: formatDate(data.etapas[index].fecha),
        estimatedTime: data.etapas[index].tiempoEstimado,
        status: this.getStatusUppercase(estadoSinAlterar),
        rawStatus: estadoSinAlterar
      };
    }
    return null;
  });

  constructor(
    private router: Router
  ) {}

  onStageChange(stageIndex: number) {
    this.currentStageIndex.set(stageIndex);
  }

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  consultarOtraCTG() {
    this.router.navigate(['/search']);
  }

  getStatusUppercase(status: string): string {
    return returnStatusUppercase(status, this.cargoData().rechazado);
  }

  getStatusClass(status: string): string {
    return returnStatusClass(status, this.cargoData().rechazado);
  }
}