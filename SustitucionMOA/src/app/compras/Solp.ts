import { Time, WeekDay } from "@angular/common";
import { WeekDayItem } from "../common/models/weekDayItem";
import { SelectItem } from 'primeng/api';
import * as uuid from 'uuid';
import { forEach } from "@angular/router/src/utils/collection";
import { EspecificacionesViewModel } from "./PliegoPasos/solapaTres/especificacionesViewModel";
import { CampoObligatorioViewModel } from "./campo-obligatorio-viewModel";
import { FormGroup } from "@angular/forms";
import { SubPosicionViewModel } from "./PliegoPasos/solapaSubposiciones/subPosicionViewModel";

export class Solp {
    public id: number;

    //paso 1
    public nombreDePedido: string;
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

    //paso 3
    public especificacionesViewModel: EspecificacionesViewModel = new EspecificacionesViewModel();

    // paso 4
    public ejecucion: any;
    public jornadaLaboralDias: WeekDayItem[];
    public comienzoJornadaLaboral: Date;
    public terminoJornadaLaboral: Date;
    public observacionesCotizacion: string;

    //inicio Cabecera == paso 5
    public selectClaseDocumento: any

    public posiciones: PosicionSolp[];
    public posicionActual: PosicionSolp;
    // fin cabecera


    //variables auxiliares de inicio de solp
    public cargoPasoUno: boolean = false;
    public cargoPasoDos: boolean = false;
    public cargoPasoTres: boolean = false;
    public cargoPasoCuatro: boolean = false;
    public cargoPasoCinco: boolean = false;
    public cargoPasoSeis: boolean = false;

    public centroPorDefecto: any;
    public monedaPorDefecto: any;
    

    // // dashboard
    // public selectEstadoSolp: any;
    // public fechaSolp: Date;
    

    constructor() {
        this.posiciones = [];
        this.agregarNuevaPosicion();

    }

    agregarNuevaPosicion() {
        this.posiciones = [...this.posiciones, new PosicionSolp(this.posiciones.length + 1, this.fiscalContrato)]
        this.posicionActual = this.posiciones[this.posiciones.length - 1];
    }

    eliminarPosicion() {
        this.posicionActual = this.posiciones[0];
        this.posiciones = this.posiciones.filter(x => x.id != this.posicionActual.id);
        if (this.posiciones.length == 0) {
            this.agregarNuevaPosicion();
        }
        this.ordenarPosiciones();
    }

    ordenarPosiciones() {
        var i = 1;
        this.posiciones.forEach(x => x.numeroPosicion = i++);
    }

    posicionesValidas(){
        return !this.posiciones.find(x=>!x.posicionValida);
    }

    setearPosicionPorDefecto(){
        if(this.posiciones && this.posiciones.length > 0){
            this.posicionActual = this.posiciones[0];
        }
    }
}



export class PosicionSolp {
    public id: any;
    public numeroPosicion: number;

    public servicio: string;
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
    public selectCentroEntrega: any;
    public selectAlmacenEntrega: any;
    public centroPorDefecto: any;
    public monedaPorDefecto: any;

    
    public nombreEntrega: string;
    public calleEntrega: string;
    public numeroEntrega: string;
    public codigoPostalEntrega: string;
    public paisEntrega: string;

    // grupo de compras
    public selectGrupoCompras: any;
    public selectSolicitanteCompras: any;
    public necesidadCompras: string;
    public selectArticuloCompras: any;

    // proveedores 
    public rubroElectrico: boolean;
    public rubroCivil: boolean;
    public rubroMecanico: boolean;
    public rubroIngenieria: boolean;
    public rubroConsultoria: boolean;
    public tipoImputacion: string;

    public proveedoresValidos: string[] = [];
    public proveedoresInvalidos: string[] = [];
    public proveedoresNoSugeridos: string[] = [];

    // Moneda
    public selectMonedaCompras: any;
    public monedaSeleccionada: any;
    public totalPosicion() {

        if(this.listadoSubPosiciones && this.listadoSubPosiciones.length > 0){
            let total = 0;
            this.listadoSubPosiciones.forEach(x=>{
                total += (x.precioBruto || 0)*(parseInt(x.cuentaTd) || 0);
            });

            return total;
        }

        return 0;
    }

    public posicionValida: boolean;

    //subPosiciones
    listadoSubPosiciones :  Array<SubPosicionViewModel>;

    constructor(numeroPosicion, fiscalContrato) {
        this.id = uuid.v4();
        this.plazoDeEntrega = "0";
        this.numeroPosicion = numeroPosicion;
        this.fechaEntregaServicio = new Date();
        this.fechaDeLiberacion = new Date();
        this.listadoSubPosiciones = new Array<SubPosicionViewModel>();
        //agrega un fila por defecto
        this.listadoSubPosiciones.push(new SubPosicionViewModel(0));
        this.servicio = 'SERVICIO';
        this.selectSolicitanteCompras = fiscalContrato;
    }
}

