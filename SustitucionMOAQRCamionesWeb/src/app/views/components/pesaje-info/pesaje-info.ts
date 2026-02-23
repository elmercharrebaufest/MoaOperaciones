import { Component, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pesaje-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pesaje-info.html',
  styleUrls: ['./pesaje-info.scss']
})
export class PesajeInfoComponent {
  @Input() pesadaBruto: number | null = null;
  @Input() pesadaTara: number | null = null;
  @Input() pesadaDescargado: number | null = null;
  @Input() fechaPesajeBruto: string = '';
  
  isExpanded = signal(false);

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  formatWeight(weight: number | null): string {
    if (weight === null || weight === undefined) {
      return 'N/A';
    }
    return `${weight.toLocaleString('es-AR')} kg`;
  }
}