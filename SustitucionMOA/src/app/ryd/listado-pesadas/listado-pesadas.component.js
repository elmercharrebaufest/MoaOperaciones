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
import { RYDService, RYDListadoPesadasService } from './../ryd.service';
import { RYDBaseComponent } from './../ryd.component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
var ListadoPesadasComponent = /** @class */ (function (_super) {
    __extends(ListadoPesadasComponent, _super);
    function ListadoPesadasComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroCommodity = Array();
        _this.filtroExportador = Array();
        _this.commoditySelected = "";
        _this.exportadorSelected = "";
        _this.fechaInicio = "";
        _this.fechaFin = "";
        _this.tituloArchivo = "ReporteListadoPesadas.xls";
        _this.spinnerSmallComponent = new SpinnerSmallComponent();
        return _this;
    }
    ListadoPesadasComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR LISTADO PESADAS"); };
    ListadoPesadasComponent.prototype.ngAfterViewInit = function () {
        $('.form_datetime').datetimepicker({
            format: 'dd/mm/yyyy',
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            pickTime: false,
            minView: 2,
            maxView: 4
        });
    };
    ListadoPesadasComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("ryd", "Listado de Pesadas");
    };
    ListadoPesadasComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.getFiltros();
    };
    ListadoPesadasComponent.prototype.getFiltros = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getFiltros().subscribe(function (result) {
            //this.spinnerComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
                _this.filtroCommodity = undefined;
                _this.filtroExportador = undefined;
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
                _this.filtroCommodity = undefined;
                _this.filtroExportador = undefined;
            }
            else {
                _this.filtroCommodity = result.data.commodities;
                _this.filtroExportador = result.data.exportadores;
            }
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
            _this.filtroCommodity = undefined;
            _this.filtroExportador = undefined;
        });
    };
    ListadoPesadasComponent.prototype.setFiltroCommodity = function (commodity) {
        this.commoditySelected = commodity;
    };
    ListadoPesadasComponent.prototype.setFiltroExportador = function (exportador) {
        this.exportadorSelected = exportador;
    };
    ListadoPesadasComponent.prototype.getDataEvent = function (fecha_inicio, fecha_fin) {
        this.fechaInicio = this.parseFecha(fecha_inicio);
        this.fechaFin = this.parseFecha(fecha_fin);
        this.getData();
        return false;
    };
    ListadoPesadasComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.data = undefined;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getListado(this.commoditySelected, this.exportadorSelected, this.fechaInicio, this.fechaFin).subscribe(function (result) {
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
    ListadoPesadasComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelListadoPesada(this.commoditySelected, this.exportadorSelected, this.fechaInicio, this.fechaFin).subscribe(function (result) {
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
    ListadoPesadasComponent.prototype.parseFecha = function (fechaRaw) {
        if (fechaRaw != undefined) {
            var fecha, hora;
            var fechaRawArray = fechaRaw.split(" ");
            if (fechaRawArray.length > 1) {
                var fechaArray = fechaRawArray[0].split("-");
                if (fechaArray.length > 2) {
                    fecha = fechaArray[2] + "/" + fechaArray[1] + "/" + fechaArray[0];
                }
                else {
                    return fechaRaw;
                }
                return fecha;
            }
        }
        return fechaRaw;
    };
    ListadoPesadasComponent.prototype.isVisible = function () {
        return this.data != undefined && this.data.pesadas.length != 0;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], ListadoPesadasComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], ListadoPesadasComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild(SpinnerSmallComponent),
        __metadata("design:type", SpinnerSmallComponent)
    ], ListadoPesadasComponent.prototype, "spinnerSmallComponent", void 0);
    ListadoPesadasComponent = __decorate([
        Component({
            selector: 'app-ryd-listado-pesadas',
            templateUrl: "listado-pesadas.component.html",
            providers: [{ provide: RYDService, useClass: RYDListadoPesadasService }]
        }),
        __metadata("design:paramtypes", [RYDListadoPesadasService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], ListadoPesadasComponent);
    return ListadoPesadasComponent;
}(RYDBaseComponent));
export { ListadoPesadasComponent };
//# sourceMappingURL=listado-pesadas.component.js.map