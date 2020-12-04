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
import { RYDMantenimientoService, RYDMantenimientoBalanzaService } from './../ryd-mantenimiento.service';
import { RYDMantenimientoBaseComponent } from './../ryd-mantenimiento.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
var RYDMantenimientoBalanzaComponent = /** @class */ (function (_super) {
    __extends(RYDMantenimientoBalanzaComponent, _super);
    function RYDMantenimientoBalanzaComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.filtroTipo = Array();
        _this.filtroCodigoCabezal = Array();
        _this.filtroITC = Array();
        _this.filtroNroPuesto = Array();
        _this.filtroTipoAcceso = Array();
        _this.tipoSelected = "";
        _this.codigoCabezalSelected = "";
        _this.itcSelected = "";
        _this.nroPuestoSelected = "";
        _this.tipoAccesoSelected = "";
        _this.codigo = "";
        _this.descripcionBusqueda = "";
        _this.tipoBusqueda = "";
        _this.cabezalBusqueda = "";
        _this.numeroSAPBusqueda = "";
        _this.modelBusquedaVisible = false;
        _this.codigoInputDisable = false;
        _this.itemsPerPage = 5;
        _this.InputTipoComponent = new DropdownComponent();
        _this.InputCodigoCabezalComponent = new DropdownComponent();
        _this.InputITCComponent = new DropdownComponent();
        _this.InputNroPuestoComponent = new DropdownComponent();
        _this.InputTipoAccesoComponent = new DropdownComponent();
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    RYDMantenimientoBalanzaComponent.prototype.checkPermisos = function () {
        this.securityService.tienePermisoRedirect("ABM BALANZAS");
    };
    RYDMantenimientoBalanzaComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("ryd-mantenimiento", "Balanzas");
    };
    RYDMantenimientoBalanzaComponent.prototype.getDataInputs = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getInputDropDown().subscribe(function (result) {
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
                _this.filtroTipo = result.data.tipo;
                _this.filtroCodigoCabezal = result.data.codigoCabezal;
                _this.filtroITC = result.data.itc;
                _this.filtroNroPuesto = result.data.nroPuesto;
                _this.filtroTipoAcceso = result.data.tipoAcceso;
            }
        }, function (error) {
            //this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    RYDMantenimientoBalanzaComponent.prototype.getNroPuesto = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.filtroNroPuesto = null;
        this.service.getNroPuesto(this.itcSelected).subscribe(function (result) {
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
                _this.filtroNroPuesto = result.data.nroPuesto;
                if (_this.nroPuestoSelected != undefined && _this.nroPuestoSelected != "") {
                    _this.InputNroPuestoComponent.setSelectItem(_this.nroPuestoSelected);
                }
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    RYDMantenimientoBalanzaComponent.prototype.aplicarTabla = function (codigo) {
        this.vaciarInputs();
        this.codigo = codigo;
        this.aplicar();
        return false;
    };
    RYDMantenimientoBalanzaComponent.prototype.aplicar = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.aplicar(this.codigo).subscribe(function (result) {
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
                _this.codigoInputDisable = true;
                _this.data = result.data;
                if (result.data.length > 0) {
                    _this.descripcion = result.data[0].descripcion;
                    _this.automatico = result.data[0].automatico;
                    _this.toleria = result.data[0].toleria;
                    _this.centroEmisor = result.data[0].centroEmisor;
                    _this.tolerX = result.data[0].tolerX;
                    _this.pesoMaximo = result.data[0].pesoMaximo;
                    _this.codigoSAP = result.data[0].codigoSAP;
                    _this.setFiltroTipo(result.data[0].tipo);
                    _this.setFiltroCodigoCabezal(result.data[0].codigoCabezal);
                    _this.setFiltroNroPuesto(result.data[0].nroPuesto);
                    _this.setFiltroTipoAcceso(result.data[0].tipoAcceso);
                    _this.setFiltroITC(result.data[0].itc);
                }
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    RYDMantenimientoBalanzaComponent.prototype.modalBusquedaShow = function () {
        this.modelBusquedaVisible = true;
        this.codigoInputDisable = true;
        return false;
    };
    RYDMantenimientoBalanzaComponent.prototype.modalBusquedaHide = function () {
        this.modelBusquedaVisible = false;
        this.codigoInputDisable = false;
        this.vaciarInputsBusqueda();
        return false;
    };
    RYDMantenimientoBalanzaComponent.prototype.isVisibleSearch = function () {
        return this.balanzas != undefined && this.balanzas.length != 0;
    };
    RYDMantenimientoBalanzaComponent.prototype.buscarBalanza = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.buscarBalanza(this.descripcionBusqueda, this.tipoBusqueda, this.cabezalBusqueda, this.numeroSAPBusqueda).subscribe(function (result) {
                _this.visibleButton = true;
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
                    _this.balanzas = result.data;
                }
            }, function (error) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoBalanzaComponent.prototype.guardarBalanza = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.guardarBalanza(this.codigo, this.descripcion, this.automatico, this.toleria, this.centroEmisor, this.tolerX, this.tipoSelected, this.pesoMaximo, this.codigoSAP, this.codigoCabezalSelected, this.itcSelected, this.nroPuestoSelected, this.tipoAccesoSelected).subscribe(function (result) {
                _this.visibleButton = true;
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
                    _this.vaciarInputs();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
            }, function (error) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoBalanzaComponent.prototype.actualizarBalanza = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.actualizarBalanza(this.codigo, this.descripcion, this.automatico, this.toleria, this.centroEmisor, this.tolerX, this.tipoSelected, this.pesoMaximo, this.codigoSAP, this.codigoCabezalSelected, this.itcSelected, this.nroPuestoSelected, this.tipoAccesoSelected).subscribe(function (result) {
                _this.visibleButton = true;
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
                    _this.vaciarInputs();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
            }, function (error) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoBalanzaComponent.prototype.borrarBalanza = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.borrarBalanza(this.codigo).subscribe(function (result) {
                _this.visibleButton = true;
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
                    _this.vaciarInputs();
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
            }, function (error) {
                _this.visibleButton = true;
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    RYDMantenimientoBalanzaComponent.prototype.cancelarBalanza = function () {
        this.codigoInputDisable = false;
        this.codigo = "";
        this.vaciarInputs();
        return false;
    };
    RYDMantenimientoBalanzaComponent.prototype.setFiltroTipo = function (tipo) {
        this.tipoSelected = tipo;
        this.InputTipoComponent.setSelectItem(tipo);
    };
    RYDMantenimientoBalanzaComponent.prototype.setFiltroCodigoCabezal = function (codigoCabezal) {
        this.codigoCabezalSelected = codigoCabezal;
        this.InputCodigoCabezalComponent.setSelectItem(codigoCabezal);
    };
    RYDMantenimientoBalanzaComponent.prototype.setFiltroITC = function (itc) {
        this.itcSelected = itc;
        this.InputITCComponent.setSelectItem(itc);
        this.getNroPuesto();
    };
    RYDMantenimientoBalanzaComponent.prototype.setFiltroNroPuesto = function (nroPuesto) {
        this.nroPuestoSelected = nroPuesto;
    };
    RYDMantenimientoBalanzaComponent.prototype.setFiltroTipoAcceso = function (tipoAcceso) {
        this.tipoAccesoSelected = tipoAcceso;
        this.InputTipoAccesoComponent.setSelectItem(tipoAcceso);
    };
    RYDMantenimientoBalanzaComponent.prototype.vaciarInputs = function () {
        this.codigo = "";
        this.codigoInputDisable = false;
        this.setFiltroTipo("");
        this.setFiltroCodigoCabezal("");
        this.setFiltroITC("");
        this.setFiltroNroPuesto("");
        this.setFiltroTipoAcceso("");
        this.data = null;
        this.descripcion = "";
        this.automatico = "";
        this.toleria = "";
        this.centroEmisor = "";
        this.tolerX = "";
        this.pesoMaximo = "";
        this.codigoSAP = "";
        this.vaciarInputsBusqueda();
        this.modelBusquedaVisible = false;
    };
    ;
    RYDMantenimientoBalanzaComponent.prototype.vaciarInputsBusqueda = function () {
        this.descripcionBusqueda = "";
        this.tipoBusqueda = "";
        this.cabezalBusqueda = "";
        this.numeroSAPBusqueda = "";
        this.balanzas = null;
    };
    __decorate([
        ViewChild("dropdown_tipo"),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "InputTipoComponent", void 0);
    __decorate([
        ViewChild("dropdown_codCabezal"),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "InputCodigoCabezalComponent", void 0);
    __decorate([
        ViewChild("dropdown_itc"),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "InputITCComponent", void 0);
    __decorate([
        ViewChild("dropdown_nroPuesto"),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "InputNroPuestoComponent", void 0);
    __decorate([
        ViewChild("dropdown_tipoAcceso"),
        __metadata("design:type", DropdownComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "InputTipoAccesoComponent", void 0);
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], RYDMantenimientoBalanzaComponent.prototype, "spinnerComponent", void 0);
    RYDMantenimientoBalanzaComponent = __decorate([
        Component({
            selector: 'app-ryd-mantenimiento-balanzas',
            templateUrl: "balanzas.component.html",
            providers: [{ provide: RYDMantenimientoService, useClass: RYDMantenimientoBalanzaService }]
        }),
        __metadata("design:paramtypes", [RYDMantenimientoBalanzaService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], RYDMantenimientoBalanzaComponent);
    return RYDMantenimientoBalanzaComponent;
}(RYDMantenimientoBaseComponent));
export { RYDMantenimientoBalanzaComponent };
//# sourceMappingURL=balanzas.component.js.map