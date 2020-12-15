import { Component, OnInit, ViewChild } from '@angular/core';
import { ReporteBaseComponent } from './../reporte.component';
import { ReporteService, ReporteContratoService } from './../reporte.service';
declare var $: any;


@Component({
    selector: 'app-reporte-contrato',
    templateUrl: `reporte.contrato.component.html`,
    providers: [{ provide: ReporteService, useClass: ReporteContratoService }]
})
export class ReporteContratoComponent extends ReporteBaseComponent {

    fechaDesde: any = null;
    fechaHasta: any = null;
    entregaDesde: any = null;
    entregaHasta: any = null;
    fijacionHasta: any = null;
    
    proveedor: any = null;
    boletoId: string = "";
    clasificacionId: string = "";
    destinoId: string = "";
    estadoId: string = "";
    materialId: string = "";
    campaniaId: string = "";
    tipoNegocioId: string = "";
    pagoDiferidoTercero: boolean = null;
    calidadTercero: boolean = null;
    dolarizadoTercero: boolean = null;

    setTabs() {
        this.setMenuSeccionTab("reporte", "Contratos");
    }

    ngAfterViewInit(): void {


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


    }   

    obteneContratos() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        if (this.validar()) {
            this.unsubscribe();
            this.subscription = this.service.obteneContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId,
                this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero).subscribe(
                    result => {
                        this.spinnerComponent.hideIt();
                        var resultlist = JSON.parse(result);
                        this.data = resultlist.Data;
                        this.data = resultlist.Data.map(function (x) {
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
                            };
                            return item;
                        });
                        console.log(this.data);
                    },
                    error => {
                        this.spinnerComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
        }
    }

    validar() {
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
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId,
            this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, "Contratos.xls");
                    } else {
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
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;  // <- Prevent href del a
    }
}