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
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { throwError as observableThrowError } from 'rxjs';
import { map, timeoutWith } from 'rxjs/operators';
import { BaseService } from './../common/services/BaseService';
var DatoFiscalService = /** @class */ (function (_super) {
    __extends(DatoFiscalService, _super);
    function DatoFiscalService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    DatoFiscalService.prototype.getDatosFiscales = function (vendedor) {
        var params = new URLSearchParams();
        params.set('vendedor', vendedor);
        return this.http
            .get('/api/vendedor/getDatoFiscales', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    };
    DatoFiscalService.prototype.getVendedores = function (fecha_inicio, fecha_fin) {
        var params = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/vendedor/getVendedores', { search: params })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    DatoFiscalService.prototype.getVendedoresPendientes = function () {
        var params = new URLSearchParams();
        return this.http
            .get('/api/vendedor/getVendedoresPendientes', { search: params })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    DatoFiscalService.prototype.agregarVendedor = function (nuevoVendedorRazonSocial, nuevoVendedorCUIT) {
        var params = new URLSearchParams();
        params.set('cuit', nuevoVendedorCUIT);
        params.set('razonSocial', nuevoVendedorRazonSocial);
        return this.http
            .get('/api/vendedor/agregarVendedor', { search: params })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    DatoFiscalService = __decorate([
        Injectable()
    ], DatoFiscalService);
    return DatoFiscalService;
}(BaseService));
export { DatoFiscalService };
//# sourceMappingURL=dato-fiscal.service.js.map