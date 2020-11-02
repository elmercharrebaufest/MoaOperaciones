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
import { LoginService } from './login.service';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';
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
        if (result.esNuevoUsuario) {
            if (result.granosFlag == "A") {
                sessionStorage.setItem("granosSelected", "G");
                this.navService.navegarSeccion('/alta-empresa-granos');
            }
            else {
                sessionStorage.setItem("granosSelected", result.granosFlag);
                if (result.granosFlag == "G") {
                    this.navService.navegarSeccion('/alta-empresa-granos');
                }
                else {
                    this.navService.navegarSeccion('/alta-empresa-no-granos');
                }
            }
        }
        else {
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
        }
    };
    return LoginCommonComponent;
}(BaseComponent));
export { LoginCommonComponent };
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
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerSmallComponent = new SpinnerSmallComponent();
        return _this;
    }
    LoginComponent.prototype.ngOnInit = function () {
        this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');
        this.validarLoginAzure();
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
    LoginComponent.prototype.validarLoginAzure = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.loginButtonEnable = false;
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.validarLoginAzure().subscribe(function (result) {
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
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], LoginComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerSmallComponent),
        __metadata("design:type", SpinnerSmallComponent)
    ], LoginComponent.prototype, "spinnerSmallComponent", void 0);
    LoginComponent = __decorate([
        Component({
            selector: 'app-login',
            templateUrl: "login.component.html",
            providers: [LoginService]
        }),
        __metadata("design:paramtypes", [LoginService, SessionDataService, NavService, Router, Renderer, SecurityService, FloatMsgService, ModalService])
    ], LoginComponent);
    return LoginComponent;
}(LoginCommonComponent));
export { LoginComponent };
//# sourceMappingURL=login.component.js.map