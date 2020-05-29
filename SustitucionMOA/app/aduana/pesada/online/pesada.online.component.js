"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
var NavService_1 = require("./../../../common/services/NavService");
var FloatMsgService_1 = require("./../../../common/services/FloatMsgService");
var SessionDataService_1 = require("./../../../common/services/SessionDataService");
var SecurityService_1 = require("./../../../common/services/SecurityService");
var aduana_component_1 = require("./../../aduana.component");
var Formatter_1 = require("./../../../common/formatter/Formatter");
var ModalService_1 = require("./../../../common/services/ModalService");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
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
        this.horaInicio = Formatter_1.Formatter.parseHora(hora_inicio);
        this.horaFin = Formatter_1.Formatter.parseHora(hora_fin);
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
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], PesadaOnlineComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], PesadaOnlineComponent.prototype, "spinnerComponent", void 0);
    PesadaOnlineComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/aduana/pesada/online/pesada.online.component.html?v=" + new Date().getTime(),
            providers: [aduana_service_1.AduanaService]
        }),
        __metadata("design:paramtypes", [NavService_1.NavService, aduana_service_1.AduanaService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], PesadaOnlineComponent);
    return PesadaOnlineComponent;
}(aduana_component_1.AduanaBaseComponent));
exports.PesadaOnlineComponent = PesadaOnlineComponent;
//# sourceMappingURL=pesada.online.component.js.map