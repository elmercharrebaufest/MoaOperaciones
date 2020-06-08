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
var BaseService_1 = require("./../services/BaseService");
var base_component_1 = require("./base-component");
var filtro_fecha_component_1 = require("./../view-child/filtro-fecha/filtro-fecha.component");
var mensaje_component_1 = require("./../view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../view-child/spinner/spinner.component");
var spinner_small_component_1 = require("./../view-child/spinner-small/spinner-small.component");
var dropdown_component_1 = require("./../view-child/dropdown/dropdown.component");
var SessionDataService_1 = require("./../services/SessionDataService");
var NavService_1 = require("./../services/NavService");
var SecurityService_1 = require("./../services/SecurityService");
var FloatMsgService_1 = require("./../services/FloatMsgService");
var ModalService_1 = require("./../services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
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
        _this.filtroFechaComponent = new filtro_fecha_component_1.FiltroFechaComponent();
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.itemsPerPageComponent = new dropdown_component_1.DropdownComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        _this.spinnerSmallComponent = new spinner_small_component_1.SpinnerSmallComponent();
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
        core_1.ViewChild(filtro_fecha_component_1.FiltroFechaComponent),
        __metadata("design:type", filtro_fecha_component_1.FiltroFechaComponent)
    ], ListBaseComponent.prototype, "filtroFechaComponent", void 0);
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], ListBaseComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(dropdown_component_1.DropdownComponent),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], ListBaseComponent.prototype, "itemsPerPageComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], ListBaseComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_small_component_1.SpinnerSmallComponent),
        __metadata("design:type", spinner_small_component_1.SpinnerSmallComponent)
    ], ListBaseComponent.prototype, "spinnerSmallComponent", void 0);
    ListBaseComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: "",
            providers: [BaseService_1.BaseService]
        }),
        __metadata("design:paramtypes", [BaseService_1.BaseService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], ListBaseComponent);
    return ListBaseComponent;
}(base_component_1.BaseComponent));
exports.ListBaseComponent = ListBaseComponent;
//# sourceMappingURL=list-base-component.js.map