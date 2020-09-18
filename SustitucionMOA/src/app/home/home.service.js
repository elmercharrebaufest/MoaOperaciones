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
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { throwError as observableThrowError } from 'rxjs';
import { map, timeoutWith } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
var HomeService = /** @class */ (function (_super) {
    __extends(HomeService, _super);
    function HomeService(http) {
        var _this = _super.call(this, http) || this;
        _this.http = http;
        return _this;
    }
    HomeService.prototype.getHomeInfo = function (fecha_inicio, fecha_fin) {
        var params = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/home/getHomeInfo', { search: params, headers: this.headers }).pipe(map(this.extractData), timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    };
    HomeService.prototype.descargarDocumentoPDF = function (documento, ejercicio) {
        var params = new URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers }).pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))), map(this.extractData));
    };
    HomeService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [Http])
    ], HomeService);
    return HomeService;
}(BaseService));
export { HomeService };
var HomeNGService = /** @class */ (function (_super) {
    __extends(HomeNGService, _super);
    function HomeNGService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    HomeNGService.prototype.getHomeInfo = function (fecha_inicio, fecha_fin) {
        var params = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/home/getHomeNGInfo', { search: params, headers: this.headers }).pipe(map(this.extractData), timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    };
    HomeNGService = __decorate([
        Injectable()
    ], HomeNGService);
    return HomeNGService;
}(HomeService));
export { HomeNGService };
//# sourceMappingURL=home.service.js.map