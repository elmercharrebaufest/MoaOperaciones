import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InformationCargaComponent } from '../../components/information-carga/information-carga';
import { InformationDocumentosComponent } from '../../components/information-documentos/information-documentos';
import { TrackingService } from '../../../infrastructure/services/external/tracking.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-information',
  standalone: true,
  imports: [
    CommonModule,
    InformationCargaComponent,
    InformationDocumentosComponent
  ],
  templateUrl: './information.html',
  styleUrls: ['./information.scss']
})
export class InformationComponent implements OnInit, OnDestroy {
  detailType = signal<'carga' | 'documentos'>('carga');
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
    return this.detailType() === 'carga' ? 'Información de la carga' : 'Descarga de documentos';
  }
}