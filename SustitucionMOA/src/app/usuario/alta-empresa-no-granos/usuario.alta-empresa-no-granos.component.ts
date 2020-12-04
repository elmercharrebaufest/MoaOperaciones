import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from "@angular/router";
import { DropdownOption, DropdownComponent } from '../../common/view-child/dropdown/dropdown.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { Seccion } from './../../common/models/seccion';
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
    providers: [UsuarioService]
})
export class UsuarioAltaEmpresaNoGranosComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('dropdown_rol')
    protected rolDropdownComponent: DropdownComponent;

    constructor(protected service: UsuarioService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private route: ActivatedRoute, ) {
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
    observacionesParaElProveedor: string = "";

    ngOnInit() {
        this.securityService.tienePermisoRedirect("ALTA EMPRESA NO GRANOS");
        this.getRubrosOptions();
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) {
                this.proveedorId = params["id"];
            }
            if (params["cuit"] > 0) {
                this.CUIT = params["cuit"];
                this.readonlyCUIT = true;
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
                result => {
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


    grabar() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        console.log(this.facturacionAnual);
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }
        try {
            this.service.grabarNuevoProveedorNoGranos(this.RazonSocial, this.CUIT, this.Email, this.Telefono, this.RealizarAnalisisNOSIS, this.IdRubro, this.condicionDePago,
                this.servicioPrestado, this.organizacionDeCompra, this.razonDeEleccion, this.facturacionAnual, this.solicitanteInterno, this.proveedorId,
                this.observacionesParaElProveedor, this.RequiereVerificacionCompras).subscribe(
                    result => {
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

    rechazar() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        console.log(this.facturacionAnual);
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }
        try {
            this.service.rechazarNuevoProveedorNoGranos(this.proveedorId, this.observacionesParaElProveedor).subscribe(
                    result => {
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
        this.solicitanteInterno = "";
        this.observacionesParaElProveedor = "";
        this.proveedorId = null;
        this.readonlyCUIT = false;
        this.readonlyEmail = false;
    }

}