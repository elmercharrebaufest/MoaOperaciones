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
import { CrearContratoService, CrearContratoFijacionService } from './../crear-contrato.service';
import { ContratoFijacion } from "../../common/models/contratoFijacion";
var CrearContratoFijacionComponent = /** @class */ (function (_super) {
    __extends(CrearContratoFijacionComponent, _super);
    function CrearContratoFijacionComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.contrato = new ContratoFijacion();
        return _this;
    }
    CrearContratoFijacionComponent.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
        console.log("inicia el componente");
        this.negocioHabilitado(this.contrato);
    };
    CrearContratoFijacionComponent.prototype.ngAfterViewInit = function () {
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
    };
    CrearContratoFijacionComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("crear-contrato", "Fijacion");
    };
    CrearContratoFijacionComponent.prototype.selectEventProveedor = function (item) {
        this.contrato.MaterialId = null;
        this.pendientesFijar = [];
        this.pendienteFijar = null;
        this.contrato.ProveedorId = item.Id;
        this.cuitProveedorSeleccionado = item.CUIT;
        console.log("prov: ", item.Id);
        if (item != null && item.Id != null && item.Id > 0) {
            this.obtenerDatosCompraNet(this.contrato, item.Id);
        }
    };
    CrearContratoFijacionComponent.prototype.onChangeSearchProveedor = function (term) {
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
    CrearContratoFijacionComponent.prototype.selectEventLocalidad = function (item) {
        this.contrato.LocalidadId = item.LocalidadId;
        this.contrato.ProvinciaId = item.ProvinciaId;
        if (item.ProvinciaId != 1) {
            this.contrato.EstablecimientoPropio = null;
        }
    };
    CrearContratoFijacionComponent.prototype.onChangeSearchLocalidad = function (term) {
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
    CrearContratoFijacionComponent.prototype.onBoletoSelected = function () {
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
    CrearContratoFijacionComponent.prototype.isVisiblePagoDiferido = function () {
        return this.contrato.MonedaId == "ARP  ";
    };
    CrearContratoFijacionComponent.prototype.isVisibleDolarizado = function () {
        return this.contrato.MonedaId == "USDM ";
    };
    CrearContratoFijacionComponent.prototype.isVisibleBolsa = function () {
        return this.contrato.BoletoId != 3;
    };
    CrearContratoFijacionComponent.prototype.isVisibleEstablecimiento = function () {
        return this.contrato.ProvinciaId == 1;
    };
    CrearContratoFijacionComponent.prototype.grabarContratoFijacion = function () {
        var _this = this;
        console.log(this.contrato);
        if (!this.validarContrato()) {
            return false;
        }
        var hoy = new Date();
        var hoysinhora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
        //var dateParts = $("#noCursor").val().split("/");
        //this.contrato.FechaDesde = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        //dateParts = $("#noCursor2").val().split("/");
        //this.contrato.FechaHasta = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        //this.contrato.FechaEntrega = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        this.contrato.FechaOperacion = hoysinhora;
        this.contrato.Fecha = hoy;
        this.contrato.Id = 0;
        this.contrato.EstadoId = 9;
        this.contrato.ProvinciaId;
        this.contrato.TipoNegocioId = 3;
        this.contrato.StandardDeCalidadId = null;
        this.contrato.PrecioNeto = this.contrato.Precio;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        try {
            this.unsubscribe();
            this.subscription = this.service.grabarContratoFijacion(this.contrato).subscribe(function (result) {
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
                        _this.contrato.ContratoSAP = null;
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
    CrearContratoFijacionComponent.prototype.validarContrato = function () {
        if (this.contrato.ContratoSAP == null || this.contrato.ContratoSAP == undefined || this.contrato.ContratoSAP == "" || this.pendienteFijar == null || this.pendienteFijar == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar el Contrato.");
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
        if (this.contrato.LocalidadId == null || this.contrato.LocalidadId == undefined) {
            this.mensajeComponent.setErrorMsg("Debe completar la Localidad.");
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
        if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.DolarizadoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación la fecha de Dolarizado.");
            return false;
        }
        if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.PagoDiferidoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación el detalle de Pago Diferido.");
            return false;
        }
        return true;
    };
    CrearContratoFijacionComponent.prototype.changePlanCanje = function (event) {
        this.contrato.Consignatario = false;
    };
    CrearContratoFijacionComponent.prototype.changeConsignatario = function (event) {
        this.contrato.PlanCanje = false;
    };
    CrearContratoFijacionComponent.prototype.changeClasificacion = function (event) {
        if (this.contrato.ClasificacionId == 1) {
            this.contrato.PlanCanje = false;
            this.contrato.Consignatario = false;
        }
    };
    CrearContratoFijacionComponent.prototype.isNotProductor = function (event) {
        if (this.contrato.ClasificacionId == 1) {
            return false;
        }
        else {
            return true;
        }
    };
    CrearContratoFijacionComponent.prototype.onChangeMaterial = function () {
        if (this.contrato.MaterialId != null) {
            this.pendientesFijar = [];
            this.pendienteFijar = null;
            this.contrato.CampanaId = null;
            this.habilitaciones(this.contrato);
            this.contrato.Precio = 0;
            this.contrato.MonedaId = null;
            this.contrato.Pizarra = false;
        }
    };
    CrearContratoFijacionComponent.prototype.changePizarra = function () {
        this.contrato.Precio = 0;
        this.contrato.MonedaId = null;
    };
    CrearContratoFijacionComponent.prototype.disablePrecio = function () {
        return this.contrato.Pizarra == true;
    };
    CrearContratoFijacionComponent.prototype.changeMoneda = function (event) {
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
                    }
                }
            }
        }
        else {
            this.contrato.Precio = 0;
        }
    };
    CrearContratoFijacionComponent.prototype.selectEventContratoId = function (item) {
        console.log(item);
        this.contrato.ContratoSAP = item.ContratoId;
        this.contrato.Posicion = item.Posicion;
        var dateParts = item.DesdeEntrega.split("-");
        var dateObject = new Date(+dateParts[2], +dateParts[1] - 1, +dateParts[0]);
        this.contrato.FechaDesde = dateObject;
        //console.log("FechaDesde");
        //console.log("dateParts", dateParts);
        //console.log("dateObject", dateObject);
        //console.log("this.contrato.FechaDesde", this.contrato.FechaDesde);
        dateParts = item.HastaEntrega.split("-");
        dateObject = new Date(+dateParts[2], +dateParts[1] - 1, +dateParts[0]);
        this.contrato.FechaHasta = dateObject;
        this.contrato.FechaEntrega = dateObject;
        //console.log("FechaHasta");
        //console.log("dateParts", dateParts);
        //console.log("dateObject", dateObject);
        //console.log("this.contrato.FechaHasta", this.contrato.FechaHasta);
        this.contrato.CampanaId = item.CampanaId;
        this.contrato.DestinoId = item.Centro;
        this.contrato.TrigoEspecial = item.Calidad;
    };
    CrearContratoFijacionComponent.prototype.onChangeSearchContratoId = function (term) {
        //if (term.length > 2) {
        //    this.unsubscribe();
        //    this.subscription = this.service.searchLocalidad(term).subscribe(
        //        result => {
        //            this.localidades = result;
        //        },
        //        error => {
        //            this.mensajeComponent.setErrorMsg(error.message);
        //        }
        //    );
        //}
    };
    CrearContratoFijacionComponent.prototype.isVisibleContratoDetalle = function () {
        var result = !(this.pendienteFijar == null || this.pendienteFijar == "" || this.pendienteFijar == undefined);
        return result;
    };
    CrearContratoFijacionComponent = __decorate([
        Component({
            selector: 'app-crear-contrato-fijacion',
            templateUrl: "crear-contrato.fijacion.component.html",
            providers: [{ provide: CrearContratoService, useClass: CrearContratoFijacionService }]
        })
    ], CrearContratoFijacionComponent);
    return CrearContratoFijacionComponent;
}(CrearContratoBaseComponent));
export { CrearContratoFijacionComponent };
//# sourceMappingURL=crear-contrato.fijacion.component.js.map