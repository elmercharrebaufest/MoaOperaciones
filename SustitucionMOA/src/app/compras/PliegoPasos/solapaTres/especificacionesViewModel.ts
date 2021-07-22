import { AdjuntosEspecificaciones } from "./adjuntos-Especificaciones";

export class EspecificacionesViewModel {

    constructor() {
        this.archivosGuardadosEspecificaciones = new Array<AdjuntosEspecificaciones>();
        this.archivosAdjuntosNuevos = new Array<File>();

        //inicializador por defecto de las observaciones
        this.observaciones = this.valorPorDefecto;
    }

    public observaciones: string;
    public archivosAdjuntosNuevos:  Array<File>; //para archivos nuevos que se van agregando
    public archivosGuardadosEspecificaciones: Array<AdjuntosEspecificaciones> //los que ya contiene la solp
    readonly valorPorDefecto: string ="<span class=\"ql-size-small\">Especificación técnica de la obra, generalidades, detalles, puntos de control, planos, diagramas , etc.</span>";

    public ObservacionesEsValorPorDefecto() : boolean
    {
        return this.observaciones == this.valorPorDefecto;

    }

    
}