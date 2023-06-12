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
import { Paginator } from 'primeng/paginator';
import { PeticionDeOfertaDto } from '../../modelos/peticion-de-oferta-model';
import { forEach } from '@angular/router/src/utils/collection';

declare var $: any;


@Component({
    selector: 'dashboard',
    templateUrl: `dashboard.component.html`,
    styleUrls: ['../compras.component.css',
        './dashboard.component.css']

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
    private calendar: any;
    nroSolp: string = "";
    sap: boolean = false;
    mantenimiento: boolean = false;
    web: boolean = false;
    orden: string;
    columnaOrden: string;
    length = 0;
    pageSize: number = 10;
    pageIndex: number = 1;
    @ViewChild('paginator') paginator: Paginator
    public peticion: PeticionDeOfertaDto;
    public ordenCompra: any;

    displayRevisionTecnica: boolean;

    displayCircular: boolean = false;
    combos: any;
    usuariosResult: any;
    
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

    filteredfechas: any;
    solpFecha: any = new Array();
    fechaInicio: any = null;
    fechaFin: any = null;
    rangeDates: Date[];
    tipoFiltroFecha = 1;


    desdeDashboard: Date;
    hastaDashboard: Date;
    estadoSolpItem: SelectItem[];
    selectEstadoSolp: string[] = [];
    buscarDashboard: string;
    fechaSolp: any;
    hoy: Date = new Date();
    es: any;
    display: boolean = false;
    tablaSolp: any[];
    tablaSolpCopy: any[];

    usuarioFiltro: SelectItem[];
    selectUsuario: number | null;

    cols: any[];
    serviciosDashboard: any = "Servicios"
    solp: Solp = new Solp();
    usuario: string;// = "Prueba";

    checkedFilterSap = false;
    checkedFilterMantenimiento = false;
    checkedFilterWeb = false;

    verTodas: boolean = this.isAuthorized('VER TODAS SOLPS');

    showDialog() {
        this.display = true;
    }

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
        this.getListarSolp();

        this.desdeDashboard = new Date();
        this.hastaDashboard = new Date();
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
        } else {
            let d = new Date(Date.parse(event));
            this.fechaFin = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
            if (this.rangeDates[1]) { // If second date is selected
                this.calendar.overlayVisible = false;
            }
        }
    }

    filtrarPorSap() {
        this.checkedFilterSap = !this.checkedFilterSap;
        this.filtrarTablaPorTipoSolp();
    }

    filtrarPorMantenimiento() {
        this.checkedFilterMantenimiento = !this.checkedFilterMantenimiento;
        this.filtrarTablaPorTipoSolp();
    }

    filtrarPorWeb() {
        this.checkedFilterWeb = !this.checkedFilterWeb;
        this.filtrarTablaPorTipoSolp();
    }

    filtrarTablaPorTipoSolp() {

        var fechaDesde = this.fechaInicio;
        var fechaHasta = this.fechaFin + " 23:59:59";

        let tablaPrincipal = this.tablaSolpCopy;
        if (this.checkedFilterMantenimiento && this.checkedFilterWeb && this.checkedFilterSap) {
            tablaPrincipal = tablaPrincipal;
            this.tabla.first = 0;
        } else if (this.checkedFilterSap && this.checkedFilterMantenimiento) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.SAP || x.TipoSolpSap === EnumTipoSolpSap.Mantenimiento);
            this.tabla.first = 0;
        } else if (this.checkedFilterSap && this.checkedFilterWeb) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.SAP || x.TipoSolpSap === EnumTipoSolpSap.Web);
            this.tabla.first = 0;
        } else if (this.checkedFilterMantenimiento && this.checkedFilterWeb) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.Mantenimiento || x.TipoSolpSap === EnumTipoSolpSap.Web);
            this.tabla.first = 0;
        } else if (this.checkedFilterMantenimiento) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.Mantenimiento);
            this.tabla.first = 0;
        } else if (this.checkedFilterSap) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.SAP);
            this.tabla.first = 0;
        } else if (this.checkedFilterWeb) {
            tablaPrincipal = tablaPrincipal.filter(x => x.TipoSolpSap === EnumTipoSolpSap.Web);
            this.tabla.first = 0;
        } this.tabla.first = 0;


        if (fechaDesde != null && fechaHasta != null) {
            tablaPrincipal = tablaPrincipal.filter(x =>
                new Date(Date.parse(x.FechaCreacion)) >= new Date(fechaDesde) &&
                new Date(Date.parse(x.FechaCreacion)) <= new Date(fechaHasta)
            )
            this.tabla.first = 0;
        }
        this.tablaSolp = tablaPrincipal;

        if (this.tablaSolp.length == 0) {
            this.mensajeComponent.setInfoMsg("No se encontraron Solps")
        }
        else {
            this.mensajeComponent.setMsgsEmpty();
        }
    }

    ngAfterViewInit(): void {
        this.getCombos();
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
            this.subscription = this.service.getListarSolp(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp,
                this.fechaInicio, this.fechaFin, this.sap, this.mantenimiento, this.web, multiSelectValues, this.selectUsuario
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
                        result.EstadosSolpSap.forEach(cd => this.estadoSolpItem.push({
                            label: cd.Descripcion, value: cd.Id
                        }));
                        result.Usuarios.forEach(x => x.forEach(d => this.usuarioFiltro.push({
                            label: d.Mail, value: d.Id
                        })))
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

    eliminarPosicionDashboard(idSolp) {
        this.confirmationService.confirm({
            message: '¿Está seguro que desea eliminar la SOLP?',
            accept: () => {
                this.borrarSolp(idSolp)
            },
            reject: () => {
            }
        });
    }

    generarZipPliego(idSolp) {
        this.blockUI.start('Generando ')
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
                }
            )
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
                    }
                )
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
        this.getListarSolp()

    }

    obtenerPeticionDeOferta(Id) {
        this.blockUI.start('Cargando...')
        this.service.obtenerPeticionDeOferta(Id)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.peticion = result.data;
                        this.displayRevisionTecnica = true;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
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
                }
            )
    }

    cerrarModalRevisionTecnica() {
        this.displayRevisionTecnica = false;
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
                }
            )
    }

    descargarAdjuntosCotizacion({ cotizacionId }) {
        this.blockUI.start("Descargando...");
        this.service.DescargarAdjuntosCotizacion(cotizacionId)
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

    grabarRevisionTecnica() {
        this.blockUI.start('Grabando...');
        this.service.grabarRevisionTecnica(this.peticion.Usuarios)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (result) {

                        }
                        this.displayRevisionTecnica = false;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    cerrarCircular() {
        this.displayCircular = false;
    }

    onBuscar() {
        this.paginator.changePage(0);
        this.pageIndex = 1;
        this.getListarSolp();
    }
}

