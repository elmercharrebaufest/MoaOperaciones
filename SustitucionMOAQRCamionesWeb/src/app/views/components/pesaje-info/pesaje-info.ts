import { Component, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pesaje-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pesaje-info.html',
  styleUrls: ['./pesaje-info.scss']
})
export class PesajeInfoComponent {
  pesadaBruto = input.required<number>();
  pesadaTara = input.required<number>();
  pesadaDescargado = input.required<number>();
  fechaPesajeBruto = input.required<string>();
  
  isExpanded = signal(false);

  toggleExpanded() {
    this.isExpanded.update(value => !value);
  }

  formatWeight(weight: number): string {
    return weight.toLocaleString('es-AR') + ' KG';
  }
}