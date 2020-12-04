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
import { Component, ViewChild } from '@angular/core';
import { RYDMantenimientoService } from './ryd-mantenimiento.service';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { BaseComponent } from './../common/base-components/base-component';
import { Seccion } from './../common/models/Seccion';
import { ModalService } from './../common/services/ModalService';
var RYDMantenimientoBaseComponent = /** @class */ (function (_super) {
    __extends(RYDMantenimientoBaseComponent, _super);
    function RYDMantenimientoBaseComponent(service, navService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.visibleButton = true;
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    RYDMantenimientoBaseComponent.prototype.checkPermisos = function () { };
    RYDMantenimientoBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/ryd-mantenimiento/balanzas', 'ryd-mantenimiento', 'Balanzas'), new Seccion('/ryd-mantenimiento/commodities', 'ryd-mantenimiento', 'Commodities'), new Seccion('/ryd-mantenimiento/exportadores', 'ryd-mantenimiento', 'Exportadores')]);
        //super.ngOnInit();
        this.getDataInputs();
    };
    RYDMantenimientoBaseComponent.prototype.getDataInputs = function () { };
    RYDMantenimientoBaseComponent.prototype.initRequestBotones = function () {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
    };
    RYDMantenimientoBaseComponent.prototype.endRequestBotones = function () {
        this.visibleButton = true;
        this.spinnerComponent.hideIt();
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], RYDMantenimientoBaseComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], RYDMantenimientoBaseComponent.prototype, "spinnerComponent", void 0);
    RYDMantenimientoBaseComponent = __decorate([
        Component({
            selector: 'app-ryd-mantenimiento',
            template: "",
            providers: [RYDMantenimientoService]
        }),
        __metadata("design:paramtypes", [RYDMantenimientoService, NavService, SecurityService, FloatMsgService, ModalService])
    ], RYDMantenimientoBaseComponent);
    return RYDMantenimientoBaseComponent;
}(BaseComponent));
export { RYDMantenimientoBaseComponent };
//# sourceMappingURL=ryd-mantenimiento.component.js.map