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
var ContratoService = /** @class */ (function (_super) {
    __extends(ContratoService, _super);
    function ContratoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    ContratoService.prototype.getDetalle = function (numero_contrato) {
        var params = new http_1.URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/getDetalle', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.getDetalleFijacion = function (numero_contrato, fijacion) {
        var params = new http_1.URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/getDetalleFijacion', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.downloadBoletoFisico = function (numero_contrato) {
        var params = new http_1.URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadBoletoFisico', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.exportExcelDetalle = function (numero_contrato) {
        var params = new http_1.URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadDetalle', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.exportPDFCalidad = function (numero_contrato) {
        var params = new http_1.URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/exportPDFCalidad', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.exportExcelDetalleFijacion = function (numero_contrato, fijacion) {
        var params = new http_1.URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/downloadDetalleFijacion', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.getContratosCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.getContratosNoCumplidosCommon = function (periodo, fecha_inicio, fecha_fin, method, contratos) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers })
            .map(this.extractData);
    };
    ContratoService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    ContratoService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    ContratoService = __decorate([
        core_1.Injectable()
    ], ContratoService);
    return ContratoService;
}(BaseService_1.BaseService));
exports.ContratoService = ContratoService;
var ContratoVigenteService = /** @class */ (function (_super) {
    __extends(ContratoVigenteService, _super);
    function ContratoVigenteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoVigenteService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosCommon(periodo, fecha_inicio, fecha_fin, 'getVigentes');
    };
    ContratoVigenteService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadVigentes');
    };
    ContratoVigenteService = __decorate([
        core_1.Injectable()
    ], ContratoVigenteService);
    return ContratoVigenteService;
}(ContratoService));
exports.ContratoVigenteService = ContratoVigenteService;
var ContratoFijacionService = /** @class */ (function (_super) {
    __extends(ContratoFijacionService, _super);
    function ContratoFijacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoFijacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getFijaciones', new Array());
    };
    ContratoFijacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadFijaciones');
    };
    ContratoFijacionService = __decorate([
        core_1.Injectable()
    ], ContratoFijacionService);
    return ContratoFijacionService;
}(ContratoService));
exports.ContratoFijacionService = ContratoFijacionService;
var ContratoAnulacionService = /** @class */ (function (_super) {
    __extends(ContratoAnulacionService, _super);
    function ContratoAnulacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoAnulacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAnulaciones', new Array());
    };
    ContratoAnulacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAnulaciones');
    };
    ContratoAnulacionService = __decorate([
        core_1.Injectable()
    ], ContratoAnulacionService);
    return ContratoAnulacionService;
}(ContratoService));
exports.ContratoAnulacionService = ContratoAnulacionService;
var ContratoAmpliacionService = /** @class */ (function (_super) {
    __extends(ContratoAmpliacionService, _super);
    function ContratoAmpliacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ContratoAmpliacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAmpliaciones', new Array());
    };
    ContratoAmpliacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAmpliaciones');
    };
    ContratoAmpliacionService = __decorate([
        core_1.Injectable()
    ], ContratoAmpliacionService);
    return ContratoAmpliacionService;
}(ContratoService));
exports.ContratoAmpliacionService = ContratoAmpliacionService;
//# sourceMappingURL=contrato.service.js.map