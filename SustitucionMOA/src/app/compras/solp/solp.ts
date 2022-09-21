import { Time } from "@angular/common";
import { WeekDayItem } from "../../common/models/weekDayItem";

import { EspecificacionesViewModel } from "./steps/especificaciones/especificacionesViewModel";
import { CommonResponse } from "../../common/models/common-response";
import { ArchivoModel } from "./steps/archivo.model";
import { EnumTipoSolpSap } from "../enum-tipo-solp-sap";
import { SolpPosicion } from "./solp-posicion";
import { ContratoMarcoPosicion } from "./steps/posicion/obtener-contrato-marco/contrato-marco.model";

export class Solp extends CommonResponse {
    public id: number;
    public tipoSolp: string;
    public tipoSolpSap: EnumTipoSolpSap;
    public vincularAPliego: boolean = false;
    public nroSolp: number;
    public NroSolp: string;
    public Adjuntos?: { Id: number, Nombre: string }[];

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
    public supervisorSector: string[] = [];
    public visitaDeObraFecha: Date;
    public visitaDeObraHora: Date;
    public supervisorTrabajo: string[] = [];
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
    public usuarioComprasId: number;

    //paso 3
    public especificacionesViewModel: EspecificacionesViewModel = new EspecificacionesViewModel();
    public tieneCondicionesGenerales: boolean = false;

    // paso 4
    public ejecucion: any;
    public jornadaLaboralDias: WeekDayItem[];
    public comienzoJornadaLaboral: Date;
    public terminoJornadaLaboral: Date;
    public observacionesCotizacion: string;

    public archivosCotizacionesNuevos: Array<File>;
    public archivosCotizaciones: Array<ArchivoModel>

    //inicio Cabecera == paso 5
    public selectClaseDocumento: any
    public selectTipoPosicion: any
    public posiciones: SolpPosicion[];
    public posicionActual: SolpPosicion;
    public pasoCompletado: number;
    public estadoPasos: string;
    public tableHide: boolean;

    public get ultimaPosicion(): SolpPosicion {
        //comentar linea de abajo si se quiere que no se ordene por Fecha (Mas actual primero)
        //this.setearPosicionMasFutura();
        return this._ultimaPosicion;
    }

    private _ultimaPosicion: SolpPosicion;
    // fin cabecera

    //variables auxiliares de inicio de solp
    public cargoPasoUno: boolean = false;
    public cargoPasoDos: boolean = false;
    public cargoPasoTres: boolean = false;
    public cargoPasoCuatro: boolean = false;
    public cargoPasoCinco: boolean = false;
    // public cargoPasoSeis: boolean = false;

    public centroPorDefecto: any;
    public direccionCentroPorDefecto: any;
    public monedaPorDefecto: any;
    public imputacionPorDefecto: any;
    public enviarSap: boolean;
    public Finalizar: boolean;

    public revisadoPor: string;

    public valorTotalPorMoneda: Array<ValorTotalPorMoneda>

    public emailLinkToken: string;

    constructor() {
        super();
        this.posiciones = [];
        this.fechaEntrega = new Date();
        this.fechaEntrega.setDate(this.fechaEntrega.getDate() + 7);
        this._ultimaPosicion = this.posicionActual;
    }

    nuevaPosicion(posicion: any, centro: any, direccionCentro: any, moneda: any) {
        let numeroPosicion = this.posiciones.length + 1;        
        return new SolpPosicion(numeroPosicion, 
            this.fiscalContrato, 
            this.fechaEntrega, 
            posicion, 
            centro, 
            direccionCentro,
            moneda,
            this.selectTipoPosicion
            );
    }

    agregarNuevaPosicion(posicion: SolpPosicion) {
        this.posiciones = [...this.posiciones,
                            this.nuevaPosicion(posicion, 
                                this.centroPorDefecto, 
                                this.direccionCentroPorDefecto,
                                this.monedaPorDefecto)];
        this.posicionActual = this.posiciones[this.posiciones.length - 1];
        this._ultimaPosicion = this.posicionActual;
    }

    agregarNuevaPosicionDesdeContratoMarco(posicion: SolpPosicion){
        if (posicion != null) {
            this.posiciones = [...this.posiciones, posicion];
            this.posicionActual = this.posiciones[this.posiciones.length - 1];
            this._ultimaPosicion = this.posicionActual;        
        }
    }

    eliminarPosicion(posicionBorrar: any) {
        if (this.nroSolp > 0) {
            if (posicionBorrar.isNewRow) {
                this.posiciones = this.posiciones.filter(x => x.id != posicionBorrar.id);
            }
            else {
                this.posiciones.filter(x => x.id == posicionBorrar.id).forEach(x => x.estado = false);
            }
        } else {
            this.posiciones = this.posiciones.filter(x => x.id != posicionBorrar.id);
        }

        if (this.posiciones.length == 0) {
            this.agregarNuevaPosicion(null as SolpPosicion);
        }

        this.ordenarPosiciones();
        this.posicionActual = this.posiciones[0];
    }

    recuperarPosicion(posicion: SolpPosicion) {
        let posicionActualId = posicion.id;
        this.posiciones.filter(pos => pos.id === posicionActualId).forEach(pos => {
            pos.estado = true;
        });
        this.ordenarPosiciones();
        this.posicionActual = this.posiciones[0];
    }

    ordenarPosiciones() {
        var i = 1;
        this.posiciones.forEach(x => x.numeroPosicion = i++);
    }

    posicionesValidas() {
        return !this.posiciones.find(x => !x.posicionValida);
    }

    setearPosicionPorDefecto() {
        if (this.posiciones && this.posiciones.length > 0) {
            this.posicionActual = this.posiciones[0];
        }
    }

    setearPosicionMasFutura() {
        let posicionesOrdenadas = this.posiciones.sort((a, b) => {
            return (b.fechaEntregaServicio.getTime() - a.fechaEntregaServicio.getTime())
        });
        this._ultimaPosicion = posicionesOrdenadas[0];
    }

    calcularValorTotalPorMoneda() {
        this.valorTotalPorMoneda = new Array<ValorTotalPorMoneda>();
        const monedas = this.posiciones.map(item => item.monedaSeleccionada.Codigo).filter((value, index, self) => self.indexOf(value) === index);
        monedas.forEach(moneda => {
            let valorTotal = this.posiciones.filter(p => p.monedaSeleccionada.Codigo == moneda).reduce((sum, current) => sum + current.valorTotal, 0);
            if (isNaN(valorTotal)) {
                valorTotal = 0;
            }
            this.valorTotalPorMoneda.push({moneda, valorTotal} as ValorTotalPorMoneda);
        });
    }

}

export class ValorTotalPorMoneda {
    public moneda: string;
    public valorTotal: number;
}