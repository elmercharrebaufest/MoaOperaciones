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
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
var ReporteService = /** @class */ (function (_super) {
    __extends(ReporteService, _super);
    function ReporteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ReporteService.prototype.getDatosContratos = function (numero_contrato, fijacion) {
        var params = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/CrearContrato/GetContratos', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    ReporteService.prototype.getDatosCombos = function () {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        var params = new URLSearchParams();
        params.set('tiponegocio', "1");
        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    };
    ReporteService.prototype.obtenerMateriales = function () {
        return this.http
            .get('/api/AltaEmpresaGranos/GetMateriales', { headers: this.headers }).pipe(map(this.extractData));
    };
    ReporteService.prototype.obteneContratos = function (fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId, boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, sustentableTercero) {
        var params = new URLSearchParams();
        params.set('fechaDesde', fechaDesde);
        params.set('fechaHasta', fechaHasta);
        params.set('entregaDesde', entregaDesde);
        params.set('entregaHasta', entregaHasta);
        params.set('fijacionHasta', fijacionHasta);
        params.set('corredorId', corredorId);
        params.set('proveedorId', proveedorId);
        params.set('boletoId', boletoId);
        params.set('clasificacionId', clasificacionId);
        params.set('destinoId', destinoId);
        params.set('estadoId', estadoId);
        params.set('materialId', materialId);
        params.set('campaniaId', campaniaId);
        params.set('tipoNegocioId', tipoNegocioId);
        params.set('pagoDiferidoTercero', pagoDiferidoTercero);
        params.set('calidadTercero', calidadTercero);
        params.set('dolarizadoTercero', dolarizadoTercero);
        params.set('sustentableTercero', sustentableTercero);
        return this.http
            .get('/api/CrearContrato/GetContratos', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    };
    ReporteService.prototype.buscarProveedoresConCorredor = function (term) {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        var params = new URLSearchParams();
        params.set('filtro', term);
        return this.http.get('/api/CrearContrato/BuscarProveedoresConCorredor', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    };
    ReporteService.prototype.validarDirecto = function () {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/CrearContrato/ValidarDirecto', { headers: this.headers })
            .pipe(map(this.extractData));
    };
    ReporteService.prototype.exportContratos = function (fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId, boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, sustentableTercero) {
        var params = new URLSearchParams();
        params.set('fechaDesde', fechaDesde);
        params.set('fechaHasta', fechaHasta);
        params.set('entregaDesde', entregaDesde);
        params.set('entregaHasta', entregaHasta);
        params.set('fijacionHasta', fijacionHasta);
        params.set('corredorId', corredorId);
        params.set('proveedorId', proveedorId);
        params.set('boletoId', boletoId);
        params.set('clasificacionId', clasificacionId);
        params.set('destinoId', destinoId);
        params.set('estadoId', estadoId);
        params.set('materialId', materialId);
        params.set('campaniaId', campaniaId);
        params.set('tipoNegocioId', tipoNegocioId);
        params.set('pagoDiferidoTercero', pagoDiferidoTercero);
        params.set('calidadTercero', calidadTercero);
        params.set('dolarizadoTercero', dolarizadoTercero);
        params.set('sustentableTercero', sustentableTercero);
        return this.http
            .get('/api/CrearContrato/ExportContratos', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    };
    ReporteService = __decorate([
        Injectable()
    ], ReporteService);
    return ReporteService;
}(BaseService));
export { ReporteService };
var ReporteContratoService = /** @class */ (function (_super) {
    __extends(ReporteContratoService, _super);
    function ReporteContratoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ReporteContratoService = __decorate([
        Injectable()
    ], ReporteContratoService);
    return ReporteContratoService;
}(ReporteService));
export { ReporteContratoService };
var ReporteCupoService = /** @class */ (function (_super) {
    __extends(ReporteCupoService, _super);
    function ReporteCupoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ReporteCupoService = __decorate([
        Injectable()
    ], ReporteCupoService);
    return ReporteCupoService;
}(ReporteService));
export { ReporteCupoService };
//# sourceMappingURL=reporte.service.js.map