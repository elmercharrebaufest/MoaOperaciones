import { Injectable, signal, computed } from '@angular/core';
import { Etapa } from '../../../models/estado-etapas.model';;

@Injectable({
  providedIn: 'root'
})
export class StageStateService {
  private stages = signal<Etapa[]>([]);
  private currentIndex = signal<number>(0);
  private isExpanded = signal<boolean>(false);
  private rechazado = signal<boolean>(false);

  // Computed value to always find the en-proceso stage
  readonly enProcesoIndex = computed(() => {
    const stagesData = this.stages();
    return stagesData.findIndex(stage => stage.estado === 'en-proceso');
  });

  // Current stage based on en-proceso or selected index
  readonly currentStage = computed(() => {
    const stagesData = this.stages();
    const enProcesoIdx = this.enProcesoIndex();
    const currentIdx = this.currentIndex();
    
    // If there's an en-proceso stage and no manual selection has been made
    if (enProcesoIdx !== -1 && currentIdx === enProcesoIdx) {
      return stagesData[enProcesoIdx];
    }
    
    return stagesData[currentIdx] || null;
  });

  // Initialize or update data
  updateStages(stages: Etapa[], rechazado: boolean) {
    this.stages.set(stages);
    this.rechazado.set(rechazado);
    
    // Always set initial index to en-proceso stage if it exists
    const enProcesoIndex = stages.findIndex(stage => stage.estado === 'en-proceso');
    if (enProcesoIndex !== -1) {
      this.currentIndex.set(enProcesoIndex);
    }
  }

  // Getters
  getStages() { return this.stages; }
  getCurrentIndex() { return this.currentIndex; }
  getIsExpanded() { return this.isExpanded; }
  getRechazado() { return this.rechazado; }
  getEnProcesoIndex() { return this.enProcesoIndex; }

  // Actions
  setCurrentIndex(index: number) {
    this.currentIndex.set(index);
  }

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  // Reset to en-proceso stage
  resetToEnProceso() {
    const enProcesoIndex = this.enProcesoIndex();
    if (enProcesoIndex !== -1) {
      this.currentIndex.set(enProcesoIndex);
    }
  }
}