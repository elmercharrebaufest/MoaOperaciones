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
var usuario_service_1 = require("./../usuario.service");
var base_component_1 = require("./../../common/base-components/base-component");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var dropdown_component_1 = require("./../../common/view-child/dropdown/dropdown.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var seccion_1 = require("./../../common/models/seccion");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
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
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        _this.perfilDropdownComponent = new dropdown_component_1.DropdownComponent();
        _this.tipoDropdownComponent = new dropdown_component_1.DropdownComponent();
        return _this;
    }
    AltaUsuarioComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab('usuario', 'Alta Usuario');
    };
    AltaUsuarioComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("ABM USUARIOS");
        this.navService.setSeccionList([new seccion_1.Seccion('/usuario/list', 'usuario', 'Listado Usuarios'), new seccion_1.Seccion('/usuario/alta', 'usuario', 'Alta Usuario')]);
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], AltaUsuarioComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], AltaUsuarioComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild('dropdown_perfil'),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], AltaUsuarioComponent.prototype, "perfilDropdownComponent", void 0);
    __decorate([
        core_1.ViewChild('dropdown_tipo'),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], AltaUsuarioComponent.prototype, "tipoDropdownComponent", void 0);
    AltaUsuarioComponent = __decorate([
        core_1.Component({
            selector: 'cambio-contrasenia',
            templateUrl: "./app/usuario/alta/usuario.alta.component.html?v=" + new Date().getTime(),
            providers: [usuario_service_1.UsuarioService]
        }),
        __metadata("design:paramtypes", [usuario_service_1.UsuarioService, NavService_1.NavService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], AltaUsuarioComponent);
    return AltaUsuarioComponent;
}(base_component_1.BaseComponent));
exports.AltaUsuarioComponent = AltaUsuarioComponent;
//# sourceMappingURL=usuario.alta.component.js.map