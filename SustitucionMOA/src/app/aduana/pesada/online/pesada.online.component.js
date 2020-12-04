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
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { AduanaBaseComponent } from './../../aduana.component';
import { Formatter } from './../../../common/formatter/Formatter';
import { ModalService } from './../../../common/services/ModalService';
var PesadaOnlineComponent = /** @class */ (function (_super) {
    __extends(PesadaOnlineComponent, _super);
    function PesadaOnlineComponent(navService, service, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, service, securityService, floatMsgService, modalService) || this;
        _this.navService = navService;
        _this.service = service;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.titulo = "";
        _this.fecha = new Date().toLocaleDateString('en-GB');
        _this.horaInicio = "00:00";
        _this.horaFin = "23:59";
        _this.itemsPerPage = 20;
        _this.pesadasList = null;
        return _this;
    }
    PesadaOnlineComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR PESADAS"); };
    PesadaOnlineComponent.prototype.ngAfterViewInit = function () {
        $('.form_datetime_Inicio').datetimepicker({
            format: 'hh:ii',
            language: 'es',
            autoclose: 1,
            startView: 1,
            forceParse: 0,
            minuteStep: 1
        });
        $('.form_datetime_Fin').datetimepicker({
            format: 'hh:ii',
            language: 'es',
            autoclose: 1,
            startView: 1,
            forceParse: 0,
            minuteStep: 1
        });
    };
    PesadaOnlineComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("aduana", "Pesada Online");
    };
    PesadaOnlineComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.getData();
    };
    PesadaOnlineComponent.prototype.getData = function () {
        var _this = this;
        this.pesadasList = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getPesada("1029", this.fecha + " " + this.horaInicio, this.fecha + " " + this.horaFin).subscribe(function (result) {
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
    PesadaOnlineComponent.prototype.getDataEvent = function (hora_inicio, hora_fin) {
        this.horaInicio = Formatter.parseHora(hora_inicio);
        this.horaFin = Formatter.parseHora(hora_fin);
        this.getData();
        return false;
    };
    PesadaOnlineComponent.prototype.isVisible = function () {
        return this.pesadasList != null;
    };
    PesadaOnlineComponent.prototype.horaCambio = function (event) {
        this.horaFin = event;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], PesadaOnlineComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], PesadaOnlineComponent.prototype, "spinnerComponent", void 0);
    PesadaOnlineComponent = __decorate([
        Component({
            selector: 'app-aduana-pesada-online',
            templateUrl: "pesada.online.component.html",
            providers: [AduanaService]
        }),
        __metadata("design:paramtypes", [NavService, AduanaService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], PesadaOnlineComponent);
    return PesadaOnlineComponent;
}(AduanaBaseComponent));
export { PesadaOnlineComponent };
//# sourceMappingURL=pesada.online.component.js.map