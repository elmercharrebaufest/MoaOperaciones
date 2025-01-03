import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { CampoProveedor, CampoSustentable, CampoCosecha, CampoProveedorDetalle, SugerenciaCampo } from './../sustentable'
import { DeclaracionConformidadComponent } from '../declaracion-conformidad/declaracion-conformidad.component';
import { isUndefined } from 'util';
import { VendedorProveedor } from '../../common/models/vendedorProveedor';
import { finalize } from 'rxjs/operators';
import { Message, MessageService } from 'primeng/api';
import { RenspaExiste } from '../renspa-existe.interface';
import { ApiResponse } from '../../common/models/response';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';

export interface DatosCopiar {
    NombreCampo: string;
    NombreCosecha: string;
    HectareasTotales: number;
    HectareasSoja: number;
    ToneladasAprobadas: number;
    Latitud: string;
    Longitud: string;
    CampoCosechaId: number;
    ProveedorNombre: string;
    LocalidadNombre: string;
    Localidad_Id: number;
    CampoSustentableId: number;
    CosechaId: number;
    CUIT: string;
    Archivo_Id: number;
    Proveedor_Id: number;
    CodigoProveedor: string;
    Renspa: string;
    error?: string;
    info?: string;
    logout?: boolean;
}

@Component({
    selector: 'app-alta',
    templateUrl: './alta.component.html',
    providers: [VentaSustentableService, MessageService],
    styleUrls: ['./alta.component.css']
})
export class AltaComponent extends BaseComponent implements OnInit {

    @ViewChild('mensajeGeneral')
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(DeclaracionConformidadComponent)
    protected declaracionComformidad: DeclaracionConformidadComponent;

    @ViewChild(SpinnerSmallComponent)
    protected spinnerExportarSugerencias: SpinnerSmallComponent;

    @ViewChild('fileInputSugerencia')
    fileInputSugerencia: ElementRef;

    @ViewChild('mensajeEdicionSugerencia')
    mensajeEdicionSugerencia: MensajeComponent;

    @BlockUI() blockUI: NgBlockUI;

    constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private messageService: MessageService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    nombreEstablecimiento: string;
    renspa: string;
    renspaExiste: RenspaExiste;
    pais: string;
    dataLocalidades = [];
    myLocalidades = <any>[];
    localidades: any = [];

    cosechas: any[];
    cosechaId: any;

    localidadId: any;
    provinciaId: any;
    nombreCosecha: string;
    sojaParcial: boolean;
    sojaTotal: boolean;

    proveedorId: any;
    proveedorIdCorredor: any;
    hectareasTotales: number;
    hectareasSoja: number;
    latitud: string;
    longitud: string;
    CUIT: string = "";
    CUITInicial: string = "";
    file: any;
    Archivo_Id: number;
    UsarArchivo_Id: boolean = false;
    Proveedor_Id: number;
    operarComo: number = 1;

    ingresarProveedorPorCUIT: boolean;
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    codigoProveedor: string = sessionStorage.getItem("proveedor") || "";
    campoCosechaId: any;
    CodigoProveedorEdit: string = "";
    campoProveedorE: any;
    localidadPreseleccionada: { IdLocalidad: number, NombreLocalidad: string };

    proveedores: VendedorProveedor[] = [];
    proveedorSeleccionado: VendedorProveedor;
    sePreseleccionoProveedor = false;
    proveedorTexto: string = "";
    corredorDebeCargarCuit = false;

    mostrarSugerenciasCamposNuevos: boolean = false;
    camposNuevosSugeridos: SugerenciaCampo[] = [];
    camposSugeridosSeleccionaTodos: boolean = false;

    mostrarEdicionSugerencia: boolean = false;
    campoSugeridoEnEdicion: SugerenciaCampo = {} as SugerenciaCampo;
    localidadCampoSugeridoEnEdicion: { IdLocalidad: number, NombreLocalidad: string };
    archivosNuevosSugerencias: File[] = [];

    ngOnInit() {
        this.renspaExiste = { RenspaExiste: false, MismoCuit: false };

        this.navService.setSeccionList([]);
        this.CUIT = "";

        this.proveedorId = this.getProveedorId(this.codigoProveedor);
        this.CUITInicial = this.CUIT;

        this.declaracionComformidad.esCorredor = this.esCorredor;
        this.ingresarProveedorPorCUIT = true;

        if (this.esCorredor) {
            this.ingresarProveedorPorCUIT = false;
        }

        this.operarComo = 1;
        this.declaracionComformidad.operarComo = 1;
        this.getParams();
        this.getCosechas();
    }

    getParams() {
        try {
            const queryString = window.location.href;
            this.campoCosechaId = queryString.split('=')[1].split(';')[0];
            this.proveedorId = queryString.split('=')[2]
        } catch (e) {

        }
    }

    cargarDatosCopiar() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getCampoProveedor(this.proveedorId, this.campoCosechaId).subscribe(
                (result: DatosCopiar) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.campoProveedorE = result;
                        this.nombreEstablecimiento = result.NombreCampo;
                        this.localidadId = result.Localidad_Id;
                        this.localidadPreseleccionada = { IdLocalidad: result.Localidad_Id, NombreLocalidad: result.LocalidadNombre };
                        this.renspa = result.Renspa;
                        this.hectareasTotales = result.HectareasTotales;
                        this.hectareasSoja = result.HectareasSoja;

                        this.latitud = result.Latitud;
                        this.longitud = result.Longitud;
                        this.CUIT = result.CUIT;
                        this.Archivo_Id = result.Archivo_Id;
                        this.UsarArchivo_Id = true;
                        this.Proveedor_Id = result.Proveedor_Id;
                        this.declaracionComformidad.cargarDatosCopiar(result)

                        if (this.esCorredor) {
                            if (this.Proveedor_Id != this.proveedorId) {
                                this.operarComo = 1;
                                this.CodigoProveedorEdit = result.CodigoProveedor;
                            } else {
                                this.operarComo = 2;
                                this.cambiarModoOperacion();
                                this.CUIT = result.CUIT;
                            }
                            this.proveedorId = result.Proveedor_Id;
                        }
                        this.mensajeComponent.setInfoMsg("Si el contorno del lote presentado este año es diferente al del año anterior adjuntar nuevo KMZ");
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

    setTabs() {
        this.setMenuSeccionTab("Alta", "Dar de Alta");
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    }

    onselectProveedor(proveedor?: VendedorProveedor) {
        if (proveedor) {
            if (this.codigoProveedor == proveedor.idVendedor) {
                this.mostrarWarningCuitsIguales();
            }
            this.proveedorSeleccionado = proveedor;
            this.getProveedorId(this.proveedorSeleccionado.idVendedor);
        }
    }

    getProveedorId(codigo: string) {
        this.blockUI.start("Seleccionando proveedor")
        this.subscription = this.service.getProveedor(codigo)
            .pipe(finalize(() => this.blockUI.stop()))
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

                        this.proveedorId = result.Id;

                        const cuit = result.CUIT ? result.CUIT : this.proveedorSeleccionado.cuit;
                        const razonSocial = result.RazonSocial ? result.RazonSocial : this.proveedorSeleccionado.descVendedor;

                        //if (!this.esCorredor) {
                        this.CUIT = cuit;
                        this.declaracionComformidad.CUITDeclaracion = cuit;
                        this.declaracionComformidad.razonSocialDeclaracion = razonSocial;

                        if (this.CUITInicial.length == 0) {
                            this.CUITInicial = this.CUIT;
                        }
                        //}

                        this.validarModalDeclaracion();

                        return result.Id;
                    }
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    onChangeCosecha() {
        if (this.cosechaId > 0) {
            if (!this.esCorredor || (this.esCorredor && (this.operarComo == 2 || !this.sePreseleccionoProveedor))) {
                this.mensajeComponent.setMsgsEmpty();
                if (this.CUIT == "" || this.CUIT.length != 11) {
                    this.mensajeComponent.setErrorMsg("El CUIT ingresado no es válido");
                    setTimeout(() => {
                        this.cosechaId = 0;
                    }, 100);
                    return false
                }
            }

            this.validarModalDeclaracion();

            this.renspaChanged();
        }
    }

    private validarModalDeclaracion() {
        if (this.cosechaId > 0) {
            if (this.esCorredor) {

                if (this.operarComo == 2) {
                    this.declaracionComformidad.CUIT = this.CUIT;
                    this.declaracionComformidad.razonSocialDeclaracion = "";
                }

                this.declaracionComformidad.proveedorId = this.proveedorId;
                this.declaracionComformidad.cosechaId = this.cosechaId;
                this.declaracionComformidad.nombreCosecha = this.cosechas.find(c => c.Id == this.cosechaId).Nombre;
                this.revisarDeclaracionJurada()
            }
            else {
                if (this.proveedorId > 0) {

                    //Si cambió el CUIT le saco la razón social
                    if (this.CUITInicial != this.CUIT) {
                        this.declaracionComformidad.razonSocialDeclaracion = "";
                    }
                    this.declaracionComformidad.proveedorId = this.proveedorId;
                    this.declaracionComformidad.cosechaId = this.cosechaId;
                    this.declaracionComformidad.nombreCosecha = this.cosechas.find(c => c.Id == this.cosechaId).Nombre;
                    this.revisarDeclaracionJurada()
                }
            }
        }
    }

    campoProveedorAgregar() {
        this.blockUI.start('Informando campo sustentable.');

        let campoProveedor: CampoProveedor;
        let campoSustentable: CampoSustentable;
        let campoCosecha: CampoCosecha;

        if (this.validar()) {
            this.blockUI.stop();
            return;
        }

        campoSustentable = {
            Nombre: this.nombreEstablecimiento,
            Localidad_Id: this.localidadId,
            Renspa: this.renspa
        }

        campoCosecha = {
            Campo: campoSustentable, Cosecha_Id: this.cosechaId
        }

        campoProveedor = {
            HectareasTotales: this.hectareasTotales, HectareasSoja: this.hectareasSoja,
            CUIT: this.CUIT,
            Latitud: this.latitud,
            Longitud: this.longitud,
            Proveedor_Id: this.proveedorId,
            CampoCosecha: campoCosecha,
            Archivo_Id: this.UsarArchivo_Id ? this.Archivo_Id : 0
        }

        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.campoProveedorAgregar(campoProveedor, this.file, this.UsarArchivo_Id).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {

                        this.mensajeComponent.setSuccessMsg(result.Mensaje);
                        setTimeout(() => {
                            this.redirigirAListado();
                        }, 3000);
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    verificarCUITIngresado() {
        this.declaracionComformidad.CUITDeclaracion = this.CUIT;

        this.revisarDeclaracionJurada()
    }

    getCosechas() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.subscription = this.service.getCosechasCampo().subscribe(
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
                        this.cosechas = [{ Id: 0, Nombre: "Seleccione" }, ...this.cosechas];

                        this.cosechaId = 0;
                        this.onChangeCosecha();

                        if (this.campoCosechaId != null && this.campoCosechaId > 0) {
                            this.cargarDatosCopiar();
                        } else {
                            this.campoProveedorE = {};
                        }
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

    validar() {
        this.mensajeComponent.setMsgsEmpty();

        if (this.nombreEstablecimiento == "" || !this.nombreEstablecimiento) {
            this.mensajeComponent.setErrorMsg("Falta completar nombre del establecimiento.");
            return true;
        }

        if (!this.localidadId || this.localidadId <= 0) {
            this.mensajeComponent.setErrorMsg("Falta seleccionar la localidad.");
            return true;
        }

        if (this.renspa == "" || !this.renspa || this.renspa.length < 13) {
            this.mensajeComponent.setErrorMsg("Falta completar RENSPA.");
            return true;
        }

        if (!this.esCorredor || (this.esCorredor && this.operarComo == 2)) {
            if (!this.CUIT) {
                this.mensajeComponent.setErrorMsg("El CUIT ingresado no es válido");
                return true
            }

            if (this.CUIT.length != 11) {
                this.mensajeComponent.setErrorMsg("El CUIT ingresado no es válido");
                return true
            }
        }
        if (this.esCorredor && this.operarComo == 1 && (!this.CUIT || !this.proveedorTexto)) {
            this.mensajeComponent.setErrorMsg("Falta seleccionar un proveedor.");
            return true
        }

        if (!this.cosechaId || this.cosechaId <= 0) {
            this.mensajeComponent.setErrorMsg("Falta seleccionar la cosecha.");
            return true;
        }

        if (!this.hectareasTotales) {
            this.mensajeComponent.setErrorMsg("Falta completar hectáreas totales.");
            return true;
        }
        if (this.hectareasTotales <= 0) {
            this.mensajeComponent.setErrorMsg("Hectáreas totales no puede ser 0 o un número negativo.");
            return true;
        }
        if (!this.hectareasSoja) {
            this.mensajeComponent.setErrorMsg("Falta completar hectáreas de soja.");
            return true;
        }
        if (this.hectareasSoja <= 0) {
            this.mensajeComponent.setErrorMsg("Hectáreas de soja no puede ser 0 o un número negativo.");
            return true;
        }
        if (this.hectareasSoja > this.hectareasTotales) {
            this.mensajeComponent.setErrorMsg("Usted declaró mayor cantidad de hectáreas de soja que hectáreas totales.");
            return true;
        }
        if (!this.latitud || this.latitud == "") {
            this.mensajeComponent.setErrorMsg("Falta completar Latitud.");
            return true;
        }
        if (!this.longitud || this.longitud == "") {
            this.mensajeComponent.setErrorMsg("Falta completar Longitud.");
            return true;
        }
        if ((!this.file || this.file.length < 1) && (!this.UsarArchivo_Id || this.Archivo_Id <= 0 || isUndefined(this.Archivo_Id))) {
            this.mensajeComponent.setErrorMsg("Falta adjuntar el archivo Kmz.");
            return true;
        }

        return false
    }

    redirigirAListado() {
        this.goToSeccion('/sustentable/listado-campos');

    }

    onResultadoDeclaracion(result: boolean) {
        if (!result) {
            this.cosechaId = 0;
        }
        else {
            this.consultarCamposAnterioresParaSugerir();
        }
    }

    cambiarModoOperacion() {
        this.CUIT = "";
        this.ingresarProveedorPorCUIT = this.operarComo == 2;
        this.declaracionComformidad.operarComo = this.operarComo;
        this.proveedorSeleccionado = null;
        this.corredorDebeCargarCuit = false;
        this.sePreseleccionoProveedor = false;
    }

    revisarProveedorSeleccionado() {
        if (this.proveedorSeleccionado)
            return;

        this.corredorDebeCargarCuit = !this.sePreseleccionoProveedor;
        if (this.corredorDebeCargarCuit) {
            this.CUIT = "";
            this.proveedorSeleccionado = null;
        } else {
            this.revisarDeclaracionJurada();
        }
    }

    revisarDeclaracionJurada() {
        if ((this.CUIT == "" || this.CUIT.length == 11) && this.cosechaId > 0) {
            if (this.esCorredor && this.operarComo == 1
                && (!this.proveedorSeleccionado) && this.proveedorTexto) {
                this.declaracionComformidad.razonSocialDeclaracion = this.proveedorTexto
            }
            this.declaracionComformidad.verificarDeclaracion();
        }
        if (this.CUIT == sessionStorage.getItem('cuit')) {
            this.mostrarWarningCuitsIguales();
        }
    }

    onSePreseleccionaProveedor() {
        this.sePreseleccionoProveedor = true;
        this.corredorDebeCargarCuit = false;
    }

    onQuery(value: string) {
        this.proveedorTexto = value;
        this.proveedorSeleccionado = null;
    }

    consultarCamposAnterioresParaSugerir() {
        let proveedorId: number = this.proveedorId;
        let cosechaId: number = this.cosechaId;
        let cuitTitularCP: string = this.CUIT;

        if (proveedorId && cosechaId && cuitTitularCP) {
            this.spinnerComponent.showIt();
            this.subscription = this.service.obtenerSugerenciaCamposNuevaCosecha(proveedorId, cosechaId, cuitTitularCP).subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    let camposSugeridos = this.manejarErroresApiResponse(result);
                    if (camposSugeridos && camposSugeridos.some(x => !x.CampoYaPresentado)) {
                        this.camposNuevosSugeridos = camposSugeridos.map(x => {
                            x.CosechaId = cosechaId;
                            return x;
                        });
                        this.mostrarSugerenciasCamposNuevos = true;
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                    console.error("Error al obtener sugerencia de campos de campaña previa", error);
                }
            );
        }
        else {
            console.error(`Parámetro faltante. ProveedorId: ${proveedorId}. CosechaId: ${cosechaId}. CUIT: ${cuitTitularCP}.`);
        }
    }

    mostrarWarningCuitsIguales() {
        const mensajeCuitsIguales: Message = {
            severity: 'warn',
            summary: 'Atención',
            detail: 'Recuerde que en el campo Proveedor y CUIT tiene que poner los datos del titular de la Carta de Porte',
            life: 10000
        }
        this.messageService.add(mensajeCuitsIguales);
    }

    renspaChanged(): void {
        this.service.renspaExiste(this.renspa, this.CUIT, this.cosechaId).subscribe((result: RenspaExiste) => {
            this.renspaExiste = result;
        });
    }

    cancelarSugerenciaCamposNuevos() {
        this.mostrarSugerenciasCamposNuevos = false;
        this.camposNuevosSugeridos = [];
    }

    grabarSugerenciaCamposNuevos() {
        this.blockUI.start();
        this.mostrarSugerenciasCamposNuevos = false;
        this.mensajeComponent.setMsgsEmpty();

        let camposAGuardar = this.camposNuevosSugeridos.filter(x => x.Seleccionado);
        this.service.guardarSugerenciasCamposNuevaCosecha(camposAGuardar, this.archivosNuevosSugerencias).subscribe(
            (result) => {
                this.blockUI.stop();
                this.mensajeComponent.setSuccessMsg("Los campos se han guardado correctamente");
                setTimeout(() => {
                    this.redirigirAListado();
                }, 3000);
            },
            error => {
                this.blockUI.stop();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    onSugeridosSeleccionarTodosChange() {
        let seleccionar = this.camposSugeridosSeleccionaTodos;
        this.camposNuevosSugeridos
            .filter(x => !x.CampoYaPresentado)
            .forEach(x => { x.Seleccionado = seleccionar });
    }

    onSugerenciaSeleccionadaChange(sugerencia: SugerenciaCampo) {
        if (sugerencia.Seleccionado) {
            this.camposSugeridosSeleccionaTodos = this.camposNuevosSugeridos.every(x => x.CampoYaPresentado || x.Seleccionado);
        }
        else {
            this.camposSugeridosSeleccionaTodos = false;
        }
    }

    haySugerenciaSeleccionada(): boolean {
        return this.camposNuevosSugeridos.some(x => !x.CampoYaPresentado && x.Seleccionado);
    }

    descargarKmz(campoCosechaId: number, proveedorId: number) {
        this.service.descargarArchivoKMZ(campoCosechaId, proveedorId).subscribe(
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

    getNombreCosecha(): string {
        return (this.cosechaId) ? this.cosechas.find(x => x.Id == this.cosechaId).Nombre : "";
    }

    abrirEdicionCampoSugerido(campo: SugerenciaCampo) {
        this.mensajeEdicionSugerencia.setMsgsEmpty();
        this.campoSugeridoEnEdicion = {...campo};
        this.setLocalidadCampoSugeridoEnEdicion(campo.Localidad_Id, campo.LocalidadNombre);
        this.fileInputSugerencia.nativeElement.value = '';
        this.mostrarEdicionSugerencia = true;
    }

    setLocalidadCampoSugeridoEnEdicion(idLocalidad: number, nombreLocalidad: string) {
        this.localidadCampoSugeridoEnEdicion = { IdLocalidad: idLocalidad, NombreLocalidad: nombreLocalidad };
    }

    cancelarEdicionSugerencia() {
        this.mostrarEdicionSugerencia = false;
    }

    guardarEdicionSugerencia() {
        if (!this.validarEdicionSugerencia()) {
            return;
        }
        this.mostrarEdicionSugerencia = false;
        let campoEditado = this.camposNuevosSugeridos.find(x => x.Renspa == this.campoSugeridoEnEdicion.Renspa);
        if (campoEditado) {
            campoEditado.NombreCampo = this.campoSugeridoEnEdicion.NombreCampo;
            campoEditado.Localidad_Id = this.campoSugeridoEnEdicion.Localidad_Id;
            campoEditado.LocalidadNombre = this.campoSugeridoEnEdicion.LocalidadNombre;
            campoEditado.HectareasTotales = this.campoSugeridoEnEdicion.HectareasTotales;
            campoEditado.HectareasSoja = this.campoSugeridoEnEdicion.HectareasSoja;
            campoEditado.Latitud = this.campoSugeridoEnEdicion.Latitud;
            campoEditado.Longitud = this.campoSugeridoEnEdicion.Longitud;
            campoEditado.NombreNuevoKmz = this.campoSugeridoEnEdicion.NombreNuevoKmz;
            campoEditado.Archivo_Id = this.campoSugeridoEnEdicion.Archivo_Id;
        }
    }

    validarEdicionSugerencia() {
        this.mensajeEdicionSugerencia.setMsgsEmpty();

        if (this.campoSugeridoEnEdicion.NombreCampo == "" || !this.campoSugeridoEnEdicion.NombreCampo) {
            this.mensajeEdicionSugerencia.setErrorMsg("Falta completar nombre del establecimiento.");
            return false;
        }

        if (!this.campoSugeridoEnEdicion.Localidad_Id || this.campoSugeridoEnEdicion.Localidad_Id <= 0) {
            this.mensajeEdicionSugerencia.setErrorMsg("Falta seleccionar la localidad.");
            return false;
        }
        
        if (!this.campoSugeridoEnEdicion.HectareasTotales || this.campoSugeridoEnEdicion.HectareasTotales <= 0) {
            this.mensajeEdicionSugerencia.setErrorMsg("Falta completar hectáreas totales.");
            return false;
        }
        if (!this.campoSugeridoEnEdicion.HectareasSoja || this.campoSugeridoEnEdicion.HectareasSoja <= 0) {
            this.mensajeEdicionSugerencia.setErrorMsg("Falta completar hectáreas de soja.");
            return false;
        }
        if (this.campoSugeridoEnEdicion.HectareasSoja > this.campoSugeridoEnEdicion.HectareasTotales) {
            this.mensajeEdicionSugerencia.setErrorMsg("Se declaró mayor cantidad de hectáreas de soja que hectáreas totales.");
            return false;
        }

        if (!this.campoSugeridoEnEdicion.Latitud || this.campoSugeridoEnEdicion.Latitud == "") {
            this.mensajeEdicionSugerencia.setErrorMsg("Falta completar Latitud.");
            return false;
        }
        if (!this.campoSugeridoEnEdicion.Longitud || this.campoSugeridoEnEdicion.Longitud == "") {
            this.mensajeEdicionSugerencia.setErrorMsg("Falta completar Longitud.");
            return false;
        }
        return true;
    }

    cargarArchivoEnSugerencia(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let archivo = fileList[0];
            if (this.archivosNuevosSugerencias.some(x => x.name == archivo.name)) {
                const msj: Message = { severity: 'warn', summary: 'Error', detail: 'Archivo ya cargado para otro campo', life: 5000 };
                this.messageService.add(msj);
                event.target.value = '';
            }
            else {
                this.campoSugeridoEnEdicion.NombreNuevoKmz = archivo.name;
                this.archivosNuevosSugerencias.push(archivo);
                this.campoSugeridoEnEdicion.Archivo_Id = 0;
            }
        }
    }

    exportarSugerencias() {
        let proveedorId: number = this.proveedorId;
        let cosechaId: number = this.cosechaId;
        let cuitTitularCP: string = this.CUIT;
        let nombreArchivo = "Sugerencia campos nueva cosecha.xls";
        
        if (proveedorId && cosechaId && cuitTitularCP) {
            this.spinnerExportarSugerencias.showIt();
            this.subscription = this.service.exportarSugerenciaCamposNuevaCosecha(proveedorId, cosechaId, cuitTitularCP).subscribe(
                (result) => {
                    this.spinnerExportarSugerencias.hideIt();
                    let camposExportados = this.manejarErroresApiResponse(result);
                    if (camposExportados) {
                        var blob = new Blob([camposExportados], { type: 'application/octet-stream' });
                        if (window.navigator.msSaveOrOpenBlob) {
                            //IE11
                            window.navigator.msSaveOrOpenBlob(blob, nombreArchivo);
                        }
                        else {
                            let url = window.URL.createObjectURL(blob);
                            let link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = nombreArchivo;
                            link.click();
                            setTimeout(() => { window.URL.revokeObjectURL(url); }, 0);
                        }
                    }
                },
                error => {
                    this.spinnerExportarSugerencias.hideIt();
                    console.error(error);
                }
            );
        }
        return false;
    }

    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | undefined {
        this.mensajeComponent.setMsgsEmpty();
        if (response.logout) {
            this.sessionDataService.logout();
            return undefined;
        }
        if (response.error) {
            this.mensajeComponent.setErrorMsg(response.error);
            return undefined;
        }
        if (response.info) {
            this.mensajeComponent.setInfoMsg(response.info);
        }
        return response.data;
    }
}
