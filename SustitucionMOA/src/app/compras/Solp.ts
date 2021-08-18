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
    public fechaEntrega: Date;

    //paso 2
    public visitaDeObra: boolean;
    public supervisorSector: string;
    public visitaDeObraFecha: Date;
    public visitaDeObraHora: Date;
    public supervisorTrabajo: string;
    public obradores: boolean;
    public descripcionTecnica: boolean;
    public modoElevacion: boolean;
    public andamio: boolean;
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

    public get ultimaPosicion(): PosicionSolp{
        this.setearPosicionMasFutura();
        return this._ultimaPosicion;
    }

    private _ultimaPosicion: PosicionSolp;


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
    public enviarSap: boolean;
    

    // // dashboard
    // public selectEstadoSolp: any;
    // public fechaSolp: Date;
    

    constructor() {
        this.posiciones = [];
        this.fechaEntrega = new Date();
        this.fechaEntrega.setDate(this.fechaEntrega.getDate() + 7);
        this.agregarNuevaPosicion();
        this._ultimaPosicion = this.posicionActual;

        
       
    }

    agregarNuevaPosicion() {
        this.posiciones = [...this.posiciones, new PosicionSolp(this.posiciones.length + 1, this.fiscalContrato, this.fechaEntrega, this.posicionActual)]
        this.posicionActual = this.posiciones[this.posiciones.length - 1];
    }

    eliminarPosicion() {
        this.posiciones = this.posiciones.filter(x => x.id != this.posicionActual.id);
        if (this.posiciones.length == 0) {
            this.agregarNuevaPosicion();
        }
        this.ordenarPosiciones();
        this.posicionActual = this.posiciones[0];
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

    setearPosicionMasFutura(){
        let posicionesOrdenadas = this.posiciones.sort((a, b) => {
            return (b.fechaEntregaServicio.getTime() - a.fechaEntregaServicio.getTime())
        });
        this._ultimaPosicion = posicionesOrdenadas[0];
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
    public plazoDeEntrega: number;
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



    constructor(numeroPosicion, fiscalContrato, fechaEntrega, posicionADuplicar) {
        this.id = uuid.v4();
        this.numeroPosicion = numeroPosicion;
        this.plazoDeEntrega = 10;
        this.fechaEntregaServicio = new Date(fechaEntrega);
        this.fechaEntregaServicio.setDate(fechaEntrega.getDate() + parseInt(this.plazoDeEntrega.toString()));  

        this.fechaDeLiberacion = new Date();
        this.listadoSubPosiciones = new Array<SubPosicionViewModel>();
        //agrega un fila por defecto
        this.listadoSubPosiciones.push(new SubPosicionViewModel(1));
        this.servicio = 'SERVICIO';
        this.selectSolicitanteCompras = fiscalContrato;

        if(posicionADuplicar){
            //this.campo = posicionADuplicar.campo
            this.servicio = posicionADuplicar.servicio;
            this.centroDeCosto = posicionADuplicar.centroDeCosto;
            this.ordenDeOt = posicionADuplicar.ordenDeOt;
            this.ordenDeInversion = posicionADuplicar.ordenDeInversion;
            this.siniestroBeneficio = posicionADuplicar.siniestroBeneficio;
            // this.textoGenerico = posicionADuplicar.textoGenerico;
            this.fechaEntregaServicio = posicionADuplicar.fechaEntregaServicio;
            this.fechaDeLiberacion = posicionADuplicar.fechaDeLiberacion;
            this.plazoDeEntrega = posicionADuplicar.plazoDeEntrega;
            this.concluido = posicionADuplicar.concluido;
            this.indiceFijacion = posicionADuplicar.indiceFijacion;
            this.selectCentroEntrega = posicionADuplicar.selectCentroEntrega;
            this.selectAlmacenEntrega = posicionADuplicar.selectAlmacenEntrega;
            this.centroPorDefecto = posicionADuplicar.centroPorDefecto;
            //this.monedaPorDefecto = posicionADuplicar.monedaPorDefecto;
            this.nombreEntrega = posicionADuplicar.nombreEntrega;
            this.calleEntrega = posicionADuplicar.calleEntrega;
            this.numeroEntrega = posicionADuplicar.numeroEntrega;
            this.codigoPostalEntrega = posicionADuplicar.codigoPostalEntrega;
            this.paisEntrega = posicionADuplicar.paisEntrega;
            this.selectGrupoCompras = posicionADuplicar.selectGrupoCompras;
            this.selectSolicitanteCompras = posicionADuplicar.selectSolicitanteCompras;
            this.necesidadCompras = posicionADuplicar.necesidadCompras;
            this.selectArticuloCompras = posicionADuplicar.selectArticuloCompras;
            this.rubroElectrico = posicionADuplicar.rubroElectrico;
            this.rubroCivil = posicionADuplicar.rubroCivil;
            this.rubroMecanico = posicionADuplicar.rubroMecanico;
            this.rubroIngenieria = posicionADuplicar.rubroIngenieria;
            this.rubroConsultoria = posicionADuplicar.rubroConsultoria;
            this.tipoImputacion = posicionADuplicar.tipoImputacion;
            this.proveedoresValidos = posicionADuplicar.proveedoresValidos;
            this.proveedoresInvalidos = posicionADuplicar.proveedoresInvalidos;
            this.proveedoresNoSugeridos = posicionADuplicar.proveedoresNoSugeridos;
            //this.selectMonedaCompras = posicionADuplicar.selectMonedaCompras;
            this.monedaSeleccionada = posicionADuplicar.monedaSeleccionada;
        }


    
    }
}

