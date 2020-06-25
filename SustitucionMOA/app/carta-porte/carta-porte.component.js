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
var carta_porte_service_1 = require("./carta-porte.service");
var list_base_component_1 = require("./../common/base-components/list-base-component");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var SecurityService_1 = require("./../common/services/SecurityService");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var seccion_1 = require("./../common/models/seccion");
var ModalService_1 = require("./../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var CartaPorteBaseComponent = /** @class */ (function (_super) {
    __extends(CartaPorteBaseComponent, _super);
    function CartaPorteBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroCCPP = "";
        _this.filtroProducto = null;
        _this.filtroContrato = "";
        _this.productoSelected = "";
        _this.filtroVendedor = null;
        _this.vendedorSelected = "";
        return _this;
    }
    CartaPorteBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        var secciones = [new seccion_1.Seccion('/carta-porte/descarga', 'carta-porte', 'Descargas'), new seccion_1.Seccion('/carta-porte/aplicacion', 'carta-porte', 'Aplicaciones')];
        if (this.isAuthorized("CREAR FORMULARIO CCPP"))
            secciones.push(new seccion_1.Seccion('/carta-porte/formulario', 'carta-porte', 'Formulario'));
        this.navService.setSeccionList(secciones);
        this.getData();
    };
    CartaPorteBaseComponent.prototype.setFiltroProducto = function (producto) {
        this.productoSelected = producto;
    };
    CartaPorteBaseComponent.prototype.setFiltroVendedor = function (vendedor) {
        this.vendedorSelected = vendedor;
    };
    CartaPorteBaseComponent.prototype.isVisible = function () {
        if (this.data && this.data.cartasPorte.length != 0)
            return true;
        else
            return false;
    };
    CartaPorteBaseComponent.prototype.vaciarFiltros = function () {
        this.filtroProducto = null;
        this.filtroVendedor = null;
        this.filtroCCPP = "";
        this.productoSelected = "";
        this.vendedorSelected = "";
    };
    CartaPorteBaseComponent.prototype.cargarFiltrosVariables = function (result) {
        if (result.filtroProducto != undefined)
            this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined)
            this.filtroVendedor = result.filtroVendedor.options;
    };
    CartaPorteBaseComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/carta-porte/carta-porte.component.html?v=" + new Date().getTime(),
            providers: [carta_porte_service_1.CartaPorteService]
        }),
        __metadata("design:paramtypes", [carta_porte_service_1.CartaPorteService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], CartaPorteBaseComponent);
    return CartaPorteBaseComponent;
}(list_base_component_1.ListBaseComponent));
exports.CartaPorteBaseComponent = CartaPorteBaseComponent;
//# sourceMappingURL=carta-porte.component.js.map