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
import { throwError as observableThrowError } from 'rxjs';
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
var LiquidacionService = /** @class */ (function (_super) {
    __extends(LiquidacionService, _super);
    function LiquidacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    LiquidacionService.prototype.getLiquidacionesCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/liquidacion/' + method, { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))))
            .pipe(map(this.extractData));
    };
    LiquidacionService.prototype.getVinculacion = function (contrato, secuencia) {
        var params = new URLSearchParams();
        params.set('contrato', contrato);
        params.set('secuencia', secuencia);
        return this.http
            .get('/api/liquidacion/getVinculacion', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    LiquidacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    LiquidacionService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/liquidacion/' + method, { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))))
            .pipe(map(this.extractData));
    };
    LiquidacionService.prototype.descargarDocumentoPDF = function (documento, ejercicio) {
        var params = new URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    LiquidacionService = __decorate([
        Injectable()
    ], LiquidacionService);
    return LiquidacionService;
}(BaseService));
export { LiquidacionService };
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
        Injectable()
    ], LiquidacionAprobadaService);
    return LiquidacionAprobadaService;
}(LiquidacionService));
export { LiquidacionAprobadaService };
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
        Injectable()
    ], LiquidacionObservadaService);
    return LiquidacionObservadaService;
}(LiquidacionService));
export { LiquidacionObservadaService };
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
        Injectable()
    ], LiquidacionPagaService);
    return LiquidacionPagaService;
}(LiquidacionService));
export { LiquidacionPagaService };
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
        Injectable()
    ], LiquidacionNGAprobadaService);
    return LiquidacionNGAprobadaService;
}(LiquidacionService));
export { LiquidacionNGAprobadaService };
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
        Injectable()
    ], LiquidacionNGObservadaService);
    return LiquidacionNGObservadaService;
}(LiquidacionService));
export { LiquidacionNGObservadaService };
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
        Injectable()
    ], LiquidacionNGPagaService);
    return LiquidacionNGPagaService;
}(LiquidacionService));
export { LiquidacionNGPagaService };
var LiquidacionProformaService = /** @class */ (function (_super) {
    __extends(LiquidacionProformaService, _super);
    function LiquidacionProformaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    LiquidacionProformaService.prototype.getDataProforma = function (fijacion) {
        var params = new URLSearchParams();
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/liquidacion/getProforma', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    LiquidacionProformaService.prototype.exportExcelProforma = function (fijacion) {
        var params = new URLSearchParams();
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/liquidacion/descargarProforma', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    LiquidacionProformaService.prototype.getFleteProcedencia = function (contrato) {
        var params = new URLSearchParams();
        params.set('contrato', contrato);
        return this.http
            .get('/api/liquidacion/getFleteProcedencia', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    LiquidacionProformaService = __decorate([
        Injectable()
    ], LiquidacionProformaService);
    return LiquidacionProformaService;
}(LiquidacionService));
export { LiquidacionProformaService };
//# sourceMappingURL=liquidacion.service.js.map