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
import { CrearContratoService, CrearContratoAFijarService } from './../crear-contrato.service';
import { ContratoAFijar } from "../../common/models/contratoAFijar";
var CrearContratoAFijarComponent = /** @class */ (function (_super) {
    __extends(CrearContratoAFijarComponent, _super);
    function CrearContratoAFijarComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.contrato = new ContratoAFijar();
        return _this;
    }
    CrearContratoAFijarComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("crear-contrato", "A Fijar");
    };
    CrearContratoAFijarComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        this.contrato.TipoNegocioId = 1;
        this.negocioHabilitado(this.contrato);
        this.contrato.CondicionFijacionId = 7;
    };
    CrearContratoAFijarComponent.prototype.ngAfterViewInit = function () {
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
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
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
                todayBtn: 1,
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
        //--
        $(document).on("mouseover", '.form_datetime_fInicio', function () {
            $('.form_datetime_fInicio').datetimepicker({
                format: 'dd/mm/yyyy',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4
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
                    $("#noCursor2f").val(diastring + '/' + messtring + '/' + anio);
                }
            });
        });
        $(document).on("mouseover", '.form_datetime_fFin', function () {
            $('.form_datetime_fFin').datetimepicker({
                format: 'dd/mm/yyyy',
                language: 'es',
                weekStart: 1,
                todayBtn: 1,
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
        this.fechafInicio = hoysinhora.toLocaleDateString('en-GB');
        this.fechafFin = hoymesqueviene.toLocaleDateString('en-GB');
        //--
    };
    CrearContratoAFijarComponent.prototype.selectEventProveedor = function (item) {
        this.contrato.ProveedorId = item.Id;
        console.log("prov: ", item.Id);
        if (item != null && item.Id != null && item.Id > 0) {
            this.obtenerDatosCompraNet(this.contrato, item.Id);
        }
    };
    CrearContratoAFijarComponent.prototype.onChangeSearchProveedor = function (term) {
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
    CrearContratoAFijarComponent.prototype.selectEventLocalidad = function (item) {
        this.contrato.LocalidadId = item.LocalidadId;
        this.contrato.ProvinciaId = item.ProvinciaId;
        if (item.ProvinciaId != 1) {
            this.contrato.EstablecimientoPropio = null;
        }
    };
    CrearContratoAFijarComponent.prototype.onChangeSearchLocalidad = function (term) {
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
    CrearContratoAFijarComponent.prototype.onBoletoSelected = function () {
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
    CrearContratoAFijarComponent.prototype.isVisibleBolsa = function () {
        return this.contrato.BoletoId != 3;
    };
    CrearContratoAFijarComponent.prototype.isVisibleEstablecimiento = function () {
        return this.contrato.ProvinciaId == 1 && this.contrato.ClasificacionId == 1;
    };
    CrearContratoAFijarComponent.prototype.grabarContratoAFijar = function () {
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
        dateParts = $("#noCursorf").val().split("/");
        this.contrato.DesdeFijacion = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        dateParts = $("#noCursor2f").val().split("/");
        this.contrato.HastaFijacion = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        this.contrato.MonedaId = "ARP  ";
        this.contrato.Precio = 0;
        this.contrato.PrecioNeto = 0;
        this.contrato.FechaOperacion = hoysinhora;
        this.contrato.Fecha = hoy;
        this.contrato.Id = 0;
        this.contrato.EstadoId = 9;
        this.contrato.ProvinciaId;
        this.contrato.TipoNegocioId = 1;
        if (this.contrato.MaterialId == 1) {
            this.contrato.StandardDeCalidadId = 2;
        }
        if (this.contrato.MaterialId == 2) {
            this.contrato.StandardDeCalidadId = 7;
        }
        if (this.contrato.MaterialId == 3) {
            this.contrato.StandardDeCalidadId = 4;
        }
        if (this.contrato.MaterialId == 4) {
            this.contrato.StandardDeCalidadId = 5;
        }
        if (this.contrato.MaterialId == 5) {
            this.contrato.StandardDeCalidadId = 5;
        }
        this.contrato.ObservacionTercero = this.ObservacionTercero;
        if (this.contrato.CalidadTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Calidad: " + this.ObservacionCalidadTercero;
        }
        if (this.contrato.SustentableTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Sustentable: " + this.ObservacionSustentableTercero;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        console.log(this.contrato);
        try {
            this.unsubscribe();
            this.subscription = this.service.grabarContratoAFijar(this.contrato).subscribe(function (result) {
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
    CrearContratoAFijarComponent.prototype.validarContrato = function () {
        //if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.DolarizadoTercero == true) {
        //  this.mensajeComponent.setErrorMsg("Debe completar en la observación la fecha de Dolarizado.");        //    return false;
        //}
        //if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.PagoDiferidoTercero == true) {
        //    this.mensajeComponent.setErrorMsg("Debe completar en la observación el detalle de Pago Diferido.");
        //    return false;
        //}
        if ((this.ObservacionCalidadTercero == "" || this.ObservacionCalidadTercero == undefined) && this.contrato.CalidadTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación el detalle de la calidad.");
            return false;
        }
        if ((this.ObservacionSustentableTercero == "" || this.ObservacionSustentableTercero == undefined) && this.contrato.SustentableTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación la Tarifa Sustentable.");
            return false;
        }
        if ((this.contrato.CorredorId != null || this.contrato.CorredorId > 0) && (this.contrato.ProveedorId == null || this.contrato.ProveedorId == undefined)) {
            this.mensajeComponent.setErrorMsg("Debe seleccionar el Proveedor.");
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
        if (this.contrato.Cantidad == null || this.contrato.Cantidad == undefined || this.contrato.Cantidad <= 0) {
            this.mensajeComponent.setErrorMsg("Debe completar la Cantidad.");
            return false;
        }
        //if (this.contrato.MonedaId == null || this.contrato.MonedaId == undefined || this.contrato.MonedaId == "") {
        //    this.mensajeComponent.setErrorMsg("Debe completar la Moneda.");
        //    return false;
        //}
        return true;
    };
    CrearContratoAFijarComponent.prototype.changePlanCanje = function (event) {
        this.contrato.Consignatario = false;
    };
    CrearContratoAFijarComponent.prototype.changeConsignatario = function (event) {
        this.contrato.PlanCanje = false;
    };
    CrearContratoAFijarComponent.prototype.changeClasificacion = function (event) {
        if (this.contrato.ClasificacionId == 1) {
            this.contrato.PlanCanje = false;
            this.contrato.Consignatario = false;
        }
        else {
            this.contrato.EstablecimientoPropio = null;
        }
    };
    CrearContratoAFijarComponent.prototype.isNotProductor = function (event) {
        if (this.contrato.ClasificacionId == 1) {
            return false;
        }
        else {
            return true;
        }
    };
    CrearContratoAFijarComponent.prototype.onChangeMaterial = function () {
        if (this.contrato.MaterialId != null) {
            this.habilitaciones(this.contrato);
            this.contrato.CalidadTercero = false;
            this.contrato.SustentableTercero = false;
        }
    };
    CrearContratoAFijarComponent.prototype.isSoja = function () {
        return this.contrato.MaterialId == 3;
    };
    CrearContratoAFijarComponent = __decorate([
        Component({
            selector: 'app-crear-contrato-afijar',
            templateUrl: "crear-contrato.afijar.component.html",
            providers: [{ provide: CrearContratoService, useClass: CrearContratoAFijarService }]
        })
    ], CrearContratoAFijarComponent);
    return CrearContratoAFijarComponent;
}(CrearContratoBaseComponent));
export { CrearContratoAFijarComponent };
//# sourceMappingURL=crear-contrato.afijar.component.js.map