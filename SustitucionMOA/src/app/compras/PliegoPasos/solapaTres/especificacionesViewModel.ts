import { AdjuntosEspecificaciones } from "./adjuntos-Especificaciones";

export class EspecificacionesViewModel {

    constructor() {
        this.archivosGuardadosEspecificaciones = new Array<AdjuntosEspecificaciones>();
        this.archivosAdjuntos = new Array<File>();

        //inicializador por defecto de las observaciones
        this.observaciones = "<p>3.b - Consideraciones particulares:</p><p>3.b.1 Especificaciones técnica de tares:</p><p><span class='ql-size-small'>Especificación técnica de la obra, generalidades, detalles, puntos de control, planos, diagramas , etc.</span></p></div>"
    }

    public observaciones: string;
    public archivosAdjuntos:  Array<File>;;
    public archivosGuardadosEspecificaciones: Array<AdjuntosEspecificaciones>
}