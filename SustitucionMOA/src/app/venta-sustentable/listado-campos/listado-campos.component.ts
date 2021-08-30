import { Component, OnInit, ViewChild } from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { DropdownOption } from '../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';

@Component({
    selector: 'app-listado-campos',
    templateUrl: './listado-campos.component.html',
    providers: [VentaSustentableService],
    styleUrls: ['./listado-campos.component.css']
})
export class ListadoCamposComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    data: any;
    esInterno: boolean = this.isAuthorized('VER TODOS CAMPOS SUSTENTABLE');
    editarCampos: boolean = this.isAuthorized('EDICION CAMPOS CREADOS')
    borrarCampos: boolean = this.isAuthorized('EDICION CAMPOS CREADOS')
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    opcionesProveedores: any;

    filtroId: string = "";
    filtroNombreCampo: string = "";
    filtroProveedor: string = "";

    orderedByColumn: string = "Nombre";
    orderDirection: number = 1;
    itemsPerPage = 20;

    tituloArchivo: string = "Reporte de Campos Sustentables.xls";

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.getCamposSustentables();
    }

    getCamposSustentables() {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getCamposProveedores().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result;

                    if (result.length > 0) {
                        let allProveedores = result.map(cp => { return { value: cp.Proveedor.CodigoProveedor, label: cp.Proveedor.RazonSocial } });
                        this.opcionesProveedores = [...new Map(allProveedores.map(item => [item.value, item])).values()]
                        this.opcionesProveedores.unshift({ value: "", label: "Todos" })
                    }
                    else {
                        this.mensajeComponent.setInfoMsg("No hay campos sustentables cargados.");
                    }
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );

        return false;
    }

    eliminarCampo(campoCosechaId: number, proveedorId: number) {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.campoProveedorBorrar(campoCosechaId, proveedorId).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.getCamposSustentables();
                    this.mensajeComponent.setSuccessMsg("Se elimino el campo " + campoCosechaId + " correctamente.");
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    proveedorSeleccionado(event: string) {
        this.filtroProveedor = event;
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();

        this.subscription = this.service.exportExcel().subscribe(
            result => {
                if (result.logout == true) {
                    this.spinnerSmallComponent.hideIt();
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/octet-stream' });

                    if (window.navigator.msSaveOrOpenBlob) {
                        //IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivo);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivo;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        //return false;
                    }

                    this.spinnerSmallComponent.hideIt();
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
                this.spinnerSmallComponent.hideIt();
            }
        );

        return false;
    }

    descargarKMZ(campoCosechaId: number, proveedorId: number) {
        this.service.descargarArchivoKMZ(campoCosechaId, proveedorId).subscribe(
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
                        return false;
                    }
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        )
    }
}
