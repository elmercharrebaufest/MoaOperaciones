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
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var AduanaService = /** @class */ (function (_super) {
    __extends(AduanaService, _super);
    function AduanaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    /*public exportExcelCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params })
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    }*/
    AduanaService.prototype.getPesada = function (centro, fecha_inicio, fecha_fin) {
        var params = new http_1.URLSearchParams();
        params.set('centro', centro);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/aduana/getPesada', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    AduanaService.prototype.getPesadaDetalle = function (centro, nroOrden) {
        var params = new http_1.URLSearchParams();
        params.set('centro', centro);
        params.set('nroOrden', nroOrden);
        return this.http
            .get('/api/aduana/getPesadaDetalle', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    AduanaService.prototype.getImagenCamaraConsolidacion = function (url, nombre) {
        var params = new http_1.URLSearchParams();
        params.set('url', url);
        params.set('nombre', nombre);
        return this.http
            .get('/api/aduana/obtenerImagenCamaraConsolidacion', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    AduanaService = __decorate([
        core_1.Injectable()
    ], AduanaService);
    return AduanaService;
}(BaseService_1.BaseService));
exports.AduanaService = AduanaService;
//# sourceMappingURL=aduana.service.js.map