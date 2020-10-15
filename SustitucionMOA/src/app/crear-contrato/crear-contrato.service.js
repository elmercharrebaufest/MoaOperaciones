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
import { timeoutWith, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
var CrearContratoService = /** @class */ (function (_super) {
    __extends(CrearContratoService, _super);
    function CrearContratoService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CrearContratoService.prototype.obteneDatosContrato = function () {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { headers: this.headers })
            .pipe(map(this.extractData));
    };
    CrearContratoService.prototype.obtenerDatosCompraNet = function () {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/CrearContrato/ObtenerDatosCompraNet', { headers: this.headers })
            .pipe(map(this.extractData));
    };
    CrearContratoService.prototype.searchLocalidad = function (term) {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        var params = new URLSearchParams();
        params.set('localidad', term);
        return this.http.get('/api/CrearContrato/GetLocalidadCombo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    };
    CrearContratoService.prototype.grabarContratoAPrecio = function (contrato) {
        var payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAPrecio', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    CrearContratoService.prototype.ObtenerFechaHasta = function (fechaBase) {
        var hoy = fechaBase != undefined ? fechaBase : new Date();
        var anio = hoy.getFullYear();
        var mesPost = hoy.getMonth() + 1;
        var dia = hoy.getDate();
        var ultimoDia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
        if (dia === 1) {
            dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
            mesPost = hoy.getMonth() + 1;
        }
        if (dia === ultimoDia || (mesPost === 2 && dia >= 29)) {
            dia = new Date(anio, mesPost, 0).getDate();
        }
        if (mesPost === 13) {
            mesPost = 1;
            anio += 1;
        }
        //if (mesPost < 10) {
        //    mesPost = "0" + mesPost.toString();
        //}
        //if (dia < 10) {
        //    dia = "0" + dia.toString();
        //}
        return new Date(anio, mesPost, dia); // dia + '-' + mesPost + '-' + anio;
    };
    CrearContratoService.prototype.grabarContratoAFijar = function (contrato) {
        var payload = new FormData();
        payload.append("contrato", JSON.stringify(contrato));
        return this.http
            .post('/api/CrearContrato/CrearContratoAFijar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    CrearContratoService = __decorate([
        Injectable()
    ], CrearContratoService);
    return CrearContratoService;
}(BaseService));
export { CrearContratoService };
var CrearContratoAPrecioService = /** @class */ (function (_super) {
    __extends(CrearContratoAPrecioService, _super);
    function CrearContratoAPrecioService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CrearContratoAPrecioService = __decorate([
        Injectable()
    ], CrearContratoAPrecioService);
    return CrearContratoAPrecioService;
}(CrearContratoService));
export { CrearContratoAPrecioService };
var CrearContratoAFijarService = /** @class */ (function (_super) {
    __extends(CrearContratoAFijarService, _super);
    function CrearContratoAFijarService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CrearContratoAFijarService = __decorate([
        Injectable()
    ], CrearContratoAFijarService);
    return CrearContratoAFijarService;
}(CrearContratoService));
export { CrearContratoAFijarService };
var CrearContratoFijacionService = /** @class */ (function (_super) {
    __extends(CrearContratoFijacionService, _super);
    function CrearContratoFijacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CrearContratoFijacionService = __decorate([
        Injectable()
    ], CrearContratoFijacionService);
    return CrearContratoFijacionService;
}(CrearContratoService));
export { CrearContratoFijacionService };
//# sourceMappingURL=crear-contrato.service.js.map