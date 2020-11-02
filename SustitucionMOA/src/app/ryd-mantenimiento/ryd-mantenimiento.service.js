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
var RYDMantenimientoService = /** @class */ (function (_super) {
    __extends(RYDMantenimientoService, _super);
    function RYDMantenimientoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDMantenimientoService.prototype.getFiltros = function (method) {
        return this.http
            .get('/api/RYDMantenimiento/getFiltros' + method, { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.getInputDropDown = function () {
        return this.http
            .get('/api/RYDMantenimiento/getInputDropDown', { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.getNroPuesto = function (itcId) {
        var params = new URLSearchParams();
        params.set('itcID', itcId);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosNroPuesto', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.buscarBalanza = function (descripcion, tipoSelected, codigoCabezalSelected, codigoSAP) {
        var params = new URLSearchParams();
        params.set('descripcion', descripcion);
        params.set('tipoId', tipoSelected);
        params.set('codigoSAP', codigoSAP);
        params.set('codigoCabezalId', codigoCabezalSelected);
        return this.http
            .post('/api/RYDMantenimiento/buscarBalanza', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.guardarBalanza = function (codigo, descripcion, automatico, toleria, centroEmisor, tolerX, tipoSelected, pesoMaximo, codigoSAP, codigoCabezalSelected, itcSelected, nroPuestoSelected, tipoAccesoSelected) {
        var params = new URLSearchParams();
        params.set('codigo', codigo);
        params.set('descripcion', descripcion);
        params.set('automatico', automatico);
        params.set('toleria', toleria);
        params.set('centroEmisor', centroEmisor);
        params.set('tolerX', tolerX);
        params.set('tipoId', tipoSelected);
        params.set('pesoMaximo', pesoMaximo);
        params.set('codigoSAP', codigoSAP);
        params.set('codigoCabezalId', codigoCabezalSelected);
        params.set('itcId', itcSelected);
        params.set('nroPuestoId', nroPuestoSelected);
        params.set('tipoAccesoId', tipoAccesoSelected);
        return this.http
            .post('/api/RYDMantenimiento/guardarBalanza', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.actualizarBalanza = function (codigo, descripcion, automatico, toleria, centroEmisor, tolerX, tipoSelected, pesoMaximo, codigoSAP, codigoCabezalSelected, itcSelected, nroPuestoSelected, tipoAccesoSelected) {
        var params = new URLSearchParams();
        params.set('codigo', codigo);
        params.set('descripcion', descripcion);
        params.set('automatico', automatico);
        params.set('toleria', toleria);
        params.set('centroEmisor', centroEmisor);
        params.set('tolerX', tolerX);
        params.set('tipoId', tipoSelected);
        params.set('pesoMaximo', pesoMaximo);
        params.set('codigoSAP', codigoSAP);
        params.set('codigoCabezalId', codigoCabezalSelected);
        params.set('itcId', itcSelected);
        params.set('nroPuestoId', nroPuestoSelected);
        params.set('tipoAccesoId', tipoAccesoSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarBalanza', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.borrarBalanza = function (codigo) {
        var params = new URLSearchParams();
        params.set('codigo', codigo);
        return this.http
            .post('/api/RYDMantenimiento/borrarBalanza', params, this.headersPost).pipe(map(this.extractData));
    };
    /*public extractData(res: Response) {
        return res.json();
    }*/
    RYDMantenimientoService.prototype.busqueda = function (busquedas) {
        var body = JSON.stringify(busquedas);
        return this.http
            .post('/api/RYDMantenimiento/busqueda', body, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoService.prototype.aplicar = function (codigo) {
        var params = new URLSearchParams();
        params.set('codigoId', codigo);
        return this.http
            .get('/api/RYDMantenimiento/aplicar', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoService = __decorate([
        Injectable()
    ], RYDMantenimientoService);
    return RYDMantenimientoService;
}(BaseService));
export { RYDMantenimientoService };
var RYDMantenimientoBalanzaService = /** @class */ (function (_super) {
    __extends(RYDMantenimientoBalanzaService, _super);
    function RYDMantenimientoBalanzaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDMantenimientoBalanzaService = __decorate([
        Injectable()
    ], RYDMantenimientoBalanzaService);
    return RYDMantenimientoBalanzaService;
}(RYDMantenimientoService));
export { RYDMantenimientoBalanzaService };
var RYDMantenimientoCommoditiesService = /** @class */ (function (_super) {
    __extends(RYDMantenimientoCommoditiesService, _super);
    function RYDMantenimientoCommoditiesService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDMantenimientoCommoditiesService.prototype.getFiltros = function () {
        return _super.prototype.getFiltros.call(this, "Commodities");
    };
    RYDMantenimientoCommoditiesService.prototype.getCommodities = function (commoditie) {
        var params = new URLSearchParams();
        params.set('commoditie', commoditie);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosCommodities', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoCommoditiesService.prototype.getInputCommodities = function () {
        return this.http
            .get('/api/RYDMantenimiento/getCommodities', { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoCommoditiesService.prototype.guardarCommodity = function (materialSAP, almacenOrigen, descripcion) {
        //let body = JSON.stringify(commodity);
        var params = new URLSearchParams();
        params.set('materialSAP', materialSAP);
        params.set('almacenOrigen', almacenOrigen);
        params.set('descripcion', descripcion);
        return this.http
            .post('/api/RYDMantenimiento/guardarCommodity', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoCommoditiesService.prototype.actualizarCommodity = function (materialSAP, almacenOrigen, descripcion, commoditieSelected) {
        //let body = JSON.stringify(commodity);
        var params = new URLSearchParams();
        params.set('materialSAP', materialSAP);
        params.set('almacenOrigen', almacenOrigen);
        params.set('descripcion', descripcion);
        params.set('commodityId', commoditieSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarCommodity', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoCommoditiesService.prototype.borrarCommodity = function (commoditieSelected) {
        //let body = JSON.stringify(commodity);
        var params = new URLSearchParams();
        params.set('commodityId', commoditieSelected);
        return this.http
            .post('/api/RYDMantenimiento/borrarCommodity', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoCommoditiesService = __decorate([
        Injectable()
    ], RYDMantenimientoCommoditiesService);
    return RYDMantenimientoCommoditiesService;
}(RYDMantenimientoService));
export { RYDMantenimientoCommoditiesService };
var RYDMantenimientoExportadorService = /** @class */ (function (_super) {
    __extends(RYDMantenimientoExportadorService, _super);
    function RYDMantenimientoExportadorService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    RYDMantenimientoExportadorService.prototype.getFiltros = function () {
        return _super.prototype.getFiltros.call(this, "Exportador");
    };
    RYDMantenimientoExportadorService.prototype.getExportador = function (exportador) {
        var params = new URLSearchParams();
        params.set('exportador', exportador);
        return this.http
            .get('/api/RYDMantenimiento/getFiltrosExportador', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoExportadorService.prototype.getInputExportador = function () {
        return this.http
            .get('/api/RYDMantenimiento/getExportador', { headers: this.headers })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .pipe(map(this.extractData));
    };
    RYDMantenimientoExportadorService.prototype.guardarExportador = function (almacenSAP, descripcion) {
        //let body = JSON.stringify(commodity);
        var params = new URLSearchParams();
        params.set('almacenSAP', almacenSAP);
        params.set('descripcion', descripcion);
        return this.http
            .post('/api/RYDMantenimiento/guardarExportador', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoExportadorService.prototype.actualizarExportador = function (almacenSAP, descripcion, exportadorSelected) {
        //let body = JSON.stringify(commodity);
        var params = new URLSearchParams();
        params.set('almacenSAP', almacenSAP);
        params.set('descripcion', descripcion);
        params.set('exportadorId', exportadorSelected);
        return this.http
            .post('/api/RYDMantenimiento/actualizarExportador', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoExportadorService.prototype.borrarExportador = function (exportadorSelected) {
        //let body = JSON.stringify(commodity);
        var params = new URLSearchParams();
        params.set('exportadorId', exportadorSelected);
        return this.http
            .post('/api/RYDMantenimiento/borrarExportador', params, this.headersPost).pipe(map(this.extractData));
    };
    RYDMantenimientoExportadorService = __decorate([
        Injectable()
    ], RYDMantenimientoExportadorService);
    return RYDMantenimientoExportadorService;
}(RYDMantenimientoService));
export { RYDMantenimientoExportadorService };
//# sourceMappingURL=ryd-mantenimiento.service.js.map