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
var ryd_mantenimiento_service_1 = require("./ryd-mantenimiento.service");
var mensaje_component_1 = require("./../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../common/view-child/spinner/spinner.component");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var SecurityService_1 = require("./../common/services/SecurityService");
var base_component_1 = require("./../common/base-components/base-component");
var Seccion_1 = require("./../common/models/Seccion");
var ModalService_1 = require("./../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
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
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        return _this;
    }
    RYDMantenimientoBaseComponent.prototype.checkPermisos = function () { };
    RYDMantenimientoBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion_1.Seccion('/ryd-mantenimiento/balanzas', 'ryd-mantenimiento', 'Balanzas'), new Seccion_1.Seccion('/ryd-mantenimiento/commodities', 'ryd-mantenimiento', 'Commodities'), new Seccion_1.Seccion('/ryd-mantenimiento/exportadores', 'ryd-mantenimiento', 'Exportadores')]);
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], RYDMantenimientoBaseComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], RYDMantenimientoBaseComponent.prototype, "spinnerComponent", void 0);
    RYDMantenimientoBaseComponent = __decorate([
        core_1.Component({
            selector: 'app-ryd-mantenimiento',
            template: "",
            providers: [ryd_mantenimiento_service_1.RYDMantenimientoService]
        }),
        __metadata("design:paramtypes", [ryd_mantenimiento_service_1.RYDMantenimientoService, NavService_1.NavService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], RYDMantenimientoBaseComponent);
    return RYDMantenimientoBaseComponent;
}(base_component_1.BaseComponent));
exports.RYDMantenimientoBaseComponent = RYDMantenimientoBaseComponent;
//# sourceMappingURL=ryd-mantenimiento.component.js.map