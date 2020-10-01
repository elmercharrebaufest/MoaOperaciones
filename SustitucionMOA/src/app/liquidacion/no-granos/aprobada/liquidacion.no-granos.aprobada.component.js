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
import { LiquidacionService, LiquidacionNGAprobadaService } from './../../liquidacion.service';
import { LiquidacionNGBaseComponent } from './../liquidacion.no-granos.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';
var LiquidacionNGAprobadaComponent = /** @class */ (function (_super) {
    __extends(LiquidacionNGAprobadaComponent, _super);
    function LiquidacionNGAprobadaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "ReporteComprobantesAprobados.xls";
        return _this;
    }
    LiquidacionNGAprobadaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("comprobante-ngs", "Aprobados");
    };
    LiquidacionNGAprobadaComponent.prototype.showModal = function () { return false; };
    LiquidacionNGAprobadaComponent = __decorate([
        Component({
            selector: 'app-liquidacion-no-granos-aprobada',
            templateUrl: "liquidacion.no-granos.aprobada.component.html",
            providers: [{ provide: LiquidacionService, useClass: LiquidacionNGAprobadaService }]
        }),
        __metadata("design:paramtypes", [LiquidacionNGAprobadaService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], LiquidacionNGAprobadaComponent);
    return LiquidacionNGAprobadaComponent;
}(LiquidacionNGBaseComponent));
export { LiquidacionNGAprobadaComponent };
//# sourceMappingURL=liquidacion.no-granos.aprobada.component.js.map