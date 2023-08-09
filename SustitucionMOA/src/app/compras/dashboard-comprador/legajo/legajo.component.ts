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
    error: string;
    visualizarAlert = false;


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
        var archivoWeb = event.files.reduce((sum, file) => sum + file.size, 0);      
        if(archivoWeb > 10000000){ 
            this.error = "El archivo adjuntado no debe superar los 10Mb";   
            fileUpload.clear();
            return  this.visualizarAlert = true;               
        }else{   
        this.adjuntarArchivoLegajoEmitter.next(event.files);
        fileUpload.clear();
        this.visualizarAlert = false;
        }
    }
}
