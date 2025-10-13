import { Component, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-shipment-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './shipment-card.html',
  styleUrls: ['./shipment-card.scss']
})
export class ShipmentCardComponent {
  ctg = input.required<string>();
  patente = input.required<string>();
  patenteAcoplado = input.required<string>();
  isExpanded = signal(false);

  toggleExpand() {
    this.isExpanded.update(value => !value);
  }
}