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
var http_1 = require("@angular/http");
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var VendedorStatusService = /** @class */ (function (_super) {
    __extends(VendedorStatusService, _super);
    function VendedorStatusService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    VendedorStatusService.prototype.getVendedorStatus = function (cuit) {
        var params = new http_1.URLSearchParams();
        params.set('cuit', cuit);
        return this.http
            .get('/api/vendedor/getVendedorStatus', { search: params, headers: this.headers })
            .map(this.extractData);
    };
    VendedorStatusService = __decorate([
        core_1.Injectable()
    ], VendedorStatusService);
    return VendedorStatusService;
}(BaseService_1.BaseService));
exports.VendedorStatusService = VendedorStatusService;
//# sourceMappingURL=vendedor_status.service.js.map