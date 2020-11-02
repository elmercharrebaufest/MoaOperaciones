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
import { ContratoService, ContratoFijacionService } from './../contrato.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Seccion } from './../../common/models/Seccion';
import { ModalService } from './../../common/services/ModalService';
var ContratoDetalleFijacionComponent = /** @class */ (function (_super) {
    __extends(ContratoDetalleFijacionComponent, _super);
    function ContratoDetalleFijacionComponent(route, router, service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.route = route;
        _this.router = router;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.data = null;
        _this.tituloArchivoExcel = "ReporteFijacionDetalle.xls";
        _this.numeroContratoId = "";
        _this.fijacion = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    ContratoDetalleFijacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("contrato", "Detalle Fijaciones");
    };
    ContratoDetalleFijacionComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.navService.setSeccionList([new Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.navService.setSeccionActive("Detalle");
        this.getData();
    };
    ContratoDetalleFijacionComponent.prototype.ngAfterViewInit = function () {
        this.spinnerSmallExportComponent = new SpinnerSmallComponent();
        this.spinnerSmallPDFComponent = new SpinnerSmallComponent();
    };
    ContratoDetalleFijacionComponent.prototype.getData = function () {
        var _this = this;
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach(function (params) {
            _this.numeroContratoId = params['id'];
            _this.fijacion = params['id2'];
            _this.unsubscribe();
            _this.subscription = _this.service.getDetalleFijacion(_this.numeroContratoId, _this.fijacion).subscribe(function (result) {
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
    ContratoDetalleFijacionComponent.prototype.exportarExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallExportComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalleFijacion(this.numeroContratoId, this.fijacion).subscribe(function (result) {
            _this.spinnerSmallExportComponent.hideIt();
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
            _this.spinnerSmallExportComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ContratoDetalleFijacionComponent.prototype.showDataPlus = function (registro) {
        return this.tieneData(registro.kgNetos) || this.tieneData(registro.kgDto) || this.tieneData(registro.kgApli) || this.tieneData(registro.dto);
    };
    ContratoDetalleFijacionComponent.prototype.tieneData = function (value) {
        return value != undefined && value != 0 && value != "" && value != "0 KG" && value != "0%";
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], ContratoDetalleFijacionComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], ContratoDetalleFijacionComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("spinnerSmallExport"),
        __metadata("design:type", SpinnerSmallComponent)
    ], ContratoDetalleFijacionComponent.prototype, "spinnerSmallExportComponent", void 0);
    __decorate([
        ViewChild("spinnerSmallPDF"),
        __metadata("design:type", SpinnerSmallComponent)
    ], ContratoDetalleFijacionComponent.prototype, "spinnerSmallPDFComponent", void 0);
    ContratoDetalleFijacionComponent = __decorate([
        Component({
            selector: 'app-contrato-detalle-fijacion',
            templateUrl: "contrato.detalle-fijacion.component.html",
            providers: [{ provide: ContratoService, useClass: ContratoFijacionService }]
        }),
        __metadata("design:paramtypes", [ActivatedRoute,
            Router,
            ContratoService,
            NavService,
            SecurityService,
            SessionDataService,
            FloatMsgService,
            ModalService])
    ], ContratoDetalleFijacionComponent);
    return ContratoDetalleFijacionComponent;
}(BaseComponent));
export { ContratoDetalleFijacionComponent };
//# sourceMappingURL=contrato.detalle-fijacion.component.js.map