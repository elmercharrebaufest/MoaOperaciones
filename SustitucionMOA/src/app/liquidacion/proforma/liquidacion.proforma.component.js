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
import { LiquidacionService, LiquidacionProformaService } from './../liquidacion.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
var LiquidacionProformaComponent = /** @class */ (function (_super) {
    __extends(LiquidacionProformaComponent, _super);
    function LiquidacionProformaComponent(route, router, service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivoExcel = "ReporteProformaDeLiquidacion.xls";
        _this.fijacion = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    LiquidacionProformaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("liquidacion", "Proforma");
    };
    LiquidacionProformaComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.getData();
    };
    LiquidacionProformaComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    };
    LiquidacionProformaComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            _this.fijacion = params['id'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDataProforma(_this.fijacion).subscribe(function (result) {
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
                    _this.data = result.data;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        });
    };
    LiquidacionProformaComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelProforma(this.fijacion).subscribe(function (result) {
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
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    LiquidacionProformaComponent.prototype.isExportVisible = function () {
        this.spinnerSmallComponent.visible;
    };
    LiquidacionProformaComponent.prototype.isNegative = function (valor) {
        return valor < 0;
    };
    LiquidacionProformaComponent.prototype.showModalTableResponsive = function () {
        this.modalService.openModalTableResponsive("Procedencia Flete", [
            { etiqueta: "Vencimiento", valor: 'a' },
            { etiqueta: "Tipo", valor: 'b' },
            { etiqueta: "Comprobante", valor: 'c' },
            { etiqueta: "Producto", valor: 'd' },
            { etiqueta: "Liquidacion", valor: 'e' },
            { etiqueta: "Total", valor: 'f' },
            { etiqueta: "Contrato", valor: 'g' }
        ]);
        return false;
    };
    LiquidacionProformaComponent.prototype.showModalProcedencia = function (contrato) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getFleteProcedencia(contrato).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.floatMsgService.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.floatMsgService.setInfoMsg(result.info);
            }
            else {
                //this.data = result.data;
                _this.modalService.openModalFleteProcedencia("Procedencia Flete (" + contrato + ")", contrato, result.procedenciasFlete);
            }
            return false;
        }, function (error) {
            // this.spinnerComponent.hideIt();
            _this.floatMsgService.setErrorMsg(error.message);
            return false;
        });
        return false;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], LiquidacionProformaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], LiquidacionProformaComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("smallSpinner"),
        __metadata("design:type", SpinnerSmallComponent)
    ], LiquidacionProformaComponent.prototype, "spinnerSmallComponent", void 0);
    LiquidacionProformaComponent = __decorate([
        Component({
            selector: 'app-liquidacion-proforma',
            templateUrl: "liquidacion.proforma.component.html",
            providers: [{ provide: LiquidacionService, useClass: LiquidacionProformaService }]
        }),
        __metadata("design:paramtypes", [ActivatedRoute, Router, LiquidacionProformaService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], LiquidacionProformaComponent);
    return LiquidacionProformaComponent;
}(BaseComponent));
export { LiquidacionProformaComponent };
//# sourceMappingURL=liquidacion.proforma.component.js.map