import { Time, WeekDay } from "@angular/common";
import { WeekDayItem } from "../common/models/weekDayItem";
import { EspecificacionesViewModel } from "./PliegoPasos/solapaTres/especificacionesViewModel";
import {SelectItem} from 'primeng/api';
import * as uuid from 'uuid';
import { forEach } from "@angular/router/src/utils/collection";

export class Solp {
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
    public observacionesGeneracion: string;
    public listaVisitas: any;

    //paso 3
    public especificacionesViewModel : EspecificacionesViewModel = new EspecificacionesViewModel();

    // paso 4
    public ejecucion: any;
    public jornadaLaboralDias: WeekDayItem[];
    public comienzoJornadaLaboral: Date;
    public terminoJornadaLaboral: Date;
    public observacionesCotizacion: string;

    //inicio Cabecera == paso 5
    public selectClaseDocumento: string

    // posiciones

    public posiciones: PosicionSolp[];
    posicionActual: PosicionSolp;
    // fin cabecera



    constructor() {
        this.posiciones = [];
        this.agregarNuevaPosicion(); 
    }

    agregarNuevaPosicion(){
        this.posiciones = [...this.posiciones, new PosicionSolp(this.posiciones.length + 1)]
        this.posicionActual = this.posiciones[this.posiciones.length - 1];
    }

    eliminarPosicion(){
        this.posicionActual = this.posiciones[0];
        this.posiciones = this.posiciones.filter(x => x.id != this.posicionActual.id);
        if (this.posiciones.length == 0) {
            this.agregarNuevaPosicion();
        }
        this.ordenarPosiciones();
    }

    ordenarPosiciones(){
        var i = 1;
        this.posiciones.forEach(x => x.numeroPosicion = i++);
    }
}



export class PosicionSolp{
    public id: any;
    public numeroPosicion: number;

    public servicio: boolean;
    public centroDeCosto: boolean;
    public ordenDeOt: boolean;
    public ordenDeInversion: boolean;
    public siniestroBeneficio: boolean;
    public textoGenerico: string;

    // fechas
    public fechaEntregaServicio: Date;
    public fechaDeLiberacion: Date;
    public plazoDeEntrega: string;
    public concluido: boolean;
    public indiceFijacion: boolean;

    // direccion de entrega
    public selectCentroEntrega: string;
    public selectAlmacenEntrega: string;

    public nombreEntrega: string;
    public calleEntrega: string;
    public numeroEntrega: string;
    public codigoPostalEntrega: string;
    public paisEntrega: string;

    // grupo de compras
    public selectGrupoCompras: string;
    public selectSolicitanteCompras: string;
    public necesidadCompras: string;
    public selectArticuloCompras: string;

    // proveedores 
    public rubroElectrico: boolean;
    public rubroCivil: boolean;
    public rubroMecanico: boolean;
    public rubroIngenieria: boolean;
    public rubroConsultoria: boolean;

    public proveedoresValidos: string[] = [];
    public proveedoresInvalidos: string[] = [];
    public proveedoresNoSugeridos: string[] = [];
   
    // Moneda
    public selectMonedaCompras: string;
    public totalPosicion: number;

    constructor(numeroPosicion) {
        this.id = uuid.v4();
        this.plazoDeEntrega = "0";   
        this.numeroPosicion = numeroPosicion;
        this.fechaEntregaServicio = new Date();
        this.fechaDeLiberacion = new Date();
    
        
    }
}

