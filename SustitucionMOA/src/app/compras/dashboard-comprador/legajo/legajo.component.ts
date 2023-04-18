import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    selector: 'app-legajo',
    templateUrl: './legajo.component.html',
    styleUrls: ['./legajo.component.css']
})
export class LegajoComponent implements OnInit {

    @Input() displayLegajo: boolean;
    @Input() usuarioProveedor: boolean;
    @Input() legajo: any;

    @Output() cerrarLegajoEmitter = new EventEmitter();
    @Output() descargarLegajoEmitter = new EventEmitter();
    @Output() descargarArchivoEmitter = new EventEmitter<{ archivoId: number }>();
    @Output() adjuntarArchivoLegajoEmitter = new EventEmitter<{ files: any }>();


    constructor() { }

    ngOnInit() {
    }

    onCerrarLegajo() {
        this.cerrarLegajoEmitter.next();
    }

    onHideLeajoDialog() {
        this.cerrarLegajoEmitter.next();
    }

    descargarArchivo(archivoId: number) {
        this.descargarArchivoEmitter.next({ archivoId: archivoId });
    }
    descargarLegajo() {
        this.descargarLegajoEmitter.next();
    }
    onBasicUploadAuto(event, fileUpload) {
        this.adjuntarArchivoLegajoEmitter.next(event.files);
        fileUpload.clear();
    }
}
