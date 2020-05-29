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
var liquidacion_service_1 = require("./../liquidacion.service");
var liquidacion_component_1 = require("./../liquidacion.component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
//import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
//import { ListBaseComponent } from './../common/base-components/list-base-component'
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var LiquidacionObservadaComponent = /** @class */ (function (_super) {
    __extends(LiquidacionObservadaComponent, _super);
    function LiquidacionObservadaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "ReporteLiquidacionesObservadas.xls";
        return _this;
    }
    LiquidacionObservadaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("liquidacion", "Observadas");
    };
    LiquidacionObservadaComponent.prototype.acortar = function (value) {
        return value.slice(0, 8);
    };
    LiquidacionObservadaComponent.prototype.showModalTableResponsive = function (liquidacion) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Vencimiento", valor: liquidacion.emitido },
            { etiqueta: "Tipo", valor: liquidacion.tipo },
            { etiqueta: "Comprobante", valor: liquidacion.comprobante },
            { etiqueta: "Producto", valor: liquidacion.producto },
            { etiqueta: "Liquidacion", valor: liquidacion.liquidadoString },
            { etiqueta: "Total", valor: liquidacion.importeString },
            { etiqueta: "Contrato", valor: liquidacion.contrato },
            { etiqueta: "Falta", valor: liquidacion.observaciones }
        ]);
        return false;
    };
    LiquidacionObservadaComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/liquidacion/observada/liquidacion.observada.component.html?v=" + new Date().getTime(),
            providers: [{ provide: liquidacion_service_1.LiquidacionService, useClass: liquidacion_service_1.LiquidacionObservadaService }]
        }),
        __metadata("design:paramtypes", [liquidacion_service_1.LiquidacionObservadaService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], LiquidacionObservadaComponent);
    return LiquidacionObservadaComponent;
}(liquidacion_component_1.LiquidacionBaseComponent));
exports.LiquidacionObservadaComponent = LiquidacionObservadaComponent;
//# sourceMappingURL=liquidacion.observada.component.js.map