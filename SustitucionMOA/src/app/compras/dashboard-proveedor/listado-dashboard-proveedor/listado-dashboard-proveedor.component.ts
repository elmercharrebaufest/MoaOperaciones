import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Table } from 'primeng/table';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
import { Paginator } from 'primeng/paginator';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { SelectItem } from 'primeng/api';

@Component({
    selector: 'app-listado-dashboard-proveedor',
    templateUrl: './listado-dashboard-proveedor.component.html',
    styleUrls: ['../../compras.component.css', './listado-dashboard-proveedor.component.css']
})
export class ListadoDashboardProveedorComponent extends ListBaseComponent {

    protected locale: any = {
        firstDayOfWeek: 0,
        dayNames: ["Domingo", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado"],
        dayNamesShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        today: 'Hoy',
        clear: 'Borrar'
    };

    @ViewChild("tabla")
    protected tabla: Table;

    @BlockUI() blockUI: NgBlockUI;

    @Input('po')
    protected po: PeticionDeOfertaDto;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    @ViewChild('myCalendar', undefined)
    nroSolp: string = "";
    nroPo: string = "";
    nombrePedido: string = "";
    orden: string;
    columnaOrden: string;
    length = 0;
    pageSize: number = 10;
    pageIndex: number = 1;
    @ViewChild('paginator') paginator: Paginator
    subscripcionPO: Subscription
    peticion: PeticionDeOfertaDto;
    displayLegajo: boolean = false;
    legajo: any;
    usuarioProveedor: boolean = true;
    itemSelected: any;
    tablaPO: any[];
    cols: any[];
    usuario: string;// = "Prueba";
    // verTodas: boolean = this.isAuthorized('VER SOLPS PROVEEDOR');
    estadoCotizacion: SelectItem[] = [{ label: "Sin Cotizar", value: 0 }, { label: "Cotizado", value: 1 }, { label: "Incompleta", value: 2 }];
    selectEstadoCotizacion: number | null = null;
    estadoLicitacion: SelectItem[] = [{ label: "Abierto", value: 1 }, { label: "Cerrado", value: 2 }];
    selectEstadoLicitacion: number | null = null;
    fechaDesde: string = null;
    fechaHasta: string = null;
    rangeDates: Date[];
    filtrosProveedor: {
        nroSolp: string;
        nroPo: string;
        nombrePedido: string;
        usuarios: string[];
        estadoLicitacion: number | null;
        estadoCotizacion: number | null;
        fechaDesde: string;
        fechaHasta: string;
    } = {
            nroSolp: "",
            nroPo: "",
            nombrePedido: "",
            usuarios: [],
            estadoLicitacion: null,
            estadoCotizacion: null,
            fechaDesde: null,
            fechaHasta: null,
        };

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");
    }

    ngOnInit() {
        this.recuperarFiltros();
        this.getListarPO();
        this.listarPO();
    }

    ngOnDestroy(): void {
        this.subscripcionPO.unsubscribe();
    }

    getListarPO() {
        try {
            this.spinnerComponent.showIt();
            this.subscripcionPO = this.service.observableListaPO.subscribe(
                (result: any) => {

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.tablaPO = result.data;
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

    listarPO() {
        this.spinnerComponent.showIt();
        this.service.getListarPOProveedor(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp, this.nroPo, this.nombrePedido, this.selectEstadoLicitacion, this.selectEstadoCotizacion, this.fechaDesde, this.fechaHasta);
    }

    onOrder(columna: string) {
        if (this.columnaOrden != columna) {
            this.orden = "DESC"
        } else {
            this.orden = this.orden == "DESC" ? "ASC" : "DESC";
        }
        this.columnaOrden = columna;
        this.listarPO();
    }

    handlePageEvent(e: any) {
        this.pageSize = e.rows;
        this.pageIndex = e.page + 1;
        this.listarPO();
    }

    verLegajo(item) {
        this.blockUI.start('Cargando...')
        this.itemSelected = item;
        this.itemSelected.Usuarios[0].CircularSinLeer = false;
        this.service.verLegajo(item.Id, item.Usuarios[0].Id)
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

    descargarLegajo() {
        let idPeticion = this.legajo[0].PeticionDeOfertaId;
        this.blockUI.start('Generando...')
        this.service.descargarLegajo(this.itemSelected.Id, this.itemSelected.Usuarios[0].Id)
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

    publicarCotizacion(Id: string) {
        this.goToSeccionParam('/compras/dashboard-proveedor/cotizacion', Id);
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
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            )
    }

    descargarArchivo({ archivoId }) {
        if (archivoId == 0) {
            let SolpId = this.legajo[0].SolpId;
            this.blockUI.start("Generando...");
            this.service.getPdf(SolpId)
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
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                )
        } else if (archivoId < 0) {
            this.blockUI.start("Generando...");
            this.service.getPdfPeticionDeOfertaUsuario(archivoId)
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
                        this.blockUI.stop();
                    },
                    (error) => {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                )
        }
        else {
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

    onBuscar() {
        this.filtrosProveedor.nroSolp = this.nroSolp;
        this.filtrosProveedor.nroPo = this.nroPo;
        this.filtrosProveedor.nombrePedido = this.nombrePedido;
        this.filtrosProveedor.estadoLicitacion = this.selectEstadoLicitacion;
        this.filtrosProveedor.estadoCotizacion = this.selectEstadoCotizacion;
        this.filtrosProveedor.fechaDesde = this.fechaDesde;
        this.filtrosProveedor.fechaHasta = this.fechaHasta;
        this.spinnerComponent.showIt();
        this.service.getListarPOProveedor(1, 10, "", "", this.nroSolp, this.nroPo, this.nombrePedido, this.selectEstadoLicitacion, this.selectEstadoCotizacion, this.fechaDesde, this.fechaHasta);
        sessionStorage.setItem('filtrosProveedor', JSON.stringify(this.filtrosProveedor));
    }

    returnToTodaysDate() {
        this.fechaDesde = "";
        this.fechaHasta = "";
        if (this.tablaPO.length > 0) {
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
        const filtrosGuardados = JSON.parse(sessionStorage.getItem('filtrosProveedor'));
        if (filtrosGuardados) {
            this.nroSolp = filtrosGuardados.nroSolp;
            this.nroPo = filtrosGuardados.nroPo;
            this.nombrePedido = filtrosGuardados.nombrePedido;
            this.selectEstadoLicitacion = filtrosGuardados.estadoLicitacion;
            this.selectEstadoCotizacion = filtrosGuardados.estadoCotizacion;
            this.fechaDesde = filtrosGuardados.fechaDesde;
            this.fechaHasta = filtrosGuardados.fechaHasta;
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
}