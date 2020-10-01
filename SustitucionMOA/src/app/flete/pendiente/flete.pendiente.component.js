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
import { FleteBaseComponent } from './../flete.component';
import { FleteService, FletePendienteService } from './../flete.service';
var FletePendienteComponent = /** @class */ (function (_super) {
    __extends(FletePendienteComponent, _super);
    function FletePendienteComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.tituloArchivo = "FletesPendientes.xls";
        return _this;
    }
    FletePendienteComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("flete", "Viajes Pendientes");
    };
    FletePendienteComponent.prototype.showModalTableResponsive = function (Viaje) {
        this.modalService.openModalTableResponsive("Viaje Pendiente", [
            { etiqueta: "CCPP", valor: Viaje.ccpp },
            { etiqueta: "Fecha", valor: Viaje.fechaCCPP },
            { etiqueta: "Patente", valor: Viaje.patente },
            { etiqueta: "KG CCPP", valor: Viaje.kgString },
            { etiqueta: "Descripción", valor: Viaje.descMat },
            { etiqueta: "Origen", valor: Viaje.origen },
            { etiqueta: "Destino", valor: Viaje.destino },
            { etiqueta: "Peaje", valor: Viaje.peajeString },
            { etiqueta: "Playas", valor: Viaje.playaString }
        ]);
        return false;
    };
    FletePendienteComponent = __decorate([
        Component({
            selector: 'app-flete-pendiente',
            templateUrl: "flete.pendiente.component.html",
            providers: [{ provide: FleteService, useClass: FletePendienteService }]
        })
    ], FletePendienteComponent);
    return FletePendienteComponent;
}(FleteBaseComponent));
export { FletePendienteComponent };
//# sourceMappingURL=flete.pendiente.component.js.map