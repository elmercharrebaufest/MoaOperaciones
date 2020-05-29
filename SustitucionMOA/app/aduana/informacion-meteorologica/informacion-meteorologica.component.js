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
var aduana_component_1 = require("./../aduana.component");
var aduana_service_1 = require("./../aduana.service");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var ModalService_1 = require("./../../common/services/ModalService");
var InformacionMeteorologicaComponent = /** @class */ (function (_super) {
    __extends(InformacionMeteorologicaComponent, _super);
    function InformacionMeteorologicaComponent(navService, service, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, service, securityService, floatMsgService, modalService) || this;
        _this.navService = navService;
        _this.service = service;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        return _this;
    }
    InformacionMeteorologicaComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR INFORMACION METEOROL"); };
    InformacionMeteorologicaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("aduana", "Inf. Meteorológica");
    };
    InformacionMeteorologicaComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        if (this.securityService.tienePermiso("CONSULTAR INFORMACION METEOROL")) {
            this.abrirInfoMet();
        }
    };
    InformacionMeteorologicaComponent.prototype.abrirInfoMet = function () {
        window.open("Template/mb1.htm");
        return false;
    };
    InformacionMeteorologicaComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/aduana/informacion-meteorologica/informacion-meteorologica.component.html?v=" + new Date().getTime(),
        }),
        __metadata("design:paramtypes", [NavService_1.NavService, aduana_service_1.AduanaService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], InformacionMeteorologicaComponent);
    return InformacionMeteorologicaComponent;
}(aduana_component_1.AduanaBaseComponent));
exports.InformacionMeteorologicaComponent = InformacionMeteorologicaComponent;
//# sourceMappingURL=informacion-meteorologica.component.js.map