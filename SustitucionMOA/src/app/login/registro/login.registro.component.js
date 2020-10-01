var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
import { Component, ViewChild, Renderer } from '@angular/core';
import { Router } from "@angular/router";
import { LoginService } from './../login.service';
import { LoginCommonComponent } from './../login.component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
var RegistroUsuarioComponent = /** @class */ (function (_super) {
    __extends(RegistroUsuarioComponent, _super);
    function RegistroUsuarioComponent(service, renderer, sessionDataService, router, navService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, sessionDataService, router, renderer, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.renderer = renderer;
        _this.sessionDataService = sessionDataService;
        _this.router = router;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.visibleButton = true;
        _this.renderer.setElementClass(document.body, 'loginBody', true);
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    RegistroUsuarioComponent.prototype.registrar = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.validarContrasenias();
            this.subscription = this.service.registrar(this.numeroProveedor, this.claveActivacion, this.username, this.contrasenia).subscribe(function (result) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.loginUser(result);
                }
            }, function (error) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RegistroUsuarioComponent.prototype.validarContrasenias = function () {
        this.contraseniaSinValor(this.contrasenia, "Ingresada");
        this.contraseniaSinValor(this.contraseniaConfirmacion, "de Confirmación");
        if (this.contrasenia != this.contraseniaConfirmacion)
            throw "La contraseña Ingresada y de Confirmación deben ser iguales";
    };
    RegistroUsuarioComponent.prototype.contraseniaSinValor = function (contrasenia, nombreCampo) {
        if (contrasenia == undefined || contrasenia == null || contrasenia == "")
            throw "Complete el campo " + nombreCampo;
    };
    RegistroUsuarioComponent.prototype.ngOnDestroy = function () {
        this.renderer.setElementClass(document.body, 'loginBody', false);
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], RegistroUsuarioComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], RegistroUsuarioComponent.prototype, "spinnerComponent", void 0);
    RegistroUsuarioComponent = __decorate([
        Component({
            selector: 'app-login-registro',
            templateUrl: "login.registro.component.html",
            providers: [LoginService]
        }),
        __metadata("design:paramtypes", [LoginService, Renderer, SessionDataService, Router, NavService, SecurityService, FloatMsgService, ModalService])
    ], RegistroUsuarioComponent);
    return RegistroUsuarioComponent;
}(LoginCommonComponent));
export { RegistroUsuarioComponent };
//# sourceMappingURL=login.registro.component.js.map