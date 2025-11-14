import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-shipment-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './shipment-card.html',
  styleUrls: ['./shipment-card.scss']
})
export class ShipmentCardComponent {
  @Input({ required: true }) ctg!: string;
  @Input({ required: true }) patente!: string;
  @Input({ required: true }) patenteAcoplado!: string;
}