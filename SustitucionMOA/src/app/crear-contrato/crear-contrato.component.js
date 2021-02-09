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
import { BlockUI } from 'ng-block-ui';
import { ActivatedRoute } from "@angular/router";
var CrearContratoBaseComponent = /** @class */ (function (_super) {
    __extends(CrearContratoBaseComponent, _super);
    function CrearContratoBaseComponent(service, navService, route, sessionDataService, securityService, floatMsgService, modalService) {
        var _this = _super.call(this, service, navService, sessionDataService, securityService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.route = route;
        _this.sessionDataService = sessionDataService;
        _this.securityService = securityService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.id = 0;
        _this.esCorredorEnDataAgro = false;
        _this.retirados = null;
        _this.cuitProveedorSeleccionado = "";
        _this.localidades = [];
        _this.proveedores = [];
        _this.pendienteFijarContrato = "";
        _this.pendientesFijar = [];
        _this.keyword = 'Nombre';
        _this.keyword2 = "RazonSocial";
        _this.keyword3 = "ContratoId";
        _this.autocompleteNotFoundText = "No encontrado";
        _this.mensajeModal = "";
        _this.fechaInicio = new Date().toLocaleDateString('en-GB');
        _this.fechaFin = new Date().toLocaleDateString('en-GB');
        _this.fechafInicio = new Date().toLocaleDateString('en-GB');
        _this.fechafFin = new Date().toLocaleDateString('en-GB');
        _this.datosContrato = new Array();
        _this.materiales = new Array();
        _this.monedas = new Array();
        _this.monedasTodas = new Array();
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
        _this.listaPrecios = null;
        _this.precioPorNegocioSeleccionado = null;
        _this.precioMaiz = null;
        _this.precioTrigo = null;
        _this.precioSoja = null;
        _this.precioGirasol = null;
        _this.precioSeleccionado = false;
        _this.materialSeleccionado = "";
        _this.ObservacionPagoDiferidoTercero = "";
        _this.ObservacionDolarizadoTercero = "";
        _this.ObservacionCalidadTercero = "";
        _this.ObservacionSustentableTercero = "";
        _this.ObservacionTercero = "";
        _this.spinnerComponent = new SpinnerComponent();
        _this.mensajeComponent = new MensajeComponent();
        _this.spinnerCampana = new SpinnerSmallComponent();
        return _this;
    }
    CrearContratoBaseComponent.prototype.checkPermisos = function () { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); };
    CrearContratoBaseComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            //new Seccion('/crear-contrato/cargarnegocio', 'crear-contrato', 'Seleccione un negcio'),
            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),
        ]);
        //this.obteneDatosContrato();
        this.route.params.forEach(function (params) {
            if (params["id"] > 0) {
                _this.id = params["id"];
            }
            ;
        });
    };
    CrearContratoBaseComponent.prototype.negocioHabilitado = function (contrato) {
        var _this = this;
        this.blockUI.start('');
        this.unsubscribe();
        this.subscription = this.service.traerPrecioMoaMateriales(contrato.TipoNegocioId).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.blockUI.stop();
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.blockUI.stop();
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var precio = JSON.parse(result);
                _this.retirados = true;
                _this.listaPrecios = precio;
                for (var i = 0; i < precio.length; i++) {
                    var preciopornegocio = precio[i].filter(function (x) { return x.TipoNegocioId == contrato.TipoNegocioId; });
                    var matRetirado = true;
                    if (preciopornegocio.filter(function (x) { return x.Retirado == false; }).length > 0 ||
                        preciopornegocio[0].Pizarra == true) {
                        _this.retirados = false;
                        matRetirado = false;
                    }
                    if (preciopornegocio[0].MaterialId == 1) {
                        _this.precioMaiz = matRetirado == false;
                        if (matRetirado) {
                            $("#precioMaiz").css("color", "red");
                        }
                        else {
                            $("#precioMaiz").css("color", "#017940");
                        }
                    }
                    if (preciopornegocio[0].MaterialId == 2) {
                        _this.precioTrigo = matRetirado == false;
                        if (matRetirado) {
                            $("#precioTrigo").css("color", "red");
                        }
                        else {
                            $("#precioTrigo").css("color", "#017940");
                        }
                    }
                    if (preciopornegocio[0].MaterialId == 3) {
                        _this.precioSoja = matRetirado == false;
                        if (matRetirado) {
                            $("#precioSoja").css("color", "red");
                        }
                        else {
                            $("#precioSoja").css("color", "#017940");
                        }
                    }
                    if (preciopornegocio[0].MaterialId == 4 || preciopornegocio[0].MaterialId == 5) {
                        _this.precioGirasol = matRetirado == false;
                        if (matRetirado) {
                            $("#precioGirasol").css("color", "red");
                        }
                        else {
                            $("#precioGirasol").css("color", "#017940");
                        }
                    }
                }
                if (_this.retirados == false || contrato.Id > 0) {
                    _this.obteneDatosContrato(contrato);
                }
                else {
                    _this.blockUI.stop();
                    _this.mensajeComponent.setErrorMsg("Negocio no disponible");
                }
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.obteneDatosContrato = function (contrato) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato(contrato.TipoNegocioId).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.blockUI.stop();
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.blockUI.stop();
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var obj = JSON.parse(result);
                _this.datosContrato = obj;
                //obj.Datos.material.forEach(element => {
                //    let el = {
                //        Id: element.MaterialId,
                //        Descripcion: element.Descripcion
                //    }
                //    this.materiales.push(el);
                //});
                obj.Datos.Bolsa.forEach(function (element) {
                    if (element.Id != 8 && element.Id != 9) {
                        var el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        };
                        _this.bolsasConfirma.push(el);
                        if (el.Descripcion == "Bs As" || el.Descripcion == "Rosario")
                            _this.bolsasFisico.push(el);
                        if (element.Descripcion == "Bs As")
                            _this.bolsasCarta.push(el);
                    }
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
                    //this.monedas.push(el);
                    _this.monedasTodas.push(el);
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
            _this.blockUI.stop();
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
                _this.blockUI.stop();
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.blockUI.stop();
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var obj = JSON.parse(result);
                if (obj != null && obj > 0) {
                    contrato.CorredorId = obj;
                    _this.esCorredorEnDataAgro = true;
                    _this.blockUI.stop();
                    if (_this.id > 0) {
                        _this.habilitaciones(contrato);
                    }
                }
                else {
                    _this.esCorredorEnDataAgro = false;
                    _this.blockUI.stop();
                    if (_this.id == 0) {
                        _this.obtenerDatosCompraNet(contrato, "");
                    }
                    else {
                        _this.habilitaciones(contrato);
                    }
                }
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.habilitaciones = function (contrato) {
        var _this = this;
        this.blockUI.start('');
        this.unsubscribe();
        this.subscription = this.service.habilitaciones(contrato.MaterialId, contrato.TipoNegocioId).subscribe(function (result) {
            if (result.logout == true) {
                _this.blockUI.stop();
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.blockUI.stop();
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.blockUI.stop();
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
                    if (contrato.TipoNegocioId == 3) {
                        _this.obtenerFijacionesAutomaticas(contrato.MaterialId, "");
                    }
                    else {
                        _this.blockUI.stop();
                    }
                }
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.obtenerDatosCompraNet = function (contrato, idProveedorDataAgro) {
        var _this = this;
        this.blockUI.start('');
        this.unsubscribe();
        this.subscription = this.service.obtenerDatosCompraNet(idProveedorDataAgro).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined && result.error != "") {
                _this.blockUI.stop();
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.blockUI.stop();
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                var obj = JSON.parse(result);
                _this.datosCompraNet = obj;
                contrato.ClasificacionId = obj.ClasificacionCompraNetId;
                contrato.BolsaId = obj.BolsaCompraNetId;
                contrato.BoletoId = obj.BoletoCompraNetId;
                if (obj.BoletoCompraNetId == 1) {
                    _this.bolsasSelect = _this.bolsasConfirma;
                }
                if (obj.BoletoCompraNetId == 2) {
                    _this.bolsasSelect = _this.bolsasFisico;
                }
                if (obj.BoletoCompraNetId == 3) {
                    _this.bolsasSelect = [];
                }
                if (obj.BoletoCompraNetId == 4) {
                    _this.bolsasSelect = _this.bolsasCarta;
                }
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
                if (contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 2) {
                    _this.validarProveedor(idProveedorDataAgro, contrato);
                }
                if (contrato.TipoNegocioId == 3) {
                    _this.obtenerFijacionesAutomaticas(contrato.MaterialId, "");
                }
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.isVisibleProveedor = function () {
        return this.esCorredorEnDataAgro == true;
    };
    CrearContratoBaseComponent.prototype.isVisiblePizarra = function () {
        return this.datosPizarra == true;
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
                _this.blockUI.stop();
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.blockUI.stop();
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
                    if (_this.contratoEditar != null && _this.contratoEditar.DatosFijacion.ContratoId) {
                        var afijar = _this.pendientesFijar.filter(function (a) { return a.ContratoId == _this.contratoEditar.DatosFijacion.ContratoId; });
                        if (afijar.length == 1) {
                            console.log(afijar[0]);
                            _this.pendienteFijar = afijar[0];
                        }
                        _this.contratoEditar = null;
                    }
                    else {
                        //this.pendienteFijarContrato = "";
                    }
                }
                _this.blockUI.stop();
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ;
    CrearContratoBaseComponent.prototype.irACargas = function () {
        this.navService.navegarSeccion("/reporte/contrato");
    };
    CrearContratoBaseComponent.prototype.validarProveedor = function (proveedorId, contrato) {
        var _this = this;
        this.subscription = this.service.validarProveedor(proveedorId).subscribe(function (result) {
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
                    obj = JSON.parse(obj);
                }
                if ((contrato.ClasificacionId == 2 || contrato.ClasificacionId == 3) &&
                    ((contrato.PlanCanje == true && obj.Ruca.Acopiador.PlanCanje == "NO") ||
                        (contrato.Consignatario == true && obj.Ruca.Acopiador.Consignatario == "NO") ||
                        (contrato.PlanCanje == false && contrato.Consignatario == false && obj.Ruca.Acopiador.Directo == "NO"))) {
                    _this.mensajeModal = "No est\u00E1 habilitado en Ruca";
                    document.getElementById("openModalMensajeModal").click();
                    return;
                }
                if (obj.FechaActualizacion == "NO") {
                    _this.mensajeModal = "Falta fecha de actualizaci\u00F3n de legajo";
                    document.getElementById("openModalMensajeModal").click();
                    return;
                }
                _this.blockUI.stop();
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    CrearContratoBaseComponent.prototype.preciosDisponibles = function (MaterialId, TipoNegocioId) {
        if ((MaterialId == 1 && this.precioMaiz != true)
            || (MaterialId == 2 && this.precioTrigo != true)
            || (MaterialId == 3 && this.precioSoja != true)
            || (MaterialId == 4 && this.precioGirasol != true)) {
            this.mensajeModal = "Material no disponible.";
            document.getElementById("openModalMensajeModal").click();
            return;
        }
        var precio = this.listaPrecios;
        this.precioPorNegocioSeleccionado = precio[MaterialId - 1].filter(function (x) { return x.TipoNegocioId == TipoNegocioId && (!(x.Precio == 0 && x.MonedaId != "Pizarra") || x.TipoNegocioId == 1); });
        for (var i = 0; i < this.precioPorNegocioSeleccionado.length; i++) {
            if (this.precioPorNegocioSeleccionado[i].DesdeEntrega != null && typeof this.precioPorNegocioSeleccionado[i].DesdeEntrega === 'string') {
                this.precioPorNegocioSeleccionado[i].DesdeEntrega = new Date(parseInt(this.precioPorNegocioSeleccionado[i].DesdeEntrega.substr(6)));
            }
            if (this.precioPorNegocioSeleccionado[i].HastaEntrega != null && typeof this.precioPorNegocioSeleccionado[i].HastaEntrega === 'string') {
                this.precioPorNegocioSeleccionado[i].HastaEntrega = new Date(parseInt(this.precioPorNegocioSeleccionado[i].HastaEntrega.substr(6)));
            }
            if (this.precioPorNegocioSeleccionado[i].DesdeFijacion != null && typeof this.precioPorNegocioSeleccionado[i].DesdeFijacion === 'string') {
                this.precioPorNegocioSeleccionado[i].DesdeFijacion = new Date(parseInt(this.precioPorNegocioSeleccionado[i].DesdeFijacion.substr(6)));
            }
            if (this.precioPorNegocioSeleccionado[i].HastaFijacion != null && typeof this.precioPorNegocioSeleccionado[i].HastaFijacion === 'string') {
                this.precioPorNegocioSeleccionado[i].HastaFijacion = new Date(parseInt(this.precioPorNegocioSeleccionado[i].HastaFijacion.substr(6)));
            }
        }
        document.getElementById("openModalPreciosModal").click();
    };
    CrearContratoBaseComponent.prototype.seleccionarPrecio = function (precio, contrato) {
        this.monedas = new Array();
        if (precio.TipoNegocioId == 2 || precio.TipoNegocioId == 3) {
            if (precio.MonedaId == "ARP") {
                this.monedas.push({ Id: "ARP  ", Descripcion: "ARP" });
                contrato.MonedaId = "ARP  ";
                if (precio.TipoNegocioId == 2) {
                    contrato.DolarizadoTercero = false;
                }
            }
            if (precio.MonedaId == "USD") {
                this.monedas.push({ Id: "USDM ", Descripcion: "USD" });
                contrato.MonedaId = "USDM ";
                if (precio.TipoNegocioId == 2) {
                    contrato.PagoDiferidoTercero = false;
                }
            }
            if (precio.MonedaId == "Pizarra") {
                contrato.Pizarra = true;
                this.datosPizarra = true;
            }
            else {
                contrato.Pizarra = false;
                this.datosPizarra = false;
            }
            contrato.Precio = precio.Precio;
        }
        if (precio.TipoNegocioId == 2 || precio.TipoNegocioId == 1) {
            //entregadesde hasta
            contrato.FechaDesde = precio.DesdeEntrega;
            contrato.FechaHasta = precio.HastaEntrega;
            this.fechaInicio = contrato.FechaDesde.toLocaleDateString('en-GB');
            this.fechaFin = contrato.FechaHasta.toLocaleDateString('en-GB');
            $("#noCursor").val(contrato.FechaDesde.toLocaleDateString("en-GB"));
            $("#noCursor2").val(contrato.FechaHasta.toLocaleDateString("en-GB"));
        }
        if (precio.TipoNegocioId == 1) {
            //fijacion desde hasta
            contrato.DesdeFijacion = precio.DesdeFijacion;
            contrato.HastaFijacion = precio.HastaFijacion;
            this.fechafInicio = contrato.DesdeFijacion.toLocaleDateString('en-GB');
            this.fechafFin = contrato.HastaFijacion.toLocaleDateString('en-GB');
            $("#noCursorf").val(contrato.DesdeFijacion.toLocaleDateString("en-GB"));
            $("#noCursor2f").val(contrato.HastaFijacion.toLocaleDateString("en-GB"));
        }
        this.materiales = new Array();
        this.materiales.push({ Id: precio.MaterialId, Descripcion: precio.Material });
        contrato.MaterialId = precio.MaterialId;
        this.precioSeleccionado = true;
        this.materialSeleccionado = precio.Material;
        if (contrato.Id == 0) {
            var CampaniaIdActual = this.datosContrato.Datos.material.filter(function (x) { return x.Descripcion == precio.Material; })[0].CampaniaIdActual;
            contrato.CampanaId = CampaniaIdActual;
        }
        this.habilitaciones(contrato);
    };
    CrearContratoBaseComponent.prototype.isVisibleGrabar = function () {
        return !this.spinnerComponent.visible;
    };
    CrearContratoBaseComponent.prototype.isMaterialSeleccionado = function (material) {
        return this.materialSeleccionado == "" || this.materialSeleccionado == material;
    };
    CrearContratoBaseComponent.prototype.traerContratoCompleto = function (contrato) {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.traerContratoCompleto(contrato.Id, contrato.TipoNegocioId).subscribe(function (result) {
            if (result.logout == true) {
                _this.sessionDataService.logout();
                _this.blockUI.stop();
            }
            else if (result.error != undefined && result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
                _this.blockUI.stop();
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
                _this.blockUI.stop();
            }
            else {
                _this.pendientesFijar = [];
                if (result != "") {
                    var obj = JSON.parse(result);
                    _this.contratoEditar = obj;
                    console.log("contratoEditar", obj);
                    _this.precioSeleccionado = true;
                    _this.materialSeleccionado = obj.Material;
                    contrato.TipoNegocioId = obj.TipoNegocioId;
                    contrato.MaterialId = obj.MaterialId;
                    contrato.EstadoId = obj.EstadoId;
                    contrato.ProvinciaId = obj.ProvinciaId;
                    if (obj.ObservacionTercero != null) {
                        _this.ObservacionTercero = obj.ObservacionTercero.split("|")[0];
                        var p = obj.ObservacionTercero.split("|");
                        if (obj.CalidadTercero == true) {
                            var f = p.filter(function (e) { return e.includes("Calidad:"); });
                            if (f) {
                                _this.ObservacionCalidadTercero = f[0].split(":")[1].trim();
                            }
                        }
                        if (obj.DolarizadoTercero == true) {
                            var f = p.filter(function (e) { return e.includes("Dolarizado:"); });
                            if (f) {
                                _this.ObservacionDolarizadoTercero = f[0].split(":")[1].trim();
                            }
                        }
                        if (obj.SustentableTercero == true) {
                            var n = "";
                            var f = p.filter(function (e) { return e.includes("Sustentable:"); });
                            if (f) {
                                _this.ObservacionSustentableTercero = f[0].split(":")[1].trim();
                            }
                        }
                        if (obj.PagoDiferidoTercero == true) {
                            var f = p.filter(function (e) { return e.includes("Pago Diferido:"); });
                            if (f) {
                                _this.ObservacionPagoDiferidoTercero = f[0].split(":")[1].trim();
                            }
                        }
                    }
                    contrato.Proveedorid = obj.Proveedorid;
                    contrato.DestinoId = obj.DestinoId;
                    contrato.Cantidad = obj.Cantidad;
                    contrato.MonedaId = obj.MonedaId;
                    contrato.Precio = obj.Precio;
                    contrato.CantidadCamiones = obj.CantidadCamiones;
                    contrato.ComercialId = obj.ComercialId;
                    contrato.Pizarra = obj.Pizarra;
                    contrato.ClasificacionId = obj.ClasificacionId;
                    contrato.LocalidadId = obj.LocalidadId;
                    contrato.Consignatario = obj.Consignatario;
                    contrato.PlanCanje = obj.PlanCanje;
                    contrato.BoletoId = obj.BoletoId;
                    contrato.BolsaId = obj.BolsaId;
                    contrato.EstablecimientoPropio = obj.EstablecimientoPropio;
                    contrato.ContratoVendedor = obj.ContratoVendedor;
                    contrato.ContratoCorredor = obj.ContratoCorredor;
                    contrato.PagoDiferidoTercero = obj.PagoDiferidoTercero;
                    contrato.DolarizadoTercero = obj.DolarizadoTercero;
                    contrato.CalidadTercero = obj.CalidadTercero;
                    contrato.SustentableTercero = obj.SustentableTercero;
                    contrato.CampanaId = obj.CampanaId;
                    contrato.ProveedorId = obj.ProveedorId;
                    _this.proveedor = obj.Proveedor;
                    if (obj.Corredor != null && obj.Corredor != "" && obj.ComercialId != 22) {
                        contrato.ComercialId = 0;
                    }
                    else {
                        contrato.ComercialId = obj.ComercialId;
                    }
                    contrato.Fecha = new Date(parseInt(obj.Fecha.substr(6)));
                    contrato.FechaOperacion = new Date(parseInt(obj.FechaOperacion.substr(6)));
                    if (contrato.TipoNegocioId == 2 || contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 3) {
                        //entregadesde hasta
                        contrato.FechaDesde = new Date(parseInt(obj.FechaDesde.substr(6)));
                        contrato.FechaHasta = new Date(parseInt(obj.FechaHasta.substr(6)));
                        _this.fechaInicio = contrato.FechaDesde.toLocaleDateString('en-GB');
                        _this.fechaFin = contrato.FechaHasta.toLocaleDateString('en-GB');
                        $("#noCursor").val(contrato.FechaDesde.toLocaleDateString("en-GB"));
                        $("#noCursor2").val(contrato.FechaHasta.toLocaleDateString("en-GB"));
                    }
                    if (contrato.TipoNegocioId == 1) {
                        //fijacion desde hasta
                        contrato.DesdeFijacion = new Date(parseInt(obj.DesdeFijacion.substr(6)));
                        contrato.HastaFijacion = new Date(parseInt(obj.HastaFijacion.substr(6)));
                        _this.fechafInicio = contrato.DesdeFijacion.toLocaleDateString('en-GB');
                        _this.fechafFin = contrato.HastaFijacion.toLocaleDateString('en-GB');
                        $("#noCursorf").val(contrato.DesdeFijacion.toLocaleDateString("en-GB"));
                        $("#noCursor2f").val(contrato.HastaFijacion.toLocaleDateString("en-GB"));
                    }
                    _this.localidad = obj.Localidad + " (" + obj.Provincia + ")";
                    if (contrato.TipoNegocioId == 3) {
                        contrato.ContratoSAP = obj.DatosFijacion.ContratoId;
                        contrato.Posicion = obj.Posicion;
                        contrato.TrigoEspecial = obj.TrigoEspecial;
                        _this.pendienteFijarContrato = obj.DatosFijacion.ContratoId;
                    }
                    if (contrato.MonedaId == "ARP  ") {
                        _this.monedas.push({ Id: "ARP  ", Descripcion: "ARP" });
                        contrato.MonedaId = "ARP  ";
                        if (contrato.TipoNegocioId == 2) {
                            contrato.DolarizadoTercero = false;
                        }
                    }
                    if (contrato.MonedaId == "USDM ") {
                        _this.monedas.push({ Id: "USDM ", Descripcion: "USD" });
                        contrato.MonedaId = "USDM ";
                        if (contrato.TipoNegocioId == 2) {
                            contrato.PagoDiferidoTercero = false;
                        }
                    }
                    if (obj.BoletoId == 1) {
                        _this.bolsasSelect = _this.bolsasConfirma;
                    }
                    if (obj.BoletoId == 2) {
                        _this.bolsasSelect = _this.bolsasFisico;
                    }
                    if (obj.BoletoId == 3) {
                        _this.bolsasSelect = [];
                    }
                    if (obj.BoletoId == 4) {
                        _this.bolsasSelect = _this.bolsasCarta;
                    }
                    if (contrato.Pizarra == true) {
                        _this.datosPizarra = true;
                    }
                    else {
                        _this.datosPizarra = false;
                    }
                    _this.materiales = new Array();
                    _this.materiales.push({ Id: obj.MaterialId, Descripcion: obj.Material });
                    _this.precioSeleccionado = true;
                    _this.cuitProveedorSeleccionado = obj.Cuit;
                    _this.negocioHabilitado(contrato);
                }
                //this.blockUI.stop();
            }
        }, function (error) {
            _this.blockUI.stop();
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ;
    __decorate([
        BlockUI(),
        __metadata("design:type", Object)
    ], CrearContratoBaseComponent.prototype, "blockUI", void 0);
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
        __metadata("design:paramtypes", [CrearContratoService,
            NavService,
            ActivatedRoute,
            SessionDataService,
            SecurityService,
            FloatMsgService,
            ModalService])
    ], CrearContratoBaseComponent);
    return CrearContratoBaseComponent;
}(ListBaseComponent));
export { CrearContratoBaseComponent };
//# sourceMappingURL=crear-contrato.component.js.map