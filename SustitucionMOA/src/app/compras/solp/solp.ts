import { Time } from "@angular/common";
import { WeekDayItem } from "../../common/models/weekDayItem";

import * as uuid from 'uuid';
import { EspecificacionesViewModel } from "./steps/especificaciones/especificacionesViewModel";
import { CommonResponse } from "../../common/models/common-response";
import { ArchivoModel } from "./steps/archivo.model";
import { EnumTipoSolpSap } from "../enum-tipo-solp-sap";
import { SolpPosicion } from "./solp-posicion";
import { SubPosicionViewModel } from './steps/posicion/tab-subposicion/sub-posicion-view-model';
import { setupJornadaLaboralDias } from "./solp.utils";

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
    public fechaDeEntregaDeOfertasHora: Date;
    public horaEntrega: Date;
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
    public grillaPersonal: boolean;
    public fabricacionTallerExterno: boolean;
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
    public trabajoHecho: boolean;
    public adicional: boolean;
    public urgencia: boolean;
    public monedaOC: string;
    public proveedorAsignado_Id: number;
    public proveedorAsignado: string;
    public ordenDeCompra: string;
    public codigoProveedorSap: string;
    public RazonSocialSap: string;
    public validacionCheck: boolean = true;
    public validarAdicional: boolean;
    public mensajeCotizacion: string;
    public archivosCotizacionesNuevos: Array<File>;
    public archivosCotizaciones: Array<ArchivoModel>;
    public liberadoresSap: any[] = [];
    public proveedorDefinido: boolean;
    public editarCondicionesEspeciales: boolean;

    //inicio Cabecera == paso 5
    public selectClaseDocumento: any;
    public selectTipoPosicion: any;
    public posiciones: SolpPosicion[];
    public posicionActual: SolpPosicion;
    public pasoCompletado: number;
    public estadoPasos: string;
    public tableHide: boolean;
    proveedorIdAdicional: number;
    proveedorRazonSocialAdicional: string;
    deshabilitarAdicional: boolean;
    ordenDeCompraOriginal: string;


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

    public titulo: string = "";
    public tituloNroSolp: string = "";

    public selectUsuarioCompras: any;
    public usuarioComprasList: any[] = [];

    constructor(solp: any = null) {

        super();

        let fechaLimiteFecha = new Date();
        this.tipoSolpSap = EnumTipoSolpSap.Web;
        this.estadoPasos = "0,0,0,0,0";
        this.jornadaLaboralDias = setupJornadaLaboralDias();
        this.horaEntrega = new Date(1, 1, 1, 12, 0, 0, 0);
        this.fechaLimiteFecha = this.sumarDias(fechaLimiteFecha, 6);
        this.fechaLimiteHora = new Date(1, 1, 1, 10, 0, 0, 0);
        this.visitaDeObraFecha = new Date();
        this.visitaDeObraHora = new Date(1, 1, 1, 10, 0, 0, 0);
        this.listaVisitas = [
            {
                id: uuid.v4(),
                visitaDeObraFecha: new Date(),
                visitaDeObraHora: new Date(1, 1, 1, 10, 0, 0, 0)
            }];

        this.comienzoJornadaLaboral = new Date(1, 1, 1, 7, 0, 0, 0);
        this.terminoJornadaLaboral = new Date(1, 1, 1, 16, 0, 0, 0);
        this.ejecucion = "30";
        // this.observacionesCotizacion = "Indicar la cantidad de días con que se cuenta a partir de tener el equipo disponible, en una parada programada o que el trabajo depende de otros";

        this.archivosCotizacionesNuevos = new Array<File>();
        this.archivosCotizaciones = new Array<ArchivoModel>();

        if (solp != null) {
            // Paso 1
            this.id = solp.Id;
            this.tipoSolp = solp.TipoSolp && solp.TipoSolp.Codigo || '';
            this.tipoSolpSap = solp.TipoSolpSap || '';
            this.vincularAPliego = (this.tipoSolpSap == EnumTipoSolpSap.Mantenimiento || this.tipoSolpSap == EnumTipoSolpSap.SAP);
            this.titulo = this.vincularAPliego ? "Vincular pliego" : solp.TipoSolp.Codigo;
            this.tituloNroSolp = solp.NroSolp ? "| SOLP #" + solp.NroSolp : "";
            this.nroSolp = solp.NroSolp || 0;
            this.nombreDePedido = solp.NombreDeObra || '';
            this.fiscalContrato = solp.FiscalContrato || '';
            this.telefono = solp.Telefono || '';
            this.mail = solp.Email || sessionStorage.getItem("username");

            if (solp.FechaHoraEntrega != null) {
                this.fechaEntrega = new Date(this.getDateFromAspNetFormat(solp.FechaHoraEntrega));
                this.horaEntrega = new Date(this.getDateFromAspNetFormat(solp.FechaHoraEntrega));
            } else {
                this.fechaEntrega = new Date();
                this.fechaEntrega.setDate(this.fechaEntrega.getDate() + 7);
            }

            //this.fechaEntrega = new Date(this.getDateFromAspNetFormat(solp.FechaHoraEntrega));
            this.emailLinkToken = solp.EmailLinkToken;
            this.selectTipoPosicion = solp.TipoPosicion && solp.TipoPosicion.Codigo || ''
                ;


            // Paso 2
            this.supervisorSector = solp.SupervisorSector || '';
            this.supervisorTrabajo = solp.SupervisorTrabajo || '';
            this.listaVisitas = solp.VisitasObraMasiva.map(x => {
                return {
                    id: x.Codigo,
                    visitaDeObraFecha: new Date(this.getDateFromAspNetFormat(x.FechaHora)),
                    visitaDeObraHora: new Date(this.getDateFromAspNetFormat(x.FechaHora))
                } || '';
            });
            this.visitaDeObraMasiva = solp.TieneVisitaObraMasiva;
            this.visitaDeObra = solp.TieneVisitaObra;
            this.obradores = solp.TieneObradores;
            this.modoElevacion = solp.TieneMedioElevacion;
            this.andamio = solp.TieneAndamio;
            this.tecnicoSeguridad = solp.TieneTecnicoSeguridad;
            this.grillaPersonal = solp.TieneGrillaPersonal;
            this.fabricacionTallerExterno = solp.TieneFabricacionTallerExterno;
            this.usuarioComprasId = solp.UsuarioCompras.Id || 0;
            this.descripcionTecnica = solp.TieneDescripcionTecnica;
            this.entregaDocumentacion = solp.TieneDocumentacionTecnica;
            if (solp.FechaHoraLimiteConsulta != null) {
                this.fechaLimiteFecha = new Date(this.getDateFromAspNetFormat(solp.FechaHoraLimiteConsulta));
                this.fechaLimiteHora = new Date(this.getDateFromAspNetFormat(solp.FechaHoraLimiteConsulta));
            }
            this.observacionesGeneracion = solp.ObservacionesGeneracion;

            // Paso 3
            this.especificacionesViewModel = new EspecificacionesViewModel();
            this.especificacionesViewModel.archivosEspecificaciones = solp.Adjuntos
                .filter(x => x.FileKey == "adjuntoSolp")
                .map(x => {
                    return {
                        id: x.Id,
                        nombreArchivo: x.Nombre
                    }
                }),
                this.especificacionesViewModel.observaciones = solp.EspecificacionesTecnicas || this.especificacionesViewModel.valorPorDefecto;

            this.tieneCondicionesGenerales = solp.TieneCondicionesGenerales;

            // Paso 4
            this.archivosCotizaciones = solp.Adjuntos
                .filter(x => x.FileKey == "adjuntoCotizacionesSolp")
                .map(x => {
                    return {
                        id: x.Id,
                        nombreArchivo: x.Nombre,
                    }
                });
            this.jornadaLaboralDias.forEach(k => {
                k.selected = solp.JornadaLaboral.includes(k.weekDay);
            });
            this.comienzoJornadaLaboral = new Date(this.getDateFromAspNetFormat(solp.JornadaLaboralDesde));
            this.terminoJornadaLaboral = new Date(this.getDateFromAspNetFormat(solp.JornadaLaboralHasta));
            this.ejecucion = solp.DiasEjecucion || '';
            this.observacionesCotizacion = solp.ObservacionesCotizacion;
            this.proveedorAsignado_Id = solp.ProveedorAsignadoId;
            this.proveedorAsignado = solp.ProveedorAsignado;
            this.trabajoHecho = solp.TrabajoYaHecho;
            this.adicional = solp.Adicional;
            this.urgencia = solp.Urgencia;
            this.proveedorDefinido = solp.ProveedorDefinido;
            this.ordenDeCompra = solp.NroOrdenDeCompraAdicional;
            this.ordenDeCompraOriginal = solp.NroOrdenDeCompraAdicional;
            this.proveedorIdAdicional = solp.ProveedorIdAdicional;
            this.proveedorRazonSocialAdicional = solp.ProveedorRazonSocialAdicional
            this.deshabilitarAdicional = solp.DeshabilitarAdicional;
            this.monedaOC = solp.MonedaOC;
            this.liberadoresSap = solp.LiberadoresSapSolp;
            this.editarCondicionesEspeciales = solp.EditarCondicionesEspeciales;

            //pop up finalizar
            this.revisadoPor = solp.RevisadoPor || '';

            // Paso 5
            this.selectClaseDocumento = solp.ClaseDocumento;
            this.pasoCompletado = solp.PasoCompletado;
            this.estadoPasos = solp.EstadoPasos;

            this.selectUsuarioCompras = this.usuarioComprasId > 0 ? this.usuarioComprasList.find(x => x.Id === this.usuarioComprasId) : this.usuarioComprasList[0];

            if (solp.Posiciones && solp.Posiciones.length > 0) {
                let ultimaPos = solp.Posiciones[solp.Posiciones.length - 1];

                this.posiciones = [];

                this.agregarNuevaPosicion(null as SolpPosicion);

                let posActual = this.posicionActual;

                this.selectTipoPosicion = solp.Posiciones[0].TipoPosicion;

                solp.Posiciones.forEach(x => {
                    posActual.id = x.Codigo;
                    posActual.plazoDeEntrega = x.PlazoEntrega;
                    posActual.fechaEntregaServicio = new Date(this.getDateFromAspNetFormat(x.FechaEntregaServicio));
                    posActual.fechaDeLiberacion = new Date(this.getDateFromAspNetFormat(x.FechaLiberacion));
                    posActual.selectCentroEntrega = x.Centro;
                    posActual.selectAlmacenEntrega = x.Almacen.Codigo === null ? '' : x.Almacen;
                    posActual.nombreEntrega = x.NombreEntrega;
                    posActual.calleEntrega = x.CalleEntrega;
                    posActual.numeroEntrega = x.NumeroEntrega;
                    posActual.codigoPostalEntrega = x.CpEntrega;
                    posActual.paisEntrega = x.PaisEntrega;
                    posActual.selectSolicitanteCompras = x.Solicitante;
                    posActual.necesidadCompras = x.NroNecesidad;
                    posActual.selectGrupoCompras = x.GrupoCompras;
                    posActual.selectArticuloCompras = x.GrupoArticulo;
                    posActual.textoSuministro = x.TextoSuministro;
                    posActual.motivo = x.Motivo;
                    posActual.modelo = x.Modelo;
                    posActual.monedaSeleccionada = x.Moneda;
                    posActual.tipoPosicion = x.TipoPosicion;
                    posActual.tipoImputacion = x.TipoImputacion;
                    posActual.estado = x.Estado;
                    posActual.indice = x.numeroPosicion;
                    posActual.concluido = x.EsConcluido;
                    this.selectTipoPosicion = x.TipoPosicion;
                    posActual.codigoServicio = this.tipoSolp == "SERVICIO" ? x.CodigoServicioSap : x.CodigoMaterialSap;
                    posActual.tareaSubcontratarObj = { Descripcion: x.Tarea };
                    posActual.tareaSubcontratar = x.Tarea;
                    posActual.cuentaTd = x.Cantidad;
                    posActual.unidadSeleccionada = x.Unidad;
                    posActual.precioBruto = x.PrecioBruto;
                    posActual.selectProvincia = x.Provincia;

                    posActual.valorImputacion = x.TipoImputacionValor;
                    posActual.cuentaMayor = x.CuentaMayor;
                    posActual.provedorFijo = x.ProveedorFijo,
                        posActual.nombreProveedor = x.NombreProveedor,
                        posActual.numeroContratoSuperior = x.NumeroContratoSuperior,
                        posActual.numeroPosicionContratoSuperior = x.NumeroPosicionContratoSuperior,
                        posActual.orgCompras = x.OrganizacionCompras,

                        posActual.proveedoresValidos = x.Proveedores.filter(p => p.TipoFiltroProveedorSolp.Codigo == 'VALIDO').map(p => p.RazonSocial);
                    posActual.proveedoresNoSugeridos = x.Proveedores.filter(p => p.TipoFiltroProveedorSolp.Codigo == 'NOSUGERIDO').map(p => p.RazonSocial);
                    posActual.proveedoresInvalidos = x.Proveedores.filter(p => p.TipoFiltroProveedorSolp.Codigo == 'INVALIDO').map(p => p.RazonSocial);

                    if (x.Subposiciones) {
                        posActual.listadoSubPosiciones = [];
                        let i = 1;

                        x.Subposiciones.forEach(sp => {
                            let subpos = new SubPosicionViewModel(i);

                            subpos.id = sp.Codigo;
                            subpos.codigoServicio = sp.CodigoServicioSap;
                            subpos.tareaSubcontratarObj = { Descripcion: sp.Tarea };
                            subpos.tareaSubcontratar = sp.Tarea;
                            subpos.cuentaMayor = sp.CuentaMayor;
                            subpos.cuentaTd = sp.Cantidad;
                            subpos.unidadSeleccionada = sp.Unidad;
                            subpos.tipoImputacion = sp.TipoImputacionValor;
                            subpos.precioBruto = sp.PrecioBruto;
                            subpos.subPosicion = sp.Numero;
                            subpos.monedaSeleccionada = x.Moneda;
                            subpos.calcularValorNeto();

                            posActual.listadoSubPosiciones.push(subpos);
                            i++;
                        });
                    }
                    posActual.isNewRow = false;
                    posActual.calcularValorTotal();

                    if (ultimaPos.Codigo != x.Codigo) {
                        this.agregarNuevaPosicion(null as SolpPosicion);
                        posActual = this.posicionActual;
                    }
                });
                this.calcularValorTotalPorMoneda();
                this.setearPosicionPorDefecto();
                this.tituloSolp();
            }

        } else {
            this.posiciones = [];
            this.fechaEntrega = new Date();
            this.fechaEntrega.setDate(this.fechaEntrega.getDate() + 7);
            this._ultimaPosicion = this.posicionActual;
        }
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

    agregarNuevaPosicionDesdeContratoMarco(posicion: SolpPosicion) {
        if (posicion != null) {
            this.posiciones = [...this.posiciones, posicion];
            this.posicionActual = this.posiciones[this.posiciones.length - 1];
            this._ultimaPosicion = this.posicionActual;
        }
    }

    eliminarPosicion(posicionBorrar: any) {
        if (this.nroSolp > 0) {
            if (posicionBorrar.concluido != true) {
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
            this.valorTotalPorMoneda.push({ moneda, valorTotal } as ValorTotalPorMoneda);
        });
    }

    tituloSolp() {
        switch (this.tipoSolp) {
            case "CON_PLIEGO":
                this.titulo = "Generación de SOLP con documento de pliego"
                break;
            case "SIN_PLIEGO":
                this.titulo = "Generación de SOLP sin documento de pliego"
                break;
            default:
        }
    }

    sumarDias(fecha, dias) {
        fecha.setDate(fecha.getDate() + dias);
        return fecha;
    }

    public getSelectedTipoPosicion(posiciones: any) {
        let tipoPosicion = undefined;
        if (posiciones != undefined && posiciones.length > 0) {
            let posicion = posiciones.filter(p => p.TipoPosicion.Codigo != undefined);
            if (posicion != undefined) {
                tipoPosicion = posicion[0].TipoPosicion.Codigo;
            }
        }
        return tipoPosicion;
    }

    public getDateFromAspNetFormat(date: string): number {
        if (date) {
            const re = /-?\d+/;
            const m = re.exec(date);
            return parseInt(m[0], 10);
        }
        return null
    }

}

export class ValorTotalPorMoneda {
    public moneda: string;
    public valorTotal: number;
}