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
import { EstadoEtapasService } from '../../../infrastructure/services/external/estado-etapas.service';

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
  private updateCount = signal<number>(0);

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

  filaTag = computed(() => {
    const data = this.cargoData();
    const stage = this.stageStateService.currentStage();
    
    if (!data || !stage || stage.estado !== 'en-proceso') {
      return null;
    }

    const stageName = stage.nombre.toLowerCase();
    const datosAdicionales = data.datosAdicionales;

    if (stageName === 'pre calado' && datosAdicionales?.pre_calado_fila) {
      return `FILA ${datosAdicionales.pre_calado_fila}`;
    }

    if (stageName === 'post calado' && datosAdicionales?.post_calado_fila) {
      return `FILA ${datosAdicionales.post_calado_fila}`;
    }

    return null;
  });

  caladoEstado = computed(() => {
    const data = this.cargoData();
    return data?.datosAdicionales?.calado_estado || null;
  });

  constructor(
    private router: Router,
    public stageStateService: StageStateService,
    private trackingService: TrackingService,
    private estadoEtapasService: EstadoEtapasService
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
    if (!environment.production) {
      const currentCount = this.updateCount();
      
      if (currentCount === 6) {
        this.trackingService.resetToInitialMock();
        this.estadoEtapasService.resetMockCycle();
        this.updateCount.set(0);
        this.initializeData();
        return;
      }

      const data = this.cargoData();
      if (data) {
        this.estadoEtapasService.getEstadoEtapas(data.ctg, data.camion.patente)
          .subscribe({
            next: (response) => {
              if (response.resultado && response.data) {
                this.trackingService.updateFromEstadoEtapas(response.data);
                this.updateCount.update(count => count + 1);
                this.initializeData();
              }
            },
            error: (error) => {
              console.error('Error updating estado etapas:', error);
            }
          });
      }
    } else {
      const data = this.cargoData();
      if (data) {
        this.estadoEtapasService.getEstadoEtapas(data.ctg, data.camion.patente)
          .subscribe({
            next: (response) => {
              if (response.resultado && response.data) {
                this.trackingService.updateFromEstadoEtapas(response.data);
                this.initializeData();
              }
            },
            error: (error) => {
              console.error('Error updating estado etapas:', error);
            }
          });
      }
    }
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