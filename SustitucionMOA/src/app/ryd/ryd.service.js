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
import "rxjs/add/observable/defer";
import { BaseService } from './../common/services/BaseService';
var RYDService = /** @class */ (function (_super) {
    __extends(RYDService, _super);
    function RYDService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDService.prototype.getInputsCargaPesadas = function () {
        return this.http
            .get('/api/ryd/getDataInputsCargaPesadas', { headers: this.headers }).pipe(timeoutWith(1200000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDService.prototype.getFiltros = function (method) {
        return this.http
            .get('/api/ryd/getFiltros' + method, { headers: this.headers }).pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDService.prototype.postRegistrarPesadas = function (balanza, fecha, bodega, commodity, destino, exportador, vapor, pesoProgramado, pesoAcumulado, numeroPesada, fechaPesada, pesoTara, pesoBruto) {
        var params = new URLSearchParams();
        params.set('balanza', balanza);
        params.set('fecha', fecha);
        params.set('bodega', bodega);
        params.set('commodity', commodity);
        params.set('destino', destino);
        params.set('exportador', exportador);
        params.set('vapor', vapor);
        params.set('pesoProgramado', pesoProgramado);
        params.set('pesoAcumulado', pesoAcumulado);
        params.set('numeroPesada', numeroPesada);
        params.set('fechaPesada', fechaPesada);
        params.set('pesoTara', pesoTara);
        params.set('pesoBruto', pesoBruto);
        return this.http
            .post('/api/ryd/registrarPesada', params, this.headersPost).pipe(timeoutWith(1200000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDService.prototype.postFinalizarCargaPesadas = function (balanza, fecha) {
        var params = new URLSearchParams();
        params.set('balanza', balanza);
        params.set('fecha', fecha);
        return this.http
            .post('/api/ryd/finalizarCargaPesadas', params, this.headersPost).pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDService.prototype.verificarBalanzaEnProceso = function (balanza) {
        var params = new URLSearchParams();
        params.set('balanza', balanza);
        return this.http
            .post('/api/ryd/verificarBalanza', params, this.headersPost).pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDService = __decorate([
        Injectable()
    ], RYDService);
    return RYDService;
}(BaseService));
export { RYDService };
var RYDInformeService = /** @class */ (function (_super) {
    __extends(RYDInformeService, _super);
    function RYDInformeService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDInformeService.prototype.getFiltros = function () {
        return _super.prototype.getFiltros.call(this, "Informe");
    };
    RYDInformeService.prototype.getInforme = function (balanza) {
        var params = new URLSearchParams();
        params.set('balanza', balanza);
        return this.http
            .get('/api/ryd/getInforme', { search: params, headers: this.headers }).pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDInformeService.prototype.exportExcel = function (balanza) {
        var params = new URLSearchParams();
        params.set('balanza', balanza);
        return this.http
            .get('/api/ryd/downloadInforme', { search: params, headers: this.headers }).pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    RYDInformeService = __decorate([
        Injectable()
    ], RYDInformeService);
    return RYDInformeService;
}(RYDService));
export { RYDInformeService };
var RYDListadoPesadasService = /** @class */ (function (_super) {
    __extends(RYDListadoPesadasService, _super);
    function RYDListadoPesadasService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDListadoPesadasService.prototype.getListado = function (commodity, exportador, fechaInicio, fechaFin) {
        var params = new URLSearchParams();
        params.set('commodity', commodity);
        params.set('exportador', exportador);
        params.set('fechaInicio', fechaInicio);
        params.set('fechaFin', fechaFin);
        return this.http
            .get('/api/ryd/getListadoPesadas', { search: params, headers: this.headers }).pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))), map(this.extractData));
    };
    RYDListadoPesadasService.prototype.getFiltros = function () {
        return _super.prototype.getFiltros.call(this, "ListadoPesadas");
    };
    RYDListadoPesadasService.prototype.exportExcelListadoPesada = function (commodity, exportador, fechaInicio, fechaFin) {
        var params = new URLSearchParams();
        params.set('commodity', commodity);
        params.set('exportador', exportador);
        params.set('fechaInicio', fechaInicio);
        params.set('fechaFin', fechaFin);
        return this.http
            .get('/api/ryd/downloadListadoPesada', { search: params, headers: this.headers }).pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))), map(this.extractData));
    };
    RYDListadoPesadasService = __decorate([
        Injectable()
    ], RYDListadoPesadasService);
    return RYDListadoPesadasService;
}(RYDService));
export { RYDListadoPesadasService };
//# sourceMappingURL=ryd.service.js.map