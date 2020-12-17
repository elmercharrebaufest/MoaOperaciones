var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
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
import { Router, ActivatedRoute } from '@angular/router';
import { CuentaCorrientePartidasAbiertasService } from './../cuenta-corriente.service';
import { CuentaCorrienteBaseComponent } from './../cuenta-corriente.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { Seccion } from '../../common/models/seccion';
var CuentaCorrientePartidasAbiertasComponent = /** @class */ (function (_super) {
    __extends(CuentaCorrientePartidasAbiertasComponent, _super);
    function CuentaCorrientePartidasAbiertasComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.tituloArchivo = "ReporteCuentasCorrientesAgrupadas.xls";
        _this.itemsEnPantalla = 5;
        _this.showMostrarMas = true;
        _this.filtroNroLegal = "";
        _this.granosSelected = sessionStorage.getItem("granosSelected");
        sessionDataService.granosSelected$.subscribe(function (granosSelected) {
            _this.granosSelected = granosSelected;
        });
        return _this;
    }
    CuentaCorrientePartidasAbiertasComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("cuenta-corriente", "Partidas Abiertas");
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.isVisible = function () {
        if (this.data) {
            if (this.isSinAgruparVisible() || this.isAgrupadasVisible) {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.isSinAgruparVisible = function () {
        return this.data.cuentasCorrientesSinAgrupar && this.data.cuentasCorrientesSinAgrupar.cuentasCorrientes.length != 0;
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.isAgrupadasVisible = function () {
        return this.data.cuentasCorrientesAgrupadas && this.data.cuentasCorrientesAgrupadas.length != 0;
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.verMas = function () {
        this.itemsEnPantalla = this.itemsEnPantalla + 5;
        if (this.data.cuentasCorrientesAgrupadas.length <= this.itemsEnPantalla) {
            this.showMostrarMas = false;
        }
        return false;
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.vaciarFiltros = function () {
        this.filtroNroLegal = "";
        this.itemsEnPantalla = 5;
        this.showMostrarMas = true;
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.isGranos = function () {
        return this.granosSelected == "G";
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.showModalTableAgrupadaResponsive = function (CuentaCorriente, agrupador) {
        this.modalService.openModalTableResponsive("Detalle de pagos", [
            { etiqueta: "Agrupador", valor: agrupador },
            { etiqueta: "F. Emisión", valor: CuentaCorriente.docDate },
            { etiqueta: "Descripción", valor: CuentaCorriente.descripcion },
            { etiqueta: "Nº Legal", valor: CuentaCorriente.xblnr },
            { etiqueta: "Contrato", valor: CuentaCorriente.contrato },
            { etiqueta: "Importe AR$", valor: CuentaCorriente.importeArgString }
        ]);
        return false;
    };
    CuentaCorrientePartidasAbiertasComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/cuenta-corriente/simple', 'cuenta-corriente', 'Cuenta Corriente'),
            new Seccion('/cuenta-corriente/agrupada', 'cuenta-corriente', 'Saldos'),
            new Seccion('/cuenta-corriente/partidas-abiertas', 'cuenta-corriente', 'Partidas Abiertas'),
        ]);
        this.getData();
    };
    CuentaCorrientePartidasAbiertasComponent = __decorate([
        Component({
            selector: 'app-cuenta-corriente-partidas-abiertas',
            templateUrl: "cuenta-corriente.partidas-abiertas.component.html",
            providers: [CuentaCorrientePartidasAbiertasService]
        }),
        __metadata("design:paramtypes", [CuentaCorrientePartidasAbiertasService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService, ActivatedRoute, Router])
    ], CuentaCorrientePartidasAbiertasComponent);
    return CuentaCorrientePartidasAbiertasComponent;
}(CuentaCorrienteBaseComponent));
export { CuentaCorrientePartidasAbiertasComponent };
//# sourceMappingURL=cuenta-corriente.partidas-abiertas.component.js.map