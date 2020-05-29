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
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var Observable_1 = require("rxjs/Observable");
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var PesificacionService = /** @class */ (function (_super) {
    __extends(PesificacionService, _super);
    function PesificacionService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PesificacionService.prototype.getData = function () {
        return this.getFechaPesificacion();
    };
    PesificacionService.prototype.setData = function (contrato, fijacion, cantidad) {
        return this.setComprobantePesificacion(contrato, fijacion, cantidad);
    };
    PesificacionService.prototype.setMassiveData = function (file) {
        return this.setComprobantesPesificacion(file);
    };
    PesificacionService.prototype.getFechaPesificacion = function () {
        return this.http
            .get('/api/pesificacion/getFechaPesificacion')
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    };
    PesificacionService.prototype.setComprobantePesificacion = function (contrato, fijacion, cantidad) {
        var payload = new FormData();
        var data = { contrato: contrato, fijacion: fijacion, cantidad: cantidad };
        payload.append("contrato", JSON.stringify(data));
        return this.http
            .post('/api/pesificacion/setComprobante', payload)
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    };
    PesificacionService.prototype.setComprobantesPesificacion = function (file) {
        var payload = new FormData();
        payload.append("file", file);
        return this.http
            .post('/api/pesificacion/setComprobantes', payload, this.headersPost)
            .timeoutWith(30000, Observable_1.Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    };
    PesificacionService = __decorate([
        core_1.Injectable()
    ], PesificacionService);
    return PesificacionService;
}(BaseService_1.BaseService));
exports.PesificacionService = PesificacionService;
//# sourceMappingURL=pesificacion.service.js.map