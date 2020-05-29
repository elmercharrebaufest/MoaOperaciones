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
var pago_service_1 = require("./../../pago.service");
var pago_component_1 = require("./../../pago.component");
var SessionDataService_1 = require("./../../../common/services/SessionDataService");
var ModalService_1 = require("./../../../common/services/ModalService");
var FloatMsgService_1 = require("./../../../common/services/FloatMsgService");
var NavService_1 = require("./../../../common/services/NavService");
var SecurityService_1 = require("./../../../common/services/SecurityService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
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
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/pago/no-granos/emitido/pago.no-granos.emitido.component.html?v=" + new Date().getTime(),
            providers: [{ provide: pago_service_1.PagoService, useClass: pago_service_1.PagoEmitidoNGService }]
        }),
        __metadata("design:paramtypes", [pago_service_1.PagoEmitidoNGService,
            NavService_1.NavService,
            SessionDataService_1.SessionDataService,
            SecurityService_1.SecurityService,
            FloatMsgService_1.FloatMsgService,
            ModalService_1.ModalService])
    ], PagoEmitidoNGSComponent);
    return PagoEmitidoNGSComponent;
}(pago_component_1.PagoComponent));
exports.PagoEmitidoNGSComponent = PagoEmitidoNGSComponent;
//# sourceMappingURL=pago.no-granos.emitido.component.js.map