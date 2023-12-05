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
import { formatDate } from '@angular/common';
import * as XLSX from 'xlsx';
import { FiltroFechaComponent } from '../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { BehaviorSubject } from 'rxjs';
import { DropdownOption } from '../../common/view-child/dropdown/dropdown.component';
import { take } from 'rxjs/operators';
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

    @ViewChild("mensajeModalComponent")
    protected mensajeModalComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("smallSpinner")
    protected spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    constructor(protected altaEmpresaService: AltaEmpresaService, protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, 
        protected modalService: ModalService, protected confirmationService: ConfirmationService) {
        super(navService, securytiService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.filtroFechaComponent = new FiltroFechaComponent();
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
    mensajeSIPERGuardado: string = "";
    idTipoProveedor: number = 0;
    pantallaEditarAlta: boolean = false;
    estadosSelected: string[] = [];
    datosAux: any[];
    tipoCambiario: number = 1;
    rubros: any;
    IdRubro: number;
    facturacionAnual: number;
    nosisObligatorio: boolean;
    RealizarAnalisisNOSIS: boolean;
    listaArchivos: Array<Archivo> = [];
    empleados: Array<RelacionConEmpleados> = [];
    funcionarios: Array<RelacionConFuncionarios> = [];
    relacionConEmpleados: string = "";
    relacionConFuncionarios: string = "";
    cuit: string = "";
    mailVendedor: string = "";
    razonSocial: string = "";
    codigoCliente: string;
    descripcionEstadoAlta: SelectItem[];
    contieneDocumentacionFisica: number = 0;
    cuitIngresado: string = "";
    puedeAltaInterna: boolean = this.isAuthorized('ALTA INTERNA GRANOS');
    puedeAltaInternaNoGranos: boolean = this.isAuthorized('ALTA INTERNA NO GRANOS');
    esAdmin : boolean = this.isAuthorized('ELIMINAR USUARIO DE WEB');
    
    ngOnInit(): void { 
        this.navService.setSeccionList([]);
        $('[data-toggle="tooltip"]').tooltip();
        this.customFiltroFecha();
        this.setEstadosDefault();
        this.getEstados();
    }

    verDetalle() {
        this.navService.navegarSeccion('/proveedor-detalle');
        return false;
    }

    customFiltroFecha(){
        this.filtroFechaComponent.setDropdownOptions([
            new DropdownOption("3", "Último mes"),
            new DropdownOption("6", "Último año"),
            new DropdownOption("4", "Entre Fechas"),
            new DropdownOption("7", "Sin rango de fecha")
        ]);
        this.filtroFechaComponent.setPeriodoInitial("3");
    }

    setEstadosDefault() {
        const estadosDefault: BehaviorSubject<Array<string>> = new BehaviorSubject<Array<string>>([
            "Alta solicitada",
            "Analisis de Nosis",
            "Etapa Final"
        ]);
        this.estadosSelected = estadosDefault.value;
    }

    getEmpresa() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.altaEmpresaService.getEmpresas(this.idTipoProveedor,
                this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin)
                .subscribe(
                    (result: any) => {
                        this.spinnerComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.data = result.data;
                            this.datosAux = this.data;
                            this.filtrarListadoAlta();

                            setTimeout(function () {
                                $('[data-toggle="popover"]').popover({ trigger: 'focus', delay: { "hide": 3000 } });

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

    getTipoCambiario() {
        try {
            this.subscriptionDropDowns = this.service.getTipoCambiario().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.tipoCambiario = result.data;
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    getEstados() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription =
                this.altaEmpresaService.getEstados().subscribe(
                    (result: any) => {
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
                            this.descripcionEstadoAlta = [];
                            this.estados.forEach(x =>
                                this.descripcionEstadoAlta.push({
                                    label: x.Value, value: x.Key
                                }));
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
    filtrarListadoAlta() {
        this.data = this.datosAux;
        if (this.estadosSelected.length < 1 || this.estadosSelected == null) {
        } else {
            this.data = this.datosAux.filter(x => this.estadosSelected.indexOf(x.EstadoAprobacionDescripcion) >= 0);
        }
    }

    guardarSIPER() {
        this.spinnerModal.showIt();
        this.subscription = this.altaEmpresaService.GuardarSIPER(this.empresaSeleccionada.Id, this.empresaSeleccionada.EstadoSIPER).subscribe(
            (result: any) => {
                this.spinnerModal.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                    this.mensajeSIPERGuardado = result.error;
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                    this.mensajeSIPERGuardado = result.info;
                    this.data = result.data;
                }
            },
            error => {
                this.spinnerModal.hideIt();
                this.mensajeSIPERGuardado = "Ocurrio un error al guardar el SIPER."
            }

        );

        if (this.mensajeSIPERGuardado == "")
            this.mensajeSIPERGuardado = "Se guardo correctamente."
    }

    isNullOrWhitespace(input: string) {

        if (typeof input === 'undefined' || input == null)
            return true;

        var userText = input.replace(/^\s+/, '').replace(/\s+$/, '');

        if (userText === '') {
            return true;
        }
        return false;
    }

    resetVariables() {
        this.mensajeSIPERGuardado = "";
        this.pantallaEditarAlta = false;
    }

    cambiarEstado(estadoId: number) {
        if (estadoId == 4 && this.observacionesProveedor == "") {
            this.mensajeError = "Debe ingresar una observacion para el Proveedor.";
            return false;
        }
        if (
            estadoId == 7
            && this.isNullOrWhitespace(this.empresaSeleccionada.EstadoSIPER)
            //Para los no granos solo valido el SIPER si es requerido que mande el siper
            && ((this.empresaSeleccionada.SiperObligatorio && this.empresaSeleccionada.IdTipoUsuario == 3)
                || (this.empresaSeleccionada.SISAEstadoCuit != "1" && this.empresaSeleccionada.IdTipoUsuario != 3))) {
            this.mensajeError = "Debe ingresar Estado en SIPER.";
            return false;
        }


        if (this.empresaSeleccionada.IdTipoUsuario == 5) {
            if (this.razonSocial == "") {
                this.mensajeError = "Debe ingresar la razón social del cliente.";
                return false;
            }

            if (this.codigoCliente == "") {
                this.mensajeError = "Debe ingresar el código SAP del cliente.";
                return false;
            }
        }

        /*
        this.observacionesProveedor = this.observacionesProveedor.replace("<", "esSignoMenor");
        this.observaciones = this.observaciones.replace("<", "esSignoMenor");
        */
        this.spinnerModal.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.altaEmpresaService.setEstadoAprobacion(this.empresaSeleccionada.Id, estadoId, this.observaciones, this.observacionesProveedor, this.empresaSeleccionada.EstadoSIPER, this.razonSocial, this.codigoCliente).subscribe(
                (result: any) => {
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

    agregarObservacion() {
        this.spinnerModal.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.altaEmpresaService.agregarObservacion(this.empresaSeleccionada.Id, this.observaciones).subscribe(
                (result: any) => {
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

    solicitarInformacion() {
        try {
            this.altaEmpresaService.solicitarInformacion(this.empresaSeleccionada.Id).subscribe(
                (result: any) => {
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
                (result: any) => {
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

    abrirEditar(proveedorId: number) {
        this.pantallaEditarAlta = true;
        this.getTipoCambiario()
        this.getRubrosOptions();
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

    AbrilModalYaVenianOperando(empresa: any) {
        this.empresaSeleccionada = empresa;
        document.getElementById("openModalVieneOperando").click();
    }

    proveedorNoGranosOperando() {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();

        if (this.validarNoGranosOperando()) return
        this.subscription = this.altaEmpresaService
            .proveedorNoGranosOperando(this.empresaSeleccionada.Id, this.empresaSeleccionada.RazonSocial).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                    else {
                        document.getElementById("hidemyModalOperando").click();
                        this.mensajeComponent.setSuccessMsg("Proveedor habilitado");
                        this.empresaSeleccionada.EstadoAprobacionDescripcion = 'Alta aceptada';
                        this.empresaSeleccionada.EstadoAprobacion = 0;

                        //this.getEmpresa();
                    }
                },
                error => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    validarNoGranosOperando() {
        this.mensajeComponent.setMsgsEmpty();
        if (this.empresaSeleccionada.RazonSocial == '' || !this.empresaSeleccionada.RazonSocial) {
            this.mensajeComponent.setErrorMsg("Falta Completar la razón social.");
            return true
        }

        return false
    }

    obtenerArchivosSubidos(mail: string, proveedorId: number) {
        this.subscription = this.service.obtenerArchivosSubidos(mail, proveedorId).subscribe(
            (result: any) => {
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

    descargarArchivos(mail: string, proveedorId: number) {
        this.service.descargarArchivosSubidos(mail, proveedorId)
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
                            return false;
                        }
                    }
                },
                (error) => {
                    this.spinnerSmallComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            )
    }

    trackListadoAlta(index: number, empresa: any) {
        return empresa
    }

    cargarSolicitudUsuario(mail: string, proveedorId: number) {
        this.subscription = this.service.cargarSolicitudUsuario(mail, proveedorId).subscribe(
            (result: any) => {
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

    copiar(str, id) {
        console.log(str, id);
        const el = document.createElement('textarea');
        el.value = str;
        el.setAttribute('readonly', '');
        el.style.position = 'absolute';
        el.style.left = '-9999px';
        document.body.appendChild(el);
        el.select();
        document.execCommand('copy');
        document.body.removeChild(el);

        $("#h" + id).popover('show');
        setTimeout(function () { $("#h" + id).popover('hide') }, 1500);

        return false;
    }

    altaInterna(empresa: Empresa) {
        this.goToSeccionParam('/alta-empresa-no-granos', empresa.Id.toString());
    }

    completarAlta(empresa: Empresa) {
        this.goToSeccionParamTres('/usuario/alta-empresa-no-granos', empresa.Id.toString(), empresa.CUIT, empresa.Mail);
    }

    getRubrosOptions() {
        try {
            this.subscriptionDropDowns = this.service.getRubros().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.rubros = result.data;

                        if (this.empresaSeleccionada.Rubro != "") {
                            this.rubros.forEach(rubro => {
                                if (rubro.Nombre == this.empresaSeleccionada.Rubro) {
                                    this.IdRubro = rubro.Id;
                                }
                            });
                        }
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    volver() {
        this.pantallaEditarAlta = false;
    }

    editarAltaNoGranos() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }

        try {
            this.service.editarProveedorNoGranos(this.empresaSeleccionada.RazonSocial, this.empresaSeleccionada.CUIT, this.empresaSeleccionada.Mail,
                this.empresaSeleccionada.Telefono,
                this.empresaSeleccionada.RealizarAnalisisNOSIS ? this.empresaSeleccionada.RealizarAnalisisNOSIS : false,
                this.IdRubro, this.empresaSeleccionada.CondicionDePago,
                this.empresaSeleccionada.ServicioPrestado, this.empresaSeleccionada.OrganizacionDeCompra, this.empresaSeleccionada.RazonDeEleccion,
                this.empresaSeleccionada.FacturacionAnual, this.empresaSeleccionada.Id,
                this.empresaSeleccionada.RequiereVerificacionCompras ? this.empresaSeleccionada.RequiereVerificacionCompras : false,
                this.empresaSeleccionada.IngresoAPlanta ? this.empresaSeleccionada.IngresoAPlanta : false,
                this.empresaSeleccionada.AltaInterna ? this.empresaSeleccionada.AltaInterna : false,
                this.empresaSeleccionada.SiperObligatorio ? this.empresaSeleccionada.SiperObligatorio : false
            ).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        setTimeout(() => {
                            this.pantallaEditarAlta = false;
                        }, 1000);
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

    calcularFacturacion() {

        if (this.tipoCambiario <= 0)
            this.tipoCambiario = 1;
        let facturacionDolares = this.empresaSeleccionada.FacturacionAnual / this.tipoCambiario


        if (facturacionDolares > 15000) {
            this.nosisObligatorio = true;
            this.empresaSeleccionada.RealizarAnalisisNOSIS = true;
        }
        else {
            this.nosisObligatorio = false;
        }
    }

    verificarIngresoAPlanta() {
        if (this.empresaSeleccionada.IngresoAPlanta)
            this.empresaSeleccionada.RequiereVerificacionCompras = true;
    }

    descargarFormularioNG(empresaId: number) {
        this.service.descargarFormularioNG(empresaId)
            .subscribe(
                (result) => {
                    this.spinnerModal.hideIt();
                    if (result.error) {
                        this.mensajeError = result.error;
                    } else {
                        var byteArray = new Uint8Array(result.data);
                        var blob = new Blob([byteArray], {
                            type: "application/pdf",
                        });
                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                "Formulario de Solicitud de alta" + ".pdf"
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = "Formulario de Solicitud de alta" + ".pdf";
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);

                            return false;
                        }
                    }
                },
                (error) => {
                    this.spinnerModal.hideIt();
                    this.mensajeError = error.message;
                }
            );
    }

    grabarAltaInternaGranos() {
        this.mensajeModalComponent.setMsgsEmpty();
        this.subscription = this.altaEmpresaService.grabarAltaInternaGranos(this.cuit, this.mailVendedor).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeModalComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeModalComponent.setInfoMsg(result.info);
                }
                else {
                    this.mensajeModalComponent.setSuccessMsg(result.info);
                    this.getEmpresa();
                    document.getElementById("hidemyModalAltaInterna").click();
                }
            },
            error => {
            }
        );
    }

    cambiarFiltroTipoProveedor(tipoProveedor: number) {
        this.idTipoProveedor = tipoProveedor;
        this.getEstados();
    }

    //exportacion a Excel
    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        let informacionExportar: any;

        informacionExportar = this.data.map(info => {
            return {
                "Código": info.CodigoProveedor || "-",
                "CUIT": info.CUIT || "-",
                "Razón Social": info.RazonSocial || "-",
                "Mail": info.Mail,
                "Corredor": info.RazonSocialCorredor || "-",
                "Comercial / Solicitante Interno": info.Comercial || "-",
                "SISA": info.SISAEstadoCuit || "-",
                "Estado Siper": info.EstadoSIPER,
                "Ultima Edición": info.UltimaEdicion != undefined ? formatDate(info.UltimaEdicion.slice(6, -2), "dd/MM/yyyy", "en-EN") : "",
                "Fecha Solicitud": info.FechaSolicitud != undefined ? formatDate(info.FechaSolicitud.slice(6, -2), "dd/MM/yyyy", "en-EN") : "",
                "Fecha alta aceptada": info.FechaAltaAceptada != undefined ? formatDate(info.FechaAltaAceptada.slice(6, -2), "dd/MM/yyyy", "en-EN") : "",
                "Estado": info.EstadoAprobacionDescripcion,
                "Presento documentación física": (info.ContieneDocumentacionFisica || false) ? "Si" : "No"
            }
        });

        if (informacionExportar.length == 0) {
            this.mensajeComponent.setInfoMsg("No existen datos para exportar.");
            return
        }

        this.DownloadJsonData(informacionExportar, "AltaProveedores");
    }

    DownloadJsonData(JSONData: any, FileTitle: string) {

        //crea la estructura inicial del archivo
        let worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(JSONData);
        let workbook: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'Alta de proveedores');

        //escribe el file para ser descargado
        const excelBuffer: any = XLSX.writeFile(workbook, FileTitle + '.xlsx');
    }

    onChangeProveedor() {
        this.contieneDocumentacionFisica = 0;
    }

    onChangeDocumentacionFisica() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }

        try {
            this.service.registrarDocumentacionFisica(this.empresaSeleccionada.Id,
                this.empresaSeleccionada.ContieneDocumentacionFisica
            ).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.getEmpresa();
                    } else {
                        setTimeout(() => {
                            this.pantallaEditarAlta = false;
                        }, 1000);
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);

        }
    }

    confirmarBorrarAlta(id: number, mail: string) {
        this.confirmationService.confirm({
            key: 'confirmarBorrado',
            message: "Se va a proceder a realizar la baja del usuario: " +
            mail + ", una vez realizada la operación, él mismo deberá volver a registrarse" +
            " para operar en el sistema.",
            accept: () => {
                this.eliminarAltaUsuario(id);
            },
            reject: () => {
            }
        });
    }

    eliminarAltaUsuario(proveedorId: number){
        this.spinnerModal.showIt();
        this.altaEmpresaService.eliminarCuitNoHabilitado(proveedorId).pipe(take(1))
        .subscribe(
            (result) => {
                this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }else{
                        this.getEmpresa();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    } 
                }
            ,
            (error) => {
                this.spinnerModal.hideIt();
                this.mensajeError = error.message;
            }
        );
    }

    revisarCUITFormatoValido(cuit: string): boolean {
        return cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number)
    }

    verificarExistenciaEmpresa(){
        this.mensajeComponent.setMsgsEmpty();
        if(!this.revisarCUITFormatoValido(this.cuitIngresado)){
            this.mensajeComponent.setErrorMsg("Debe ingresar un cuit valido.");
            return;
        }
        let msjModal = "";
        this.spinnerModal.showIt();
        this.altaEmpresaService.verificarExistenciaEmpresa(this.cuitIngresado)
        .subscribe(
            (result) => {
                this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }else{
                        if(result.data.error != undefined){
                            msjModal = result.data.error;
                        }else
                            msjModal = result.data.Mensaje;
                        this.abrirModalVerificarExisteCuit(msjModal);
                    } 
                    console.log(result);
                }
            ,
            (error) => {
                this.spinnerModal.hideIt();
                this.mensajeError = error.message;
            }
        );
    }

    abrirModalVerificarExisteCuit(mensaje: string) {
        this.confirmationService.confirm({
            key: 'verificarExistenciaCuit',
            message: mensaje,
            accept: () => {
            },
        });
    }

}
