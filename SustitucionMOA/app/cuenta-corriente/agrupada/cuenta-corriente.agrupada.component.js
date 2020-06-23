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
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var cuenta_corriente_service_1 = require("./../cuenta-corriente.service");
var cuenta_corriente_component_1 = require("./../cuenta-corriente.component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var CuentaCorrienteAgrupadaComponent = /** @class */ (function (_super) {
    __extends(CuentaCorrienteAgrupadaComponent, _super);
    function CuentaCorrienteAgrupadaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
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
    CuentaCorrienteAgrupadaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("cuenta-corriente", "Cuenta Corriente");
    };
    CuentaCorrienteAgrupadaComponent.prototype.isVisible = function () {
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
    CuentaCorrienteAgrupadaComponent.prototype.isSinAgruparVisible = function () {
        return this.data.cuentasCorrientesSinAgrupar && this.data.cuentasCorrientesSinAgrupar.cuentasCorrientes.length != 0;
    };
    CuentaCorrienteAgrupadaComponent.prototype.isAgrupadasVisible = function () {
        return this.data.cuentasCorrientesAgrupadas && this.data.cuentasCorrientesAgrupadas.length != 0;
    };
    CuentaCorrienteAgrupadaComponent.prototype.verMas = function () {
        this.itemsEnPantalla = this.itemsEnPantalla + 5;
        if (this.data.cuentasCorrientesAgrupadas.length <= this.itemsEnPantalla) {
            this.showMostrarMas = false;
        }
        return false;
    };
    CuentaCorrienteAgrupadaComponent.prototype.vaciarFiltros = function () {
        this.filtroNroLegal = "";
        this.itemsEnPantalla = 5;
        this.showMostrarMas = true;
    };
    CuentaCorrienteAgrupadaComponent.prototype.isGranos = function () {
        return this.granosSelected == "G";
    };
    CuentaCorrienteAgrupadaComponent.prototype.showModalTableAgrupadaResponsive = function (CuentaCorriente, agrupador) {
        this.modalService.openModalTableResponsive("Cuenta Corriente", [
            { etiqueta: "Agrupador", valor: agrupador },
            { etiqueta: "F. Emisión", valor: CuentaCorriente.docDate },
            { etiqueta: "Descripción", valor: CuentaCorriente.descripcion },
            { etiqueta: "Nº Legal", valor: CuentaCorriente.xblnr },
            { etiqueta: "Contrato", valor: CuentaCorriente.contrato },
            { etiqueta: "Importe AR$", valor: CuentaCorriente.importeArgString }
        ]);
        return false;
    };
    CuentaCorrienteAgrupadaComponent = __decorate([
        core_1.Component({
            selector: 'app-cuenta-corriente-agrupada',
            templateUrl: "./app/cuenta-corriente/agrupada/cuenta-corriente.agrupada.component.html?v=" + new Date().getTime(),
            providers: [cuenta_corriente_service_1.CuentaCorrienteAgrupadaService]
        }),
        __metadata("design:paramtypes", [cuenta_corriente_service_1.CuentaCorrienteAgrupadaService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService, router_1.ActivatedRoute, router_1.Router])
    ], CuentaCorrienteAgrupadaComponent);
    return CuentaCorrienteAgrupadaComponent;
}(cuenta_corriente_component_1.CuentaCorrienteBaseComponent));
exports.CuentaCorrienteAgrupadaComponent = CuentaCorrienteAgrupadaComponent;
//# sourceMappingURL=cuenta-corriente.agrupada.component.js.map