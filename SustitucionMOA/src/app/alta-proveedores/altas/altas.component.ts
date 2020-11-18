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
import { Archivo } from '../../common/models/archivo';
import { RelacionConEmpleados } from '../../common/models//RelacionConEmpleados';
import { RelacionConFuncionarios } from '../../common/models/relacionConFuncionarios';
declare var $: any;


@Component({
    selector: 'app-altas',
    templateUrl: 'altas.component.html',
    styleUrls: ['altas.component.css', '../../../../Content/css/bootstrap.min.css'],
    providers: [AltaEmpresaService]
})

export class AltasComponent extends BaseComponent implements OnInit {
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("smallSpinner")
    protected spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;


    constructor(protected altaEmpresaService: AltaEmpresaService, protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    estados: any;
    selectedEstado: string = "";
    orderedByColumn: string = "id";
    empresaSeleccionada: Empresa = new Empresa();
    empresaEstadoSeleccionada: number = 0;
    orderDirection: number = 1;
    itemsPerPage = 20;
    observaciones: string = "";
    observacionesProveedor: string = "";
    mensajeError: string = "";
    filtroAlta: string = "";


    listaArchivos: Array<Archivo> = [];

    empleados: Array<RelacionConEmpleados> = [];
    funcionarios: Array<RelacionConFuncionarios> = [];
    relacionConEmpleados: string = "";
    relacionConFuncionarios: string = "";
    ngOnInit(): void {
        this.getEstados();
        this.navService.setSeccionList([]);
        $('[data-toggle="tooltip"]').tooltip();
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

                        setTimeout(function () {
                            $('[data-toggle="popover"]').popover({ trigger: 'focus' });
                            
                        }, 100);
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
                        let estadosIntermedios = result.intermedios;
                        let estadosFinales = result.finales;
                        let estadosAgrupados = [{ Key: estadosIntermedios.map(x => x.Key).join("|"), Value: 'Altas en gestión' }, { Key: estadosFinales.map(x => x.Key).join("|"), Value: 'Altas finalizadas' }]
                        this.estados = estadosIntermedios.concat(estadosFinales).concat(estadosAgrupados);
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
        if (estadoId == 7 && this.empresaSeleccionada.SISAEstadoCuit != "1" && (this.empresaSeleccionada.EstadoSIPER == "" || this.empresaSeleccionada.EstadoSIPER == null)) {
            this.mensajeError = "Debe ingresar Estado en SIPER.";
            return false;
        }
        this.spinnerModal.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.altaEmpresaService.setEstadoAprobacion(this.empresaSeleccionada.Id, estadoId, this.observaciones, this.observacionesProveedor, this.empresaSeleccionada.EstadoSIPER).subscribe(
                result => {
                    this.getEmpresa();
                    this.spinnerModal.hideIt();
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
            this.spinnerModal.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }


    VerificarEstadoDataAgro(empresa: Empresa) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.altaEmpresaService.VerificarEstadoDataAgro(empresa.Id).subscribe(
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



    handleFileInput(files: FileList, fileKey: string) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerModal.showIt();

        this.unsubscribe();
        this.subscription = this.service
            .postFile(files, fileKey, this.empresaSeleccionada.Id)
            .subscribe(
                (result) => {
                    this.spinnerModal.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.obtenerArchivosSubidos(this.empresaSeleccionada.Mail, this.empresaSeleccionada.Id);
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    abrirModal(empresa: Empresa) {
        this.empresaSeleccionada = empresa;
        this.observaciones = "";
        this.observacionesProveedor = "";
        this.mensajeError = "";
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.cargarSolicitudUsuario(empresa.Mail, empresa.Id);
        this.obtenerArchivosSubidos(empresa.Mail, empresa.Id);
        document.getElementById("openModalHiddenButton").click();
        return false;
    }

    obtenerArchivosSubidos(mail: string, proveedorId: number) {
        this.subscription = this.service.obtenerArchivosSubidos(mail, proveedorId).subscribe(
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
        let proveedorId: number = this.empresaSeleccionada.Id;

        var param = btoa("fileKey=" + fileKey + "&mail=" + this.empresaSeleccionada.Mail + "&archivoId=" + archivoId.toString() + "&proveedorId=" + proveedorId.toString());
        var url = "/officetohtml/index.html?param=" + param;
        var link = document.createElement("a");
        document.body.appendChild(link);
        link.href = url;
        link.target = "_blank";
        link.click();
    }

    onOptionsSelected() {
        // Esto ahora lo filtramos con un pipe
        // if (this.selectedEstado != "") {
        //     this.dataFiltered = this.data.filter(t => t.EstadoAprobacionDescripcion == this.selectedEstado);
        // } else {
        //     this.dataFiltered = this.data;
        // }

    }

    cargarSolicitudUsuario(mail: string, proveedorId: number) {
        this.subscription = this.service.cargarSolicitudUsuario(mail, proveedorId).subscribe(
            result => {
                if (result.VinculoConEmpleadosDeMolinos != null) {
                    if (result.VinculoConEmpleadosDeMolinos) {
                        this.relacionConEmpleados = "Si";
                    } else {
                        this.relacionConEmpleados = "No";
                    }

                    if (result.VinculoConFuncionariosPublicos) {
                        this.relacionConFuncionarios = "Si";
                    } else {
                        this.relacionConFuncionarios = "No";
                    }

                    this.empleados = result.Empleados;
                    this.funcionarios = result.Funcionarios;
                }

            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }
    isVisibleTablaFuncionarios(): boolean {
        return this.relacionConFuncionarios == "Si";
    }
    isVisibleTablaEmpleados(): boolean {
        return this.relacionConEmpleados == "Si";
    }

    eliminarArchivo(archivo: Archivo) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerModal.showIt();
        this.unsubscribe();

        this.subscription = this.service
            .eliminarArchivoSubido(archivo.Id, this.empresaSeleccionada.Id)
            .subscribe(
                (result) => {
                    this.spinnerModal.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.obtenerArchivosSubidos(this.empresaSeleccionada.Mail, this.empresaSeleccionada.Id);
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }
    copiar(str) {
        const el = document.createElement('input');
        el.setAttribute("type", "hidden");
        el.value = str;
        document.body.appendChild(el);
        el.select();
        document.execCommand('copy');
        document.body.removeChild(el);
    }
}
