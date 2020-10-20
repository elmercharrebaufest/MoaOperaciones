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
import { CrearContratoService } from './crear-contrato.service';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';
var CrearContratoBaseComponent = /** @class */ (function (_super) {
    __extends(CrearContratoBaseComponent, _super);
    function CrearContratoBaseComponent(service, navService, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.esCorredorEnDataAgro = false;
        _this.localidades = [];
        _this.proveedores = [];
        _this.keyword = 'Nombre';
        _this.keyword2 = "RazonSocial";
        _this.autocompleteNotFoundText = "No encontrado";
        _this.datosContrato = new Array();
        _this.materiales = new Array();
        _this.monedas = new Array();
        _this.destinos = new Array();
        _this.campanias = new Array();
        _this.zona = new Array();
        _this.bolsasSelect = new Array();
        _this.bolsasConfirma = new Array();
        _this.bolsasFisico = new Array();
        _this.bolsasCarta = new Array();
        _this.condicionVendedor = new Array();
        _this.condicionFijacion = new Array();
        _this.datosCompraNet = null;
        _this.spinnerComponent = new SpinnerComponent();
        _this.mensajeComponent = new MensajeComponent();
        return _this;
    }
    CrearContratoBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); };
    CrearContratoBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/contrato/crear/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/contrato/crear/afijar', 'crear-contrato', 'A Fijar'),
        ]);
        //this.obteneDatosContrato();
    };
    CrearContratoBaseComponent.prototype.obteneDatosContrato = function (contrato) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato().subscribe(function (result) {
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
                var obj = JSON.parse(result);
                _this.datosContrato = obj;
                obj.Datos.material.forEach(function (element) {
                    var el = {
                        Id: element.MaterialId,
                        Descripcion: element.Descripcion
                    };
                    _this.materiales.push(el);
                });
                obj.Datos.Bolsa.forEach(function (element) {
                    var el = {
                        Id: element.Id,
                        Descripcion: element.Descripcion
                    };
                    _this.bolsasConfirma.push(el);
                    if (el.Descripcion == "Bs As" || el.Descripcion == "Rosario")
                        _this.bolsasFisico.push(el);
                    if (element.Descripcion == "Bs As")
                        _this.bolsasCarta.push(el);
                });
                obj.Datos.Clasificacion.forEach(function (element) {
                    var el = {
                        Id: element.Id,
                        Descripcion: element.Descripcion
                    };
                    _this.condicionVendedor.push(el);
                });
                obj.Datos.Zona.forEach(function (element) {
                    var el = {
                        Id: element.Id,
                        Descripcion: element.Descripcion
                    };
                    _this.zona.push(el);
                });
                obj.Datos.Destino.forEach(function (element) {
                    var el = {
                        Id: element.Id,
                        Descripcion: element.Descripcion
                    };
                    _this.destinos.push(el);
                });
                obj.Datos.campania.forEach(function (element) {
                    var el = {
                        Id: element.CampaniaId,
                        Descripcion: element.Descripcion
                    };
                    _this.campanias.push(el);
                });
                obj.Datos.moneda.forEach(function (element) {
                    var el = {
                        Id: element.MonedaId,
                        Descripcion: element.Descripcion
                    };
                    _this.monedas.push(el);
                });
                obj.Datos.Condicion.forEach(function (element) {
                    var el = {
                        Id: element.Id,
                        Descripcion: element.Descripcion
                    };
                    _this.condicionFijacion.push(el);
                });
                _this.validarDirecto(contrato);
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.validarDirecto = function (contrato) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.validarDirecto().subscribe(function (result) {
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
                var obj = JSON.parse(result);
                if (obj != null && obj > 0) {
                    contrato.CorredorId = obj;
                    _this.esCorredorEnDataAgro = true;
                }
                else {
                    _this.obtenerDatosCompraNet(contrato, "");
                    _this.esCorredorEnDataAgro = false;
                }
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.obtenerDatosCompraNet = function (contrato, idProveedorDataAgro) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.obtenerDatosCompraNet(idProveedorDataAgro).subscribe(function (result) {
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
                var obj = JSON.parse(result);
                _this.datosCompraNet = obj;
                contrato.ClasificacionId = obj.ClasificacionCompraNetId;
                contrato.BolsaId = obj.BolsaCompraNetId;
                if (obj.BolsaCompraNetId == 1) {
                    _this.bolsasSelect = _this.bolsasConfirma;
                }
                if (obj.BolsaCompraNetId == 2) {
                    _this.bolsasSelect = _this.bolsasFisico;
                }
                if (obj.BolsaCompraNetId == 3) {
                    _this.bolsasSelect = [];
                }
                if (obj.BolsaCompraNetId == 4) {
                    _this.bolsasSelect = _this.bolsasCarta;
                }
                contrato.BoletoId = obj.BoletoCompraNetId;
                if (obj.ClasificacionCompraNetId != 1) {
                    contrato.Consignatario = obj.Consignatario;
                    contrato.PlanCanje = obj.PlanCanje;
                }
                contrato.LocalidadId = obj.LocalidadId;
                contrato.ProvinciaId = obj.ProvinciaId;
                if (obj.ProvinciaId != null) {
                    _this.localidad = obj.Localidad + " (" + obj.Provincia + ")";
                }
                else {
                    _this.localidad = null;
                }
                if (obj.ProvinciaId != 1) {
                    contrato.EstablecimientoPropio = null;
                }
                //contrato.ClasificacionId = this.datosCompraNet.ComisionPorcentaje;
                contrato.DestinoId = _this.destinos[0].Id;
                contrato.MonedaId = _this.monedas[0].Id;
                contrato.MaterialId = _this.materiales[0].Id;
                contrato.CampanaId = _this.campanias[0].Id;
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.isVisibleProveedor = function () {
        return this.esCorredorEnDataAgro == true;
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], CrearContratoBaseComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], CrearContratoBaseComponent.prototype, "spinnerComponent", void 0);
    CrearContratoBaseComponent = __decorate([
        Component({
            selector: 'app-contrato',
            template: "",
            providers: [CrearContratoService]
        }),
        __metadata("design:paramtypes", [CrearContratoService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], CrearContratoBaseComponent);
    return CrearContratoBaseComponent;
}(ListBaseComponent));
export { CrearContratoBaseComponent };
//# sourceMappingURL=crear-contrato.component.js.map