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
        //dfechaDesde: any = null;
        //dfechaHasta: any = null;
        //dentregaDesde: any = null;
        //dentregaHasta: any = null;
        //dfijacionHasta: any = null;
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
    };
    ReporteContratoComponent.prototype.obteneContratos = function () {
        var _this = this;
        if (this.validar()) {
            this.unsubscribe();
            this.subscription = this.service.obteneContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId, this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero).subscribe(function (result) {
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