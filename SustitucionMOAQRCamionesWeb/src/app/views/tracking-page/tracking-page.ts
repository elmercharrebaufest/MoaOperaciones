import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShipmentCardComponent } from '../shipment-card/shipment-card';
import { ProgressStepperComponent } from '../progress-stepper/progress-stepper';
import { StagesListComponent } from '../stages-list/stages-list';
import { ExpandableSectionComponent } from '../expandable-section/expandable-section';
import { ContainerComponent } from '../../shared/container/container';
import { SectionWrapperComponent } from '../section-wrapper/section-wrapper';
import { MOCK_CARGO_DATA } from '../../data/mock-tracking.data';
import { Router } from '@angular/router';
import { formatDate } from '../../shared/helpers/date.helper';

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
  templateUrl: './tracking-page.html',
  styleUrls: ['./tracking-page.scss']
})
export class TrackingPageComponent {
  cargoData = signal(MOCK_CARGO_DATA);
  currentStageIndex = signal(0);
  isExpanded = signal(false);

  currentStage = computed(() => {
    const data = this.cargoData();
    const index = this.currentStageIndex();
    if (data && data.etapas && data.etapas[index]) {
      return {
        index: index,
        total: data.etapas.length,
        name: data.etapas[index].nombre,
        date: formatDate(data.etapas[index].fecha),
        estimatedTime: data.etapas[index].tiempoEstimado
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
    this.router.navigate(['/login']);
  }
}