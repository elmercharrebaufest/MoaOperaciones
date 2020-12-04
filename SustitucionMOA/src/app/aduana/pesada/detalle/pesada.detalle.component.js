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
import { Router, ActivatedRoute } from '@angular/router';
import { AduanaService } from './../../aduana.service';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { AduanaBaseComponent } from './../../aduana.component';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';
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
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
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
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], PesadaDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], PesadaDetalleComponent.prototype, "spinnerComponent", void 0);
    PesadaDetalleComponent = __decorate([
        Component({
            selector: 'app-aduana-pesada-detalle',
            templateUrl: "pesada.detalle.component.html",
            providers: [AduanaService]
        }),
        __metadata("design:paramtypes", [ActivatedRoute, Router, NavService, AduanaService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], PesadaDetalleComponent);
    return PesadaDetalleComponent;
}(AduanaBaseComponent));
export { PesadaDetalleComponent };
//# sourceMappingURL=pesada.detalle.component.js.map