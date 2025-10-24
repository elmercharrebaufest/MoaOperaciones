import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Etapa {
  nombre: string;
  estado: 'completo' | 'pendiente' | 'en_curso';
  fecha?: string;
  tiempoEstimado?: string;
}

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
  stageClicked = output<number>();

  onStageClick(index: number) {
    this.stageClicked.emit(index);
  }

  getIconPath(stageName: string): string {
    const normalizedName = stageName.toLowerCase().replace(/[^a-z0-9]/g, '_');
    return `assets/etapas/${normalizedName}_icon.svg`;
  }
}
