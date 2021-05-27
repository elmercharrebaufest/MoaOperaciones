import { Time, WeekDay } from "@angular/common";
import { WeekDayItem } from "../common/models/weekDayItem";
import * as uuid from 'uuid';
import { EspecificacionesViewModel } from "./PliegoPasos/solapaTres/especificacionesViewModel";
import { CampoObligatorioViewModel } from "./campo-obligatorio-viewModel";

export class Solp {

    constructor()
    {
         this.nombreDeObra="";
        // this.fiscalContrato="";
        // this.mail="";
        // this.supervisorSector="";
        // this.supervisorTrabajo="";
    }


    //paso 1
    public nombreDeObra: string;
    public fiscalContrato: string;
    public telefono: string;
    public mail: string;
    public fechaDeEntregaDeOfertasFecha: Date;
    public fechaDeEntregaDeOfertasHora: Time;
    public horaEntrega: any;
    public fechaEntrega: any;

    //paso 2
    public visitaDeObra: boolean;
    public supervisorSector: string;
    public visitaDeObraFecha: Date;
    public visitaDeObraHora: Date;
    public supervisorTrabajo: string;
    public obradores: boolean;
    public descripcionTecnica: boolean;
    public modoElevacion: boolean;
    public entregaDocumentacion: boolean;
    public tecnicoSeguridad: boolean;
    public fechaLimiteFecha: Date;
    public fechaLimiteHora: Date;
    public visitaDeObraMasiva: boolean;
    public observacionesGeneracion: string = "";
    public listaVisitas: any;

    // paso 4
    public ejecucion: any;
    public jornadaLaboralDias: WeekDayItem[];
    public comienzoJornadaLaboral: Date;
    public terminoJornadaLaboral: Date;
    public observacionesCotizacion: string;

    //paso 3
    public especificacionesViewModel: EspecificacionesViewModel = new EspecificacionesViewModel();
}



