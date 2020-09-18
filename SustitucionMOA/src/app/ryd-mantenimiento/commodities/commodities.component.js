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
import { RYDMantenimientoService, RYDMantenimientoCommoditiesService } from './../ryd-mantenimiento.service';
import { RYDMantenimientoBaseComponent } from './../ryd-mantenimiento.component';
import { Balanza, Commodity } from './../ryd-mantenimiento';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { ModalService } from './../../common/services/ModalService';
var RYDMantenimientoCommoditiesComponent = /** @class */ (function (_super) {
    __extends(RYDMantenimientoCommoditiesComponent, _super);
    function RYDMantenimientoCommoditiesComponent(service, navService, securityService, sessionDataService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.securityService = securityService;
        _this.sessionDataService = sessionDataService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroCommoditie = Array();
        _this.commoditieSelected = "";
        _this.balanzas = new Balanza();
        _this.descripcion = "";
        _this.materialSAP = "";
        _this.almacenOrigen = "";
        _this.guardarNuevo = new Commodity();
        _this.InputCommoditiesComponent = new DropdownComponent();
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    RYDMantenimientoCommoditiesComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("ABM COMMODITIES");
    };
    RYDMantenimientoCommoditiesComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("ryd-mantenimiento", "Commodities");
    };
    RYDMantenimientoCommoditiesComponent.prototype.getFiltros = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getFiltros().subscribe(function (result) {
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
                _this.filtroCommoditie = result.data;
            }
        }, function (error) {
            //this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    RYDMantenimientoCommoditiesComponent.prototype.setFiltroCommoditie = function (commoditie) {
        this.commoditieSelected = commoditie;
        this.getData();
    };
    RYDMantenimientoCommoditiesComponent.prototype.getDataInputs = function () {
        var _this = this;
        this.subscriptionDropDowns = this.service.getInputCommodities().subscribe(function (result) {
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
                _this.filtroCommoditie = result.data.commodities;
            }
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    RYDMantenimientoCommoditiesComponent.prototype.getData = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getCommodities(this.commoditieSelected).subscribe(function (result) {
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
                _this.materialSAP = result.data.MaterialSap;
                _this.almacenOrigen = result.data.AlmacenOrigen;
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    RYDMantenimientoCommoditiesComponent.prototype.guardarCommodity = function () {
        var _this = this;
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.guardarCommodity(this.materialSAP, this.almacenOrigen, this.descripcion).subscribe(function (result) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.vaciarInputs();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getDataInputs();
                    _this.commoditieSelected = null;
                    _this.endRequestBotones();
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
    RYDMantenimientoCommoditiesComponent.prototype.actualizarCommodity = function () {
        var _this = this;
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.actualizarCommodity(this.materialSAP, this.almacenOrigen, this.descripcion, this.commoditieSelected).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.vaciarInputs();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getDataInputs();
                    _this.endRequestBotones();
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
    RYDMantenimientoCommoditiesComponent.prototype.borrarCommodity = function () {
        var _this = this;
        this.initRequestBotones();
        try {
            this.unsubscribe();
            this.subscription = this.service.borrarCommodity(this.commoditieSelected).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.endRequestBotones();
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.vaciarInputs();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getDataInputs();
                    _this.endRequestBotones();
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
    RYDMantenimientoCommoditiesComponent.prototype.setInputCommodity = function (value) {
        this.balanzas.commodityId = value;
    };
    RYDMantenimientoCommoditiesComponent.prototype.vaciarInputs = function () {
        this.InputCommoditiesComponent.setSelectItem("");
        this.descripcion = "";
        this.materialSAP = "";
        this.almacenOrigen = "";
    };
    __decorate([
        ViewChild(DropdownComponent),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoCommoditiesComponent.prototype, "InputCommoditiesComponent", void 0);
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], RYDMantenimientoCommoditiesComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], RYDMantenimientoCommoditiesComponent.prototype, "spinnerComponent", void 0);
    RYDMantenimientoCommoditiesComponent = __decorate([
        Component({
            selector: 'app-ryd-mantenimiento-commodities',
            templateUrl: "commodities.component.html",
            providers: [{ provide: RYDMantenimientoService, useClass: RYDMantenimientoCommoditiesService }]
        }),
        __metadata("design:paramtypes", [RYDMantenimientoCommoditiesService, NavService, SecurityService, SessionDataService, FloatMsgService, ModalService])
    ], RYDMantenimientoCommoditiesComponent);
    return RYDMantenimientoCommoditiesComponent;
}(RYDMantenimientoBaseComponent));
export { RYDMantenimientoCommoditiesComponent };
//# sourceMappingURL=commodities.component.js.map