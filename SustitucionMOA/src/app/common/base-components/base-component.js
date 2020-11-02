var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component } from '@angular/core';
import { NavService } from './../services/NavService';
import { SecurityService } from './../services/SecurityService';
import { FloatMsgService } from './../services/FloatMsgService';
import { ModalService } from './../services/ModalService';
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
        return this.tipoUsuario.toUpperCase() == "CORR" || this.tipoUsuario.toUpperCase() == "NUECORR";
    };
    BaseComponent = __decorate([
        Component({
            selector: 'app-base',
            template: ""
        }),
        __metadata("design:paramtypes", [NavService, SecurityService, FloatMsgService, ModalService])
    ], BaseComponent);
    return BaseComponent;
}());
export { BaseComponent };
//# sourceMappingURL=base-component.js.map