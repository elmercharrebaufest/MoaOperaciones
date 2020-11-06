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

    //dfechaDesde: any = null;
    //dfechaHasta: any = null;
    //dentregaDesde: any = null;
    //dentregaHasta: any = null;
    //dfijacionHasta: any = null;

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
        if (this.validar()) {
            this.unsubscribe();
            this.subscription = this.service.obteneContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId,
                this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero).subscribe(
                    result => {
                        var resultlist = JSON.parse(result);

                        this.proveedores = resultlist.map(prov => {
                            return { Id: prov.Id, RazonSocial: prov.RazonSocial + " (" + prov.Cuit + ")", CUIT: prov.Cuit }
                        })
                        //this.proveedores = JSON.parse(result);
                    },
                    error => {
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
}