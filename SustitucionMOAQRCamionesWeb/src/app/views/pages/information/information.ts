import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ContainerComponent } from '../../../shared/container/container';
import { TrackingService } from '../../../infrastructure/services/external/tracking.service';
import { formatDate } from '../../../shared/helpers/date.helper';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-information',
  standalone: true,
  imports: [CommonModule, ContainerComponent],
  templateUrl: './information.html',
  styleUrls: ['./information.scss']
})
export class InformationComponent implements OnInit, OnDestroy {
  detailType = signal<'carga' | 'planta'>('carga');
  cargoData = computed(() => this.trackingService.trackingData());

  constructor(
    private route: ActivatedRoute,
    private location: Location,
    private router: Router,
    private trackingService: TrackingService
  ) {}

  ngOnInit() {
    if (!this.cargoData() && environment.production) {
      this.router.navigate(['/search']);
      return;
    }

    this.route.queryParams.subscribe(params => {
      this.detailType.set(params['type'] || 'carga');
    });

    document.body.style.overflow = 'hidden';
  }

  ngOnDestroy() {
    document.body.style.overflow = '';
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
    const data = this.cargoData();
    return data ? formatDate(data.fechaHoraIngreso) : '';
  }
}