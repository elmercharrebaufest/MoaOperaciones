import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from "@angular/router";
import { DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { UsuarioService } from './../usuario.service';

@Component({
    selector: 'app-usuario-alta-empresa-no-granos',
    templateUrl: `usuario.alta-empresa-no-granos.component.html`,
    styleUrls: [
        './usuario.alta-empresa-no-granos.component.css',
    ],
    providers: [UsuarioService]
})
export class UsuarioAltaEmpresaNoGranosComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('dropdown_rol')
    protected rolDropdownComponent: DropdownComponent;

    constructor(protected service: UsuarioService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private route: ActivatedRoute,) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.rolDropdownComponent = new DropdownComponent();
    }


    RazonSocial: string = "";
    CUIT: string = "";
    Email: string = "";
    Telefono: string = "";
    RealizarAnalisisNOSIS: boolean = false;
    RequiereVerificacionCompras: boolean = false;
    IdRubro: number = 0;
    condicionDePago: string = "";
    servicioPrestado: string = "";
    organizacionDeCompra: string = "";
    razonDeEleccion: string = "";
    facturacionAnual: number = null;
    solicitanteInterno: string = "";
    rubros: any = [];
    proveedorId: number = null;
    readonlyCUIT: boolean = false;
    readonlyEmail: boolean = false;
    ingresoAPlanta: boolean = false;
    altaInterna: boolean = false;
    tipoCambiario: number = 1;
    nosisObligatorio: boolean = false;
    siperDisabled: boolean = false;
    readonlyRazonSocial: boolean = false;
    siperObligatorio: boolean = false;
    observacionInterna: string = "";
    mensajeSuccess: string = "";
    IdProveedorResultado: number = 0;

    observacionesParaElProveedor: string = "";

    ngOnInit() {
        this.securityService.tienePermisoRedirect("ALTA EMPRESA NO GRANOS");
        this.getRubrosOptions();
        this.getTipoCambiario();

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) {
                this.proveedorId = params["id"];
            }
            if (params["cuit"] > 0) {
                this.CUIT = params["cuit"];
                this.readonlyCUIT = true;
                this.obtenerRazonSocial();
            }
            if (params["mail"] != "" && params["mail"] != undefined && params["mail"] != null) {
                this.Email = params["mail"];
                this.readonlyEmail = true;
            }
        });
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

    obtenerRazonSocial() {
        try {
            //Lo comento hasta que podamos usar otro servicio que funcione en QA
            return true;
            this.subscriptionDropDowns = this.service.getRazonSocial(this.CUIT).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        if (result != "") {
                            this.RazonSocial = result;
                            this.readonlyRazonSocial = true;
                        }
                        else {
                            this.readonlyRazonSocial = false;
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

    grabar() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();

        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }
        try {
            this.service.grabarNuevoProveedorNoGranos(this.RazonSocial, this.CUIT, this.Email, this.Telefono, this.RealizarAnalisisNOSIS, this.IdRubro, this.condicionDePago,
                this.servicioPrestado, this.organizacionDeCompra, this.razonDeEleccion, this.facturacionAnual, this.solicitanteInterno, this.proveedorId,
                this.observacionesParaElProveedor, this.RequiereVerificacionCompras, this.ingresoAPlanta, this.altaInterna, this.siperObligatorio,
                this.observacionInterna
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
                        //this.mensajeComponent.setSuccessMsg(result.data.Mensaje);
                        this.mensajeSuccess = result.data.Mensaje;

                        this.IdProveedorResultado = result.data.IdEntidad;

                        document
                            .getElementById("openModalNotificacion")
                            .click();
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

    rechazar() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        console.log(this.facturacionAnual);
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }
        try {

            this.service.rechazarNuevoProveedorNoGranos(this.proveedorId, this.observacionesParaElProveedor).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.limpiarCampos();
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

        if(this.IdRubro == 5)
        return;

        if (this.tipoCambiario <= 0)
            this.tipoCambiario = 1;
        let facturacionDolares = this.facturacionAnual / this.tipoCambiario;

        
        if (facturacionDolares > 15000) {
            this.nosisObligatorio = true;
            this.RealizarAnalisisNOSIS = true;
        }
        else{
            this.nosisObligatorio = false;
        }
    }

    limpiarCampos() {
        this.RazonSocial = "";
        this.CUIT = "";
        this.Email = "";
        this.Telefono = "";
        this.RealizarAnalisisNOSIS = false;
        this.RequiereVerificacionCompras = false;
        this.IdRubro = 0;
        this.condicionDePago = "";
        this.servicioPrestado = "";
        this.organizacionDeCompra = "";
        this.razonDeEleccion = "";
        this.facturacionAnual = null;
        this.observacionesParaElProveedor = "";
        this.proveedorId = null;
        this.readonlyCUIT = false;
        this.readonlyEmail = false;
        this.altaInterna = false;
        this.ingresoAPlanta = false;
        this.nosisObligatorio = false;

        this.siperObligatorio = false;
        this.observacionInterna = "";
        this.IdProveedorResultado = 0;
    }

    verificarIngresoAPlanta() {
        if (this.ingresoAPlanta)
            this.RequiereVerificacionCompras = true;
    }

    redirigir() {
        document
            .getElementById("openModalNotificacion")
            .click();

        if (this.altaInterna) {
            this.goToSeccionParam('/alta-empresa-no-granos', this.IdProveedorResultado.toString());
        }
        else {
            this.limpiarCampos();
        }
    }

    esObligatorioFlete() {
        if(this.IdRubro == 5){
            this.RealizarAnalisisNOSIS = true;
            this.nosisObligatorio = true;
            this.siperObligatorio = true;
            this.siperDisabled = true;
        }
        else{
            this.RealizarAnalisisNOSIS = false;
            this.nosisObligatorio = false;
            this.siperObligatorio = false;
            this.siperDisabled = false;
        }
        this.calcularFacturacion();
    }
}