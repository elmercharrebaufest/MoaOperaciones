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
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var liquidacion_service_1 = require("./../liquidacion.service");
var liquidacion_component_1 = require("./../liquidacion.component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var Seccion_1 = require("./../../common/models/Seccion");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var LiquidacionNGBaseComponent = /** @class */ (function (_super) {
    __extends(LiquidacionNGBaseComponent, _super);
    function LiquidacionNGBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        return _this;
    }
    LiquidacionNGBaseComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("CONSULTAR LIQUIDACIONES NG");
    };
    LiquidacionNGBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion_1.Seccion('/comprobante-ngs/aprobada', 'comprobante-ngs', 'Aprobados'), new Seccion_1.Seccion('/comprobante-ngs/observada', 'comprobante-ngs', 'Observados') /*, new Seccion('/comprobante-ngs/paga', 'comprobante-ngs', 'Pagos')*/]);
        this.getData();
    };
    LiquidacionNGBaseComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: "",
            providers: [liquidacion_service_1.LiquidacionService]
        }),
        __metadata("design:paramtypes", [liquidacion_service_1.LiquidacionService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], LiquidacionNGBaseComponent);
    return LiquidacionNGBaseComponent;
}(liquidacion_component_1.LiquidacionBaseComponent));
exports.LiquidacionNGBaseComponent = LiquidacionNGBaseComponent;
//# sourceMappingURL=liquidacion.no-granos.component.js.map