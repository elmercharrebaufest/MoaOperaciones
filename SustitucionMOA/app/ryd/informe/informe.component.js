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
var ryd_service_1 = require("./../ryd.service");
var ryd_component_1 = require("./../ryd.component");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var spinner_small_component_1 = require("./../../common/view-child/spinner-small/spinner-small.component");
var dropdown_component_1 = require("./../../common/view-child/dropdown/dropdown.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var DataService_1 = require("./../../common/services/DataService");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var ModalService_1 = require("./../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var InformeComponent = /** @class */ (function (_super) {
    __extends(InformeComponent, _super);
    function InformeComponent(service, navService, dataService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.dataService = dataService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroBalanza = Array();
        _this.balanzaSelected = "";
        _this.tituloArchivo = "ReporteInforme.xls";
        _this.inputBalanzaComponent = new dropdown_component_1.DropdownComponent();
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        _this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
        return _this;
    }
    InformeComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR INFORME"); };
    InformeComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("ryd", "Informe");
    };
    InformeComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.getFiltros();
    };
    InformeComponent.prototype.getFiltros = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getFiltros().subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
                _this.filtroBalanza = null;
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
                _this.filtroBalanza = null;
            }
            else {
                _this.filtroBalanza = result.data;
            }
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
            _this.filtroBalanza = null;
        });
    };
    InformeComponent.prototype.setFiltroBalanza = function (balanza) {
        this.balanzaSelected = balanza;
        this.getData();
    };
    InformeComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = undefined;
        this.unsubscribe();
        this.subscription = this.service.getInforme(this.balanzaSelected).subscribe(function (result) {
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
        return false;
    };
    InformeComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcel(this.balanzaSelected).subscribe(function (result) {
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
                    window.navigator.msSaveOrOpenBlob(blob, _this.tituloArchivo);
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = _this.tituloArchivo;
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
    InformeComponent.prototype.isVisible = function () {
        return this.data != undefined && this.data.pesadas.length != 0;
    };
    __decorate([
        core_1.ViewChild(dropdown_component_1.DropdownComponent),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], InformeComponent.prototype, "inputBalanzaComponent", void 0);
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], InformeComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], InformeComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_small_component_1.SpinnerSmallComponent),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], InformeComponent.prototype, "spinnerSmallComponent", void 0);
    InformeComponent = __decorate([
        core_1.Component({
            selector: 'app-ryd-informe',
            templateUrl: "./app/ryd/informe/informe.component.html?v=" + new Date().getTime(),
            providers: [{ provide: ryd_service_1.RYDService, useClass: ryd_service_1.RYDInformeService }]
        }),
        __metadata("design:paramtypes", [ryd_service_1.RYDInformeService, NavService_1.NavService, DataService_1.DataService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], InformeComponent);
    return InformeComponent;
}(ryd_component_1.RYDBaseComponent));
exports.InformeComponent = InformeComponent;
//# sourceMappingURL=informe.component.js.map