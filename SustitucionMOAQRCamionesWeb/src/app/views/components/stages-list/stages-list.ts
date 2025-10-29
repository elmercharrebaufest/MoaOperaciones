import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Etapa } from '../../../models/estado-etapas.model';
// Helpers
import { returnStatusUppercase, returnStatusClass } from '../../../shared/helpers/status.helper'

@Component({
  selector: 'app-stages-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stages-list.html',
  styleUrls: ['./stages-list.scss']
})
export class StagesListComponent {
  stages = input.required<Etapa[]>();
  rechazado = input.required<boolean>();
  currentStageIndex = input.required<number>();
  stageClicked = output<number>();

  onStageClick(index: number) {
    this.stageClicked.emit(index);
  }

  getIconPath(stageName: string): string {
    const normalizedName = stageName.toLowerCase().replace(/[^a-z0-9]/g, '_');
    return `assets/etapas/${normalizedName}_icon.svg`;
  }

  getStatusUppercase(status: string): string {
    const rej = typeof this.rechazado === 'function' ? this.rechazado() : this.rechazado;
    return returnStatusUppercase(status, rej);
  }

  getStatusClass(status: string): string {
    const rej = typeof this.rechazado === 'function' ? this.rechazado() : this.rechazado;
    return returnStatusClass(status, rej);
  }
}
