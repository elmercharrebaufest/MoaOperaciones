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
import { PagoService } from './../pago.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
var PagoDetalleComponent = /** @class */ (function (_super) {
    __extends(PagoDetalleComponent, _super);
    function PagoDetalleComponent(route, router, service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivoExcel = "ReportePagoDetalle.xls";
        _this.numeroPagoId = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    PagoDetalleComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("pago", "Detalle");
    };
    PagoDetalleComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.navService.setSeccionList([]);
        this.securityService.tienePermisoRedirect("CONSULTAR PAGOS DETALLE");
        this.getData();
    };
    PagoDetalleComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    };
    PagoDetalleComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            _this.numeroPagoId = params['id'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDetalle(_this.numeroPagoId).subscribe(function (result) {
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
                    _this.data = result;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
            //this.navService.setSeccionActive('Fijaciones');
        });
    };
    PagoDetalleComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.numeroPagoId).subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
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
                var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivoExcel);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivoExcel;
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    PagoDetalleComponent.prototype.isExportVisible = function () {
        this.spinnerSmallComponent.visible;
    };
    PagoDetalleComponent.prototype.isNegative = function (valor) {
        return valor < 0;
    };
    PagoDetalleComponent.prototype.descargaPDF = function (nroDoc) {
        return false;
    };
    PagoDetalleComponent.prototype.showCorredorTotal = function (mercCorredor, ivaCorredor) {
        return (mercCorredor != 0 || ivaCorredor != 0);
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], PagoDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], PagoDetalleComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("smallSpinner"),
        __metadata("design:type", SpinnerSmallComponent)
    ], PagoDetalleComponent.prototype, "spinnerSmallComponent", void 0);
    PagoDetalleComponent = __decorate([
        Component({
            selector: 'app-pago-detalle',
            templateUrl: "pago.detalle.component.html",
            providers: [PagoService]
        }),
        __metadata("design:paramtypes", [ActivatedRoute, Router, PagoService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], PagoDetalleComponent);
    return PagoDetalleComponent;
}(BaseComponent));
export { PagoDetalleComponent };
//# sourceMappingURL=pago.detalle.component.js.map