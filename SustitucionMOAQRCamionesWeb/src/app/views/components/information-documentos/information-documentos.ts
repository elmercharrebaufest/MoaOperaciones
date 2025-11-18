import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContainerComponent } from '../../../shared/container/container';
import { TrackingData } from '../../../models/tracking-data.model';

interface Document {
  title: string;
  icon: string;
  url?: string;
  available: boolean;
}

@Component({
  selector: 'app-information-documentos',
  standalone: true,
  imports: [CommonModule, ContainerComponent],
  templateUrl: './information-documentos.html',
  styleUrls: ['./information-documentos.scss']
})
export class InformationDocumentosComponent {
  @Input({ required: true }) data!: TrackingData;

  documents: Document[] = [
    {
      title: 'Pago de tasa municipal con QR',
      icon: 'document',
      available: true,
      url: ''
    },
    {
      title: 'Manuales de SHYMA',
      icon: 'document',
      available: true,
      url: ''
    },
    {
      title: 'Planos de planta',
      icon: 'document',
      available: true,
      url: ''
    },
    {
      title: 'Información del pesaje',
      icon: 'document',
      available: false
    },
    {
      title: 'Preguntas frecuentes',
      icon: 'document',
      available: true,
      url: ''
    }
  ];

  onDownload(document: Document) {
    if (document.available && document.url) {
      // Implement download logic here
      console.log('Downloading:', document.title);
      // window.open(document.url, '_blank');
    }
  }
}