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
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Seccion } from './../../common/models/seccion';
import { ModalService } from './../../common/services/ModalService';
var AltaUsuarioComponent = /** @class */ (function (_super) {
    __extends(AltaUsuarioComponent, _super);
    function AltaUsuarioComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tipoOptions = [];
        _this.perfilOptions = [];
        _this.perfiles = [];
        _this.usuarios = new Array();
        _this.visibleButton = true;
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.perfilDropdownComponent = new DropdownComponent();
        _this.tipoDropdownComponent = new DropdownComponent();
        return _this;
    }
    AltaUsuarioComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab('usuario', 'Alta Usuario');
    };
    AltaUsuarioComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("ABM USUARIOS");
        this.navService.setSeccionList([new Seccion('/usuario/list', 'usuario', 'Listado Usuarios'), new Seccion('/usuario/alta', 'usuario', 'Alta Usuario')]);
        this.getPerfilesOptions();
    };
    AltaUsuarioComponent.prototype.alta = function () {
        var _this = this;
        var usuario = { numeroProveedor: this.numeroProveedor, email: this.email, perfil: this.perfilDropdownComponent.selectedOption, tipo: this.tipoDropdownComponent.selectedOption };
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        //this.usuarios.push(usuario); -> Soporta un solo usuario
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.alta(usuario).subscribe(function (result) {
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
                    _this.vaciarInputs();
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
    AltaUsuarioComponent.prototype.getPerfilesOptions = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.service.getPerfiles().subscribe(function (result) {
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
                    _this.perfilOptions = result.data.perfiles;
                    _this.tipoOptions = result.data.tipos;
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    AltaUsuarioComponent.prototype.tienePerfiles = function () {
        return this.perfiles.length > 0;
    };
    AltaUsuarioComponent.prototype.vaciarInputs = function () {
        this.numeroProveedor = "";
        this.usuarioId = "";
        this.email = "";
        this.perfiles = [];
        this.usuarios = [];
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], AltaUsuarioComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], AltaUsuarioComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild('dropdown_perfil'),
        __metadata("design:type", DropdownComponent)
    ], AltaUsuarioComponent.prototype, "perfilDropdownComponent", void 0);
    __decorate([
        ViewChild('dropdown_tipo'),
        __metadata("design:type", DropdownComponent)
    ], AltaUsuarioComponent.prototype, "tipoDropdownComponent", void 0);
    AltaUsuarioComponent = __decorate([
        Component({
            selector: 'app-usuario-alta-contrasenia',
            templateUrl: "usuario.alta.component.html",
            providers: [UsuarioService]
        }),
        __metadata("design:paramtypes", [UsuarioService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], AltaUsuarioComponent);
    return AltaUsuarioComponent;
}(BaseComponent));
export { AltaUsuarioComponent };
//# sourceMappingURL=usuario.alta.component.js.map