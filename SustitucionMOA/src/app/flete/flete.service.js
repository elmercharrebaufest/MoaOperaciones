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
import { map, timeoutWith } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
var FleteService = /** @class */ (function (_super) {
    __extends(FleteService, _super);
    function FleteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    FleteService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) { return null; };
    FleteService.prototype.getDataCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/flete/' + method, { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    FleteService.prototype.exportPDF = function (periodo, fecha_inicio, fecha_fin, proforma) { return null; };
    FleteService.prototype.exportPDFCommon = function (periodo, fecha_inicio, fecha_fin, proforma, method) {
        var params = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('proforma', proforma);
        return this.http
            .get('/api/flete/' + method, { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    FleteService.prototype.validarImporte = function (importe, proforma) { return null; };
    FleteService.prototype.guardarDatosProforma = function (data, file) {
        var payload = new FormData();
        payload.append("proforma", JSON.stringify(data));
        payload.append("file", file);
        return this.http
            .post('/api/flete/guardarDatosProforma', payload, this.headersPost).pipe(map(this.extractData));
    };
    FleteService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        return null;
    };
    FleteService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/flete/' + method, { search: params, headers: this.headers }).pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))), map(this.extractData));
    };
    FleteService = __decorate([
        Injectable()
    ], FleteService);
    return FleteService;
}(BaseService));
export { FleteService };
var FletePendienteService = /** @class */ (function (_super) {
    __extends(FletePendienteService, _super);
    function FletePendienteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    FletePendienteService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getDataCommon(periodo, fecha_inicio, fecha_fin, "getViajesPendientes");
    };
    FletePendienteService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, "downloadViajesPendientes");
    };
    FletePendienteService = __decorate([
        Injectable()
    ], FletePendienteService);
    return FletePendienteService;
}(FleteService));
export { FletePendienteService };
var FleteAFacturarService = /** @class */ (function (_super) {
    __extends(FleteAFacturarService, _super);
    function FleteAFacturarService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    FleteAFacturarService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getDataCommon(periodo, fecha_inicio, fecha_fin, "getViajesAFacturar");
    };
    FleteAFacturarService.prototype.exportPDF = function (periodo, fecha_inicio, fecha_fin, proforma) {
        return this.exportPDFCommon(periodo, fecha_inicio, fecha_fin, proforma, "exportarPDFAFacturar");
    };
    FleteAFacturarService.prototype.validarImporte = function (importe, proforma) {
        var params = new URLSearchParams();
        params.set('importe', importe);
        params.set('proforma', proforma);
        return this.http
            .get('/api/flete/validarImporte', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    FleteAFacturarService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, "downloadViajesAFacturar");
    };
    FleteAFacturarService = __decorate([
        Injectable()
    ], FleteAFacturarService);
    return FleteAFacturarService;
}(FleteService));
export { FleteAFacturarService };
var FleteFacturadoService = /** @class */ (function (_super) {
    __extends(FleteFacturadoService, _super);
    function FleteFacturadoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    FleteFacturadoService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getDataCommon(periodo, fecha_inicio, fecha_fin, "getViajesFacturados");
    };
    FleteFacturadoService.prototype.exportPDF = function (periodo, fecha_inicio, fecha_fin, proforma) {
        return this.exportPDFCommon(periodo, fecha_inicio, fecha_fin, proforma, "exportarPDFFacturado");
    };
    FleteFacturadoService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, "downloadViajesFacturados");
    };
    FleteFacturadoService = __decorate([
        Injectable()
    ], FleteFacturadoService);
    return FleteFacturadoService;
}(FleteService));
export { FleteFacturadoService };
//# sourceMappingURL=flete.service.js.map