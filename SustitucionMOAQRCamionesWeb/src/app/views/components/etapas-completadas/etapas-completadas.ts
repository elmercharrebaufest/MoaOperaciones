import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Etapa } from '../../../models/estado-etapas.model';
import { StagesListComponent } from '../stages-list/stages-list';

@Component({
  selector: 'app-etapas-completadas',
  standalone: true,
  imports: [CommonModule, StagesListComponent],
  templateUrl: './etapas-completadas.html',
  styleUrls: ['./etapas-completadas.scss']
})
export class EtapasCompletadasComponent {
  stages = input.required<Etapa[]>();
  currentStageIndex = input.required<number>();
  rechazado = input.required<boolean>();
  caladoEstado = input<string | null>(null);
  stageClicked = output<number>();
  
  isExpanded = signal(false);

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  onStageClick(index: number) {
    this.stageClicked.emit(index);
  }
}