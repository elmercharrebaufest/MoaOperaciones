import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContainerComponent } from '../../../shared/container/container';
import { TrackingData } from '../../../models/tracking-data.model';
import { formatDate } from '../../../shared/helpers/date.helper';

@Component({
  selector: 'app-information-carga',
  standalone: true,
  imports: [CommonModule, ContainerComponent],
  templateUrl: './information-carga.html',
  styleUrls: ['./information-carga.scss']
})
export class InformationCargaComponent {
  @Input({ required: true }) data!: TrackingData;

  formatFechaHoraIngreso(): string {
    return this.data ? formatDate(this.data.fechaHoraIngreso) : '';
  }

  displayValue(value: string | number | null | undefined): string {
    if (value === null || value === undefined || value === '') {
      return 'N/A';
    }
    return String(value);
  }

  displayBoolean(value: string): string {
    if (value.toLowerCase() === "true") {
      return 'Si';
    }
    return 'No'
  }
}