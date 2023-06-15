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

declare var $: any;


@Component({
    selector: 'app-listado-dashboard-comprador',
    templateUrl: `listado-dashboard-comprador.component.html`,
    styleUrls: ['../../compras.component.css',
        './listado-dashboard-comprador.component.css']

})
export class ListadoDashboardCompradorComponent extends ListBaseComponent {

    protected locale: any;

    @ViewChild("tabla")
    protected tabla: Table;

    @BlockUI() blockUI: NgBlockUI;

    @Input('model')
    protected model: Solp;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

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
    ordenesDeCompra: any;

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");

    }
    //#region Variables 
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
    //#endregion

    ngOnInit() {
        this.getListarSolp();
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
                        console.log(result.data, "Compras solp")
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
        this.service.getListarSolpCompras(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp);

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

    verLegajo(Id) {
        this.blockUI.start('Cargando...')
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

    descargarLegajo() {
        let idPeticion = this.legajo[0].PeticionDeOfertaId;
        this.blockUI.start('Generando...')
        this.service.descargarLegajo(idPeticion,null)
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

        this.blockUI.start('Subiendo archivos...')
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

    verOfertas(Id : string) {
        this.goToSeccionParam('/compras/ver-ofertas', Id);
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
        this.blockUI.start('Cargando...')
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

    verDetalleOrdenDeCompra(id: any) {
        this.obtenerAdjudicacion(id);
        this.displayOrdenDeCompra = true;
    }

    obtenerAdjudicacion(adjudicacionId){
        console.log("obtenerAdjudicacion" ,adjudicacionId)
        this.blockUI.start('Cargando...')
        this.service.obtenerAdjudicacion(adjudicacionId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.ordenDeCompra = result.data;
                        console.log("obtenerAdjudicacion" ,result.data)
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    listarAdjudicaciones(solpId){      
        this.blockUI.start('Cargando...')
        this.service.listarAdjudicaciones(solpId)
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        this.ordenesDeCompra = result.data;
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

}

