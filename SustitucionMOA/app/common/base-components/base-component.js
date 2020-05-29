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
var NavService_1 = require("./../services/NavService");
var SecurityService_1 = require("./../services/SecurityService");
var FloatMsgService_1 = require("./../services/FloatMsgService");
var ModalService_1 = require("./../services/ModalService");
var BaseComponent = /** @class */ (function () {
    function BaseComponent(navService, securityService, floatMsgService, modalService) {
        this.navService = navService;
        this.securityService = securityService;
        this.floatMsgService = floatMsgService;
        this.modalService = modalService;
        this.tipoUsuario = sessionStorage.getItem("tipoUsuario");
    }
    BaseComponent.prototype.ngOnDestroy = function () {
        this.modalService.close();
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
    };
    BaseComponent.prototype.goToSeccion = function (path) {
        this.navService.navegarSeccion(path);
        return false;
    };
    BaseComponent.prototype.goToSeccionParam = function (path, param) {
        this.navService.navegarSeccionParam(path, param);
        return false;
    };
    BaseComponent.prototype.goToSeccionParamDos = function (path, param, param2) {
        this.navService.navegarSeccionParamDos(path, param, param2);
        return false;
    };
    BaseComponent.prototype.setMenuSeccionTab = function (menu, seccion) {
        this.navService.setMenuSeccionTab(menu, seccion);
        return false;
    };
    BaseComponent.prototype.unsubscribe = function () {
        if (this.subscription != undefined)
            this.subscription.unsubscribe();
        if (this.subscriptionDropDowns != undefined)
            this.subscriptionDropDowns.unsubscribe();
    };
    BaseComponent.prototype.setTabs = function () { };
    BaseComponent.prototype.isAuthorized = function (permiso) {
        return this.securityService.tienePermiso(permiso);
    };
    BaseComponent.prototype.isCorredor = function () {
        return this.tipoUsuario == "CORR";
    };
    BaseComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: ""
        }),
        __metadata("design:paramtypes", [NavService_1.NavService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], BaseComponent);
    return BaseComponent;
}());
exports.BaseComponent = BaseComponent;
//# sourceMappingURL=base-component.js.map