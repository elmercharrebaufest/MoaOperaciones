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
var router_1 = require("@angular/router");
var cuenta_corriente_service_1 = require("./cuenta-corriente.service");
var list_base_component_1 = require("./../common/base-components/list-base-component");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var SecurityService_1 = require("./../common/services/SecurityService");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var Seccion_1 = require("./../common/models/Seccion");
var ModalService_1 = require("./../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var CuentaCorrienteBaseComponent = /** @class */ (function (_super) {
    __extends(CuentaCorrienteBaseComponent, _super);
    function CuentaCorrienteBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService, route, router) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.route = route;
        _this.router = router;
        _this.tituloArchivo = "ReporteCuentasCorrientes.xls";
        _this.tituloArchivoPDF = "Documento";
        _this.filtroNroCteContrato = "";
        _this.filtroNroLegal = "";
        _this.contrato = "";
        _this.pago = "";
        _this.retencion = "";
        return _this;
    }
    CuentaCorrienteBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR CUENTA CORRIENTE"); };
    CuentaCorrienteBaseComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("cuenta-corriente", "Movimientos");
    };
    CuentaCorrienteBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion_1.Seccion('/cuenta-corriente/agrupada', 'cuenta-corriente', 'Cuenta Corriente'), new Seccion_1.Seccion('/cuenta-corriente/simple', 'cuenta-corriente', 'Movimientos'),]);
        this.orderedByColumn = "orden";
        this.orderDirection = 1;
        this.getData();
    };
    CuentaCorrienteBaseComponent.prototype.getData = function () {
        var _this = this;
        this.data = null;
        this.vaciarFiltros();
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            _this.getInfoParam(params['id']);
            _this.unsubscribe();
            _this.subscription = _this.service.getData(_this.filtroFechaComponent.periodo, _this.filtroFechaComponent.fecha_inicio, _this.filtroFechaComponent.fecha_fin, _this.contrato, _this.pago, _this.retencion).subscribe(function (result) {
                _this.data = null;
                _this.mensajeComponent.setMsgsEmpty();
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.data = result.data;
                    _this.cargarFiltrosVariables(result);
                    if (result.data.msj != undefined) {
                        _this.mensajeComponent.setInfoMsg(result.data.msj);
                    }
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        });
        return false;
    };
    CuentaCorrienteBaseComponent.prototype.descargaPDF = function (documento, ejercicio) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(function (result) {
            _this.spinnerComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.floatMsgService.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.floatMsgService.setInfoMsg(result.info);
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoPDF + documento + ".pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoPDF + documento + ".pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    CuentaCorrienteBaseComponent.prototype.isVisible = function () {
        if (this.data && this.data.cuentasCorrientes.length != 0)
            return true;
        else
            return false;
    };
    CuentaCorrienteBaseComponent.prototype.vaciarFiltros = function () {
        this.filtroNroCteContrato = "";
        this.filtroNroLegal = "";
        this.orderedByColumn = "orden";
        this.orderDirection = 1;
    };
    CuentaCorrienteBaseComponent.prototype.showModalTableResponsive = function (cuentaCorriente) {
        this.modalService.openModalTableResponsive("Detalle de Movimiento", [
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
    CuentaCorrienteBaseComponent.prototype.getInfoParam = function (value) {
        if (value != null && value != undefined && value != "") {
            if (value.length == 2) {
                this.retencion = value;
            }
            else {
                try {
                    var valueInt = Number(value);
                    var valueString = valueInt.toString();
                    var cantCeros = 4 - valueString.length;
                    var pagoFormat = "";
                    for (var i = 0; i < cantCeros; i++) {
                        pagoFormat += "0";
                    }
                    pagoFormat += valueString;
                    this.pago = pagoFormat;
                }
                catch (_a) { }
            }
        }
    };
    CuentaCorrienteBaseComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/cuenta-corriente/cuenta-corriente.component.html?v=" + new Date().getTime(),
            providers: [cuenta_corriente_service_1.CuentaCorrienteService]
        }),
        __metadata("design:paramtypes", [cuenta_corriente_service_1.CuentaCorrienteService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService, router_1.ActivatedRoute, router_1.Router])
    ], CuentaCorrienteBaseComponent);
    return CuentaCorrienteBaseComponent;
}(list_base_component_1.ListBaseComponent));
exports.CuentaCorrienteBaseComponent = CuentaCorrienteBaseComponent;
//# sourceMappingURL=cuenta-corriente.component.js.map