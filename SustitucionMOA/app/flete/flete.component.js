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
var flete_service_1 = require("./flete.service");
var list_base_component_1 = require("./../common/base-components/list-base-component");
var SessionDataService_1 = require("./../common/services/SessionDataService");
var SecurityService_1 = require("./../common/services/SecurityService");
var NavService_1 = require("./../common/services/NavService");
var FloatMsgService_1 = require("./../common/services/FloatMsgService");
var ModalService_1 = require("./../common/services/ModalService");
var Seccion_1 = require("./../common/models/Seccion");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var FleteBaseComponent = /** @class */ (function (_super) {
    __extends(FleteBaseComponent, _super);
    function FleteBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroProducto = null;
        _this.filtroCCPP = "";
        _this.filtroProforma = "";
        _this.filtroNroLegal = "";
        _this.productoSelected = "";
        return _this;
    }
    FleteBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CONSULTAR FLETE"); };
    FleteBaseComponent.prototype.ngOnInit = function () {
        var _this = this;
        if (this.modalServiceSusbcription == undefined) {
            this.modalServiceSusbcription = this.modalService.dataGuardarFlete.subscribe(function (dataGuardarFlete) {
                _this.guardarDatosProforma(dataGuardarFlete);
            });
        }
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion_1.Seccion('/flete/a-facturar', 'flete', 'Viajes A Facturar'), new Seccion_1.Seccion('/flete/pendiente', 'flete', 'Viajes Pendientes'), new Seccion_1.Seccion('/flete/facturado', 'flete', 'Viajes Facturados')]);
        this.getData();
    };
    FleteBaseComponent.prototype.setFiltroProducto = function (producto) {
        this.productoSelected = producto;
    };
    FleteBaseComponent.prototype.descargarPDF = function (proforma) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDF(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin, proforma).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.floatMsgService.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.floatMsgService.setInfoMsg(result.info);
            }
            else {
                var byteArray = new Uint8Array(result.data);
                var blob = new Blob([byteArray], { type: 'application/pdf' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, "Proforma(" + proforma + ").pdf");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Proforma(" + proforma + ").pdf";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.floatMsgService.setErrorMsg(error.message);
        });
        return false;
    };
    FleteBaseComponent.prototype.showModalCargaDatos = function (proforma) {
        var _this = this;
        this.floatMsgService.setMsgsEmpty();
        var viajeProforma = this.data.viajes.filter(function (viaje) { return _this.filterByProforma(viaje, proforma); })[0];
        if (viajeProforma != undefined) {
            this.modalService.openModalFlete("Datos Proforma", viajeProforma);
        }
        else {
            this.floatMsgService.setErrorMsg("El valor del número de proforma no es válido");
        }
        return false;
    };
    FleteBaseComponent.prototype.guardarDatosProforma = function (viajeProforma) {
        var _this = this;
        this.unsubscribe();
        var file = viajeProforma.pdf;
        viajeProforma.pdf = null;
        this.subscription = this.service.guardarDatosProforma(viajeProforma, file).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.modalService.setErrorMsjModal(result.error);
            }
            else if (result.info != undefined) {
                _this.modalService.setErrorMsjModal(result.error);
            }
            else {
                _this.modalService.setSuccessMsjModal("Los datos se han guardado correctamente");
            }
        }, function (error) {
            _this.modalService.setErrorMsjModal(error.message);
        });
        return false;
    };
    FleteBaseComponent.prototype.filterByProforma = function (obj, proforma) {
        return obj.proforma == proforma;
    };
    FleteBaseComponent.prototype.vaciarFiltros = function () {
        this.filtroProducto = null;
        this.filtroCCPP = "";
        this.filtroProforma = "";
        this.filtroNroLegal = "";
        this.productoSelected = "";
    };
    FleteBaseComponent.prototype.cargarFiltrosVariables = function (result) {
        if (result.filtroProducto != undefined)
            this.filtroProducto = result.filtroProducto.options;
    };
    FleteBaseComponent.prototype.isVisible = function () {
        return this.data && this.data.viajes && this.data.viajes.length != 0;
    };
    FleteBaseComponent.prototype.ngOnDestroy = function () {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        if (this.modalServiceSusbcription != undefined) {
            this.modalServiceSusbcription.unsubscribe();
        }
    };
    FleteBaseComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            template: "",
            providers: [flete_service_1.FleteService]
        }),
        __metadata("design:paramtypes", [flete_service_1.FleteService, NavService_1.NavService, SessionDataService_1.SessionDataService, SecurityService_1.SecurityService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], FleteBaseComponent);
    return FleteBaseComponent;
}(list_base_component_1.ListBaseComponent));
exports.FleteBaseComponent = FleteBaseComponent;
//# sourceMappingURL=flete.component.js.map