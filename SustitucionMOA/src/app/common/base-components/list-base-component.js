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
import { BaseService } from './../services/BaseService';
import { BaseComponent } from './base-component';
import { FiltroFechaComponent } from './../view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../view-child/spinner-small/spinner-small.component';
import { DropdownComponent } from './../view-child/dropdown/dropdown.component';
import { SessionDataService } from './../services/SessionDataService';
import { NavService } from './../services/NavService';
import { SecurityService } from './../services/SecurityService';
import { FloatMsgService } from './../services/FloatMsgService';
import { ModalService } from './../services/ModalService';
var ListBaseComponent = /** @class */ (function (_super) {
    __extends(ListBaseComponent, _super);
    function ListBaseComponent(service, navService, sessionDataService, securytiService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securytiService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securytiService = securytiService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.tituloArchivo = "";
        _this.itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
        _this.tipoDropdown = 'numberItems';
        _this.orderedByColumn = "fechaDescargaDate";
        _this.orderDirection = 1;
        _this.data = null;
        _this.filtroFechaComponent = new FiltroFechaComponent();
        _this.mensajeComponent = new MensajeComponent();
        _this.itemsPerPageComponent = new DropdownComponent();
        _this.spinnerComponent = new SpinnerComponent();
        _this.spinnerSmallComponent = new SpinnerSmallComponent();
        return _this;
    }
    ListBaseComponent.prototype.checkPermisos = function () { };
    ListBaseComponent.prototype.ngOnInit = function () {
        this.checkPermisos();
        this.getData();
    };
    ListBaseComponent.prototype.setItemsPerPage = function (numberOfItems) {
        this.itemsPerPage = numberOfItems;
    };
    ListBaseComponent.prototype.orderColumnBy = function (column) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        }
        else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    };
    ListBaseComponent.prototype.getData = function () {
        var _this = this;
        this.data = null;
        this.vaciarFiltros();
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getData(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(function (result) {
            _this.data = null;
            _this.mensajeComponent.setMsgsEmpty();
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
                _this.cargarFiltrosVariables(result);
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ListBaseComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcel(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(function (result) {
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
                //var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var blob = new Blob([result], { type: 'application/octet-stream' });
                //var uagent = navigator.userAgent.toLowerCase();
                //if (/safari/.test(uagent) && !/chrome/.test(uagent)) {
                //console.log("SAFARI");
                //} else 
                if (window.navigator.msSaveOrOpenBlob) {
                    //IE11
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
                    //return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false; // <- Prevent href del a
    };
    ListBaseComponent.prototype.vaciarFiltros = function () { };
    ListBaseComponent.prototype.cargarFiltrosVariables = function (result) { };
    __decorate([
        ViewChild(FiltroFechaComponent),
        __metadata("design:type", FiltroFechaComponent)
    ], ListBaseComponent.prototype, "filtroFechaComponent", void 0);
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], ListBaseComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(DropdownComponent),
        __metadata("design:type", DropdownComponent)
    ], ListBaseComponent.prototype, "itemsPerPageComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], ListBaseComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild(SpinnerSmallComponent),
        __metadata("design:type", SpinnerSmallComponent)
    ], ListBaseComponent.prototype, "spinnerSmallComponent", void 0);
    ListBaseComponent = __decorate([
        Component({
            selector: 'app-list-base',
            template: "",
            providers: [BaseService]
        }),
        __metadata("design:paramtypes", [BaseService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], ListBaseComponent);
    return ListBaseComponent;
}(BaseComponent));
export { ListBaseComponent };
//# sourceMappingURL=list-base-component.js.map