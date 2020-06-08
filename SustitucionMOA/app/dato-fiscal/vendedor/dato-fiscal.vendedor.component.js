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
var dato_fiscal_service_1 = require("./../dato-fiscal.service");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var seccion_1 = require("./../../common/models/seccion");
var base_component_1 = require("./../../common/base-components/base-component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var VendedoresListComponent = /** @class */ (function (_super) {
    __extends(VendedoresListComponent, _super);
    function VendedoresListComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.orderedByColumn = "vendedor";
        _this.orderDirection = 1;
        _this.itemsPerPage = 20;
        _this.filtroVendedor = "";
        _this.filtroNroVendedor = "";
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        return _this;
    }
    VendedoresListComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("dato-fiscal", "Mis Vendedores");
    };
    VendedoresListComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR VENDEDORES");
        var secciones = [];
        if (this.isAuthorized("CONSULTAR DATOS FISCALES"))
            secciones.push(new seccion_1.Seccion('/dato-fiscal/situacion-fiscal', 'dato-fiscal', 'Mi Situacion Fiscal'));
        if (this.isAuthorized("CONSULTAR VENDEDORES") && this.isCorredor())
            secciones.push(new seccion_1.Seccion('/dato-fiscal/vendedor', 'dato-fiscal', 'Mis Vendedores'));
        if (this.isAuthorized("CONSULTAR DOCUMENTACION"))
            secciones.push(new seccion_1.Seccion('/dato-fiscal/documentacion', 'dato-fiscal', 'Documentacion'));
        this.navService.setSeccionList(secciones);
        this.getUsuario();
    };
    VendedoresListComponent.prototype.getUsuario = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.getVendedores("", "").subscribe(function (result) {
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
                    _this.data = result.data.vendedores;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
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
    VendedoresListComponent.prototype.isVisible = function () {
        return this.data && this.data.length != 0;
    };
    VendedoresListComponent.prototype.orderColumnBy = function (column) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        }
        else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    };
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], VendedoresListComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], VendedoresListComponent.prototype, "spinnerComponent", void 0);
    VendedoresListComponent = __decorate([
        core_1.Component({
            selector: 'list',
            templateUrl: "./app/dato-fiscal/vendedor/dato-fiscal.vendedor.component.html?v=" + new Date().getTime(),
            providers: [dato_fiscal_service_1.DatoFiscalService]
        }),
        __metadata("design:paramtypes", [dato_fiscal_service_1.DatoFiscalService, NavService_1.NavService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], VendedoresListComponent);
    return VendedoresListComponent;
}(base_component_1.BaseComponent));
exports.VendedoresListComponent = VendedoresListComponent;
//# sourceMappingURL=dato-fiscal.vendedor.component.js.map