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
var ryd_mantenimiento_service_1 = require("./../ryd-mantenimiento.service");
var ryd_mantenimiento_component_1 = require("./../ryd-mantenimiento.component");
var ryd_mantenimiento_1 = require("./../ryd-mantenimiento");
var mensaje_component_1 = require("./../../common/view-child/mensaje/mensaje.component");
var spinner_component_1 = require("./../../common/view-child/spinner/spinner.component");
var NavService_1 = require("./../../common/services/NavService");
var FloatMsgService_1 = require("./../../common/services/FloatMsgService");
var SecurityService_1 = require("./../../common/services/SecurityService");
var SessionDataService_1 = require("./../../common/services/SessionDataService");
var ModalService_1 = require("./../../common/services/ModalService");
var dropdown_component_1 = require("./../../common/view-child/dropdown/dropdown.component");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var RYDMantenimientoExportadorComponent = /** @class */ (function (_super) {
    __extends(RYDMantenimientoExportadorComponent, _super);
    function RYDMantenimientoExportadorComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroExportador = Array();
        _this.exportadorSelected = "";
        _this.balanzas = new ryd_mantenimiento_1.Balanza();
        _this.descripcion = "";
        _this.almacenSAP = "";
        _this.guardarNuevo = new Array();
        _this.subscriptions = [];
        _this.InputExportadoresComponent = new dropdown_component_1.DropdownComponent();
        _this.mensajeComponent = new mensaje_component_1.MensajeComponent();
        _this.spinnerComponent = new spinner_component_1.SpinnerComponent();
        return _this;
    }
    RYDMantenimientoExportadorComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("ABM EXPORTADORES");
    };
    RYDMantenimientoExportadorComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("ryd-mantenimiento", "Exportadores");
    };
    RYDMantenimientoExportadorComponent.prototype.getFiltros = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.service.getFiltros().subscribe(function (result) {
            //this.spinnerComponent.hideIt();
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
                _this.filtroExportador = result.data;
            }
        }, function (error) {
            //this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    RYDMantenimientoExportadorComponent.prototype.setFiltroExportador = function (exportador) {
        this.exportadorSelected = exportador;
        this.getData();
    };
    RYDMantenimientoExportadorComponent.prototype.getDataInputs = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getInputExportador().subscribe(function (result) {
            //this.spinnerComponent.hideIt();
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
                _this.filtroExportador = result.data.exportadores;
            }
        }, function (error) {
            //this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    RYDMantenimientoExportadorComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getExportador(this.exportadorSelected).subscribe(function (result) {
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
                _this.descripcion = result.data.label;
                _this.almacenSAP = result.data.AlmacenSap;
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    RYDMantenimientoExportadorComponent.prototype.guardarExportador = function () {
        var _this = this;
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.guardarExportador(this.almacenSAP, this.descripcion).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.endRequestBotones();
                }
                else if (result.info != undefined) {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.vaciarInputs();
                    _this.getDataInputs();
                    _this.endRequestBotones();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
            }, function (error) {
                _this.endRequestBotones();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.endRequestBotones();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoExportadorComponent.prototype.actualizarExportador = function () {
        var _this = this;
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.actualizarExportador(this.almacenSAP, this.descripcion, this.exportadorSelected).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                    _this.endRequestBotones();
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                    _this.endRequestBotones();
                }
                else {
                    _this.vaciarInputs();
                    _this.getDataInputs();
                    _this.endRequestBotones();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
                _this.endRequestBotones();
            });
        }
        catch (e) {
            this.endRequestBotones();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoExportadorComponent.prototype.borrarExportador = function () {
        var _this = this;
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.borrarExportador(this.exportadorSelected).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                    _this.endRequestBotones();
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                    _this.endRequestBotones();
                }
                else {
                    _this.vaciarInputs();
                    _this.getDataInputs();
                    _this.endRequestBotones();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
            }, function (error) {
                _this.endRequestBotones();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.endRequestBotones();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoExportadorComponent.prototype.setInputExportador = function (value) {
        this.balanzas.exportadorId = value;
    };
    RYDMantenimientoExportadorComponent.prototype.vaciarInputs = function () {
        this.InputExportadoresComponent.setSelectItem("");
        this.descripcion = "";
        this.almacenSAP = "";
    };
    __decorate([
        core_1.ViewChild(dropdown_component_1.DropdownComponent),
        __metadata("design:type", dropdown_component_1.DropdownComponent)
    ], RYDMantenimientoExportadorComponent.prototype, "InputExportadoresComponent", void 0);
    __decorate([
        core_1.ViewChild(mensaje_component_1.MensajeComponent),
        __metadata("design:type", mensaje_component_1.MensajeComponent)
    ], RYDMantenimientoExportadorComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        core_1.ViewChild(spinner_component_1.SpinnerComponent),
        __metadata("design:type", spinner_component_1.SpinnerComponent)
    ], RYDMantenimientoExportadorComponent.prototype, "spinnerComponent", void 0);
    RYDMantenimientoExportadorComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: "./app/ryd-mantenimiento/exportadores/exportadores.component.html?v=" + new Date().getTime(),
            providers: [{ provide: ryd_mantenimiento_service_1.RYDMantenimientoService, useClass: ryd_mantenimiento_service_1.RYDMantenimientoExportadorService }]
        }),
        __metadata("design:paramtypes", [ryd_mantenimiento_service_1.RYDMantenimientoExportadorService, NavService_1.NavService, SecurityService_1.SecurityService, SessionDataService_1.SessionDataService, FloatMsgService_1.FloatMsgService, ModalService_1.ModalService])
    ], RYDMantenimientoExportadorComponent);
    return RYDMantenimientoExportadorComponent;
}(ryd_mantenimiento_component_1.RYDMantenimientoBaseComponent));
exports.RYDMantenimientoExportadorComponent = RYDMantenimientoExportadorComponent;
//# sourceMappingURL=exportadores.component.js.map