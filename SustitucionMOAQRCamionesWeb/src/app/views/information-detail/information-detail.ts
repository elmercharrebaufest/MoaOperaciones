import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ContainerComponent } from '../../shared/container/container';
import { MOCK_CARGO_DATA } from '../../data/mock-cargo-tracking.data';

@Component({
  selector: 'app-information-detail',
  standalone: true,
  imports: [CommonModule, ContainerComponent],
  templateUrl: './information-detail.html',
  styleUrls: ['./information-detail.scss']
})
export class InformationDetailComponent implements OnInit {
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
  }

  goBack() {
    this.location.back();
  }

  getTitle(): string {
    return this.detailType() === 'carga' ? 'Información de la carga' : 'Información de la planta';
  }
}