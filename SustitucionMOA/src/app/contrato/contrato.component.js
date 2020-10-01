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
import { Component } from '@angular/core';
import { ContratoService } from './contrato.service';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';
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
        this.navService.setSeccionList([new Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')]);
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
        Component({
            selector: 'app-contrato',
            template: "",
            providers: [ContratoService]
        }),
        __metadata("design:paramtypes", [ContratoService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], ContratoBaseComponent);
    return ContratoBaseComponent;
}(ListBaseComponent));
export { ContratoBaseComponent };
//# sourceMappingURL=contrato.component.js.map