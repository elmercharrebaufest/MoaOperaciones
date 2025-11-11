import { Injectable, signal, computed } from '@angular/core';
import { Etapa } from '../../../models/estado-etapas.model';

@Injectable({
  providedIn: 'root'
})
export class StageStateService {
  private stages = signal<Etapa[]>([]);
  private selectedIndex = signal<number>(0);
  private isExpanded = signal<boolean>(false);
  private rechazado = signal<boolean>(false);

  readonly enProcesoIndex = computed(() => {
    return this.stages().findIndex(stage => stage.estado === 'en-proceso');
  });

  readonly currentStage = computed(() => {
    const stagesData = this.stages();
    return stagesData[this.selectedIndex()];
  });

  updateStages(stages: Etapa[], rechazado: boolean) {
    this.stages.set(stages);
    this.rechazado.set(rechazado);
    const enProcesoIndex = stages.findIndex(stage => stage.estado === 'en-proceso');
    this.selectedIndex.set(enProcesoIndex !== -1 ? enProcesoIndex : 0);
  }

  getStages() { return this.stages; }
  getSelectedIndex() { return this.selectedIndex; }
  getIsExpanded() { return this.isExpanded; }
  getRechazado() { return this.rechazado; }

  setSelectedIndex(index: number) {
    this.selectedIndex.set(index);
  }

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }
}