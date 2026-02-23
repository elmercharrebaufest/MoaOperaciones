import { Component, Input, OnInit, inject, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs/operators';
import { ContainerComponent } from '../../../shared/container/container';
import { TrackingData } from '../../../models/tracking-data.model';
import { ApiService } from '../../../infrastructure/services/external/api.service';

interface Document {
  title: string;
  icon: string;
  url?: string;
  available: boolean;
  fileKeyword?: string;
}

@Component({
  selector: 'app-information-documentos',
  standalone: true,
  imports: [CommonModule, ContainerComponent],
  templateUrl: './information-documentos.html',
  styleUrls: ['./information-documentos.scss']
})
export class InformationDocumentosComponent implements OnInit {
  @Input({ required: true }) data!: TrackingData;
  
  private apiService = inject(ApiService);
  
  isLoading = signal<boolean>(false);

  documents: Document[] = [
    {
      title: 'Pago de tasa municipal con QR',
      icon: 'document',
      available: false,
      fileKeyword: 'Ticket Recibo Municipal'
    },
    {
      title: 'Manuales de SHYMA',
      icon: 'document',
      available: false,
      url: '' // Azure moapublic URL
    },
    {
      title: 'Planos de planta',
      icon: 'document',
      available: false,
      url: '' // Azure moapublic URL
    },
    {
      title: 'Información del pesaje',
      icon: 'document',
      available: false,
      fileKeyword: 'Certificado CP'
    },
    {
      title: 'Preguntas frecuentes',
      icon: 'document',
      available: false,
      url: '' // Azure moapublic URL
    }
  ];

  constructor() {
    effect(() => {
      const files = this.apiService.filesData();
      this.updateDocumentAvailability(files);
    });
  }

  ngOnInit() {
    if (this.data?.ctg && this.data?.camion?.patente) {
      this.isLoading.set(true);

      this.apiService.getFiles(this.data.ctg, this.data.camion.patente)
        .pipe(
          finalize(() => this.isLoading.set(false))
        )
        .subscribe({
          next: (response) => {
            if (response && !response.resultado && !response.data) {
              console.warn('Files warning:', response.mensaje);
            }
          },
          error: (error) => {
            console.error('Error loading files:', error);
          }
        });
    }
  }

  private updateDocumentAvailability(files: any[]): void {
    this.documents.forEach(doc => {
      if (doc.fileKeyword) {
        const file = this.apiService.findFileByName(doc.fileKeyword);
        doc.available = !!file;
      }
    });
  }

  onDownload(document: Document): void {
    if (!document.available) {
      return;
    }

    if (document.fileKeyword) {
      const file = this.apiService.findFileByName(document.fileKeyword);
      if (file) {
        this.apiService.downloadFile(file);
        return;
      }
    }

    if (document.url) {
      window.open(document.url, '_blank');
      return;
    }

    console.warn('No file or URL available for:', document.title);
  }
}