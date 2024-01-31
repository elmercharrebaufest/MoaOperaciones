import { ArchivoModel } from "../archivo.model";

export class EspecificacionesViewModel {

    constructor() {
        this.archivosEspecificaciones = new Array<ArchivoModel>();
        this.archivosEspecificacionesNuevos = new Array<File>();

        //inicializador por defecto de las observaciones
        this.observaciones = this.valorPorDefecto;
    }

    public observaciones: string;
    public archivosEspecificacionesNuevos:  Array<File>; //para archivos nuevos que se van agregando
    public archivosEspecificaciones: Array<ArchivoModel> //los que ya contiene la solp
    readonly valorPorDefecto: string ="<span class=\"ql-size-small\"> </span>";

    public ObservacionesEsValorPorDefecto() : boolean
    {
        return this.observaciones == this.valorPorDefecto;

    }

    
}