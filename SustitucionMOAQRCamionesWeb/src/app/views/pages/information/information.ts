import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ContainerComponent } from '../../../shared/container/container';
import { MOCK_CARGO_DATA } from '../../../data/mock-tracking.data';
import { formatDate } from '../../../shared/helpers/date.helper';

@Component({
  selector: 'app-information',
  standalone: true,
  imports: [CommonModule, ContainerComponent],
  templateUrl: './information.html',
  styleUrls: ['./information.scss']
})
export class InformationComponent implements OnInit {
  detailType = signal<'carga' | 'planta'>('carga');
  cargoData = signal(MOCK_CARGO_DATA);

  constructor(
    private route: ActivatedRoute,
    private location: Location
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.detailType.set(params['type'] || 'carga');
    });

    document.body.style.overflow = 'hidden';
  }

  goBack() {
    const container = document.querySelector('.information-detail-container');
    if (container) {
      container.classList.add('page-exit');
      setTimeout(() => {
        this.location.back();
      }, 280);
    }
  }

  getTitle(): string {
    return this.detailType() === 'carga' ? 'Información de la carga' : 'Información de la planta';
  }

  formatFechaHoraIngreso(): string {
    return formatDate(this.cargoData().fechaHoraIngreso);
  }

  ngOnDestroy() {
    document.body.style.overflow = '';
  }
}