import { Component, ViewChild, OnInit } from '@angular/core';
import { NavService } from '../../common/services/NavService';
import { BaseComponent } from '../../common/base-components/base-component';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { AltaEmpresaService } from './altas.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { Empresa } from './Empresa';
import { release } from 'os';
import { Archivo } from '../../common/models/archivo';


@Component({
    selector: 'app-altas',
    templateUrl: './app/alta-proveedores/altas/altas.component.html',
    styleUrls: ['./app/alta-proveedores/altas/altas.component.css', '../Content/css/bootstrap.min.css'],
    providers: [AltaEmpresaService]
})

export class AltasComponent extends BaseComponent implements OnInit {
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("smallSpinner")
    protected spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected altaEmpresaService: AltaEmpresaService, protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    dataFiltered: any;
    estados: any;
    selectedEstado: string = "q";
    orderedByColumn: string = "id";
    empresaSeleccionada: Empresa = new Empresa();
    empresaEstadoSeleccionada: number = 0;
    orderDirection: number = 1;
    itemsPerPage = 20;
    observaciones: string = "";
    observacionesProveedor: string = "";
    mensajeError: string = "";

    listaArchivos: Array<Archivo> = [];

    ngOnInit(): void {
        this.getEstados();
    }

    verDetalle() {
        this.navService.navegarSeccion('/proveedor-detalle');
        return false;
    }
    getEmpresa() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.altaEmpresaService.getEmpresas().subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                        this.dataFiltered = result.data;
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }
    getEstados() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.altaEmpresaService.getEstados().subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.estados = result.data;
                        this.getEmpresa();
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }
    isVisible() {
        return this.data && this.data.length != 0;
    }
    isVisibleError() {
        return this.mensajeError != "";
    }
    orderColumnBy(column: string) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }

    cambiarEstado(estadoId: number) {
        if (estadoId == 4 && this.observacionesProveedor == "") {
            this.mensajeError = "Debe ingresar una observacion para el Proveedor.";
            return false;
        }
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.altaEmpresaService.setEstadoAprobacion(this.empresaSeleccionada.Id, estadoId, this.observaciones, this.observacionesProveedor).subscribe(
                result => {
                    this.getEmpresa();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }

                    this.observaciones = "";
                    this.observacionesProveedor = "";
                    this.mensajeError = "";
                    document.getElementById("hidemyModal").click();
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }


    abrirModal(empresa: Empresa) {
        this.empresaSeleccionada = empresa;
        this.observaciones = "";
        this.observacionesProveedor = "";
        this.mensajeError = "";
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.obtenerArchivosSubidos(empresa.Mail);
        document.getElementById("openModalHiddenButton").click();
        return false;
    }

    obtenerArchivosSubidos(mail: string) {
        this.subscription = this.service.obtenerArchivosSubidos(mail).subscribe(
            result => {
                this.listaArchivos = new Array();

                result.forEach(element => {
                    let archivo = new Archivo();
                    archivo = element;
                    this.listaArchivos.push(archivo);
                });
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    descargarArchivo(archivo: Archivo) {
        if (this.mensajeComponent === undefined)
            this.mensajeComponent = new MensajeComponent();

        if (this.spinnerSmallComponent === undefined)
            this.spinnerSmallComponent = new SpinnerSmallComponent();

        let archivoId: number = archivo.Id;
        let fileKey: string = archivo.FileKey

        var param = btoa("fileKey=" + fileKey + "&mail=" + this.empresaSeleccionada.Mail + "&archivoId=" + archivoId.toString());
        var url = "/officetohtml/index.html?param=" + param;
        var link = document.createElement("a");
        document.body.appendChild(link);
        link.href = url;
        link.target = "_blank";
        link.click();
    }

    onOptionsSelected() {
        if (this.selectedEstado != "") {
            this.dataFiltered = this.data.filter(t => t.EstadoAprobacionDescripcion == this.selectedEstado);
        } else {
            this.dataFiltered = this.data;
        }

    }
}
