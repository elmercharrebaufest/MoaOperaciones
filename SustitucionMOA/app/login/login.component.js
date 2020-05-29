"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
var login_service_1 = require("./login.service");
var mensaje_component_1 = require("./../common/view-child/mensaje/mensaje.component");
var spinner_small_component_1 = require("./../common/view-child/spinner-small/spinner-small.component");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var SecurityService_1 = require("./../common/services/SecurityService");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var base_component_1 = require("./../common/base-components/base-component");
var ModalService_1 = require("./../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var LoginCommonComponent = /** @class */ (function (_super) {
    __extends(LoginCommonComponent, _super);
    function LoginCommonComponent(sessionDataService, router, renderer, navService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.sessionDataService = sessionDataService;
        _this.router = router;
        _this.renderer = renderer;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.renderer.setElementClass(document.body, 'wrapper', false);
        return _this;
    }
    LoginCommonComponent.prototype.loginUser = function (result) {
        sessionStorage.setItem("username", result.username);
        sessionStorage.setItem("nombre", result.nombre);
        sessionStorage.setItem("proveedor", result.proveedor);
        sessionStorage.setItem("granosFlag", result.granosFlag);
        sessionStorage.setItem("tipoUsuario", result.tipoUsuario);
        sessionStorage.setItem("noticias", JSON.stringify(result.noticias));
        sessionStorage.setItem("permisos", JSON.stringify(result.permisos));
        this.sessionDataService.setNombre(result.nombre);
        this.sessionDataService.setUsername(result.username);
        this.sessionDataService.setProveedor(result.proveedor);
        this.sessionDataService.setTipoUsuario(result.tipoUsuario);
        this.sessionDataService.setNoticias(result.noticias);
        this.sessionDataService.setPermisos(result.permisos);
        this.sessionDataService.setGranosFlag(result.granosFlag);
        if (result.tipoUsuario == "ADMP" || result.tipoUsuario == "ADNA" || result.tipoUsuario == "RYDD") {
            this.navService.navegarSeccion('/aduana/pesada-online');
        }
        else if (result.tipoUsuario == "CLIE") {
            this.navService.navegarSeccion('/cuenta-corriente/simple');
        }
        else {
            if (result.granosFlag == "A") {
                sessionStorage.setItem("granosSelected", "G");
                this.navService.navegarSeccion('/home');
            }
            else {
                sessionStorage.setItem("granosSelected", result.granosFlag);
                if (result.granosFlag == "G") {
                    this.navService.navegarSeccion('/home');
                }
                else {
                    this.navService.navegarSeccion('/home-ngs');
                }
            }
        }
    };
    return LoginCommonComponent;
}(base_component_1.BaseComponent));
exports.LoginCommonComponent = LoginCommonComponent;
var LoginComponent = /** @class */ (function (_super) {
    __extends(LoginComponent, _super);
    function LoginComponent(service, sessionDataService, navService, router, renderer, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, sessionDataService, router, renderer, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.sessionDataService = sessionDataService;
        _this.navService = navService;
        _this.router = router;
        _this.renderer = renderer;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.titulo = "";
        _this.username = "";
        _this.pass = "";
        _this.loginButtonEnable = true;
        _this.captchaOk = null;
        _this.renderer.setElementClass(document.body, 'loginBody', true);
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
        return _this;
    }
    LoginComponent.prototype.ngOnInit = function () {
        this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');
    };
    LoginComponent.prototype.login = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.loginButtonEnable = false;
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.login(this.username, this.pass).subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
            _this.loginButtonEnable = true;
            if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                //this.loginUser(result);
                _this.redirect(result);
            }
            return false;
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    LoginComponent.prototype.enterPressedLogin = function (event) {
        if (event.keyCode == 13 && this.loginButtonEnable) {
            this.login();
        }
    };
    LoginComponent.prototype.ngOnDestroy = function () {
        this.renderer.setElementClass(document.body, 'loginBody', false);
    };
    LoginComponent.prototype.redirect = function (result) {
        if (result.tipoUsuario == "DATAAGROLOGIN") {
            if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.url == undefined || result.url == "") {
                this.mensajeComponent.setErrorMsg("No se pudo obtener la URL destino");
            }
            else {
                location.href = result.url;
            }
        }
        else {
            this.loginUser(result);
        }
    };
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], LoginComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_small_component_1.SpinnerSmallComponent),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], LoginComponent.prototype, "spinnerSmallComponent", void 0);
    LoginComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/login/login.component.html?v=" + new Date().getTime(),
            providers: [login_service_1.LoginService]
        }),
        __metadata("design:paramtypes", [login_service_1.LoginService, SessionDataService_1.SessionDataService, NavService_1.NavService, router_1.Router, core_1.Renderer, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], LoginComponent);
    return LoginComponent;
}(LoginCommonComponent));
exports.LoginComponent = LoginComponent;
//# sourceMappingURL=login.component.js.map