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
import { AdjudicacionDto, AdjudicacionPosicionDto } from '../../../modelos/adjudicacion';
import { ChatComprasDto } from '../../chat-interno/chat-interno.interface';

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
    ordenDeCompra: any;
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
    usuariosResult: any;
    displayChatInterno: boolean = false;
    chatLeido: boolean = false;

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username"); this.onBuscar();
    }

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

    ngOnInit() {
        this.getListarSolp();
    }

    ngAfterViewInit(): void {
        this.getCombos();
    }

    ngOnDestroy(): void {
        this.subscripcionSolp.unsubscribe();
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

        return false; //<-- Prevent Refresh
    }

    listarSolp() {
        this.spinnerComponent.showIt();
        this.service.getListarSolpCompras(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp, this.selectEstadoSolp.join(","),
            this.selectUsuario.join(","), this.selectCentro.join(","), this.selectGrupoCompras.join(","));

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
        this.service.verLegajo(Id, null)
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

    publicarCotizacion(Id: string) {
        this.goToSeccionParam('/compras/peticion-de-oferta-formulario', Id);
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

    public chat: ChatComprasDto;

    obtenerPeticionDeOfertaParaChat(Id) {
        try {
            this.displayChatInterno = false;
            this.subscription = this.service.obtenerChat(Id)
              .subscribe(
                (result: any) => {
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
                        this.chatLeido = true;
                    };
                },
                (error) => {
                  this.floatMsgService.setErrorMsg(error.message);
              }
          );
      } catch (e) {
          this.floatMsgService.setErrorMsg(e);
          return false; //<-- Prevent Refresh
      }
      return false; //<-- Prevent Refresh
    }

    cerrarModalCotizacion() {
        this.displayCerrarCotizacion = false;
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
                        result.EstadosSolpSap.forEach(e => this.estadoSolpItem.push({
                            label: e.Descripcion, value: e.Id
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

    onBuscar() {
        this.service.getListarSolpCompras(1, 10, "", "", this.nroSolp, this.selectEstadoSolp.join(","),
            this.selectUsuario.join(","), this.selectCentro.join(","), this.selectGrupoCompras.join(","));
    }
}