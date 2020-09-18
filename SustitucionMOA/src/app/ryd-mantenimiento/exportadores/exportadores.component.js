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
import { RYDMantenimientoService, RYDMantenimientoExportadorService } from './../ryd-mantenimiento.service';
import { RYDMantenimientoBaseComponent } from './../ryd-mantenimiento.component';
import { Balanza } from './../ryd-mantenimiento';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
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
        _this.balanzas = new Balanza();
        _this.descripcion = "";
        _this.almacenSAP = "";
        _this.guardarNuevo = new Array();
        _this.subscriptions = [];
        _this.InputExportadoresComponent = new DropdownComponent();
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
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
        ViewChild(DropdownComponent),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoExportadorComponent.prototype, "InputExportadoresComponent", void 0);
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], RYDMantenimientoExportadorComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], RYDMantenimientoExportadorComponent.prototype, "spinnerComponent", void 0);
    RYDMantenimientoExportadorComponent = __decorate([
        Component({
            selector: 'app-ryd-mantenimiento-exportadores',
            templateUrl: "exportadores.component.html",
            providers: [{ provide: RYDMantenimientoService, useClass: RYDMantenimientoExportadorService }]
        }),
        __metadata("design:paramtypes", [RYDMantenimientoExportadorService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], RYDMantenimientoExportadorComponent);
    return RYDMantenimientoExportadorComponent;
}(RYDMantenimientoBaseComponent));
export { RYDMantenimientoExportadorComponent };
//# sourceMappingURL=exportadores.component.js.map