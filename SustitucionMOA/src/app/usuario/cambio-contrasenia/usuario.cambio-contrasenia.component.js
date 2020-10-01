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
import { Component, ViewChild } from '@angular/core';
import { UsuarioService } from './../usuario.service';
import { BaseComponent } from './../../common/base-components/base-component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
var CambioContraseniaComponent = /** @class */ (function (_super) {
    __extends(CambioContraseniaComponent, _super);
    function CambioContraseniaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.visibleButton = true;
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    CambioContraseniaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab('usuario', 'Cambio Contraseña');
    };
    CambioContraseniaComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.navService.setSeccionList([]);
    };
    CambioContraseniaComponent.prototype.guardarContrasenia = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        try {
            this.validarContrasenias();
            this.unsubscribe();
            this.subscription = this.service.cambiarContrasenia(this.contraseniaActual, this.contraseniaNueva).subscribe(function (result) {
                _this.visibleButton = true;
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
    CambioContraseniaComponent.prototype.validarContrasenias = function () {
        this.contraseniaSinValor(this.contraseniaActual, "Actual");
        this.contraseniaSinValor(this.contraseniaNueva, "Nueva");
        this.contraseniaSinValor(this.contraseniaNuevaConfirmacion, "de Confirmación");
        if (this.contraseniaNueva != this.contraseniaNuevaConfirmacion)
            throw "La contraseña Nueva y de Confirmación deben ser iguales";
    };
    CambioContraseniaComponent.prototype.contraseniaSinValor = function (contrasenia, nombreCampo) {
        if (contrasenia == undefined || contrasenia == null || contrasenia == "")
            throw "Complete el campo " + nombreCampo;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], CambioContraseniaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], CambioContraseniaComponent.prototype, "spinnerComponent", void 0);
    CambioContraseniaComponent = __decorate([
        Component({
            selector: 'app-usuario-cambio-contrasenia',
            templateUrl: "usuario.cambio-contrasenia.component.html",
            providers: [UsuarioService]
        }),
        __metadata("design:paramtypes", [UsuarioService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], CambioContraseniaComponent);
    return CambioContraseniaComponent;
}(BaseComponent));
export { CambioContraseniaComponent };
//# sourceMappingURL=usuario.cambio-contrasenia.component.js.map