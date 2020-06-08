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
var pago_service_1 = require("./../pago.service");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var spinner_small_component_1 = require("./../../common/view-child/spinner-small/spinner-small.component");
var base_component_1 = require("./../../common/base-components/base-component");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var ModalService_1 = require("./../../common/services/ModalService");
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
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
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
        this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], PagoDetalleComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], PagoDetalleComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild("smallSpinner"),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], PagoDetalleComponent.prototype, "spinnerSmallComponent", void 0);
    PagoDetalleComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/pago/detalle/pago.detalle.component.html?v=" + new Date().getTime(),
            providers: [pago_service_1.PagoService]
        }),
        __metadata("design:paramtypes", [router_1.ActivatedRoute, router_1.Router, pago_service_1.PagoService, NavService_1.NavService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], PagoDetalleComponent);
    return PagoDetalleComponent;
}(base_component_1.BaseComponent));
exports.PagoDetalleComponent = PagoDetalleComponent;
//# sourceMappingURL=pago.detalle.component.js.map