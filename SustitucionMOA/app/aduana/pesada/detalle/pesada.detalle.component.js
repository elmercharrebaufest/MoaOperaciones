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
var router_1 = require("@angular/router");
var aduana_service_1 = require("./../../aduana.service");
var mensaje_component_1 = require("./../../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../../common/view-child/spinner/spinner.component");
var aduana_component_1 = require("./../../aduana.component");
var NavService_1 = require("./../../../common/services/NavService");
var FloatMsgService_1 = require("./../../../common/services/FloatMsgService");
var SessionDataService_1 = require("./../../../common/services/SessionDataService");
var SecurityService_1 = require("./../../../common/services/SecurityService");
var ModalService_1 = require("./../../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var PesadaDetalleComponent = /** @class */ (function (_super) {
    __extends(PesadaDetalleComponent, _super);
    function PesadaDetalleComponent(route, router, navService, service, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, service, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.navService = navService;
        _this.service = service;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.titulo = "";
        _this.itemsPerPage = 20;
        _this.pesadasDetalleList = null;
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        return _this;
    }
    PesadaDetalleComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR PESADA DETALLE"); };
    PesadaDetalleComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("aduana", "Detalle");
    };
    PesadaDetalleComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.getData();
    };
    PesadaDetalleComponent.prototype.getData = function () {
        var _this = this;
        this.pesadasDetalleList = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            var centro = params['centro'];
            var nroOrden = params['nroOrden'];
            _this.unsubscribe();
            _this.subscription = _this.service.getPesadaDetalle(centro, nroOrden).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.pesadasDetalleList = result;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        });
    };
    PesadaDetalleComponent.prototype.isVisible = function () {
        return this.pesadasDetalleList != null;
    };
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], PesadaDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], PesadaDetalleComponent.prototype, "spinnerComponent", void 0);
    PesadaDetalleComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/aduana/pesada/detalle/pesada.detalle.component.html?v=" + new Date().getTime(),
            providers: [aduana_service_1.AduanaService]
        }),
        __metadata("design:paramtypes", [router_1.ActivatedRoute, router_1.Router, NavService_1.NavService, aduana_service_1.AduanaService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], PesadaDetalleComponent);
    return PesadaDetalleComponent;
}(aduana_component_1.AduanaBaseComponent));
exports.PesadaDetalleComponent = PesadaDetalleComponent;
//# sourceMappingURL=pesada.detalle.component.js.map