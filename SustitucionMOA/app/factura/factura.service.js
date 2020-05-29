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
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var BaseService_1 = require("./../common/services/BaseService");
var FacturaService = /** @class */ (function (_super) {
    __extends(FacturaService, _super);
    function FacturaService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    FacturaService.prototype.subirPDF = function (archivo) {
        var payload = new FormData();
        payload.append("factura", "");
        payload.append("file", archivo);
        return this.http
            .post('/api/factura/subirPDF', payload, this.headersPost)
            .map(this.extractData);
    };
    FacturaService = __decorate([
        core_1.Injectable()
    ], FacturaService);
    return FacturaService;
}(BaseService_1.BaseService));
exports.FacturaService = FacturaService;
//# sourceMappingURL=factura.service.js.map