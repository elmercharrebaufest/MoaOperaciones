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
var aduana_service_1 = require("./../../aduana.service");
var mensaje_component_1 = require("./../../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../../common/view-child/spinner/spinner.component");
var aduana_component_1 = require("./../../aduana.component");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var Formatter_1 = require("./../../../common/formatter/Formatter");
var NavService_1 = require("./../../../common/services/NavService");
var FloatMsgService_1 = require("./../../../common/services/FloatMsgService");
var SessionDataService_1 = require("./../../../common/services/SessionDataService");
var SecurityService_1 = require("./../../../common/services/SecurityService");
var ModalService_1 = require("./../../../common/services/ModalService");
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
        this.fechaInicio = Formatter_1.Formatter.parseFecha(fecha_inicio);
        this.fechaFin = Formatter_1.Formatter.parseFecha(fecha_fin);
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], PesadaHistoricaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], PesadaHistoricaComponent.prototype, "spinnerComponent", void 0);
    PesadaHistoricaComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/aduana/pesada/historica/pesada.historica.component.html?v=" + new Date().getTime(),
            providers: [aduana_service_1.AduanaService]
        }),
        __metadata("design:paramtypes", [NavService_1.NavService, aduana_service_1.AduanaService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], PesadaHistoricaComponent);
    return PesadaHistoricaComponent;
}(aduana_component_1.AduanaBaseComponent));
exports.PesadaHistoricaComponent = PesadaHistoricaComponent;
//# sourceMappingURL=pesada.historica.component.js.map