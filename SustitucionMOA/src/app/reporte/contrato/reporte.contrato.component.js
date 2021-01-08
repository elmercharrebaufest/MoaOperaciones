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
import { ReporteBaseComponent } from './../reporte.component';
import { ReporteService, ReporteContratoService } from './../reporte.service';
var ReporteContratoComponent = /** @class */ (function (_super) {
    __extends(ReporteContratoComponent, _super);
    function ReporteContratoComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.fechaDesde = null;
        _this.fechaHasta = null;
        _this.entregaDesde = null;
        _this.entregaHasta = null;
        _this.fijacionHasta = null;
        _this.proveedor = null;
        _this.boletoId = "";
        _this.clasificacionId = "";
        _this.destinoId = "";
        _this.estadoId = "";
        _this.materialId = "";
        _this.campaniaId = "";
        _this.tipoNegocioId = "";
        _this.pagoDiferidoTercero = null;
        _this.calidadTercero = null;
        _this.dolarizadoTercero = null;
        _this.sustentableTercero = null;
        _this.proveedores = [];
        _this.keyword2 = "RazonSocial";
        _this.proveedorid = null;
        return _this;
    }
    ReporteContratoComponent.prototype.setTabs = function () {
        this.setMenuSeccionTab("reporte", "Contratos");
    };
    ReporteContratoComponent.prototype.ngAfterViewInit = function () {
        var hoy = new Date();
        var hoysinhora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
        this.fechaDesde = hoysinhora.toLocaleDateString('en-GB');
        $(document).on("mouseover", '.form_datetime_fechaDesde', function () {
            $('.form_datetime_fechaDesde').datetimepicker({
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
                //if (ev.date.valueOf()) {
                //    var fechaBase = new Date(ev.date.valueOf());
                //    var hoy = fechaBase != undefined ? fechaBase : new Date();
                //    var anio = hoy.getFullYear();
                //    var mesPost = hoy.getMonth() + 2;
                //    var dia = hoy.getDate();
                //    var ultimoDia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
                //    if (dia === 1) {
                //        dia = new Date(anio, hoy.getMonth() + 1, 0).getDate();
                //        mesPost = hoy.getMonth() + 1;
                //    }
                //    if (dia === ultimoDia || (mesPost === 2 && dia >= 29)) {
                //        dia = new Date(anio, mesPost, 0).getDate();
                //    }
                //    if (mesPost === 13) {
                //        mesPost = 1;
                //        anio += 1;
                //    }
                //    var messtring = "" + mesPost;
                //    if (mesPost < 10) {
                //        messtring = "0" + mesPost.toString();
                //    }
                //    var diastring = "" + dia;
                //    if (dia < 10) {
                //        diastring = "0" + dia.toString();
                //    }
                //    $("#noCursor2_fechaHasta").val(diastring + '/' + messtring + '/' + anio);
                //}
            });
        });
        $(document).on("mouseover", '.form_datetime_fechaHasta', function () {
            $('.form_datetime_fechaHasta').datetimepicker({
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
        $(document).on("mouseover", '.form_datetime_entregaDesde', function () {
            $('.form_datetime_entregaDesde').datetimepicker({
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
        $(document).on("mouseover", '.form_datetime_entregaHasta', function () {
            $('.form_datetime_entregaHasta').datetimepicker({
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
        $(document).on("mouseover", '.form_datetime_fijacionHasta', function () {
            $('.form_datetime_fijacionHasta').datetimepicker({
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
        this.getDatosCombos();
    };
    ReporteContratoComponent.prototype.obteneContratos = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        if (this.validar()) {
            this.unsubscribe();
            this.subscription = this.service.obteneContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId, this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero, this.sustentableTercero).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
                var resultlist = JSON.parse(result);
                _this.data = resultlist.Data;
                _this.data = resultlist.Data.map(function (x) {
                    var item = {
                        Cuit: x.Cuit,
                        Proveedor: x.Proveedor,
                        Corredor: x.Corredor,
                        ContratoCorredor: x.ContratoCorredor,
                        TipoNegocio: x.TipoNegocio,
                        Cantidad: x.Cantidad,
                        Precio: x.Precio,
                        Moneda: x.Moneda,
                        DestinoDescripcion: x.DestinoDescripcion,
                        FechaDesde: new Date(parseInt(x.FechaDesde.substr(6))),
                        FechaHasta: new Date(parseInt(x.FechaHasta.substr(6))),
                        Material: x.Material,
                        Campania: x.Campania,
                        Clasificacion: x.Clasificacion,
                        Localidad: x.Localidad,
                        Consignatario: x.Consignatario,
                        Estado_Contrato: x.Estado_Contrato,
                        PagoDiferidoTercero: x.PagoDiferidoTercero,
                        DolarizadoTercero: x.DolarizadoTercero,
                        CalidadTercero: x.CalidadTercero,
                        SustentableTercero: x.SustentableTercero
                    };
                    return item;
                });
                console.log(_this.data);
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
    };
    ReporteContratoComponent.prototype.validar = function () {
        var dateParts = $("#noCursor_fechaDesde").val().split("/");
        var fechaDesde = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        dateParts = $("#noCursor_fechaHasta").val().split("/");
        var fechaHasta = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        this.fechaDesde = $("#noCursor_fechaDesde").val();
        this.fechaHasta = $("#noCursor_fechaHasta").val();
        this.entregaDesde = $("#noCursor_entregaDesde").val();
        this.entregaHasta = $("#noCursor_entregaHasta").val();
        this.fijacionHasta = $("#noCursor_fijacionHasta").val();
        return true;
    };
    ReporteContratoComponent.prototype.exportExcel = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId, this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero, this.sustentableTercero).subscribe(function (result) {
            _this.spinnerSmallComponent.hideIt();
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
                var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                if (window.navigator.msSaveOrOpenBlob) {
                    // IE11
                    window.navigator.msSaveOrOpenBlob(blob, "Contratos.xls");
                }
                else {
                    var url = window.URL.createObjectURL(blob);
                    var link = document.createElement("a");
                    document.body.appendChild(link);
                    link.href = url;
                    link.download = "Contratos.xls";
                    link.click();
                    setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                    return false;
                }
            }
        }, function (error) {
            _this.spinnerSmallComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false; // <- Prevent href del a
    };
    ReporteContratoComponent.prototype.getDatosCombos = function () {
        var _this = this;
        this.unsubscribe();
        this.subscription = this.service.getDatosCombos().subscribe(function (result) {
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
                _this.obtenerMateriales();
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ReporteContratoComponent.prototype.validarDirecto = function () {
        var _this = this;
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
                    _this.corredorId = obj;
                    _this.esCorredorEnDataAgro = true;
                }
                else {
                    //this.getDatosCombos();
                    _this.esCorredorEnDataAgro = false;
                }
                _this.obteneContratos();
                console.log("this.esCorredorEnDataAgro", _this.esCorredorEnDataAgro);
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
        return false;
    };
    ReporteContratoComponent.prototype.obtenerMateriales = function () {
        var _this = this;
        this.subscription = this.service.obtenerMateriales().subscribe(function (result) {
            var obj = JSON.parse(result);
            obj.Datos.forEach(function (element) {
                var el = {
                    Id: element.MaterialId.toString(),
                    Descripcion: element.Descripcion
                };
                _this.materiales.push(el);
            });
            _this.validarDirecto();
        }, function (error) {
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    ReporteContratoComponent = __decorate([
        Component({
            selector: 'app-reporte-contrato',
            templateUrl: "reporte.contrato.component.html",
            providers: [{ provide: ReporteService, useClass: ReporteContratoService }]
        })
    ], ReporteContratoComponent);
    return ReporteContratoComponent;
}(ReporteBaseComponent));
export { ReporteContratoComponent };
//# sourceMappingURL=reporte.contrato.component.js.map