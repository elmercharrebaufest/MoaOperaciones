"use strict";
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
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/operator/timeoutWith");
require("rxjs/add/observable/throw");
require("rxjs/add/observable/defer");
var BaseService = /** @class */ (function () {
    //options: any;
    function BaseService(http) {
        this.http = http;
        this.headers = new http_1.Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9');
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');
        this.headersPost = new http_1.Headers();
        this.headersPost.append('Content-Type', 'application/json; charset=utf-8');
        this.headersPost.append('Cache-control', 'no-cache');
        this.headersPost.append('Cache-control', 'no-store');
        this.headersPost.append('Expires', '0');
        this.headersPost.append('Pragma', 'no-cache');
        //this.options = new RequestOptions({ headers: this.headers });
    }
    BaseService.prototype.extractData = function (res) {
        return res.json();
    };
    BaseService.prototype.getData = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        return null;
    };
    BaseService.prototype.exportExcel = function (periodo, fecha_inicio, fecha_fin, contrato, pago, retencion) {
        return null;
    };
    BaseService.prototype.getDetalle = function (id) {
        return null;
    };
    BaseService.prototype.handleError = function (error) {
        return Promise.reject(error.message || error);
    };
    BaseService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [http_1.Http])
    ], BaseService);
    return BaseService;
}());
exports.BaseService = BaseService;
//# sourceMappingURL=BaseService.js.map