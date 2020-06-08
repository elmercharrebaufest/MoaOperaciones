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
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var DatoFiscalService = /** @class */ (function (_super) {
    __extends(DatoFiscalService, _super);
    function DatoFiscalService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    DatoFiscalService.prototype.getDatosFiscales = function (vendedor) {
        var params = new http_1.URLSearchParams();
        params.set('vendedor', vendedor);
        return this.http
            .get('/api/vendedor/getDatoFiscales', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    DatoFiscalService.prototype.getVendedores = function (fecha_inicio, fecha_fin) {
        var params = new http_1.URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/vendedor/getVendedores', { search: params })
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    };
    DatoFiscalService = __decorate([
        core_1.Injectable()
    ], DatoFiscalService);
    return DatoFiscalService;
}(BaseService_1.BaseService));
exports.DatoFiscalService = DatoFiscalService;
//# sourceMappingURL=dato-fiscal.service.js.map