var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
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
var UsuarioAltaEmpresaNoGranosComponent = /** @class */ (function (_super) {
    __extends(UsuarioAltaEmpresaNoGranosComponent, _super);
    function UsuarioAltaEmpresaNoGranosComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService, route) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.RazonSocial = "";
        _this.CUIT = "";
        _this.Email = "";
        _this.Telefono = "";
        _this.RealizarAnalisisNOSIS = false;
        _this.RequiereVerificacionCompras = false;
        _this.IdRubro = 0;
        _this.condicionDePago = "";
        _this.servicioPrestado = "";
        _this.organizacionDeCompra = "";
        _this.razonDeEleccion = "";
        _this.facturacionAnual = null;
        _this.solicitanteInterno = "";
        _this.rubros = [];
        _this.proveedorId = null;
        _this.readonlyCUIT = false;
        _this.readonlyEmail = false;
        _this.ingresoAPlanta = false;
        _this.altaInterna = false;
        _this.tipoCambiario = 0;
        _this.nosisObligatorio = false;
        _this.readonlyRazonSocial = false;
        _this.observacionesParaElProveedor = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.rolDropdownComponent = new DropdownComponent();
        return _this;
    }
    UsuarioAltaEmpresaNoGranosComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.securityService.tienePermisoRedirect("ALTA EMPRESA NO GRANOS");
        this.getRubrosOptions();
        this.getTipoCambiario();
        this.route.params.forEach(function (params) {
            if (params["id"] > 0) {
                _this.proveedorId = params["id"];
            }
            if (params["cuit"] > 0) {
                _this.CUIT = params["cuit"];
                _this.readonlyCUIT = true;
                _this.obtenerRazonSocial();
            }
            if (params["mail"] != "" && params["mail"] != undefined && params["mail"] != null) {
                _this.Email = params["mail"];
                _this.readonlyEmail = true;
            }
        });
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.getRubrosOptions = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.service.getRubros().subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.rubros = result.data;
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.getTipoCambiario = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.service.getTipoCambiario().subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.tipoCambiario = result.data;
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.obtenerRazonSocial = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.service.getRazonSocial(this.CUIT).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    if (result != "") {
                        _this.RazonSocial = result;
                        _this.readonlyRazonSocial = true;
                    }
                    else {
                        _this.readonlyRazonSocial = false;
                    }
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.grabar = function () {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        console.log(this.facturacionAnual);
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }
        try {
            this.service.grabarNuevoProveedorNoGranos(this.RazonSocial, this.CUIT, this.Email, this.Telefono, this.RealizarAnalisisNOSIS, this.IdRubro, this.condicionDePago, this.servicioPrestado, this.organizacionDeCompra, this.razonDeEleccion, this.facturacionAnual, this.solicitanteInterno, this.proveedorId, this.observacionesParaElProveedor, this.RequiereVerificacionCompras, this.ingresoAPlanta, this.altaInterna).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.limpiarCampos();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.rechazar = function () {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        console.log(this.facturacionAnual);
        if (this.facturacionAnual == null) {
            this.facturacionAnual = 0;
        }
        try {
            this.service.rechazarNuevoProveedorNoGranos(this.proveedorId, this.observacionesParaElProveedor).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.limpiarCampos();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.calcularFacturacion = function () {
        var facturacionDolares = this.facturacionAnual / this.tipoCambiario;
        if (facturacionDolares > 15000) {
            this.nosisObligatorio = true;
            this.RealizarAnalisisNOSIS = true;
        }
        else {
            this.nosisObligatorio = false;
        }
    };
    UsuarioAltaEmpresaNoGranosComponent.prototype.limpiarCampos = function () {
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
        this.altaInterna = false;
        this.ingresoAPlanta = false;
        this.nosisObligatorio = false;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], UsuarioAltaEmpresaNoGranosComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], UsuarioAltaEmpresaNoGranosComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild('dropdown_rol'),
        __metadata("design:type", DropdownComponent)
    ], UsuarioAltaEmpresaNoGranosComponent.prototype, "rolDropdownComponent", void 0);
    UsuarioAltaEmpresaNoGranosComponent = __decorate([
        Component({
            selector: 'app-usuario-alta-empresa-no-granos',
            templateUrl: "usuario.alta-empresa-no-granos.component.html",
            providers: [UsuarioService]
        }),
        __metadata("design:paramtypes", [UsuarioService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService, ActivatedRoute])
    ], UsuarioAltaEmpresaNoGranosComponent);
    return UsuarioAltaEmpresaNoGranosComponent;
}(BaseComponent));
export { UsuarioAltaEmpresaNoGranosComponent };
//# sourceMappingURL=usuario.alta-empresa-no-granos.component.js.map