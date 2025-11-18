import { Component, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Etapa } from '../../../models/estado-etapas.model';
// Components
import { ShipmentCardComponent } from '../../components/shipment-card/shipment-card';
import { ProgressStepperComponent } from '../../components/progress-stepper/progress-stepper';
import { StagesListComponent } from '../../components/stages-list/stages-list';
import { InformationButtonComponent } from '../../components/information-button/information-button';
import { ContainerComponent } from '../../../shared/container/container';
import { SectionWrapperComponent } from '../../components/section-wrapper/section-wrapper';
import { PesajeInfoComponent } from '../../components/pesaje-info/pesaje-info';
import { CircuitoFinalizadoComponent } from '../../components/circuito-finalizado/circuito-finalizado';
import { EtapasCompletadasComponent } from '../../components/etapas-completadas/etapas-completadas';
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
    SectionWrapperComponent,
    PesajeInfoComponent,
    CircuitoFinalizadoComponent,
    EtapasCompletadasComponent
  ],
  templateUrl: './tracking.html',
  styleUrls: ['./tracking.scss']
})
export class TrackingComponent {
  cargoData = computed(() => this.trackingService.trackingData());
  
  isExpanded = signal(false);
  isUpdating = signal(false);

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
        estimatedTime: (() => {
          const totalMinutes = Number(stage.tiempoEstimado);

          if (!totalMinutes || totalMinutes <= 0) {
            return '0m';
          }

          const hours = Math.floor(totalMinutes / 60);
          const minutes = totalMinutes % 60;

          const parts = [];

          if (hours > 0) {
            parts.push(`${hours}h`);
          }

          if (minutes > 0) {
            parts.push(`${minutes}m`);
          }

          return parts.join(' ');
        }),
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

    if (stageName === 'pre calado' && datosAdicionales?.preCaladoFila) {
      return `FILA ${datosAdicionales.preCaladoFila}`;
    }

    if (stageName === 'post calado' && datosAdicionales?.postCaladoFila) {
      return `FILA ${datosAdicionales.postCaladoFila}`;
    }

    return null;
  });

  caladoEstado = computed(() => {
    const data = this.cargoData();
    return data?.datosAdicionales?.caladoEstado || null;
  });

  shouldShowPesajeInfo = computed(() => {
    const data = this.cargoData();
    if (!data) return false;

    const descargaIndex = data.etapas.findIndex(
      stage => stage.nombre.toLowerCase() === 'descarga'
    );

    if (descargaIndex === -1) return false;

    const descargaStage = data.etapas[descargaIndex];
    return descargaStage.estado === 'en-proceso' || descargaStage.estado === 'completado';
  });

  pesajeBrutoDate = computed(() => {
    const data = this.cargoData();
    if (!data) return '';

    const pesajeBrutoStage = data.etapas.find(
      stage => stage.nombre.toLowerCase() === 'pesaje bruto'
    );

    return pesajeBrutoStage ? formatDate(pesajeBrutoStage.fecha) : '';
  });

  isCircuitoFinalizado = computed(() => {
    const data = this.cargoData();
    if (!data) return false;

    if (data.rechazado) return false;

    const cierreStage = data.etapas.find(
      stage => stage.nombre.toLowerCase() === 'cierre'
    );

    if (!cierreStage || cierreStage.estado !== 'completado') return false;

    const allStagesCompleted = data.etapas.every(
      stage => stage.estado === 'completado'
    );

    return allStagesCompleted;
  });

  constructor(
    private router: Router,
    public stageStateService: StageStateService,
    private trackingService: TrackingService,
    private estadoEtapasService: EstadoEtapasService
  ) {
    effect(() => {
      const data = this.trackingService.trackingData();
      
      if (!data) {
        this.router.navigate(['/search']);
        return;
      }
      
      if (data.etapas && data.etapas.length > 0) {
        this.stageStateService.updateStages(data.etapas, data.rechazado);
      }
    });
  }

  

  onStageChange(index: number) {
    this.stageStateService.setSelectedIndex(index);
  }

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  onActualizar() {
    const data = this.cargoData();
    if (!data || this.isUpdating()) return;

    this.isUpdating.set(true);

    this.estadoEtapasService.getEstadoEtapas(data.ctg, data.camion.patente)
      .subscribe({
        next: (response) => {
          this.isUpdating.set(false);
          
          if (response.resultado && response.data) {
            this.trackingService.updateFromEstadoEtapas(response.data);
            // this.cdr.detectChanges();
          } else {
            console.warn('No se pudieron actualizar los datos:', response.mensaje);
          }
        },
        error: (error) => {
          this.isUpdating.set(false);
          console.error('Error updating estado etapas:', error);
          alert('Error al actualizar los datos. Por favor, intente nuevamente.');
        }
      });
  }

  consultarOtraCTG() {
    this.trackingService.clearTrackingData();
    this.estadoEtapasService.clearEstadoEtapas();
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