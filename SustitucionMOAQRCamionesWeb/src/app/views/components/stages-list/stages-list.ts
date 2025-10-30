import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Etapa } from '../../../models/estado-etapas.model';
import { returnStatusUppercase, returnStatusClass } from '../../../shared/helpers/status.helper';
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
  stageClicked = output<number>();

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
}