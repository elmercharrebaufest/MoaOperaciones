import { Component, signal, computed, effect, type OnInit } from "@angular/core"
import { CommonModule } from "@angular/common"
import { Router } from "@angular/router"
import type { Etapa } from "../../../models/estado-etapas.model"
import { ShipmentCardComponent } from "../../components/shipment-card/shipment-card"
import { ProgressStepperComponent } from "../../components/progress-stepper/progress-stepper"
import { StagesListComponent } from "../../components/stages-list/stages-list"
import { InformationButtonComponent } from "../../components/information-button/information-button"
import { ContainerComponent } from "../../../shared/container/container"
import { SectionWrapperComponent } from "../../components/section-wrapper/section-wrapper"
import { PesajeInfoComponent } from "../../components/pesaje-info/pesaje-info"
import { CircuitoFinalizadoComponent } from "../../components/circuito-finalizado/circuito-finalizado"
import { EtapasCompletadasComponent } from "../../components/etapas-completadas/etapas-completadas"
import { TutorialWelcomeModalComponent } from "../../components/tutorial-welcome-modal/tutorial-welcome-modal"
import { TutorialTooltipComponent } from "../../components/tutorial-tooltip/tutorial-tooltip"
import { formatDate } from "../../../shared/helpers/date.helper"
import { returnStatusUppercase, returnStatusClass } from "../../../shared/helpers/status.helper"
import { StageStateService } from "../../../infrastructure/services/internal/stage-state.service"
import { TrackingService } from "../../../infrastructure/services/external/tracking.service"
import { EstadoEtapasService } from "../../../infrastructure/services/external/estado-etapas.service"
import { TutorialService } from "../../../infrastructure/services/internal/tutorial.service"
import { CookieService } from "../../../infrastructure/services/internal/cookie.service"

@Component({
  selector: "app-tracking-page",
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
    EtapasCompletadasComponent,
    TutorialWelcomeModalComponent,
    TutorialTooltipComponent,
  ],
  templateUrl: "./tracking.html",
  styleUrls: ["./tracking.scss"],
})
export class TrackingComponent implements OnInit {
  cargoData = computed(() => this.trackingService.trackingData())

  isExpanded = signal(false)
  isUpdating = signal(false)
  isRestoringSession = signal(false)

  selectedIndex = computed(() => this.stageStateService.getSelectedIndex()());

  rechazado = computed(() => {
    const data = this.cargoData();
    return data?.datosAdicionales?.rechazado ?? false;
  });

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
      var filaYNumeroArray = datosAdicionales.preCaladoFila.split(' ');
      return filaYNumeroArray[0].toUpperCase() + ' ' + filaYNumeroArray[1];
    }

    if (stageName === 'post calado' && datosAdicionales?.postCaladoFila) {
      var filaYNumeroArray = datosAdicionales.postCaladoFila.split(' ');
      return filaYNumeroArray[0].toUpperCase() + ' ' + filaYNumeroArray[1];
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

    if (data.datosAdicionales?.rechazado) return false;

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
    private estadoEtapasService: EstadoEtapasService,
    public tutorialService: TutorialService,
    private cookieService: CookieService
  ) {
    effect(() => {
      const data = this.trackingService.trackingData();
      
      if (data && data.etapas && data.etapas.length > 0) {
        this.stageStateService.updateStages(data.etapas, data.datosAdicionales?.rechazado ?? false);
      }
    });
  }

  ngOnInit() {
    const data = this.cargoData()

    if (data) {
      this.initializeData()
      this.tutorialService.initializeTutorial()
    } else {
      this.attemptSessionRestore();
    }
  }

  private attemptSessionRestore() {
    const ctg = this.cookieService.getCookie('ctg');
    const patente = this.cookieService.getCookie('patente');

    if (ctg && patente) {
      this.isRestoringSession.set(true);

      this.trackingService.getTrackingData(ctg, patente, undefined)
        .subscribe({
          next: (response) => {
            this.isRestoringSession.set(false);
            
            if (response.resultado && response.data) {
              this.initializeData();
              this.tutorialService.initializeTutorial();
            } else {
              this.router.navigate(['/search']);
            }
          },
          error: () => {
            this.isRestoringSession.set(false);
            this.router.navigate(['/search']);
          }
        });
    } else {
      this.router.navigate(['/search']);
    }
  }

  private initializeData() {
    const data = this.cargoData()

    if (data && data.etapas) {
      this.stageStateService.updateStages(data.etapas, data.datosAdicionales.rechazado)
    }
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
    this.cookieService.deleteCookie('ctg');
    this.cookieService.deleteCookie('patente');
    this.router.navigate(['/search']);
  }

  onStartTutorial() {
    this.tutorialService.startTutorial()
  }

  onSkipTutorial() {
    this.tutorialService.skipTutorial()
  }

  onNextTutorialStep() {
    this.tutorialService.nextStep()
  }

  getStatusUppercase(status: Etapa['estado']): string {
    return returnStatusUppercase(status, this.rechazado());
  }

  getStatusClass(status: Etapa['estado']): string {
    return returnStatusClass(status, this.rechazado());
  }
}