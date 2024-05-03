import { Component, Input, ViewChild, ViewChildren } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Paginator } from 'primeng/paginator';
import { ComprasService } from '../../compras.service';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { Solp } from '../../solp/solp';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { NavService } from '../../../common/services/NavService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SecurityService } from '../../../common/services/SecurityService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { Subscription } from 'rxjs';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
import { CircularDto } from '../../../modelos/circular-model';
import { AdjudicacionDto, AdjudicacionEdicionDto, AdjudicacionPosicionDto } from '../../../modelos/adjudicacion';
import { ChatComprasDto } from '../../chat-interno/chat-interno.interface';
import { EnumTipoImputacion } from '../../enum-tipo-imputacion';

declare var $: any;

@Component({
    selector: 'app-listado-dashboard-comprador',
    templateUrl: `listado-dashboard-comprador.component.html`,
    styleUrls: ['../../compras.component.css',
        './listado-dashboard-comprador.component.css']

})
export class ListadoDashboardCompradorComponent extends ListBaseComponent {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild("tabla")
    @Input('model')
    @ViewChild('paginator') paginator: Paginator
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    protected locale: any;
    protected model: Solp;
    protected tabla: Table;
    nroSolp: string = "";
    sap: boolean = false;
    mantenimiento: boolean = false;
    web: boolean = false;
    repoAutomatica: boolean = false;
    listarPendiente: boolean = false;
    contratoMarco: boolean = false;
    orden: string;
    columnaOrden: string;
    length = 0;
    pageSize: number = 10;
    pageIndex: number = 1;
    subscripcionSolp: Subscription
    displayLegajo: boolean = false;
    legajo: any;
    peticion: PeticionDeOfertaDto;
    displayCircular: boolean = false;
    circular: CircularDto
    nroCotizacion: any;
    displayOkCircular: boolean;
    displayProveedor: boolean;
    usuarioProveedor: boolean = false;
    ordenDeCompra: AdjudicacionEdicionDto;
    displayOrdenDeCompra: boolean;
    ordenDeCompraId: any;
    ordenesDeCompra: AdjudicacionDto[] = [];
    visualizarAlertCotizacion: boolean;
    usuario: string;
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
    usuariosResult: any;
    displayChatInterno: boolean = false;
    public chat: ChatComprasDto;
    tratada: any;
    fechaDesde: string = null;
    fechaHasta: string = null;
    rangeDates: Date[];
    filtrosComprador: {
        nroSolp: string;
        sap: boolean;
        mantenimiento: boolean;
        web: boolean;
        repoAutomatica: boolean;
        listarPendiente: boolean;
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
            listarPendiente: false,
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

    tablaSolp: any[];
    tablaSolpCopy: any[];
    cols: any[];
    serviciosDashboard: any = "Servicios"
    solp: Solp = new Solp();
    checkedFilterSap = false;
    checkedFilterMantenimiento = false;
    checkedFilterWeb = false;
    verTodas: boolean = this.isAuthorized('VER TODAS SOLPS');
    displayCerrarCotizacion: boolean;
    displayEditarOc: boolean;
    displayVisualizarErrores: boolean;
    errores: any = [];
    mensaje: string;
    displayAdjudicacionCreada: boolean;
    esProveedor: boolean = false;


    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username"); this.recuperarFiltros(); this.listarSolp();
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

    ngOnInit() {
        this.recuperarFiltros();
        this.getListarSolp();
    }

    ngAfterViewInit(): void {
        this.getCombos();
    }

    ngOnDestroy(): void {
        this.subscripcionSolp.unsubscribe();
    }

    listarExpand() {
        setTimeout(() => {
            $('[id^="ui-tabpanel-"]').css('padding', '0');
            $('[id^="ui-tabpanel-"]').css('transition', 'none').css('animation', 'none');
        }, 0.01);
    }

    getListarSolp() {
        try {
            this.spinnerComponent.showIt();
            this.subscripcionSolp = this.service.observableListaSolp.subscribe(
                (result: any) => {

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.tablaSolp = result.data;
                        this.length = result.data.length > 0 ? result.data[0].ItemsTotales : result.data.length;
                        this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
                        this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;
                        this.paginator.first = this.pageIndex * this.pageSize - this.pageSize;
                    }
                    this.spinnerComponent.hideIt()
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt()
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerComponent.hideIt()
            return false; //<-- Prevent Refresh
        }

        return false;
    }

    listarSolp() {
        this.spinnerComponent.showIt();
        this.service.getListarSolpCompras(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp, this.selectEstadoSolp.join(","), this.selectUsuario.join(","), this.selectCentro.join(","), this.selectGrupoCompras.join(","),
            this.fechaDesde, this.fechaHasta, this.sap, this.mantenimiento, this.web, this.repoAutomatica, this.listarPendiente, this.contratoMarco, this.selectClaseDocumento.join(","), this.selectTipoImputacion.join(","), this.selectValorTipoImputacion.join(","));
    }

    onOrder(columna: string) {
        if (this.columnaOrden != columna) {
            this.orden = "DESC"
        } else {
            this.orden = this.orden == "DESC" ? "ASC" : "DESC";
        }
        this.columnaOrden = columna;
        this.listarSolp();
    }

    handlePageEvent(e: any) {
        this.pageSize = e.rows;
        this.pageIndex = e.page + 1;
        this.filtrosComprador = {
            ...this.filtrosComprador,
            pageIndex: this.pageIndex
        };
        sessionStorage.setItem('filtrosComprador', JSON.stringify(this.filtrosComprador));
        this.listarSolp();
    }

    generarZipPliego(idSolp) {
        this.blockUI.start('Generando...');
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
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }

    verLegajo(Id) {
        this.blockUI.start('Cargando...');
        this.service.verLegajo(Id, null, this.esProveedor)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.legajo = result.data;
                        this.displayLegajo = true;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    cerrarLegajo() {
        this.displayLegajo = false;
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

    descargarLegajo() {
        let idPeticion = this.legajo[0].PeticionDeOfertaId;
        this.blockUI.start('Generando...');
        this.service.descargarLegajo(idPeticion, null)
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

    adjuntarArchivoLegajo(files) {
        let peticionId = this.legajo[0].PeticionDeOfertaId;
        //todo adjuntar los archivos

        this.blockUI.start('Subiendo archivos...');
        this.service.adjuntarArchivoLegajo(peticionId, files)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.verLegajo(peticionId);
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    publicarCotizacion(Id: string, nroSolp: string) {
        this.blockUI.start('Cargando...');
        this.service.validarSolpTratada(nroSolp)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (result) {
                            this.floatMsgService.setErrorMsg("No se puede crear una nueva PO porque la SOLP fue tratada desde SAP");
                        } else {
                            this.goToSeccionParam('/compras/peticion-de-oferta-formulario', Id);
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )


    }

    onRowDblClick(a, b) {

    }

    verOfertas(Id: string) {
        this.goToSeccionParam('/compras/ver-ofertas', Id);
    }

    obtenerPeticionDeOferta(Id) {
        this.blockUI.start('Cargando...');
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

    obtenerPeticionDeOfertaParaProveedor(Id) {
        this.blockUI.start('Cargando...');
        this.service.obtenerPeticionDeOferta(Id)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.peticion = result.data;
                        this.displayProveedor = true;
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
        this.listarSolp();
    }

    cancelarModal() {
        this.displayCircular = false;
        this.displayCerrarCotizacion = false;
    }

    cerrarModalProveedor() {
        this.displayProveedor = false;
    }

    cerrarOrdenDeCompra() {
        this.displayOrdenDeCompra = false;
    }

    verDetalleOrdenDeCompra(nroOC: any) {
        this.obtenerAdjudicacion(nroOC);
        this.displayOrdenDeCompra = true;
    }

    obtenerAdjudicacion(nroOC) {
        this.blockUI.start('Cargando...');
        this.service.obtenerAdjudicacion(nroOC)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.ordenDeCompra = result.data;
                        this.parsearFecha();
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    listarAdjudicaciones(solpId) {
        this.blockUI.start('Cargando...');
        this.service.listarAdjudicaciones(solpId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        if (this.ordenesDeCompra.length > 0) {
                            for (let i = this.ordenesDeCompra.length - 1; i >= 0; i--) {
                                if (this.ordenesDeCompra[i].Solp_Id === solpId) {
                                    this.ordenesDeCompra.splice(i, 1);
                                }
                            }
                        }
                        if (result.data) {
                            this.mapData(result.data);
                            this.blockUI.stop();
                        } else if (result.error) {
                            this.blockUI.stop();
                            this.mensajeComponent.setErrorMsg(result.error);
                        }
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
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

    obtenerPeticionDeOfertaParaCerrar(Id) {
        this.blockUI.start('Cargando...');
        this.service.obtenerPeticionDeOferta(Id)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.peticion = result.data;
                        this.displayCerrarCotizacion = true;
                        this.visualizarAlertCotizacion = false;
                        if (this.peticion.Usuarios.every(usuario => usuario.Cotizacion == null)) {
                            this.visualizarAlertCotizacion = true;
                        }
                        else {
                            if (this.peticion.Usuarios.some(usuario => usuario.Cotizacion != null && usuario.Cotizacion.CotizacionEstadoDescripcion == "Cotizado")) {
                                this.visualizarAlertCotizacion = false;
                            } else {
                                this.visualizarAlertCotizacion = true;
                            }
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
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

    cerrarModalCotizacion() {
        this.displayCerrarCotizacion = false;
        this.listarSolp();
    }

    cerrarModalChat() {
        this.displayChatInterno = false;
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
                        result.EstadosSolpSap.forEach(e => this.estadoSolpItem.push({
                            label: e.Descripcion, value: e.Id
                        }));
                        result.Usuarios.forEach(x => x.forEach(d => this.usuarioFiltro.push({
                            label: d.Id === 0 ? "" : d.Mail, value: d.Id
                        })));
                        result.Centro.forEach(c => c.FiltroComprador === true && this.centroFiltro.push({
                            label: c.Codigo + " - " + c.Descripcion, value: c.Id
                        }));
                        result.GrupoCompras.forEach(gc => gc.FiltroComprador === true && this.grupoComprasFiltro.push({
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
        return false; //<-- Prevent Refresh
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

    onBuscar() {
        this.pageIndex = 1;
        this.spinnerComponent.showIt();
        this.filtrosComprador.nroSolp = this.nroSolp;
        this.filtrosComprador.sap = this.sap;
        this.filtrosComprador.mantenimiento = this.mantenimiento;
        this.filtrosComprador.web = this.web;
        this.filtrosComprador.repoAutomatica = this.repoAutomatica;
        this.filtrosComprador.listarPendiente = this.listarPendiente;
        this.filtrosComprador.contratoMarco = this.contratoMarco;
        this.filtrosComprador.usuarios = this.selectUsuario;
        this.filtrosComprador.estadoSolp = this.selectEstadoSolp;
        this.filtrosComprador.gruposCompras = this.selectGrupoCompras;
        this.filtrosComprador.centros = this.selectCentro;
        this.filtrosComprador.claseDocumento = this.selectClaseDocumento;
        this.filtrosComprador.tipoImputacion = this.selectTipoImputacion;
        this.filtrosComprador.valorTipoImputacion = this.selectValorTipoImputacion;
        this.filtrosComprador.subtipoImputacionCombo = this.valorTipoImputacionFiltro;
        this.filtrosComprador.fechaDesde = this.fechaDesde;
        this.filtrosComprador.fechaHasta = this.fechaHasta;
        this.paginator.changePage(0);
        this.listarSolp();
        sessionStorage.setItem('filtrosComprador', JSON.stringify(this.filtrosComprador));
    }

    returnToTodaysDate() {
        this.fechaDesde = "";
        this.fechaHasta = "";
        if (this.tablaSolp.length > 0) {
            this.mensajeComponent.setMsgsEmpty();
        }
    }

    onSelect(event: any) {
        if (this.rangeDates[0] && this.rangeDates[1] == null) {
            let d = new Date(Date.parse(event));
            this.fechaDesde = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
            this.fechaHasta = '';
        } else {
            let d = new Date(Date.parse(event));
            this.fechaHasta = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
        }
    }

    recuperarFiltros() {
        const filtrosGuardados = JSON.parse(sessionStorage.getItem('filtrosComprador'));
        if (filtrosGuardados) {
            this.nroSolp = filtrosGuardados.nroSolp;
            this.sap = filtrosGuardados.sap;
            this.mantenimiento = filtrosGuardados.mantenimiento;
            this.web = filtrosGuardados.web;
            this.repoAutomatica = filtrosGuardados.repoAutomatica;
            this.listarPendiente = filtrosGuardados.listarPendiente;
            this.contratoMarco = filtrosGuardados.contratoMarco;
            this.selectUsuario = filtrosGuardados.usuarios;
            this.selectEstadoSolp = filtrosGuardados.estadoSolp;
            this.selectGrupoCompras = filtrosGuardados.gruposCompras;
            this.selectCentro = filtrosGuardados.centros;
            this.selectClaseDocumento = filtrosGuardados.claseDocumento;
            this.selectTipoImputacion = filtrosGuardados.tipoImputacion;
            this.selectValorTipoImputacion = filtrosGuardados.valorTipoImputacion;
            this.valorTipoImputacionFiltro = filtrosGuardados.subtipoImputacionCombo;
            this.fechaDesde = filtrosGuardados.fechaDesde;
            this.fechaHasta = filtrosGuardados.fechaHasta;
            this.pageIndex = filtrosGuardados.pageIndex;
            if (this.fechaDesde != undefined && this.fechaDesde.length > 0) {
                const [year, month, day] = this.fechaDesde.split('-').map(Number); //se maneja el cambio de día incorrecto por la zona horaria local
                if (this.fechaHasta != undefined && this.fechaHasta.length > 0) {
                    const [year2, month2, day2] = this.fechaHasta.split('-').map(Number);
                    this.rangeDates = [new Date(year, month - 1, day), new Date(year2, month2 - 1, day2)];
                } else {
                    this.rangeDates = [new Date(year, month - 1, day)];
                }
            }
        }
    }

    abrirModalEditarOc() {
        this.displayEditarOc = true;
    }

    cerrarEditarOc() {
        this.displayEditarOc = false;
        this.mensaje = "";
    }

    guardarEditarOc(event: any) {
        this.validarOcCompleta(event);
        if (this.mensaje == "") {
            this.guardarAdjudicacion(event);
        }
    }

    guardarAdjudicacion(ordenDeCompra) {
        this.blockUI.start("Grabando...");

        try {

            this.subscription = this.service.ModificarAdjudicacion(ordenDeCompra).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    }
                    else if (result.Errores != undefined && result.Errores != null && result.Errores.length > 0) {
                        this.errores = result.Errores;
                        this.displayVisualizarErrores = true;
                    }
                    else {
                        this.cerrarEditarOc();
                        this.displayAdjudicacionCreada = true;
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    salirVisualizarErrores() {
        this.displayVisualizarErrores = false;
    }

    salirConfirmacionDeActualizacionOc() {
        this.listarAdjudicaciones(this.ordenDeCompra.Solp_Id);
        this.displayAdjudicacionCreada = false;
    }

    validarOcCompleta(ordenDeCompra: AdjudicacionEdicionDto) {
        this.mensaje = "";
        var breakFor = false;

        if (ordenDeCompra.AdjudicacionPosiciones[0].MonedaId == 0) {
            this.mensaje = "La Moneda es obligatoria";
            breakFor = true;
            return this.mensaje;
        }
        const self = this;
        ordenDeCompra.AdjudicacionPosiciones.forEach(function (adjudicacion, i) {
            if (!breakFor) {
                if (adjudicacion.Cantidad <= 0 || adjudicacion.Cantidad == undefined) {
                    self.mensaje = "Pos. " + adjudicacion.Indice + " - La cantidad es obligatoria";
                    breakFor = true;
                    return self.mensaje;
                }

                if (adjudicacion.PrecioUnidad <= 0) {
                    self.mensaje = "Pos. " + adjudicacion.Indice + " - El Precio es obligatorio";
                    breakFor = true;
                    return self.mensaje;
                }
                const decimalPart = (adjudicacion.PrecioUnidad % 1).toFixed(2);
                if (decimalPart != '0.00' && adjudicacion.MonedaId == "CLP") {
                    self.mensaje = "Pos. " + adjudicacion.Indice + ": Para la moneda seleccionada no es posible ingresar decimales en el precio";
                    breakFor = true;
                    return self.mensaje;
                }
                if (adjudicacion.FechaEntregaServicio == null) {
                    self.mensaje = "Pos. " + adjudicacion.Indice + " - La fecha de entrega es obligatoria";
                    breakFor = true;
                    return self.mensaje;
                }
            }
            if (adjudicacion.SubposicionesCompras != null) {
                adjudicacion.SubposicionesCompras.forEach(function (subposicion, i) {
                    if (!breakFor) {
                        if (subposicion.Cantidad <= 0 || subposicion.Cantidad == undefined) {
                            self.mensaje = "Pos. " + subposicion.Numero + ": La cantidad es obligatoria";
                            breakFor = true;
                            return self.mensaje;
                        }

                        if (subposicion.PrecioBruto <= 0) {
                            self.mensaje = "Pos. " + subposicion.Numero + ": El precio es obligatorio";
                            breakFor = true;
                            return self.mensaje;
                        }
                        const decimalPart = (subposicion.PrecioBruto % 1).toFixed(2);
                        if (decimalPart != '0.00' && adjudicacion.MonedaId == "CLP") {
                            self.mensaje = "Pos. " + subposicion.Numero + ": Para la moneda seleccionada no es posible ingresar decimales en el precio";
                            breakFor = true;
                            return self.mensaje;
                        }
                    }
                });
            }
        });

        return this.mensaje;
    }

    editarOrdenDeCompra(nroOC: any) {
        this.obtenerAdjudicacion(nroOC);
        this.abrirModalEditarOc();
    }

    public parsearFecha() {
        if (this.ordenDeCompra != undefined) {
            for (let index = 0; index < this.ordenDeCompra.AdjudicacionPosiciones.length; index++) {

                if (this.ordenDeCompra.AdjudicacionPosiciones[index].FechaEntregaServicio != null) {
                    var milliseconds = parseInt(this.ordenDeCompra.AdjudicacionPosiciones[index].FechaEntregaServicio.substring(6));
                    var date = new Date(milliseconds);
                    this.ordenDeCompra.AdjudicacionPosiciones[index].FechaEntregaServicio = date;
                }
            }
        }
    }
}