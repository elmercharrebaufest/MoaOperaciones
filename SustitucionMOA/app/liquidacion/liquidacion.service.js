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
var http_1 = require("@angular/http");
var Observable_1 = require("rxjs/Observable");
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var LiquidacionService = /** @class */ (function (_super) {
    __extends(LiquidacionService, _super);
    function LiquidacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    LiquidacionService.prototype.getLiquidacionesCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/liquidacion/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    LiquidacionService.prototype.getVinculacion = function (contrato, secuencia) {
        var params = new http_1.URLSearchParams();
        params.set('contrato', contrato);
        params.set('secuencia', secuencia);
        return this.http
            .get('/api/liquidacion/getVinculacion', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LiquidacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    LiquidacionService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/liquidacion/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    LiquidacionService.prototype.descargarDocumentoPDF = function (documento, ejercicio) {
        var params = new http_1.URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LiquidacionService = __decorate([
        core_1.Injectable()
    ], LiquidacionService);
    return LiquidacionService;
}(BaseService_1.BaseService));
exports.LiquidacionService = LiquidacionService;
var LiquidacionAprobadaService = /** @class */ (function (_super) {
    __extends(LiquidacionAprobadaService, _super);
    function LiquidacionAprobadaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionAprobadaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getAprobadas');
    };
    LiquidacionAprobadaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAprobadas');
    };
    LiquidacionAprobadaService = __decorate([
        core_1.Injectable()
    ], LiquidacionAprobadaService);
    return LiquidacionAprobadaService;
}(LiquidacionService));
exports.LiquidacionAprobadaService = LiquidacionAprobadaService;
var LiquidacionObservadaService = /** @class */ (function (_super) {
    __extends(LiquidacionObservadaService, _super);
    function LiquidacionObservadaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionObservadaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getObservadas');
    };
    LiquidacionObservadaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadObservadas');
    };
    LiquidacionObservadaService = __decorate([
        core_1.Injectable()
    ], LiquidacionObservadaService);
    return LiquidacionObservadaService;
}(LiquidacionService));
exports.LiquidacionObservadaService = LiquidacionObservadaService;
var LiquidacionPagaService = /** @class */ (function (_super) {
    __extends(LiquidacionPagaService, _super);
    function LiquidacionPagaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionPagaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getPagas');
    };
    LiquidacionPagaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadPagas');
    };
    LiquidacionPagaService = __decorate([
        core_1.Injectable()
    ], LiquidacionPagaService);
    return LiquidacionPagaService;
}(LiquidacionService));
exports.LiquidacionPagaService = LiquidacionPagaService;
var LiquidacionNGAprobadaService = /** @class */ (function (_super) {
    __extends(LiquidacionNGAprobadaService, _super);
    function LiquidacionNGAprobadaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionNGAprobadaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getAprobadasNG');
    };
    LiquidacionNGAprobadaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAprobadasNG');
    };
    LiquidacionNGAprobadaService = __decorate([
        core_1.Injectable()
    ], LiquidacionNGAprobadaService);
    return LiquidacionNGAprobadaService;
}(LiquidacionService));
exports.LiquidacionNGAprobadaService = LiquidacionNGAprobadaService;
var LiquidacionNGObservadaService = /** @class */ (function (_super) {
    __extends(LiquidacionNGObservadaService, _super);
    function LiquidacionNGObservadaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionNGObservadaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getObservadasNG');
    };
    LiquidacionNGObservadaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadObservadasNG');
    };
    LiquidacionNGObservadaService = __decorate([
        core_1.Injectable()
    ], LiquidacionNGObservadaService);
    return LiquidacionNGObservadaService;
}(LiquidacionService));
exports.LiquidacionNGObservadaService = LiquidacionNGObservadaService;
var LiquidacionNGPagaService = /** @class */ (function (_super) {
    __extends(LiquidacionNGPagaService, _super);
    function LiquidacionNGPagaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionNGPagaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getPagasNG');
    };
    LiquidacionNGPagaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadPagasNG');
    };
    LiquidacionNGPagaService = __decorate([
        core_1.Injectable()
    ], LiquidacionNGPagaService);
    return LiquidacionNGPagaService;
}(LiquidacionService));
exports.LiquidacionNGPagaService = LiquidacionNGPagaService;
var LiquidacionProformaService = /** @class */ (function (_super) {
    __extends(LiquidacionProformaService, _super);
    function LiquidacionProformaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionProformaService.prototype.getDataProforma = function (fijacion) {
        var params = new http_1.URLSearchParams();
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/liquidacion/getProforma', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LiquidacionProformaService.prototype.exportExcelProforma = function (fijacion) {
        var params = new http_1.URLSearchParams();
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/liquidacion/descargarProforma', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LiquidacionProformaService.prototype.getFleteProcedencia = function (contrato) {
        var params = new http_1.URLSearchParams();
        params.set('contrato', contrato);
        return this.http
            .get('/api/liquidacion/getFleteProcedencia', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LiquidacionProformaService = __decorate([
        core_1.Injectable()
    ], LiquidacionProformaService);
    return LiquidacionProformaService;
}(LiquidacionService));
exports.LiquidacionProformaService = LiquidacionProformaService;
//# sourceMappingURL=liquidacion.service.js.map