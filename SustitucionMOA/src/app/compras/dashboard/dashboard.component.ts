import { Component, Input, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { ComprasService } from '../compras.service';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { Solp } from '../solp/solp';
import { Table } from 'primeng/table';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { EnumTipoSolpSap } from '../enum-tipo-solp-sap';
import { EnumTipoImputacion } from '../enum-tipo-imputacion';
import { Paginator } from 'primeng/paginator';
import { PeticionDeOfertaDto, PeticionDeOfertaRevisionTecnicaDto } from '../../modelos/peticion-de-oferta-model';
import { AdjudicacionDto, AdjudicacionPosicionDto } from '../../modelos/adjudicacion';
import { ChatComprasDto } from '../chat-interno/chat-interno.interface';
import { forEach } from '@angular/router/src/utils/collection';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

declare var $: any;

@Component({
    selector: 'dashboard',
    templateUrl: `dashboard.component.html`,
    styleUrls: ['../compras.component.css', './dashboard.component.css']
})
export class DashboardComponent extends ListBaseComponent {

    protected locale: any;

    @ViewChild("tabla")
    protected tabla: Table;

    @BlockUI() blockUI: NgBlockUI;

    @Input('model')
    protected model: Solp;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('myCalendar', undefined)
    nroSolp: string = "";
    sap: boolean = false;
    mantenimiento: boolean = false;
    web: boolean = false;
    repoAutomatica: boolean = false;
    contratoMarco: boolean = false;
    orden: string;
    columnaOrden: string;
    length = 0;
    pageSize: number = 10;
    pageIndex: number = 1;
    @ViewChild('paginator') paginator: Paginator
    public peticion: PeticionDeOfertaDto;
    public ordenCompra: any;
    public solicitante: boolean = true;
    displayRevisionTecnica: boolean;
    displayCircular: boolean = false;
    combos: any;
    usuariosResult: any;
    ordenesDeCompra: AdjudicacionDto[] = [];
    peticionesDeOferta: PeticionDeOfertaDto[] = [];
    ordenDeCompra: any;
    displayOrdenDeCompra: boolean;
    displayChatInterno: boolean = false;

    filtrosSolicitante: {
        nroSolp: string;
        sap: boolean;
        mantenimiento: boolean;
        web: boolean;
        repoAutomatica: boolean;
        contratoMarco: boolean;
        usuarios: string[];
        estadoSolp: string[];
        gruposCompras: string[];
        centros: string[];
        claseDocumento: string[];
        tipoImputacion: string[];
        valorTipoImputacion: string[];
        subtipoImputacionCombo: SelectItem[];
        fechaDesde: string;
        fechaHasta: string;
        pageIndex: number;
    } = {
            nroSolp: "",
            sap: false,
            mantenimiento: false,
            web: false,
            repoAutomatica: false,
            contratoMarco: false,
            usuarios: [],
            estadoSolp: [],
            gruposCompras: [],
            centros: [],
            claseDocumento: [],
            tipoImputacion: [],
            valorTipoImputacion: [],
            subtipoImputacionCombo: [],
            fechaDesde: null,
            fechaHasta: null,
            pageIndex: 1
        };

    filteredfechas: any;
    solpFecha: any = new Array();
    fechaInicio: string = null;
    fechaFin: string = null;
    rangeDates: Date[];
    tipoFiltroFecha = 1;
    desdeDashboard: Date;
    hastaDashboard: Date;
    usuarioFiltro: SelectItem[];
    selectUsuario: string[] = [];
    estadoSolpItem: SelectItem[];
    selectEstadoSolp: string[] = [];
    grupoComprasFiltro: SelectItem[];
    selectGrupoCompras: string[] = [];
    centroFiltro: SelectItem[];
    selectCentro: string[] = [];
    claseDocumentoFiltro: SelectItem[];
    selectClaseDocumento: string[] = [];
    tipoImputacionFiltro: SelectItem[];
    selectTipoImputacion: string[] = [];
    valorTipoImputacionFiltro: SelectItem[] = [];
    selectValorTipoImputacion: string[] = [];
    buscarDashboard: string;
    fechaSolp: any;
    hoy: Date = new Date();
    es: any;
    display: boolean = false;
    tablaSolp: any[];
    tablaSolpCopy: any[];
    cols: any[];
    serviciosDashboard: any = "Servicios"
    solp: Solp = new Solp();
    usuario: string;// = "Prueba";
    checkedFilterSap = false;
    checkedFilterMantenimiento = false;
    checkedFilterWeb = false;
    verTodas: boolean = this.isAuthorized('VER TODAS SOLPS');
    public chat: ChatComprasDto;
    clasesDocumento: number[] = [];

    cards = [
        { nombre: "Con documento de pliego", path: "/compras/solp/0", tipoSolp: "CON_PLIEGO" },
        { nombre: "Sin pliego", path: "/compras/solp/0", tipoSolp: "SIN_PLIEGO" },
        // { nombre: "Con documentos requerimientos", path: ""},
        // { nombre: "Sin documento", path: ""},
        // { nombre: "Emergencia", path: ""},
        // { nombre: "Adicional", path: ""}
    ]

    subtitulos = [
        { nombre: "Servicio y/o Material catalogado y sin catalogar" },
        { nombre: "Servicio y/o Material catalogado" },
    ]

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

        this.usuario = sessionStorage.getItem("username");
        this.locale = {
            firstDayOfWeek: 0,
            dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
            dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
            dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
            today: 'Hoy',
            clear: 'Borrar'
        };
    }

    showDialog() {
        this.display = true;
    }

    goToSeccion(path: string) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccion(path);
        return false;
    }

    goToSeccionParam(path: string, param: any) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccionParam(path, param);
        return false;
    }

    goToSeccionEdit(path: string, Id: any, NroSolp: any) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        let nrosol = ""
        if (NroSolp != null) nrosol = NroSolp.toString();
        let obj = Id.toString() + "," + nrosol;

        this.navService.navegarSeccionParam(path, obj);
        return false;
    }

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.listarClaseDocumento();
        this.recuperarFiltros();
        this.listarUsuarioCreadorSolp();
        this.navService.setSeccionList([]);
        this.desdeDashboard = new Date();
        this.hastaDashboard = new Date();
        this.getCombos();
    }

    returnToTodaysDate() {
        this.fechaInicio = "";
        this.fechaFin = "";
        if (this.tablaSolp.length > 0) {
            this.mensajeComponent.setMsgsEmpty();
        }
    }

    onSelect(event: any) {
        if (this.rangeDates[0] && this.rangeDates[1] == null) {
            let d = new Date(Date.parse(event));
            this.fechaInicio = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
            this.fechaFin = '';
        } else {
            let d = new Date(Date.parse(event));
            this.fechaFin = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
        }
    }

    listarExpand() {
        setTimeout(() => {
            $('[id^="ui-tabpanel-"]').css('padding', '0');
            $('[id^="ui-tabpanel-"]').css('transition', 'none').css('animation', 'none');
        }, 0.01);
    }

    getStatusDocumentoSolp(data: any): String {
        return data.PosicionesEstado && data.NroSolp != null ? 'Borrado en sap' : data.EstadoSolpSap.Descripcion;
    }

    public getColorDocumentoSolp(data: any): String {
        return data.PosicionesEstado && data.NroSolp != null ? '#DD441E' : '#333333';
    }

    getListarSolp() {
        try {
            this.spinnerComponent.showIt();
            let multiSelectValues = this.selectEstadoSolp.join(",")
            this.subscription = this.service.getListarSolp(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp, this.fechaInicio, this.fechaFin, this.sap, this.mantenimiento, this.web, this.repoAutomatica, this.contratoMarco,
                multiSelectValues, this.selectUsuario.join(","), this.selectCentro.join(","), this.selectGrupoCompras.join(","), this.selectClaseDocumento.join(","), this.selectTipoImputacion.join(","), this.selectValorTipoImputacion.join(",")
            ).subscribe(
                (result: any) => {

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.tablaSolp = result.data;
                        this.tablaSolp.forEach(x => {
                            x.FechaCreacion = new Date(this.getDateFromAspNetFormat(x.FechaCreacion));
                            x.VincularPliego = x.TipoSolpSap == EnumTipoSolpSap.Mantenimiento || x.TipoSolpSap == EnumTipoSolpSap.SAP;
                            x.PliegoVinculado = (x.TipoSolpSap == EnumTipoSolpSap.Mantenimiento || x.TipoSolpSap == EnumTipoSolpSap.SAP) &&
                                x.EstadoDocumento.Codigo == "CREADO";
                        });
                        this.tablaSolpCopy = this.tablaSolp;
                        this.tabla.first = 0;
                        this.spinnerComponent.hideIt();
                        this.length = result.data.length > 0 ? result.data[0].ItemsTotales : 0;
                        this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
                        this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;
                        this.paginator.first = this.pageIndex * this.pageSize - this.pageSize;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    borrarSolp(idSolp) {
        try {
            this.subscription = this.service.borrarSolp(idSolp).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.getListarSolp();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.usuariosResult = result.Usuarios;
                        this.estadoSolpItem = [];
                        this.usuarioFiltro = [];
                        this.centroFiltro = [];
                        this.grupoComprasFiltro = [];
                        this.claseDocumentoFiltro = [];
                        this.tipoImputacionFiltro = [];
                        result.EstadosSolpSap.forEach(cd => this.estadoSolpItem.push({
                            label: cd.Descripcion, value: cd.Id
                        }));
                        result.Usuarios.forEach(x => x.forEach(d => this.usuarioFiltro.push({
                            label: d.Id === 0 ? "" : d.Mail, value: d.Id
                        })));
                        result.Centro.forEach(c => this.centroFiltro.push({
                            label: c.Codigo + " - " + c.Descripcion, value: c.Id
                        }));
                        result.GrupoCompras.forEach(gc => this.grupoComprasFiltro.push({
                            label: gc.Codigo + " - " + gc.Descripcion, value: gc.Id
                        }));
                        result.ClaseDocumento.forEach(cd => this.claseDocumentoFiltro.push({
                            label: cd.Codigo + " - " + cd.Descripcion, value: cd.Id
                        }));
                        result.TipoImputacion.forEach(ti => this.tipoImputacionFiltro.push({
                            label: ti.Descripcion + " - " + ti.Codigo, value: ti.Codigo
                        }));
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false;
    }

    subtipoImputacionCombo() {
        var tablas: string[] = [];
        this.selectTipoImputacion.forEach(tipo => {
            switch (tipo) {
                case EnumTipoImputacion.CentroDeCosto:
                    tablas.push('CecoSolpSap');
                    break;
                case EnumTipoImputacion.OrdenDeOt:
                case EnumTipoImputacion.OrdenInversion:
                    tablas.push('OrdenSolpSap');
                    break;
                case EnumTipoImputacion.Siniestro:
                    tablas.push('CentroBeneficio');
                    break;
            }
        });

        try {
            this.subscription = this.service.listarTablaSap(tablas).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.valorTipoImputacionFiltro = [];
                        this.valorTipoImputacionFiltro = result.data.map(vti => ({
                            label: `${vti.CodigoDescripcion}`, value: vti.Id
                        }));
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        }
        catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false;
    }

    listarUsuarioCreadorSolp() {
        try {
            this.subscription = this.service.listarUsuarioCreadorSolp().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        const filtrosGuardados = JSON.parse(sessionStorage.getItem('filtrosSolicitante'));
                        result.data.forEach(x => x.forEach(x => {
                            if (x.Id == sessionStorage.getItem("usuarioId") && !filtrosGuardados && !this.selectUsuario.includes(x.Id))
                                this.selectUsuario.push(x.Id);
                        }));
                        this.getListarSolp();
                    }
                },
                error => { this.floatMsgService.setErrorMsg(error.message); }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false;
        }
        return false;
    }

    eliminarPosicionDashboard(idSolp) {
        this.confirmationService.confirm({
            key: 'eliminarSOLP',
            header: 'Eliminar SOLP',
            message: '¿Está seguro de que desea eliminar la SOLP?',
            accept: () => {
                this.borrarSolp(idSolp)
            },
            reject: () => {
            }
        });
    }

    generarZipPliego(idSolp) {
        this.blockUI.start('Generando...')
        this.service.descargarZipPliego(idSolp)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                })
    }

    descargarPdf(idSolp): void {
        if (idSolp != undefined) {
            this.service.getPdf(idSolp)
                .subscribe(
                    (result) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        }
                        else {
                            var byteArray = new Uint8Array(result.FileContents);
                            var blob = new Blob([byteArray], {
                                type: "application/octet-stream",
                            });

                            this.downloadArchivoLocal(blob, result.FileDownloadName);
                        }
                    },
                    (error) => {
                        this.spinnerSmallComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    })
        }
    }

    vincularAPliego(solpId: number) {
        this.goToSeccionParam('/compras/solp', solpId);
    }

    private downloadArchivoLocal(blob: Blob, nombreArchivo: string): void {
        if (window.navigator.msSaveOrOpenBlob) {
            // IE11
            window.navigator.msSaveOrOpenBlob(
                blob,
                nombreArchivo
            );
        } else {
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement("a");
            document.body.appendChild(link);
            link.href = url;
            link.download = nombreArchivo;
            link.click();
            setTimeout(function () {
                window.URL.revokeObjectURL(url);
            }, 0);
            return;
        }
    }

    onOrder(columna: string) {
        if (this.columnaOrden != columna) {
            this.orden = "DESC"
        } else {
            this.orden = this.orden == "DESC" ? "ASC" : "DESC";
        }
        this.columnaOrden = columna;
        this.getListarSolp();
    }

    handlePageEvent(e: any) {
        this.pageSize = e.rows;
        this.pageIndex = e.page + 1;
        this.filtrosSolicitante = {
            ...this.filtrosSolicitante,
            pageIndex: this.pageIndex
        };
        sessionStorage.setItem('filtrosSolicitante', JSON.stringify(this.filtrosSolicitante));
        this.getListarSolp()
    }

    obtenerPeticionDeOferta(Id, rowData) {
        this.blockUI.start('Cargando...')
        this.service.obtenerPeticionDeOferta(Id)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.peticion = result.data;
                        this.peticion.Solp = rowData;
                        this.displayRevisionTecnica = true;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                })
    }

    obtenerPeticionDeOfertaCircular(Id) {
        this.blockUI.start('Cargando...')
        this.service.obtenerPeticionDeOferta(Id)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.peticion = result.data;
                        this.displayCircular = true;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                })
    }

    cerrarModalRevisionTecnica() {
        this.displayRevisionTecnica = false;
        this.listarPeticiones(this.peticion.Solp);
        this.onBuscar();

    }

    cancelarModal() {
        this.displayRevisionTecnica = false;
        this.displayCircular = false;
    }

    descargarArchivo({ archivoId }) {
        this.blockUI.start("Descargando...");
        this.service.DescargarArchivo(archivoId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });
                        this.downloadArchivoLocal(blob, result.FileDownloadName);
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                })
    }

    descargarAdjuntosCotizacion({ cotizacionId }) {
        this.blockUI.start("Descargando...");
        this.service.DescargarAdjuntosCotizacion(cotizacionId, true)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }
    
    cerrarCircular() {
        this.displayCircular = false;
        this.listarPeticiones(this.peticion.Solp);
    }

    onBuscar() {
        this.pageIndex = 1;
        this.filtrosSolicitante.nroSolp = this.nroSolp;
        this.filtrosSolicitante.sap = this.sap;
        this.filtrosSolicitante.mantenimiento = this.mantenimiento;
        this.filtrosSolicitante.web = this.web;
        this.filtrosSolicitante.repoAutomatica = this.repoAutomatica;
        this.filtrosSolicitante.contratoMarco = this.contratoMarco;
        this.filtrosSolicitante.usuarios = this.selectUsuario;
        this.filtrosSolicitante.estadoSolp = this.selectEstadoSolp;
        this.filtrosSolicitante.gruposCompras = this.selectGrupoCompras;
        this.filtrosSolicitante.centros = this.selectCentro;
        this.filtrosSolicitante.claseDocumento = this.selectClaseDocumento;
        this.filtrosSolicitante.tipoImputacion = this.selectTipoImputacion;
        this.filtrosSolicitante.valorTipoImputacion = this.selectValorTipoImputacion;
        this.filtrosSolicitante.subtipoImputacionCombo = this.valorTipoImputacionFiltro;
        this.filtrosSolicitante.fechaDesde = this.fechaInicio;
        this.filtrosSolicitante.fechaHasta = this.fechaFin;
        this.paginator.changePage(0);
        this.cerrarExpansiones();
        this.getListarSolp();
        sessionStorage.setItem('filtrosSolicitante', JSON.stringify(this.filtrosSolicitante));
    }

    cerrarOrdenDeCompra() {
        this.displayOrdenDeCompra = false;
    }

    verDetalleOrdenDeCompra(nroOC: any, solpId) {
        this.obtenerAdjudicacion(nroOC, solpId);
        this.displayOrdenDeCompra = true;
    }

    obtenerAdjudicacion(nroOC, rowData) {
        this.blockUI.start('Cargando...')
        this.service.obtenerAdjudicacion(nroOC).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {
                    this.ordenDeCompra = result.data;
                    this.ordenDeCompra.Solp = rowData;
                    this.blockUI.stop();
                }
            },
            (error) => {
                this.blockUI.stop();
                this.mensajeComponent.setErrorMsg(error.message);
            })
    }

    listarAdjudicaciones(rowData) {
        this.blockUI.start('Cargando...')
        this.service.listarAdjudicaciones(rowData.Id).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {
                    rowData.OrdenesDeCompra = result.data;
                    this.blockUI.stop();
                }
            },
            (error) => {
                this.blockUI.stop();
                this.mensajeComponent.setErrorMsg(error.message);
            })
    }

    listarPeticiones(rowData) {
        if(rowData.PeticionesDeOferta != undefined && rowData.PeticionesDeOferta != null && rowData.PeticionesDeOferta.length > 0){
            return;
        }
        this.blockUI.start('Cargando...')
        this.service.listarPeticiones(rowData.Id).subscribe(
            (result) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }
                else {                    
                    rowData.PeticionesDeOferta = result.data;
                    this.blockUI.stop();
                }
            },
            (error) => {
                this.blockUI.stop();
                this.mensajeComponent.setErrorMsg(error.message);
            })
    }



    filtrarOrdenesDeCompra(solpId): AdjudicacionDto[] {
        return this.ordenesDeCompra.filter(orden => orden.Solp_Id == solpId);
    }

    mapData(data: any[]): void {
        data.forEach((item: any) => {
            const adjudicacion: AdjudicacionDto = {
                Id: item.Id,
                Cotizacion_Id: item.Cotizacion_Id,
                AdjudicacionPosiciones: item.AdjudicacionPosiciones.map((posicion: any) => {
                    const adjudicacionPosicion: AdjudicacionPosicionDto = {
                        Id: posicion.Id,
                        Adjudicacion_Id: posicion.Adjudicacion_Id,
                        CotizacionPosicion_Id: posicion.CotizacionPosicion_Id,
                        Cantidad: posicion.Cantidad,
                        SolpPosicion_Id: posicion.SolpPosicion_Id,
                    };
                    return adjudicacionPosicion;
                }),
                Solp_Id: item.Solp_Id,
                Moneda_Id: item.Moneda_Id,
                TextoDeCabecera: item.TextoDeCabecera,
                CondicionesDeEntrega: item.CondicionesDeEntrega,
                CondicionesDePago: item.CondicionesDePago,
                Garantias: item.Garantias,
                TipoPosicionCodigo: item.TipoPosicionCodigo || '',
                NumeroOrdenDeCompra: item.NumeroOrdenDeCompra || '',
                FechaCreacion: item.FechaCreacion || '',
                Proveedor: item.Proveedor || '',
                MonedaDescripcion: item.MonedaDescripcion || '',
                PrecioFinal: item.PrecioFinal || 0,
                PrecioBruto: item.PrecioBruto || 0,
                EstadoLiberacionDetalle: item.EstadoLiberacionDetalle || '',
            };
            this.ordenesDeCompra.push(adjudicacion);
        });
    }

    obtenerChatExterno(rowData) {
        try {
            this.blockUI.start('Cargando ');
            this.displayChatInterno = false;
            rowData.ChatSinLeer = false;
            this.subscription = this.service.obtenerChat(rowData.Id)
                .subscribe(
                    (result: any) => {
                        this.blockUI.stop();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            result.Mensajes = result.Mensajes.map((x) => {
                                x.FechaEnvioDate = new Date(
                                    this.getDateFromAspNetFormat(x.FechaEnvioDate)
                                );
                                return x;
                            });
                            result.FechaCreacionDate = new Date(
                                this.getDateFromAspNetFormat(result.FechaCreacionDate)
                            );
                            this.chat = result;
                            this.displayChatInterno = true;
                        };
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.floatMsgService.setErrorMsg(error.message);
                    }
                );
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    cerrarModalChat() {
        this.displayChatInterno = false;
    }

    recuperarFiltros() {
        const filtrosGuardados = JSON.parse(sessionStorage.getItem('filtrosSolicitante'));
        if (filtrosGuardados) {
            this.nroSolp = filtrosGuardados.nroSolp;
            this.sap = filtrosGuardados.sap;
            this.mantenimiento = filtrosGuardados.mantenimiento;
            this.web = filtrosGuardados.web;
            this.repoAutomatica = filtrosGuardados.repoAutomatica;
            this.contratoMarco = filtrosGuardados.contratoMarco;
            this.selectUsuario = filtrosGuardados.usuarios;
            this.selectEstadoSolp = filtrosGuardados.estadoSolp;
            this.selectGrupoCompras = filtrosGuardados.gruposCompras;
            this.selectCentro = filtrosGuardados.centros;
            this.selectClaseDocumento = filtrosGuardados.claseDocumento;
            this.selectTipoImputacion = filtrosGuardados.tipoImputacion;
            this.selectValorTipoImputacion = filtrosGuardados.valorTipoImputacion;
            this.valorTipoImputacionFiltro = filtrosGuardados.subtipoImputacionCombo;
            this.fechaInicio = filtrosGuardados.fechaDesde;
            this.fechaFin = filtrosGuardados.fechaHasta;
            this.pageIndex = filtrosGuardados.pageIndex;
            if (this.fechaInicio != undefined && this.fechaInicio.length > 0) {
                const [year, month, day] = this.fechaInicio.split('-').map(Number); //se maneja el cambio de día incorrecto por la zona horaria local
                if (this.fechaFin != undefined && this.fechaFin.length > 0) {
                    const [year2, month2, day2] = this.fechaFin.split('-').map(Number);
                    this.rangeDates = [new Date(year, month - 1, day), new Date(year2, month2 - 1, day2)];
                } else {
                    this.rangeDates = [new Date(year, month - 1, day)];
                }
            }
        }
    }

    listarClaseDocumento() {
        try {
            this.subscription = this.service.listarClaseDocumento(sessionStorage.getItem("usuarioId")).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else {
                        this.clasesDocumento = result;
                        if (this.clasesDocumento.length > 0)
                            sessionStorage.setItem('clasesDocumentoUsuario', JSON.stringify(this.clasesDocumento));
                    }
                },
                error => { this.floatMsgService.setErrorMsg(error.message); }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false;
        }
        return false;
    }

    mismaClaseDocumento(claseDocumento_Id: number): boolean {
        return this.clasesDocumento.some(x => x === claseDocumento_Id);
    }

    cerrarExpansiones(): void {
        this.tabla.value.forEach(row => {
            if (this.tabla.isRowExpanded(row)) {
              this.tabla.toggleRow(row);
            }
          });
      }
}