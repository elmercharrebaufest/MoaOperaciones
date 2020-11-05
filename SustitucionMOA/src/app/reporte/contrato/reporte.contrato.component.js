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
import { Component } from '@angular/core';
import { ReporteBaseComponent } from './../reporte.component';
import { ReporteService, ReporteContratoService } from './../reporte.service';
var ReporteContratoComponent = /** @class */ (function (_super) {
    __extends(ReporteContratoComponent, _super);
    function ReporteContratoComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ReporteContratoComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("reporte", "Contratos");
    };
    ReporteContratoComponent = __decorate([
        Component({
            selector: 'app-reporte-contrato',
            templateUrl: "reporte.contrato.component.html",
            providers: [{ provide: ReporteService, useClass: ReporteContratoService }]
        })
    ], ReporteContratoComponent);
    return ReporteContratoComponent;
}(ReporteBaseComponent));
export { ReporteContratoComponent };
//# sourceMappingURL=reporte.contrato.component.js.map