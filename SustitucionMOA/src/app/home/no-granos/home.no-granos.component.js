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
import { HomeService, HomeNGService } from './../home.service';
import { HomeComponent } from './../home.component';
var HomeNGSComponent = /** @class */ (function (_super) {
    __extends(HomeNGSComponent, _super);
    function HomeNGSComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    HomeNGSComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("home-ngs", "");
        this.sessionDataService.setGranosSelected("N");
        sessionStorage.setItem("granosSelected", "N");
    };
    HomeNGSComponent.prototype.ngOnInit = function () {
        if (this.filtroFechaComponent.getPeriodo() == "1") {
            this.filtroFechaComponent.setPeriodoInitial("2");
        }
        _super.prototype.ngOnInit.call(this);
    };
    HomeNGSComponent.prototype.checkPermisos = function () {
        if (this.securityService.esNoGranosRedirect()) {
            this.securityService.tienePermisoRedirect("CONSULTAR HOME NG");
        }
    };
    HomeNGSComponent.prototype.showModalTableResponsive = function (cuentaCorriente) {
        this.modalService.openModalTableResponsive("Movimientos", [
            { etiqueta: "F. Emisión", valor: cuentaCorriente.docDate },
            { etiqueta: "F. Vto.", valor: cuentaCorriente.fecVto },
            { etiqueta: "Nº Cte.", valor: cuentaCorriente.docNo },
            { etiqueta: "Descripción", valor: cuentaCorriente.descripcion },
            { etiqueta: "Contrato", valor: cuentaCorriente.contrato },
            { etiqueta: "Moneda", valor: cuentaCorriente.moneda },
            { etiqueta: "TC", valor: cuentaCorriente.ukursString },
            { etiqueta: "Debe", valor: cuentaCorriente.debeString },
            { etiqueta: "Haber", valor: cuentaCorriente.haberString }
        ]);
        return false;
    };
    HomeNGSComponent = __decorate([
        Component({
            selector: 'app-home-no-granos',
            //template: '<h1>{{titulo}}</h1>'
            templateUrl: "home.no-granos.component.html",
            providers: [{ provide: HomeService, useClass: HomeNGService }]
        })
    ], HomeNGSComponent);
    return HomeNGSComponent;
}(HomeComponent));
export { HomeNGSComponent };
//# sourceMappingURL=home.no-granos.component.js.map