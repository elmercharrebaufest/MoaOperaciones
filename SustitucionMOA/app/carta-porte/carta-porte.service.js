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
require("rxjs/add/operator/timeoutWith");
require("rxjs/add/observable/throw");
require("rxjs/add/observable/defer");
var BaseService_1 = require("./../common/services/BaseService");
var CartaPorteService = /** @class */ (function (_super) {
    __extends(CartaPorteService, _super);
    function CartaPorteService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CartaPorteService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    CartaPorteService.prototype.getCartasPorteCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/cartaporte/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CartaPorteService.prototype.getDetalle = function (cartaPorteId) {
        var params = new http_1.URLSearchParams();
        params.set('cartaPorteId', cartaPorteId);
        return this.http
            .get('/api/cartaporte/getDetalle', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CartaPorteService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return null;
    };
    CartaPorteService.prototype.exportExcelCommon = function (periodo, fecha_inicio, fecha_fin, method) {
        var params = new http_1.URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/cartaporte/' + method, { search: params, headers: this.headers })
            .timeoutWith(60000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CartaPorteService.prototype.exportExcelDetalle = function (cartaPorteId) {
        var params = new http_1.URLSearchParams();
        params.set('cartaPorteId', cartaPorteId);
        return this.http
            .get('/api/cartaporte/downloadDetalle', { search: params, headers: this.headers })
            .timeoutWith(60000, Observable_1.Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    };
    CartaPorteService.prototype.exportPDFCalidad = function (numero_ccpp) {
        var params = new http_1.URLSearchParams();
        params.set('numeroCCPP', numero_ccpp);
        return this.http
            .get('/api/cartaporte/exportPDFCalidad', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    CartaPorteService.prototype.getFotos = function (cartaPorteId) {
        var params = new http_1.URLSearchParams();
        params.set('cartaPorteId', cartaPorteId);
        return this.http
            .get('/api/cartaporte/GetFotos', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    CartaPorteService.prototype.getListaFotos = function (cartaPorteIds) {
        var params = new http_1.URLSearchParams();
        params.set('cartaPorteIds', cartaPorteIds);
        return this.http
            .get('/api/cartaporte/GetListaFotos', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    CartaPorteService.prototype.descargarFotos = function (cartaPorteIds) {
        var params = new http_1.URLSearchParams();
        params.set('cartaPorteIds', cartaPorteIds);
        return this.http
            .get('/api/cartaporte/DescargarFotos', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    CartaPorteService = __decorate([
        core_1.Injectable()
    ], CartaPorteService);
    return CartaPorteService;
}(BaseService_1.BaseService));
exports.CartaPorteService = CartaPorteService;
var CartaPorteDescargaService = /** @class */ (function (_super) {
    __extends(CartaPorteDescargaService, _super);
    function CartaPorteDescargaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CartaPorteDescargaService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getCartasPorteCommon(periodo, fecha_inicio, fecha_fin, 'getDescargas');
    };
    CartaPorteDescargaService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadDescargas');
    };
    CartaPorteDescargaService = __decorate([
        core_1.Injectable()
    ], CartaPorteDescargaService);
    return CartaPorteDescargaService;
}(CartaPorteService));
exports.CartaPorteDescargaService = CartaPorteDescargaService;
var CartaPorteAplicacionService = /** @class */ (function (_super) {
    __extends(CartaPorteAplicacionService, _super);
    function CartaPorteAplicacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CartaPorteAplicacionService.prototype.getData = function (periodo, fecha_inicio, fecha_fin) {
        return this.getCartasPorteCommon(periodo, fecha_inicio, fecha_fin, 'getAplicaciones');
    };
    CartaPorteAplicacionService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin) {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAplicaciones');
    };
    CartaPorteAplicacionService = __decorate([
        core_1.Injectable()
    ], CartaPorteAplicacionService);
    return CartaPorteAplicacionService;
}(CartaPorteService));
exports.CartaPorteAplicacionService = CartaPorteAplicacionService;
var CartaPorteFormularioService = /** @class */ (function (_super) {
    __extends(CartaPorteFormularioService, _super);
    function CartaPorteFormularioService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    CartaPorteFormularioService.prototype.getFormularioDropdowns = function () {
        return this.http
            .get('/api/cartaporte/getFormularioDropdowns', { headers: this.headers })
            .map(this.extractData);
    };
    CartaPorteFormularioService.prototype.getDataCTG = function (valor) {
        var params = new http_1.URLSearchParams();
        params.set('valor', valor);
        return this.http
            .get('/api/cartaporte/getDataCTG', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Por favor, intentelo nuevamente")))
            .map(this.extractData);
    };
    CartaPorteFormularioService.prototype.getCompletedPDFTemplate = function (formulario, archivo, pageSelected) {
        var payload = new FormData();
        payload.append("formularioString", JSON.stringify(formulario));
        payload.append("paginaSeleccionada", pageSelected);
        payload.append("file", archivo);
        return this.http
            .post('/api/cartaporte/getCompletedPDFTemplate', payload, this.headersPost)
            //.timeoutWith(90000, Observable.throw(new Error("Por favor, intentelo nuevamente")))
            .map(this.extractData);
    };
    CartaPorteFormularioService.prototype.getTemplate = function (formulario) {
        var payload = new FormData();
        payload.append("formularioString", JSON.stringify(formulario));
        return this.http
            .post('/api/cartaporte/getTemplate', payload, this.headersPost)
            //.timeoutWith(90000, Observable.throw(new Error("Por favor, intentelo nuevamente")))
            .map(this.extractData);
    };
    CartaPorteFormularioService = __decorate([
        core_1.Injectable()
    ], CartaPorteFormularioService);
    return CartaPorteFormularioService;
}(BaseService_1.BaseService));
exports.CartaPorteFormularioService = CartaPorteFormularioService;
//# sourceMappingURL=carta-porte.service.js.map