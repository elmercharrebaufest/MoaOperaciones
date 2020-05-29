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
var PagoService = /** @class */ (function (_super) {
    __extends(PagoService, _super);
    function PagoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PagoService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    PagoService.prototype.getDetalle = function (numero_pago) {
        var params = new http_1.URLSearchParams();
        params.set('numeroPago', numero_pago);
        return this.http
            .get('/api/pago/getDetalle', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    PagoService.prototype.getPagosCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/pago/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    PagoService.prototype.getComprobantes = function (documento, fecha, fiscYear) {
        var params = new http_1.URLSearchParams();
        params.set('documento', documento);
        params.set('fecha', fecha);
        params.set('fiscalYear', fiscYear);
        return this.http
            .get('/api/pago/getComprobantes', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    PagoService.prototype.descargarDocumentoPDF = function (documento, ejercicio) {
        var params = new http_1.URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    PagoService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    PagoService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/pago/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    PagoService.prototype.exportExcelDetalle = function (numero_pago) {
        var params = new http_1.URLSearchParams();
        params.set('numeroPago', numero_pago);
        return this.http
            .get('/api/pago/downloadDetalle', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    PagoService = __decorate([
        core_1.Injectable()
    ], PagoService);
    return PagoService;
}(BaseService_1.BaseService));
exports.PagoService = PagoService;
var PagoEmitidoService = /** @class */ (function (_super) {
    __extends(PagoEmitidoService, _super);
    function PagoEmitidoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PagoEmitidoService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getPagosCommon(periodo, fecha_inicio, fecha_fin, 'getEmitidos');
    };
    PagoEmitidoService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadEmitidos');
    };
    PagoEmitidoService = __decorate([
        core_1.Injectable()
    ], PagoEmitidoService);
    return PagoEmitidoService;
}(PagoService));
exports.PagoEmitidoService = PagoEmitidoService;
var PagoEmitidoNGService = /** @class */ (function (_super) {
    __extends(PagoEmitidoNGService, _super);
    function PagoEmitidoNGService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PagoEmitidoNGService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getPagosCommon(periodo, fecha_inicio, fecha_fin, 'getEmitidosNG');
    };
    PagoEmitidoNGService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadEmitidosNG');
    };
    PagoEmitidoNGService = __decorate([
        core_1.Injectable()
    ], PagoEmitidoNGService);
    return PagoEmitidoNGService;
}(PagoService));
exports.PagoEmitidoNGService = PagoEmitidoNGService;
//# sourceMappingURL=pago.service.js.map