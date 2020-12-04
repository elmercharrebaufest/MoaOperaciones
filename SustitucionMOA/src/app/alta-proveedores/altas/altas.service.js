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
import { BaseService } from './../../common/services/BaseService';
var AltaEmpresaService = /** @class */ (function (_super) {
    __extends(AltaEmpresaService, _super);
    function AltaEmpresaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    AltaEmpresaService.prototype.getEmpresas = function () {
        return this.http
            .get('/api/AltaEmpresa/getEmpresas', { headers: this.headers }).pipe(map(this.extractData));
    };
    AltaEmpresaService.prototype.setEstadoAprobacion = function (empresaId, estadoId, observacion, observacionesProveedor, estadoSIPER) {
        var params = new URLSearchParams();
        params.set('empresaId', empresaId.toString());
        params.set('estado', estadoId.toString());
        params.set('observacion', observacion);
        params.set('observacionParaElProveedor', observacionesProveedor);
        params.set('estadoSIPER', estadoSIPER);
        return this.http
            .get('/api/AltaEmpresa/setEstadoAprobacion', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    AltaEmpresaService.prototype.getEstados = function () {
        return this.http
            .get('/api/AltaEmpresa/getEstados', { headers: this.headers }).pipe(map(this.extractData));
    };
    AltaEmpresaService.prototype.cargarSolicitudUsuario = function () {
        return this.http
            .get('/api/AltaEmpresaGranos/CargarSolicitudUsuario')
            .pipe(map(this.extractData));
    };
    AltaEmpresaService.prototype.VerificarEstadoDataAgro = function (empresaId) {
        var params = new URLSearchParams();
        params.set('proveedorID', empresaId.toString());
        return this.http
            .get('/api/AltaEmpresa/VerificarEstadoDataAgro', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    AltaEmpresaService = __decorate([
        Injectable()
    ], AltaEmpresaService);
    return AltaEmpresaService;
}(BaseService));
export { AltaEmpresaService };
//# sourceMappingURL=altas.service.js.map