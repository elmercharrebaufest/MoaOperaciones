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
import { AduanaService } from './../../aduana.service';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { AduanaBaseComponent } from './../../aduana.component';
import { Formatter } from './../../../common/formatter/Formatter';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';
var PesadaHistoricaComponent = /** @class */ (function (_super) {
    __extends(PesadaHistoricaComponent, _super);
    function PesadaHistoricaComponent(navService, service, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, service, securityService, floatMsgService, modalService) || this;
        _this.navService = navService;
        _this.service = service;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.titulo = "";
        _this.itemsPerPage = 20;
        _this.pesadasList = null;
        _this.fechaInicio = new Date().toLocaleDateString('en-GB') + " 00:00";
        _this.fechaFin = new Date().toLocaleDateString('en-GB') + " 23:59";
        return _this;
    }
    PesadaHistoricaComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR PESADAS"); };
    PesadaHistoricaComponent.prototype.ngAfterViewInit = function () {
        $('.form_datetime_Inicio').datetimepicker({
            format: 'dd/mm/yyyy hh:ii',
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            defaultDate: 0
        });
        $('.form_datetime_Fin').datetimepicker({
            format: 'dd/mm/yyyy hh:ii',
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            defaultDate: 0
        });
    };
    PesadaHistoricaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("aduana", "Pesada Histórica");
    };
    PesadaHistoricaComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.getData();
    };
    PesadaHistoricaComponent.prototype.getData = function () {
        var _this = this;
        this.pesadasList = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getPesada("1029", this.fechaInicio, this.fechaFin).subscribe(function (result) {
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
                _this.pesadasList = result.pesadas;
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    PesadaHistoricaComponent.prototype.getDataEvent = function (fecha_inicio, fecha_fin) {
        this.fechaInicio = Formatter.parseFecha(fecha_inicio);
        this.fechaFin = Formatter.parseFecha(fecha_fin);
        this.getData();
        return false;
    };
    PesadaHistoricaComponent.prototype.isVisible = function () {
        return this.pesadasList != null;
    };
    PesadaHistoricaComponent.prototype.showModalTableResponsive = function (pesada) {
        this.modalService.openModalTableResponsive("Pesada", [
            { etiqueta: "Fecha y Hora Inicio", valor: pesada.fechaIncio },
            { etiqueta: "Balanza", valor: pesada.balanza },
            { etiqueta: "Total Embarcado [Kg]", valor: pesada.totalEmbarcado },
            { etiqueta: "Commodity", valor: pesada.commodity },
            { etiqueta: "Bodega", valor: pesada.bodega },
            { etiqueta: "Destino", valor: pesada.destino },
            { etiqueta: "Exportador", valor: pesada.exportador },
            { etiqueta: "Vapor", valor: pesada.vapor },
            { etiqueta: "Peso Programado [Kg]", valor: pesada.pesoProgramado }
        ]);
        return false;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], PesadaHistoricaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], PesadaHistoricaComponent.prototype, "spinnerComponent", void 0);
    PesadaHistoricaComponent = __decorate([
        Component({
            selector: 'app-aduana-pesada-historica',
            templateUrl: "pesada.historica.component.html",
            providers: [AduanaService]
        }),
        __metadata("design:paramtypes", [NavService, AduanaService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], PesadaHistoricaComponent);
    return PesadaHistoricaComponent;
}(AduanaBaseComponent));
export { PesadaHistoricaComponent };
//# sourceMappingURL=pesada.historica.component.js.map