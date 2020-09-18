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
import { ContratoBaseComponent } from './../contrato.component';
import { ContratoService, ContratoVigenteService } from './../contrato.service';
var ContratoVigenteComponent = /** @class */ (function (_super) {
    __extends(ContratoVigenteComponent, _super);
    function ContratoVigenteComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "ReporteContratoVigentes.xls";
        return _this;
    }
    ContratoVigenteComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Vigentes");
    };
    ContratoVigenteComponent.prototype.showModalTableResponsive = function (Contrato) {
        this.modalService.openModalTableResponsive("Contrato Vigente", [
            { etiqueta: "Último Movimiento", valor: Contrato.fecha },
            { etiqueta: "Contrato Molinos", valor: Contrato.nroContrato },
            { etiqueta: "Contrato Proveedor", valor: Contrato.contrvend },
            { etiqueta: "Estado Boleto", valor: Contrato.estado },
            { etiqueta: "Producto", valor: Contrato.material },
            { etiqueta: "Pactado", valor: Contrato.cantKilosString },
            { etiqueta: "Entregado", valor: Contrato.aplicacionesString },
            { etiqueta: "Liquidado", valor: Contrato.liquidadoString }
        ]);
        return false;
    };
    ContratoVigenteComponent = __decorate([
        Component({
            selector: 'app-contrato-vigente',
            templateUrl: "contrato.vigente.component.html",
            providers: [{ provide: ContratoService, useClass: ContratoVigenteService }]
        })
    ], ContratoVigenteComponent);
    return ContratoVigenteComponent;
}(ContratoBaseComponent));
export { ContratoVigenteComponent };
//# sourceMappingURL=contrato.vigente.component.js.map