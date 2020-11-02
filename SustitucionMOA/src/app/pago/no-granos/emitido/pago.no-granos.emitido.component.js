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
import { PagoService, PagoEmitidoNGService } from './../../pago.service';
import { PagoComponent } from './../../pago.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { ModalService } from './../../../common/services/ModalService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { NavService } from './../../../common/services/NavService';
import { SecurityService } from './../../../common/services/SecurityService';
var PagoEmitidoNGSComponent = /** @class */ (function (_super) {
    __extends(PagoEmitidoNGSComponent, _super);
    function PagoEmitidoNGSComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "ReportePagosEmitidos.xls";
        return _this;
    }
    PagoEmitidoNGSComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("pago-ngs", "Emitidos");
    };
    PagoEmitidoNGSComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("CONSULTAR PAGOS NG");
    };
    PagoEmitidoNGSComponent.prototype.showModalTableResponsive = function (pago) {
        this.modalService.openModalTableResponsive("Pago", [
            { etiqueta: "Fecha de Pago", valor: pago.fechaPago },
            { etiqueta: "Número Pago", valor: pago.numeroPago },
            { etiqueta: "Vía de Pago", valor: pago.viaPago },
            { etiqueta: "Monto Comprobantes", valor: pago.totalMercaderiaString },
            { etiqueta: "Retenciones", valor: pago.retencionString },
            { etiqueta: "Neto Acreditado", valor: pago.montoString }
        ]);
        return false;
    };
    PagoEmitidoNGSComponent = __decorate([
        Component({
            selector: 'app-pago-no-granos-emitido',
            templateUrl: "pago.no-granos.emitido.component.html",
            providers: [{ provide: PagoService, useClass: PagoEmitidoNGService }]
        }),
        __metadata("design:paramtypes", [PagoEmitidoNGService,
            NavService,
            SessionDataService,
            SecurityService,
            FloatMsgService,
            ModalService])
    ], PagoEmitidoNGSComponent);
    return PagoEmitidoNGSComponent;
}(PagoComponent));
export { PagoEmitidoNGSComponent };
//# sourceMappingURL=pago.no-granos.emitido.component.js.map