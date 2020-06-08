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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var contrato_component_1 = require("./../contrato.component");
var contrato_service_1 = require("./../contrato.service");
var ContratoAmpliacionComponent = /** @class */ (function (_super) {
    __extends(ContratoAmpliacionComponent, _super);
    function ContratoAmpliacionComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "ReporteAnulaciones.xls";
        return _this;
    }
    ContratoAmpliacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Ampliaciones");
    };
    ContratoAmpliacionComponent.prototype.showModalTableResponsive = function (contratoInfo) {
        this.modalService.openModalTableResponsive("Contrato", [
            { etiqueta: "Fecha de Anulación", valor: contratoInfo.fecha },
            { etiqueta: "Contrato Molinos", valor: contratoInfo.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: contratoInfo.contrvend },
            { etiqueta: "Producto", valor: contratoInfo.material },
            { etiqueta: "Pactado", valor: contratoInfo.cantKilosString },
            { etiqueta: "Ampliado", valor: contratoInfo.ampliadoString },
            { etiqueta: "Total", valor: contratoInfo.totalString },
            { etiqueta: "Vendedor", valor: contratoInfo.vendedor }
        ]);
        return false;
    };
    ContratoAmpliacionComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/contrato/ampliacion/contrato.ampliacion.component.html?v=" + new Date().getTime(),
            providers: [{ provide: contrato_service_1.ContratoService, useClass: contrato_service_1.ContratoAmpliacionService }]
        })
    ], ContratoAmpliacionComponent);
    return ContratoAmpliacionComponent;
}(contrato_component_1.ContratoBaseComponent));
exports.ContratoAmpliacionComponent = ContratoAmpliacionComponent;
//# sourceMappingURL=contrato.ampliacion.component.js.map