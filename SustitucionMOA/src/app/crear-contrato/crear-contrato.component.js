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
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
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
        _this.cuitProveedorSeleccionado = "";
        _this.localidades = [];
        _this.proveedores = [];
        _this.pendientesFijar = [];
        _this.keyword = 'Nombre';
        _this.keyword2 = "RazonSocial";
        _this.keyword3 = "ContratoId";
        _this.autocompleteNotFoundText = "No encontrado";
        _this.fechaInicio = new Date().toLocaleDateString('en-GB');
        _this.fechaFin = new Date().toLocaleDateString('en-GB');
        _this.fechafInicio = new Date().toLocaleDateString('en-GB');
        _this.fechafFin = new Date().toLocaleDateString('en-GB');
        _this.datosContrato = new Array();
        _this.materiales = new Array();
        _this.monedas = new Array();
        _this.destinos = new Array();
        _this.campanias = new Array();
        _this.campaniasTodas = new Array();
        _this.zona = new Array();
        _this.bolsasSelect = new Array();
        _this.bolsasConfirma = new Array();
        _this.bolsasFisico = new Array();
        _this.bolsasCarta = new Array();
        _this.condicionVendedor = new Array();
        _this.condicionFijacion = new Array();
        _this.datosCompraNet = null;
        _this.datosPizarra = null;
        _this.datosPrecioMoa = new Array();
        _this.spinnerComponent = new SpinnerComponent();
        _this.mensajeComponent = new MensajeComponent();
        return _this;
    }
    CrearContratoBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); };
    CrearContratoBaseComponent.prototype.ngOnInit = function () {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),
        ]);
        //this.obteneDatosContrato();
    };
    CrearContratoBaseComponent.prototype.obteneDatosContrato = function (contrato) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato(contrato.TipoNegocioId).subscribe(function (result) {
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
                    if (element.Id != 10) {
                        var el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        };
                        _this.destinos.push(el);
                    }
                });
                obj.Datos.campania.forEach(function (element) {
                    var el = {
                        Id: element.CampaniaId,
                        Descripcion: element.Descripcion
                    };
                    _this.campaniasTodas.push(el);
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
    CrearContratoBaseComponent.prototype.habilitaciones = function (contrato) {
        var _this = this;
        this.spinnerCampana.showIt();
        this.unsubscribe();
        this.subscription = this.service.habilitaciones(contrato.MaterialId, contrato.TipoNegocioId).subscribe(function (result) {
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
                var obj = result;
                if (obj != null) {
                    _this.campanias = new Array();
                    JSON.parse(obj.HabilitarCampana).forEach(function (element) {
                        var el = {
                            Id: element.CampaniaId,
                            Descripcion: element.Campania
                        };
                        _this.campanias.push(el);
                    });
                    if (obj.HabilitarPizarra != "") {
                        _this.datosPizarra = JSON.parse(obj.HabilitarPizarra);
                    }
                    else {
                        _this.datosPizarra = null;
                    }
                    _this.datosPrecioMoa = JSON.parse(obj.TraerPrecioMoa);
                    if (contrato.TipoNegocioId == 1) {
                        var leng = _this.datosPrecioMoa.length;
                        var items = _this.datosPrecioMoa;
                        for (var i = 0; i < leng; i++) {
                            var item = items[i];
                            console.log(item);
                            contrato.FechaDesde = new Date(parseInt(item.DesdeEntrega.substr(6)));
                            contrato.FechaHasta = _this.ObtenerFechaHasta(contrato.FechaDesde);
                            _this.fechaInicio = contrato.FechaDesde.toLocaleDateString('en-GB');
                            _this.fechaFin = contrato.FechaHasta.toLocaleDateString('en-GB');
                            var entregaHasta = new Date(parseInt(item.HastaEntrega.substr(6)));
                            $('.form_datetime_Inicio').datetimepicker('setStartDate', contrato.FechaDesde.toLocaleDateString("en-GB"));
                            $('.form_datetime_Inicio').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));
                            $('.form_datetime_Fin').datetimepicker('setStartDate', contrato.FechaDesde.toLocaleDateString("en-GB"));
                            $('.form_datetime_Fin').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));
                            contrato.DesdeFijacion = new Date(parseInt(item.DesdeFijacion.substr(6)));
                            contrato.HastaFijacion = _this.ObtenerFechaHasta(contrato.DesdeFijacion);
                            _this.fechafInicio = contrato.DesdeFijacion.toLocaleDateString('en-GB');
                            _this.fechafFin = contrato.HastaFijacion.toLocaleDateString('en-GB');
                            var HastaFijacion = new Date(parseInt(item.HastaFijacion.substr(6)));
                            $('.form_datetime_fInicio').datetimepicker('setStartDate', contrato.DesdeFijacion.toLocaleDateString("en-GB"));
                            $('.form_datetime_fInicio').datetimepicker('setEndDate', HastaFijacion.toLocaleDateString("en-GB"));
                            $('.form_datetime_fFin').datetimepicker('setStartDate', contrato.DesdeFijacion.toLocaleDateString("en-GB"));
                            $('.form_datetime_fFin').datetimepicker('setEndDate', HastaFijacion.toLocaleDateString("en-GB"));
                            break;
                        }
                    }
                    if (contrato.TipoNegocioId == 3) {
                        _this.obtenerFijacionesAutomaticas(contrato.MaterialId, "");
                    }
                }
            }
            _this.spinnerCampana.hideIt();
        }, function (error) {
            _this.spinnerCampana.hideIt();
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
                //contrato.MonedaId = this.monedas[0].Id;
                //contrato.MaterialId = this.materiales[0].Id;
                //contrato.CampanaId = this.campanias[0].Id;
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
    CrearContratoBaseComponent.prototype.isVisiblePizarra = function () {
        return this.datosPizarra != null;
    };
    CrearContratoBaseComponent.prototype.ObtenerFechaHasta = function (fechaBase) {
        var hoy = fechaBase != undefined ? fechaBase : new Date();
        var anio = hoy.getFullYear();
        var mesPost = hoy.getMonth() + 1;
        var dia = hoy.getDate();
        var ultimoDia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
        if (dia === 1) {
            dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
            mesPost = hoy.getMonth() + 1;
        }
        if (dia === ultimoDia || (mesPost === 2 && dia >= 29)) {
            dia = new Date(anio, mesPost, 0).getDate();
        }
        if (mesPost === 13) {
            mesPost = 1;
            anio += 1;
        }
        //if (mesPost < 10) {
        //    mesPost = "0" + mesPost.toString();
        //}
        //if (dia < 10) {
        //    dia = "0" + dia.toString();
        //}
        return new Date(anio, mesPost, dia); // dia + '-' + mesPost + '-' + anio;
    };
    CrearContratoBaseComponent.prototype.obtenerFijacionesAutomaticas = function (materialId, filtro) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.obtenerFijacionesAutomaticas(this.esCorredorEnDataAgro, this.cuitProveedorSeleccionado, materialId, filtro).subscribe(function (result) {
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
                _this.pendientesFijar = [];
                if (result != "") {
                    var obj = JSON.parse(result);
                    //this.pendientesFijar = obj;
                    _this.pendientesFijar = obj.map(function (con) {
                        return {
                            ARecibirSinPrecio: con.ARecibirSinPrecio,
                            Calidad: con.Calidad,
                            Calidades: con.Calidades,
                            Campana: con.Campana,
                            CampanaId: con.CampanaId,
                            Centro: con.Centro,
                            CentroDescripcion: con.CentroDescripcion,
                            ChequeElectronico: con.ChequeElectronico,
                            Color: con.Color,
                            CondicionFijacionCod: con.CondicionFijacionCod,
                            CondicionFijacionDescripcion: con.CondicionFijacionDescripcion,
                            CondicionPagoCod: con.CondicionPagoCod,
                            CondicionPagoDescripcion: con.CondicionPagoDescripcion,
                            ContratoId: con.ContratoId,
                            DesdeEntrega: con.DesdeEntrega,
                            FechaDesde: con.FechaDesde,
                            FechaHasta: con.FechaHasta,
                            Filtro: '<p class="buscar-nomb"><strong>' + con.ContratoId + '</strong> - ' +
                                'KG CTO: ' + con.KilosContrato + ' ' +
                                ' - KGS SIN PRECIO : ' + con.ARecibirSinPrecio + ' - KGS SIN FIJAR : ' + con.RecibidoSinFijar + '' +
                                ' - KILOS A FIJAR: ' + con.KilosPendiente + ' - KG APLIC: ' + con.KilosAplicados + '' +
                                ' - Hasta: ' + con.FechaHasta + ' - <strong>' + con.CentroDescripcion + '</strong></p>',
                            HastaEntrega: con.HastaEntrega,
                            ImporteAPrecio: con.ImporteAPrecio,
                            ImporteSobrePrecio: con.ImporteSobrePrecio,
                            KilosAplicados: con.KilosAplicados,
                            KilosContrato: con.KilosContrato,
                            KilosPendiente: con.KilosPendiente,
                            MonedaAPrecio: con.MonedaAPrecio,
                            MonedaSobrePrecio: con.MonedaSobrePrecio,
                            PagoDiferido: con.PagoDiferido,
                            PorcentajeAPrecio: con.PorcentajeAPrecio,
                            PorcentajeSobrePrecio: con.PorcentajeSobrePrecio,
                            Posicion: con.Posicion,
                            RecibidoSinFijar: con.RecibidoSinFijar,
                        };
                    });
                }
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ;
    CrearContratoBaseComponent.prototype.irACargas = function () {
        this.navService.navegarSeccion("reporte/contrato");
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], CrearContratoBaseComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], CrearContratoBaseComponent.prototype, "spinnerComponent", void 0);
    __decorate([
        ViewChild("spinnerCampana"),
        __metadata("design:type", SpinnerSmallComponent)
    ], CrearContratoBaseComponent.prototype, "spinnerCampana", void 0);
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