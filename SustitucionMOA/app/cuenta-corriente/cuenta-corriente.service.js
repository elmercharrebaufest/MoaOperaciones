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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
var Observable_1 = require("rxjs/Observable");
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var CuentaCorrienteService = /** @class */ (function (_super) {
    __extends(CuentaCorrienteService, _super);
    function CuentaCorrienteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CuentaCorrienteService.prototype.getData = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientes', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CuentaCorrienteService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientes', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CuentaCorrienteService.prototype.descargarDocumentoPDF = function (documento, ejercicio) {
        var params = new http_1.URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    CuentaCorrienteService = __decorate([
        core_1.Injectable()
    ], CuentaCorrienteService);
    return CuentaCorrienteService;
}(BaseService_1.BaseService));
exports.CuentaCorrienteService = CuentaCorrienteService;
var CuentaCorrienteAgrupadaService = /** @class */ (function (_super) {
    __extends(CuentaCorrienteAgrupadaService, _super);
    function CuentaCorrienteAgrupadaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CuentaCorrienteAgrupadaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientesAgrupadas', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CuentaCorrienteAgrupadaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientesAgrupadas', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CuentaCorrienteAgrupadaService = __decorate([
        core_1.Injectable()
    ], CuentaCorrienteAgrupadaService);
    return CuentaCorrienteAgrupadaService;
}(CuentaCorrienteService));
exports.CuentaCorrienteAgrupadaService = CuentaCorrienteAgrupadaService;
//# sourceMappingURL=cuenta-corriente.service.js.map