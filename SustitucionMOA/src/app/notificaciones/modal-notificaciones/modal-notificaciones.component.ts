import { Component, Input, OnInit, SecurityContext, SimpleChanges } from '@angular/core';
import { ModalService } from '../../common/services/ModalService';
import { DomSanitizer, SafeResourceUrl, SafeUrl } from '@angular/platform-browser';

@Component({
    selector: 'app-modal-notificaciones',
    templateUrl: './modal-notificaciones.component.html',
    styleUrls: ['./modal-notificaciones.component.css']
})
export class ModalNotificacionesComponent implements OnInit {

    @Input() notificacion: any;
    @Input() adjunto: any;

    isOpen: boolean = false;
    currentIndex: number = 0;
    pdfUrl: SafeResourceUrl;
    tiposAdjuntos: any;
    pdfBase64: string;

    imagenBase64: string;
    imagenSegura: SafeUrl;
    nombreAdjunto: string;

    isZoomed = false;

    private timerInterval: any;

    constructor(protected modalService: ModalService, private sanitizer: DomSanitizer) { }

    ngOnInit() {
        if (this.notificacion != null && this.notificacion.ArchivosAdjuntos > 0 && this.isOpen == true)
            this.onMouseLeave();
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes.notificacion && changes.notificacion.currentValue) {
            this.notificacion = changes.notificacion.currentValue;

            if (this.notificacion.ArchivosAdjuntos && this.notificacion.ArchivosAdjuntos.length > 0) {
                this.calcularContadorAdjuntos();
            }
        }
    }

    nextImage() {
        if (this.isOpen = true) this.currentIndex = (this.currentIndex + 1) % this.notificacion.ArchivosAdjuntos.length;
    }

    convertImage(base64: string): SafeUrl {
        this.imagenBase64 = this.decodeBase64Image(base64);
        this.imagenSegura = this.sanitizer.bypassSecurityTrustUrl(this.imagenBase64);
        return this.imagenSegura
    }

    downloadPdf(pdfData: any, nombreAdjunto: string) {
        this.pdfBase64 = this.getSafePdfDataUrl(pdfData);
        const link = document.createElement('a');
        const href = this.sanitizer.sanitize(SecurityContext.URL, this.pdfBase64);
        link.href = href;
        link.target = '_blank';
        link.download = nombreAdjunto;
        link.click();
    }

    decodeBase64Image(base64: string): string {
        return 'data:image/png;base64,' + base64;
    }

    getSafePdfDataUrl(base64Content: string): any {
        const url = `data:application/pdf;base64,${base64Content}`;

        this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);

        return this.pdfUrl;
    }

    calcularContadorAdjuntos() {
        this.notificacion.ArchivosAdjuntos = this.notificacion.ArchivosAdjuntos.filter((adjunto: any) => {
            if (adjunto.AdjuntoTipo == "previsualizacion") {
                return false;
            }
            return true;
        });

    }

    toggleZoom() {
        this.isZoomed = !this.isZoomed;
    }

    prev() {
        if (this.notificacion.ArchivosAdjuntos && this.notificacion.ArchivosAdjuntos.length > 0) {
            this.currentIndex = (this.currentIndex - 1 + this.notificacion.ArchivosAdjuntos.length) % this.notificacion.ArchivosAdjuntos.length;
        }
    }

    next() {
        if (this.notificacion.ArchivosAdjuntos && this.notificacion.ArchivosAdjuntos.length > 0) {
            this.currentIndex = (this.currentIndex + 1) % this.notificacion.ArchivosAdjuntos.length;
        }
    }

    mostrarModal() {

        this.isOpen = true;


    }

    onMouseEnter() {
        if (this.isOpen = true) {
            try {
                if (this.notificacion.ArchivosAdjuntos && this.notificacion.ArchivosAdjuntos.length > 0)
                    clearInterval(this.timerInterval);
            }
            catch { }
        }
    }

    onMouseLeave() {
        if (this.isOpen = true) {
            try {
                this.timerInterval = setInterval(() => {
                    this.nextImage();
                }, 5000);
            }
            catch { }
        }
    }

    closeModal() {
        this.isOpen = false;
        clearInterval(this.timerInterval);
    }
}
