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
var contrato_service_1 = require("./contrato.service");
var list_base_component_1 = require("./../common/base-components/list-base-component");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var SecurityService_1 = require("./../common/services/SecurityService");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var ModalService_1 = require("./../common/services/ModalService");
var Seccion_1 = require("./../common/models/Seccion");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var ContratoBaseComponent = /** @class */ (function (_super) {
    __extends(ContratoBaseComponent, _super);
    function ContratoBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroProducto = null;
        _this.filtroVendedor = null;
        _this.filtroContrato = "";
        _this.productoSelected = "";
        _this.vendedorSelected = "";
        return _this;
    }
    ContratoBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CONTRATOS"); };
    ContratoBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion_1.Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion_1.Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion_1.Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion_1.Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.getData();
    };
    ContratoBaseComponent.prototype.setFiltroProducto = function (producto) {
        this.productoSelected = producto;
    };
    ContratoBaseComponent.prototype.setFiltroVendedor = function (vendedor) {
        this.vendedorSelected = vendedor;
    };
    ContratoBaseComponent.prototype.isVisible = function () {
        if (this.data && this.data.contratosInfo.length != 0)
            return true;
        else
            return false;
    };
    ContratoBaseComponent.prototype.vaciarFiltros = function () {
        this.filtroProducto = null;
        this.filtroVendedor = null;
        this.filtroContrato = "";
        this.productoSelected = "";
        this.vendedorSelected = "";
    };
    ContratoBaseComponent.prototype.cargarFiltrosVariables = function (result) {
        if (result.filtroProducto != undefined)
            this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined)
            this.filtroVendedor = result.filtroVendedor.options;
    };
    ContratoBaseComponent = __decorate([
        core_1.Component({
            selector: 'app-contrato',
            template: "",
            providers: [contrato_service_1.ContratoService]
        }),
        __metadata("design:paramtypes", [contrato_service_1.ContratoService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], ContratoBaseComponent);
    return ContratoBaseComponent;
}(list_base_component_1.ListBaseComponent));
exports.ContratoBaseComponent = ContratoBaseComponent;
//# sourceMappingURL=contrato.component.js.map