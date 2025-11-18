import { Component, input, output, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Etapa } from '../../../models/estado-etapas.model';
import { returnStatusUppercase, returnStatusClass } from '../../../shared/helpers/status.helper';
import { formatDate } from '../../../shared/helpers/date.helper';
import { StageStateService } from '../../../infrastructure/services/internal/stage-state.service';

@Component({
  selector: 'app-stages-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stages-list.html',
  styleUrls: ['./stages-list.scss']
})
export class StagesListComponent {
  stages = input.required<Etapa[]>();
  currentStageIndex = input.required<number>();
  rechazado = input.required<boolean>();
  caladoEstado = input<string | null>(null);
  stageClicked = output<number>();

  orderedStages = computed(() => {
    const stages = this.stages();
    const stagesWithIndex = stages.map((stage, index) => ({ stage, originalIndex: index }));

    const pendientes = stagesWithIndex.filter(s => s.stage.estado === 'pendiente');
    const enProceso = stagesWithIndex.filter(s => s.stage.estado === 'en-proceso');
    const completados = stagesWithIndex.filter(s => s.stage.estado === 'completado');

    return [...pendientes, ...completados];
  });

  postCaladoStageIndex = computed(() => {
    const stages = this.stages();
    return stages.findIndex(s => s.nombre.toLowerCase() === 'post calado');
  });

  caladoStageIndex = computed(() => {
    const stages = this.stages();
    return stages.findIndex(s => s.nombre.toLowerCase() === 'calado');
  });

  shouldShowCaladoNote = computed(() => {
    const stages = this.stages();
    const caladoEstado = this.caladoEstado();
    const postCaladoIndex = this.postCaladoStageIndex();
    
    if (!caladoEstado || postCaladoIndex === -1) {
      return false;
    }

    const postCaladoStage = stages[postCaladoIndex];
    return postCaladoStage?.estado === 'en-proceso' || postCaladoStage?.estado === 'completado';
  });

  constructor(public stageStateService: StageStateService) {}

  onStageClick(index: number) {
    this.stageClicked.emit(index);
  }

  getIconPath(stageName: string): string {
    const normalizedName = stageName.toLowerCase().replace(/[^a-z0-9]/g, '_');
    return `assets/etapas/${normalizedName}_icon.svg`;
  }

  getStatusUppercase(status: Etapa['estado']): string {
    return returnStatusUppercase(status, this.rechazado());
  }

  getStatusClass(status: Etapa['estado']): string {
    return returnStatusClass(status, this.rechazado());
  }

  isActive(index: number, stage: Etapa): boolean {
    return index === this.currentStageIndex() || stage.estado === 'en-proceso';
  }

  formatStageDate(fecha: Date): string {
    return formatDate(fecha);
  }

  shouldShowDate(estado: Etapa['estado']): boolean {
    return estado === 'completado' || estado === 'en-proceso';
  }

  shouldShowCaladoNoteForStage(stageName: string): boolean {
    if (!this.shouldShowCaladoNote()) {
      return false;
    }

    const normalizedName = stageName.toLowerCase();
    return normalizedName === 'calado';
  }
}