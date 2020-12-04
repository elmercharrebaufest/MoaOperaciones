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
import { LiquidacionService, LiquidacionObservadaService } from './../liquidacion.service';
import { LiquidacionBaseComponent } from './../liquidacion.component';
import { SessionDataService } from './../../common/services/SessionDataService';
//import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
//import { ListBaseComponent } from './../common/base-components/list-base-component'
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { ModalService } from './../../common/services/ModalService';
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
        Component({
            selector: 'app-liquidacion-observada',
            templateUrl: "liquidacion.observada.component.html",
            providers: [{ provide: LiquidacionService, useClass: LiquidacionObservadaService }]
        }),
        __metadata("design:paramtypes", [LiquidacionObservadaService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], LiquidacionObservadaComponent);
    return LiquidacionObservadaComponent;
}(LiquidacionBaseComponent));
export { LiquidacionObservadaComponent };
//# sourceMappingURL=liquidacion.observada.component.js.map