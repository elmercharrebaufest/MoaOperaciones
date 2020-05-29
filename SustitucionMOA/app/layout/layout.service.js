"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
var BaseService_1 = require("./../common/services/BaseService");
var Observable_1 = require("rxjs/Observable");
require("rxjs/add/operator/map");
var LayoutService = /** @class */ (function (_super) {
    __extends(LayoutService, _super);
    function LayoutService(http) {
        var _this = _super.call(this, http) || this;
        _this.http = http;
        return _this;
    }
    LayoutService.prototype.downloadVinculacion = function (contrato, secuencia) {
        var params = new http_1.URLSearchParams();
        params.set('contrato', contrato);
        params.set('secuencia', secuencia);
        return this.http
            .get('/api/liquidacion/downloadVinculacion', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LayoutService.prototype.downloadProcedencia = function (contrato) {
        var params = new http_1.URLSearchParams();
        params.set('contrato', contrato);
        return this.http
            .get('/api/liquidacion/descargarFleteProcedencia', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    LayoutService.prototype.goToDataAgro = function () {
        return this.http
            .get('/api/dataAgro/goToDataAgro')
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    };
    LayoutService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [http_1.Http])
    ], LayoutService);
    return LayoutService;
}(BaseService_1.BaseService));
exports.LayoutService = LayoutService;
//# sourceMappingURL=layout.service.js.map