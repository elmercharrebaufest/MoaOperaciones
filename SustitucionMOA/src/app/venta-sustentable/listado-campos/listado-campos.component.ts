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
import { Permiso } from '../../common/enums/Permisos';

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
    editarCampos: boolean = this.isAuthorized('EDICION CAMPOS CREADOS') || this.isAuthorized('COMERCIAL CAMPOS SUSTENTABLES')
    borrarCampos: boolean = this.isAuthorized('BORRAR CAMPOS CREADOS')
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esComercial: boolean = this.isAuthorized(Permiso.ComercialCamposSustentables);
    selectedProveedor: any = null; // Cambia el nombre para el dropdown
    selectedCuit: any = null; // Cambia el nombre para el dropdown
    opcionesProveedores: any[] = []; // Ya lo tienes
    opcionesCuit: any[] = []; // Nueva variable para los CUIT
    cosechas: any;
    normativas: any;

    filtroId: string = "";
    filtroNombreCampo: string = "";
    filtroProveedor: string = "";
    filtroCuit: string = "";
    filtroCosechaId: string = "";
    orderedByColumn: string = "Nombre";
    orderDirection: number = 1;
    itemsPerPage = 20;
    checkRevision: boolean = false;
    mostrarDialogoRechazo: boolean = false;
    mostrarDialogoAprobacion: boolean = false;
    motivoRechazo: string = '';
    campoARechazar: any = null;
    campoSeleccionado: any = null;
    mostrarDialogoAdjuntarEvidencia: boolean = false;
    archivoEvidenciaSeleccionado: File | null = null;

    tituloArchivo: string = "Reporte de Campos Sustentables.xls";

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.getCosechas();
    }

    getCamposSustentables() {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getCamposProveedores().subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result;
                    console.log(this.data);
                    this.getNormativas();

                    if (result.length > 0) {
                        let allProveedores = result
                            .map(cp => ({
                                value: (cp && cp.Proveedor && cp.Proveedor.CodigoProveedor) ? cp.Proveedor.CodigoProveedor : '',
                                label: (cp && cp.Proveedor && cp.Proveedor.RazonSocial) ? cp.Proveedor.RazonSocial : ''
                            }))
                            .filter(p => p.label != null && String(p.label).trim() !== '');

                        this.opcionesProveedores = [...new Map(allProveedores.map(item => [item.value, item])).values()];

                        let allCuitProveedores = result
                            .map(cp => ({
                                value: cp && cp.CUITProveedor ? cp.CUITProveedor : '',
                                label: cp && cp.CUITProveedor ? cp.CUITProveedor : ''
                            }))
                            .filter(p => p.label != null && String(p.label).trim() !== '');

                        this.opcionesCuit = [...new Map(allCuitProveedores.map(item => [item.value, item])).values()];
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

    proveedorSeleccionado(event: any) {
        // event.value contiene el value seleccionado
        this.filtroProveedor = event.value || "";
    }

    cuitSeleccionado(event: any) {
        // event.value contiene el value seleccionado
        this.filtroCuit = event.value || "";
    }

    eliminarCampo(campoCosechaId: number, proveedorId: number) {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.campoProveedorBorrar(campoCosechaId, proveedorId).subscribe(
            (result: any) => {
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

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();

        this.subscription = this.service.exportExcel().subscribe(
            (result: any) => {
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


    getCosechas() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.subscription = this.service.getCosechasFiltro().subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.cosechas = result;
                        this.getCamposSustentables();
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

     getNormativas() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.subscription = this.service.getNormativas().subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.normativas = result;
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

    getCamposFiltrados(): any[] {
    if (this.checkRevision) {
        return this.data.filter(campo =>
            (campo.Validado === false && !campo.MotivoRechazo) &&
            (campo.TipoNormativa === "EPA" || campo.TipoNormativa === "2BSVS")
        );
    }
        return this.data;
    }

    descargarEPA(campoCosechaId: number, proveedorId: number) {
        this.service.descargarArchivoEPA(campoCosechaId, proveedorId).subscribe(
            (result) => {
                if (result.logout) {
                    this.sessionDataService.logout();
                }
                else {
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], { type: "application/octet-stream" });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, result.FileDownloadName);
                    }
                    else {
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

    abrirDialogoRechazo(campo: any) {
        this.campoARechazar = campo;
        this.motivoRechazo = '';
        this.mostrarDialogoRechazo = true;
    }

    confirmarRechazo() {
         this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.campoProveedorRechazar(this.campoARechazar.CampoCosechaId, this.campoARechazar.Proveedor.Id, this.campoARechazar.TipoNormativaId, this.motivoRechazo).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.getCamposSustentables();
                    this.mostrarDialogoRechazo = false;
                    this.mensajeComponent.setSuccessMsg("Se rechazo el campo " + this.campoARechazar.NombreCampo + " correctamente.");
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    abrirDialogoAprobacion(campo: any) {
        this.campoSeleccionado = campo;
        this.mostrarDialogoAprobacion = true;
    }

    confirmarAprobacion() {
         this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.campoProveedorAprobar(this.campoSeleccionado.CampoCosechaId, this.campoSeleccionado.Proveedor.Id, this.campoSeleccionado.TipoNormativaId).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.getCamposSustentables();
                    this.mostrarDialogoAprobacion = false;
                    this.mensajeComponent.setSuccessMsg("Se aprobó el campo " + this.campoSeleccionado.NombreCampo + " correctamente.");
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    onArchivoEvidenciaChange(event: any) {
    const files = event.target.files;
    this.archivoEvidenciaSeleccionado = files && files.length > 0 ? files[0] : null;
    }

    abrirDialogoEvidenciaPresentada(campo: any) {
        this.mostrarDialogoAdjuntarEvidencia = true;
        this.campoSeleccionado = campo;
    }

    confirmarSubidaEPA() {
         this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.campoProveedorAjuntarEPAValidado(this.campoSeleccionado.CampoCosechaId, this.campoSeleccionado.Proveedor.Id, this.archivoEvidenciaSeleccionado).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.getCamposSustentables();
                    this.mostrarDialogoAdjuntarEvidencia = false;
                    this.archivoEvidenciaSeleccionado = null;
                    this.mensajeComponent.setSuccessMsg("Se adjunto la evidencia correctamente.");
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    cancelarSubidaEPA() {
        this.mostrarDialogoAdjuntarEvidencia = false;
        this.archivoEvidenciaSeleccionado = null;
    }

}
