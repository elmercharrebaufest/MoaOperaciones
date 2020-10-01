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
import { RYDService, RYDInformeService } from './../ryd.service';
import { RYDBaseComponent } from './../ryd.component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { DataService } from './../../common/services/DataService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
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
        _this.inputBalanzaComponent = new DropdownComponent();
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.spinnerSmallComponent = new SpinnerSmallComponent();
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
        ViewChild(DropdownComponent),
        __metadata("design:type", DropdownComponent)
    ], InformeComponent.prototype, "inputBalanzaComponent", void 0);
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], InformeComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], InformeComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild(SpinnerSmallComponent),
        __metadata("design:type", SpinnerSmallComponent)
    ], InformeComponent.prototype, "spinnerSmallComponent", void 0);
    InformeComponent = __decorate([
        Component({
            selector: 'app-ryd-informe',
            templateUrl: "informe.component.html",
            providers: [{ provide: RYDService, useClass: RYDInformeService }]
        }),
        __metadata("design:paramtypes", [RYDInformeService, NavService, DataService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], InformeComponent);
    return InformeComponent;
}(RYDBaseComponent));
export { InformeComponent };
//# sourceMappingURL=informe.component.js.map