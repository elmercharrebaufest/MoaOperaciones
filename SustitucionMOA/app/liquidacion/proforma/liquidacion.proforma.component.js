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
var liquidacion_service_1 = require("./../liquidacion.service");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var spinner_small_component_1 = require("./../../common/view-child/spinner-small/spinner-small.component");
var base_component_1 = require("./../../common/base-components/base-component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var ModalService_1 = require("./../../common/services/ModalService");
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
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
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
        this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], LiquidacionProformaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], LiquidacionProformaComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild("smallSpinner"),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], LiquidacionProformaComponent.prototype, "spinnerSmallComponent", void 0);
    LiquidacionProformaComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/liquidacion/proforma/liquidacion.proforma.component.html?v=" + new Date().getTime(),
            providers: [{ provide: liquidacion_service_1.LiquidacionService, useClass: liquidacion_service_1.LiquidacionProformaService }]
        }),
        __metadata("design:paramtypes", [router_1.ActivatedRoute, router_1.Router, liquidacion_service_1.LiquidacionProformaService, NavService_1.NavService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], LiquidacionProformaComponent);
    return LiquidacionProformaComponent;
}(base_component_1.BaseComponent));
exports.LiquidacionProformaComponent = LiquidacionProformaComponent;
//# sourceMappingURL=liquidacion.proforma.component.js.map