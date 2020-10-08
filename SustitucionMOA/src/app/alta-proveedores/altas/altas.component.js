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
import { NavService } from '../../common/services/NavService';
import { BaseComponent } from '../../common/base-components/base-component';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { AltaEmpresaService } from './altas.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { Empresa } from './Empresa';
import { Archivo } from '../../common/models/archivo';
var AltasComponent = /** @class */ (function (_super) {
    __extends(AltasComponent, _super);
    function AltasComponent(altaEmpresaService, service, navService, sessionDataService, securytiService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securytiService, floatMsgService, modalService) || this;
        _this.altaEmpresaService = altaEmpresaService;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securytiService = securytiService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.selectedEstado = "q";
        _this.orderedByColumn = "id";
        _this.empresaSeleccionada = new Empresa();
        _this.empresaEstadoSeleccionada = 0;
        _this.orderDirection = 1;
        _this.itemsPerPage = 20;
        _this.observaciones = "";
        _this.observacionesProveedor = "";
        _this.mensajeError = "";
        _this.listaArchivos = [];
        _this.empleados = [];
        _this.funcionarios = [];
        _this.relacionConEmpleados = "";
        _this.relacionConFuncionarios = "";
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerComponent = new SpinnerComponent();
        return _this;
    }
    AltasComponent.prototype.ngOnInit = function () {
        this.getEstados();
    };
    AltasComponent.prototype.verDetalle = function () {
        this.navService.navegarSeccion('/proveedor-detalle');
        return false;
    };
    AltasComponent.prototype.getEmpresa = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.altaEmpresaService.getEmpresas().subscribe(function (result) {
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
                    _this.dataFiltered = result.data;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    AltasComponent.prototype.getEstados = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.altaEmpresaService.getEstados().subscribe(function (result) {
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
                    _this.estados = result.data;
                    _this.getEmpresa();
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    AltasComponent.prototype.isVisible = function () {
        return this.data && this.data.length != 0;
    };
    AltasComponent.prototype.isVisibleError = function () {
        return this.mensajeError != "";
    };
    AltasComponent.prototype.orderColumnBy = function (column) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        }
        else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    };
    AltasComponent.prototype.cambiarEstado = function (estadoId) {
        var _this = this;
        if (estadoId == 4 && this.observacionesProveedor == "") {
            this.mensajeError = "Debe ingresar una observacion para el Proveedor.";
            return false;
        }
        if (estadoId == 7 && this.empresaSeleccionada.SISAEstadoCuit != "1" && (this.empresaSeleccionada.EstadoSIPER == "" || this.empresaSeleccionada.EstadoSIPER == null)) {
            this.mensajeError = "Debe ingresar Estado en SIPER.";
            return false;
        }
        this.spinnerModal.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.altaEmpresaService.setEstadoAprobacion(this.empresaSeleccionada.Id, estadoId, this.observaciones, this.observacionesProveedor, this.empresaSeleccionada.EstadoSIPER).subscribe(function (result) {
                _this.getEmpresa();
                _this.spinnerModal.hideIt();
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
                    _this.mensajeComponent.setSuccessMsg(result.data);
                }
                _this.observaciones = "";
                _this.observacionesProveedor = "";
                _this.mensajeError = "";
                document.getElementById("hidemyModal").click();
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerModal.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    AltasComponent.prototype.abrirModal = function (empresa) {
        this.empresaSeleccionada = empresa;
        this.observaciones = "";
        this.observacionesProveedor = "";
        this.mensajeError = "";
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.cargarSolicitudUsuario(empresa.Mail, empresa.Id);
        this.obtenerArchivosSubidos(empresa.Mail, empresa.Id);
        document.getElementById("openModalHiddenButton").click();
        return false;
    };
    AltasComponent.prototype.obtenerArchivosSubidos = function (mail, proveedorId) {
        var _this = this;
        this.subscription = this.service.obtenerArchivosSubidos(mail, proveedorId).subscribe(function (result) {
            _this.listaArchivos = new Array();
            result.forEach(function (element) {
                var archivo = new Archivo();
                archivo = element;
                _this.listaArchivos.push(archivo);
            });
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    AltasComponent.prototype.descargarArchivo = function (archivo) {
        if (this.mensajeComponent === undefined)
            this.mensajeComponent = new MensajeComponent();
        if (this.spinnerSmallComponent === undefined)
            this.spinnerSmallComponent = new SpinnerSmallComponent();
        var archivoId = archivo.Id;
        var fileKey = archivo.FileKey;
        var proveedorId = this.empresaSeleccionada.Id;
        var param = btoa("fileKey=" + fileKey + "&mail=" + this.empresaSeleccionada.Mail + "&archivoId=" + archivoId.toString() + "&proveedorId=" + proveedorId.toString());
        var url = "/officetohtml/index.html?param=" + param;
        var link = document.createElement("a");
        document.body.appendChild(link);
        link.href = url;
        link.target = "_blank";
        link.click();
    };
    AltasComponent.prototype.onOptionsSelected = function () {
        var _this = this;
        if (this.selectedEstado != "") {
            this.dataFiltered = this.data.filter(function (t) { return t.EstadoAprobacionDescripcion == _this.selectedEstado; });
        }
        else {
            this.dataFiltered = this.data;
        }
    };
    AltasComponent.prototype.cargarSolicitudUsuario = function (mail, proveedorId) {
        var _this = this;
        this.subscription = this.service.cargarSolicitudUsuario(mail, proveedorId).subscribe(function (result) {
            if (result.VinculoConEmpleadosDeMolinos != null) {
                if (result.VinculoConEmpleadosDeMolinos) {
                    _this.relacionConEmpleados = "Si";
                }
                else {
                    _this.relacionConEmpleados = "No";
                }
                if (result.VinculoConFuncionariosPublicos) {
                    _this.relacionConFuncionarios = "Si";
                }
                else {
                    _this.relacionConFuncionarios = "No";
                }
                _this.empleados = result.Empleados;
                _this.funcionarios = result.Funcionarios;
            }
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    AltasComponent.prototype.isVisibleTablaFuncionarios = function () {
        return this.relacionConFuncionarios == "Si";
    };
    AltasComponent.prototype.isVisibleTablaEmpleados = function () {
        return this.relacionConEmpleados == "Si";
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], AltasComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], AltasComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("smallSpinner"),
        __metadata("design:type", SpinnerSmallComponent)
    ], AltasComponent.prototype, "spinnerSmallComponent", void 0);
    __decorate([
        ViewChild("spinnerModal"),
        __metadata("design:type", SpinnerSmallComponent)
    ], AltasComponent.prototype, "spinnerModal", void 0);
    AltasComponent = __decorate([
        Component({
            selector: 'app-altas',
            templateUrl: 'altas.component.html',
            styleUrls: ['altas.component.css', '../../../../Content/css/bootstrap.min.css'],
            providers: [AltaEmpresaService]
        }),
        __metadata("design:paramtypes", [AltaEmpresaService, EmpresaGranosService, NavService, SessionDataService, SecurityService, FloatMsgService, ModalService])
    ], AltasComponent);
    return AltasComponent;
}(BaseComponent));
export { AltasComponent };
//# sourceMappingURL=altas.component.js.map