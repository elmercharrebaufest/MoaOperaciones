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
import { Component } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAPrecioService } from './../crear-contrato.service';
import { ContratoAPrecio } from "../../common/models/contratoAPrecio";
var CrearContratoAPrecioComponent = /** @class */ (function (_super) {
    __extends(CrearContratoAPrecioComponent, _super);
    function CrearContratoAPrecioComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.contrato = new ContratoAPrecio();
        return _this;
    }
    CrearContratoAPrecioComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.contrato.TipoNegocioId = 2;
        this.contrato.Id = this.id;
        console.log("id", this.id);
        if (this.id > 0) {
            this.traerContratoCompleto(this.contrato);
        }
        else {
            this.negocioHabilitado(this.contrato);
        }
    };
    CrearContratoAPrecioComponent.prototype.ngAfterViewInit = function () {
        var hoy = new Date();
        var hoysinhora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
        var hoymesqueviene = this.ObtenerFechaHasta(hoysinhora);
        this.contrato.FechaDesde = hoysinhora;
        this.contrato.FechaHasta = hoymesqueviene;
        $(document).on("mouseover", '.form_datetime_Inicio', function () {
            $('.form_datetime_Inicio').datetimepicker({
                format: 'dd/mm/yyyy',
                language: 'es',
                weekStart: 1,
                //todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4,
            }).on('changeDate', function (ev) {
                if (ev.date.valueOf()) {
                    var fechaBase = new Date(ev.date.valueOf());
                    var hoy = fechaBase != undefined ? fechaBase : new Date();
                    var anio = hoy.getFullYear();
                    var mesPost = hoy.getMonth() + 2;
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
                    var messtring = "" + mesPost;
                    if (mesPost < 10) {
                        messtring = "0" + mesPost.toString();
                    }
                    var diastring = "" + dia;
                    if (dia < 10) {
                        diastring = "0" + dia.toString();
                    }
                    $("#noCursor2").val(diastring + '/' + messtring + '/' + anio);
                }
            });
        });
        $(document).on("mouseover", '.form_datetime_Fin', function () {
            $('.form_datetime_Fin').datetimepicker({
                format: 'dd/mm/yyyy',
                language: 'es',
                weekStart: 1,
                //todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
            });
        });
        this.fechaInicio = hoysinhora.toLocaleDateString('en-GB');
        this.fechaFin = hoymesqueviene.toLocaleDateString('en-GB');
    };
    CrearContratoAPrecioComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("crear-contrato", "A Precio");
    };
    CrearContratoAPrecioComponent.prototype.selectEventProveedor = function (item) {
        this.contrato.ProveedorId = item.Id;
        console.log("prov: ", item.Id);
        if (item != null && item.Id != null && item.Id > 0) {
            this.obtenerDatosCompraNet(this.contrato, item.Id);
        }
    };
    CrearContratoAPrecioComponent.prototype.onChangeSearchProveedor = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.buscarProveedoresConCorredor(term).subscribe(function (result) {
                var resultlist = JSON.parse(result);
                _this.proveedores = resultlist.map(function (prov) {
                    return { Id: prov.Id, RazonSocial: prov.RazonSocial + " (" + prov.Cuit + ")", CUIT: prov.Cuit };
                });
                //this.proveedores = JSON.parse(result);
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    CrearContratoAPrecioComponent.prototype.selectEventLocalidad = function (item) {
        this.contrato.LocalidadId = item.LocalidadId;
        this.contrato.ProvinciaId = item.ProvinciaId;
        if (item.ProvinciaId != 1) {
            this.contrato.EstablecimientoPropio = null;
        }
    };
    CrearContratoAPrecioComponent.prototype.onChangeSearchLocalidad = function (term) {
        var _this = this;
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(function (result) {
                _this.localidades = result;
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    CrearContratoAPrecioComponent.prototype.onBoletoSelected = function () {
        this.contrato.BolsaId = 0;
        if (this.contrato.BoletoId == 1) {
            this.bolsasSelect = this.bolsasConfirma;
        }
        if (this.contrato.BoletoId == 2) {
            this.bolsasSelect = this.bolsasFisico;
        }
        if (this.contrato.BoletoId == 3) {
            this.bolsasSelect = [];
        }
        if (this.contrato.BoletoId == 4) {
            this.bolsasSelect = this.bolsasCarta;
        }
    };
    CrearContratoAPrecioComponent.prototype.changePagoDiferido = function (event) {
        this.contrato.DolarizadoTercero = false;
    };
    CrearContratoAPrecioComponent.prototype.changeDolarizado = function (event) {
        this.contrato.PagoDiferidoTercero = false;
    };
    CrearContratoAPrecioComponent.prototype.isVisiblePagoDiferido = function () {
        return this.contrato.MonedaId == "ARP  ";
    };
    CrearContratoAPrecioComponent.prototype.isVisibleDolarizado = function () {
        return this.contrato.MonedaId == "USDM ";
    };
    CrearContratoAPrecioComponent.prototype.isVisibleBolsa = function () {
        return this.contrato.BoletoId != 3;
    };
    CrearContratoAPrecioComponent.prototype.isVisibleEstablecimiento = function () {
        return this.contrato.ProvinciaId == 1 && this.contrato.ClasificacionId == 1;
    };
    CrearContratoAPrecioComponent.prototype.grabarContratoAPrecio = function () {
        var _this = this;
        if (!this.validarContrato()) {
            return false;
        }
        var hoy = new Date();
        var hoysinhora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
        var dateParts = $("#noCursor").val().split("/");
        this.contrato.FechaDesde = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        dateParts = $("#noCursor2").val().split("/");
        this.contrato.FechaHasta = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        this.contrato.FechaEntrega = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        if (this.contrato.Id == 0) {
            this.contrato.FechaOperacion = hoysinhora;
            this.contrato.Fecha = hoy;
        }
        //this.contrato.Id = 0;
        this.contrato.EstadoId = 9;
        this.contrato.TipoNegocioId = 2;
        if (this.contrato.MaterialId == 1) {
            this.contrato.StandardDeCalidadId = 2;
        }
        if (this.contrato.MaterialId == 2) {
            this.contrato.StandardDeCalidadId = 7;
        }
        if (this.contrato.MaterialId == 3) {
            this.contrato.StandardDeCalidadId = 3;
        }
        if (this.contrato.MaterialId == 4) {
            this.contrato.StandardDeCalidadId = 5;
        }
        if (this.contrato.MaterialId == 5) {
            this.contrato.StandardDeCalidadId = 5;
        }
        this.contrato.ObservacionTercero = this.ObservacionTercero;
        if (this.contrato.DolarizadoTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Dolarizado: " + this.ObservacionDolarizadoTercero;
        }
        if (this.contrato.PagoDiferidoTercero == true) {
            if (this.contrato.PagoDiferidoTerceroId != -1) {
                this.ObservacionPagoDiferidoTercero = this.pagosDiferidos.find(function (x) { return x.Id == _this.contrato.PagoDiferidoTerceroId; }).Descripcion;
            }
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Pago Diferido: " + this.ObservacionPagoDiferidoTercero;
        }
        if (this.contrato.CalidadTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Calidad: " + this.ObservacionCalidadTercero;
        }
        if (this.contrato.SustentableTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Sustentable: " + this.ObservacionSustentableTercero;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        try {
            this.unsubscribe();
            this.subscription = this.service.grabarContratoAPrecio(this.contrato).subscribe(function (result) {
                _this.blockUI.stop();
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
                    var obj = JSON.parse(result);
                    if (obj.HayError) {
                        var errores_1 = "";
                        obj.Errores.forEach(function (element) {
                            if (element.Message == "Proveedor No Operable por Riesgo Comercial Alto") {
                                element.Message = "Proveedor no operable, contactese con la mesa comercial";
                            }
                            errores_1 = errores_1 + element.Message + " - ";
                        });
                        _this.mensajeComponent.setErrorMsg(errores_1);
                    }
                    else {
                        //this.mensajeComponent.setSuccessMsg("El contrato se genero correctamente.");
                        _this.contrato.MaterialId = null;
                        _this.contrato.Cantidad = null;
                        _this.contrato.CampanaId = null;
                        _this.contrato.Precio = null;
                        _this.contrato.PrecioNeto = null;
                        _this.contrato.MonedaId = null;
                        document.getElementById("openModalConfirmModal").click();
                    }
                }
            }, function (error) {
                _this.blockUI.stop();
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.blockUI.stop();
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    CrearContratoAPrecioComponent.prototype.validarContrato = function () {
        if (this.contrato.CantidadCamiones > 0) {
            var cantidadCamionesNecesarios = Math.ceil(this.contrato.Cantidad / 30000);
            if (this.contrato.CantidadCamiones > cantidadCamionesNecesarios) {
                this.mensajeComponent.setErrorMsg("La cantidad de camiones ingresados es mayor a la necesaria");
                return;
            }
            if (this.contrato.CantidadCamiones < cantidadCamionesNecesarios) {
                this.mensajeComponent.setErrorMsg("La cantidad de camiones ingresados es menor a la necesaria");
                return;
            }
        }
        if ((this.contrato.CorredorId != null || this.contrato.CorredorId > 0) && (this.contrato.ComercialId == null)) {
            this.mensajeComponent.setErrorMsg("Debe ingresar la Zona.");
            return false;
        }
        if ((this.contrato.CorredorId != null || this.contrato.CorredorId > 0) && (this.contrato.ContratoCorredor == null || this.contrato.ContratoCorredor == "")) {
            this.mensajeComponent.setErrorMsg("Debe ingresar el Contrato Corredor.");
            return false;
        }
        if ((this.contrato.CorredorId != null || this.contrato.CorredorId > 0) && (this.contrato.ProveedorId == null || this.contrato.ProveedorId == undefined)) {
            this.mensajeComponent.setErrorMsg("Debe seleccionar el Proveedor.");
            return false;
        }
        if ((this.ObservacionDolarizadoTercero == "" || this.ObservacionDolarizadoTercero == undefined) && this.contrato.DolarizadoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación la fecha de Dolarizado.");
            return false;
        }
        if (this.contrato.PagoDiferidoTerceroId == undefined && this.contrato.PagoDiferidoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe seleccionar una opcion de Pago Diferido.");
            return false;
        }
        if ((this.ObservacionPagoDiferidoTercero == "" || this.ObservacionPagoDiferidoTercero == undefined) && this.contrato.PagoDiferidoTercero == true && this.contrato.PagoDiferidoTerceroId == -1) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación el detalle de Pago Diferido.");
            return false;
        }
        if ((this.ObservacionCalidadTercero == "" || this.ObservacionCalidadTercero == undefined) && this.contrato.CalidadTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación el detalle de la calidad.");
            return false;
        }
        if ((this.ObservacionSustentableTercero == "" || this.ObservacionSustentableTercero == undefined) && this.contrato.SustentableTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación la Tarifa Sustentable.");
            return false;
        }
        if (this.contrato.DestinoId == null || this.contrato.DestinoId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar el Destino.");
            return false;
        }
        if (this.contrato.MaterialId == null || this.contrato.MaterialId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar el Material.");
            return false;
        }
        if (this.contrato.CampanaId == null || this.contrato.CampanaId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar la Campaña.");
            return false;
        }
        if (this.contrato.ClasificacionId == null || this.contrato.ClasificacionId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar la Condicion vendedor.");
            return false;
        }
        if (this.contrato.LocalidadId == null || this.contrato.LocalidadId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar la Localidad.");
            return false;
        }
        if (this.contrato.BoletoId == null || this.contrato.BoletoId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar el Boleto.");
            return false;
        }
        if ((this.contrato.BolsaId == null || this.contrato.BolsaId == undefined) && this.contrato.BoletoId != 3) {
            this.mensajeComponent.setErrorMsg("Debe completar la Bolsa.");
            return false;
        }
        if (this.contrato.Pizarra == false && (this.contrato.MonedaId == null || this.contrato.MonedaId == undefined || this.contrato.MonedaId == "")) {
            this.mensajeComponent.setErrorMsg("Debe completar la Moneda.");
            return false;
        }
        if (this.contrato.Cantidad == null || this.contrato.Cantidad == undefined || this.contrato.Cantidad <= 0) {
            this.mensajeComponent.setErrorMsg("Debe completar la Cantidad.");
            return false;
        }
        return true;
    };
    CrearContratoAPrecioComponent.prototype.changePlanCanje = function (event) {
        this.contrato.Consignatario = false;
    };
    CrearContratoAPrecioComponent.prototype.changeConsignatario = function (event) {
        this.contrato.PlanCanje = false;
    };
    CrearContratoAPrecioComponent.prototype.changeClasificacion = function (event) {
        if (this.contrato.ClasificacionId == 1) {
            this.contrato.PlanCanje = false;
            this.contrato.Consignatario = false;
        }
        else {
            this.contrato.EstablecimientoPropio = null;
        }
    };
    CrearContratoAPrecioComponent.prototype.isNotProductor = function (event) {
        if (this.contrato.ClasificacionId == 1) {
            return false;
        }
        else {
            return true;
        }
    };
    CrearContratoAPrecioComponent.prototype.onChangeMaterial = function () {
        if (this.contrato.MaterialId != null) {
            this.contrato.CampanaId = null;
            this.habilitaciones(this.contrato);
            this.contrato.Precio = 0;
            this.contrato.MonedaId = null;
            this.contrato.Pizarra = false;
            this.contrato.CalidadTercero = false;
            this.contrato.SustentableTercero = false;
        }
    };
    //changePizarra() {
    //    this.contrato.Precio = 0;
    //    this.contrato.MonedaId = null;
    //    if (this.contrato.Pizarra == true) {
    //        this.contrato.FechaDesde = new Date(parseInt(this.datosPizarra.DesdeEntrega.substr(6)));
    //        this.contrato.FechaHasta = this.ObtenerFechaHasta(this.contrato.FechaDesde);
    //        this.fechaInicio = this.contrato.FechaDesde.toLocaleDateString('en-GB');
    //        this.fechaFin = this.contrato.FechaHasta.toLocaleDateString('en-GB');
    //        var entregaHasta = new Date(parseInt(this.datosPizarra.HastaEntrega.substr(6)));
    //        $('.form_datetime_Inicio').datetimepicker('setStartDate', this.contrato.FechaDesde.toLocaleDateString("en-GB"));
    //        $('.form_datetime_Inicio').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));
    //        $('.form_datetime_Fin').datetimepicker('setStartDate', this.contrato.FechaDesde.toLocaleDateString("en-GB"));
    //        $('.form_datetime_Fin').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));
    //    } else {
    //    }
    //}
    CrearContratoAPrecioComponent.prototype.disablePrecio = function () {
        return this.contrato.Pizarra == true;
    };
    CrearContratoAPrecioComponent.prototype.changeMoneda = function (event) {
        this.contrato.PagoDiferidoTercero = false;
        this.contrato.DolarizadoTercero = false;
        if (this.contrato.MonedaId != "" && this.contrato.MonedaId != undefined && this.contrato.MonedaId != null) {
            // buscar precio moa
            var leng = this.datosPrecioMoa.length;
            var items = this.datosPrecioMoa;
            for (var i = 0; i < leng; i++) {
                var item = items[i];
                console.log(item);
                if (item.MonedaId == this.contrato.MonedaId) {
                    if (item.Precio == 0) {
                        this.contrato.Precio = null;
                    }
                    else {
                        this.contrato.Precio = item.Precio;
                        this.contrato.FechaDesde = new Date(parseInt(item.DesdeEntrega.substr(6)));
                        this.contrato.FechaHasta = this.ObtenerFechaHasta(this.contrato.FechaDesde);
                        this.fechaInicio = this.contrato.FechaDesde.toLocaleDateString('en-GB');
                        this.fechaFin = this.contrato.FechaHasta.toLocaleDateString('en-GB');
                        var entregaHasta = new Date(parseInt(item.HastaEntrega.substr(6)));
                        //$('.form_datetime_Inicio').datetimepicker('setStartDate', this.contrato.FechaDesde.toLocaleDateString("en-GB"));
                        //$('.form_datetime_Inicio').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));
                        //$('.form_datetime_Fin').datetimepicker('setStartDate', this.contrato.FechaDesde.toLocaleDateString("en-GB"));
                        //$('.form_datetime_Fin').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));
                    }
                }
            }
        }
        else {
            this.contrato.Precio = 0;
        }
    };
    CrearContratoAPrecioComponent.prototype.configuraciones = function () {
    };
    CrearContratoAPrecioComponent.prototype.isSoja = function () {
        return this.contrato.MaterialId == 3;
    };
    CrearContratoAPrecioComponent = __decorate([
        Component({
            selector: 'app-crear-contrato-aprecio',
            templateUrl: "crear-contrato.aprecio.component.html",
            providers: [{ provide: CrearContratoService, useClass: CrearContratoAPrecioService }]
        })
    ], CrearContratoAPrecioComponent);
    return CrearContratoAPrecioComponent;
}(CrearContratoBaseComponent));
export { CrearContratoAPrecioComponent };
//# sourceMappingURL=crear-contrato.aprecio.component.js.map