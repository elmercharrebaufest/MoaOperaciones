import { Component, OnInit, ViewChild } from '@angular/core';
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
import { CampoProveedor, CampoSustentable, CampoCosecha } from './../sustentable'
import { DeclaracionConformidadComponent } from '../declaracion-conformidad/declaracion-conformidad.component';
import { isUndefined } from 'util';
import { VendedorProveedor } from '../../common/models/vendedorProveedor';
import { finalize } from 'rxjs/operators';
import { Message, MessageService } from 'primeng/api';
import { RenspaExiste } from '../renspa-existe.interface';
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
    error?: string;
    info?: string;
    logout?: boolean;
}
@Component({
    selector: 'app-alta',
    templateUrl: './alta.component.html',
    providers: [VentaSustentableService, MessageService]
})
export class AltaComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild(DeclaracionConformidadComponent)
    protected declaracionComformidad: DeclaracionConformidadComponent;

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

    proveedores: VendedorProveedor[] = [];
    proveedorSeleccionado: VendedorProveedor;
    sePreseleccionoProveedor = false;
    proveedorTexto: string = "";
    corredorDebeCargarCuit = false;
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
                        this.hectareasTotales = result.HectareasTotales;
                        this.hectareasSoja = result.HectareasSoja;
                        this.localidadId = result.Localidad_Id;

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
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        this.blockUI.stop();
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
            this.mensajeComponent.setErrorMsg("Usted declaro mayor cantidad de hectareas de soja que hectareas totales.");
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
        if (!result)
            this.cosechaId = 0;
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
        this.service.renspaExiste(this.renspa, this.CUIT).subscribe((result: RenspaExiste) => {
            this.renspaExiste = result;
        });
    }
}
