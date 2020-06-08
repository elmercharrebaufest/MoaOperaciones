"use strict";
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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var login_service_1 = require("./../login.service");
var login_component_1 = require("./../login.component");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
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
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], RegistroUsuarioComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], RegistroUsuarioComponent.prototype, "spinnerComponent", void 0);
    RegistroUsuarioComponent = __decorate([
        core_1.Component({
            selector: 'cambio-contrasenia',
            templateUrl: "./app/login/registro/login.registro.component.html?v=" + new Date().getTime(),
            providers: [login_service_1.LoginService]
        }),
        __metadata("design:paramtypes", [login_service_1.LoginService, core_1.Renderer, SessionDataService_1.SessionDataService, router_1.Router, NavService_1.NavService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], RegistroUsuarioComponent);
    return RegistroUsuarioComponent;
}(login_component_1.LoginCommonComponent));
exports.RegistroUsuarioComponent = RegistroUsuarioComponent;
//# sourceMappingURL=login.registro.component.js.map