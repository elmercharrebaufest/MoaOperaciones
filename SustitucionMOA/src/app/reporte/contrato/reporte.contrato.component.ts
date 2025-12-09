import { Component, OnInit } from '@angular/core';
import { ReporteBaseComponent } from './../reporte.component';
import { ReporteService, ReporteContratoService } from './../reporte.service';
import { registerLocaleData } from '@angular/common';
import { Seccion } from './../../common/models/seccion';
import { interval, Subscription } from 'rxjs';

import es from '@angular/common/locales/es';
declare var $: any;


@Component({
    selector: 'app-reporte-contrato',
    templateUrl: `reporte.contrato.component.html`,
    providers: [{ provide: ReporteService, useClass: ReporteContratoService }]
})
export class ReporteContratoComponent extends ReporteBaseComponent implements OnInit {

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
    sustentableTercero: boolean = null;
    contratoCorredor: string = "";

    proveedores = [];
    keyword2 = "RazonSocial";
    proveedorid: any = null;
    motivoAnulacion: string = "";
    negocioParAanular: any;

    actualizarAutomaticamente: boolean = true;


    setTabs() {
        this.setMenuSeccionTab("reporte", "Contratos");
    }

    ngOnInit() {
        registerLocaleData(es);
        this.navService.setSeccionList(
            [
                new Seccion('/reporte/contrato', 'reporte', 'Contratos'),
                //new Seccion('/reporte/cupo', 'reporte', 'Cupos'),
            ]
        );
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

        this.getDatosCombos();
        const source = interval(30000);
        this.subscription = source.subscribe(val => this.refrescar());
    }

    obteneContratos() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        if (this.validar()) {
            this.unsubscribe();
            this.subscription = this.service.obteneContratos(this.fechaDesde, this.fechaHasta, this.entregaDesde, this.entregaHasta, this.fijacionHasta, this.corredorId,
                this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero, this.sustentableTercero, this.contratoCorredor).subscribe(
                    (result:any) => {
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
                                PrecioNeto: x.PrecioNeto,
                                Moneda: x.Moneda,
                                DestinoDescripcion: x.DestinoDescripcion,
                                FechaDesde: new Date(parseInt(x.FechaDesde.substr(6))),
                                FechaHasta: new Date(parseInt(x.FechaHasta.substr(6))),
                                FechaOperacion: new Date(parseInt(x.FechaOperacion.substr(6))),
                                Material: x.Material,
                                Campania: x.Campania,
                                Clasificacion: x.Clasificacion,
                                Localidad: x.Localidad,
                                Consignatario: x.Consignatario,
                                Estado_Contrato: x.Estado_Contrato,
                                Estado: x.Estado,
                                PagoDiferidoTercero: x.PagoDiferidoTercero != true ? "No" : "Si",
                                DolarizadoTercero: x.DolarizadoTercero,
                                CalidadTercero: x.CalidadTercero,
                                SustentableTercero: x.SustentableTercero,
                                TipoNegocioId: x.TipoNegocioId,
                                Id: x.Id,
                                ObservacionTercero: x.ObservacionTercero,
                                Acuerdo: x.Acuerdo,
                                Contrato: x.ContratoSAP,
                                Dias_Pesificado: x.Dias_Pesificado,
                                ImporteFinanciero: x.ImporteFinanciero,
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

    obtenerPagoDiferido(observacion) {
        var p = observacion.split("|");
        var n = "Si";
        var f = p.filter(function (e) { return e.includes("Pago Diferido:") });
        if (f) {
            n = f[0].split(":")[1].trim();
        }
        return n;
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
            this.proveedorId, this.boletoId, this.clasificacionId, this.destinoId, this.estadoId, this.materialId, this.campaniaId, this.tipoNegocioId, this.pagoDiferidoTercero, this.calidadTercero, this.dolarizadoTercero, this.sustentableTercero, this.contratoCorredor).subscribe(
                (result:any) => {
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

    getDatosCombos() {
        this.unsubscribe();
        this.subscription = this.service.getDatosCombos().subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {

                    let obj = JSON.parse(result.DatosContrato);
                    this.datosContrato = obj;

                    obj.Datos.Bolsa.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.bolsasConfirma.push(el);
                        if (el.Descripcion == "Bs As" || el.Descripcion == "Rosario")
                            this.bolsasFisico.push(el);
                        if (element.Descripcion == "Bs As")
                            this.bolsasCarta.push(el);
                    });
                    obj.Datos.Clasificacion.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.condicionVendedor.push(el);
                    });
                    obj.Datos.Zona.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.zona.push(el);
                    });
                    obj.Datos.Destino.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.destinos.push(el);
                    });
                    obj.Datos.campania.forEach(element => {
                        let el = {
                            Id: element.CampaniaId,
                            Descripcion: element.Descripcion
                        }
                        this.campanias.push(el);
                    });
                    obj.Datos.moneda.forEach(element => {
                        let el = {
                            Id: element.MonedaId,
                            Descripcion: element.Descripcion
                        }
                        this.monedas.push(el);
                    });
                    obj.Datos.Condicion.forEach(element => {
                        let el = {
                            Id: element.Id,
                            Descripcion: element.Descripcion
                        }
                        this.condicionFijacion.push(el);
                    });

                    this.obtenerMateriales();
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    validarDirecto() {
        this.subscription = this.service.validarDirecto().subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    if (obj != null && obj > 0) {
                        this.corredorId = obj;
                        this.esCorredorEnDataAgro = true;
                    } else {
                        //this.getDatosCombos();
                        this.esCorredorEnDataAgro = false;
                    }
                    this.obteneContratos();
                    console.log("this.esCorredorEnDataAgro", this.esCorredorEnDataAgro);
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }


    obtenerMateriales() {

        this.subscription = this.service.obtenerMateriales().subscribe(
            (result:any) => {
                result.Datos.forEach((element: any) => {
                    let el = {
                        Id: element.MaterialId.toString(),
                        Descripcion: element.Descripcion
                    }
                    this.materiales.push(el);
                });
                this.validarDirecto();
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    anularNegocio() {
        if (this.motivoAnulacion == null || this.motivoAnulacion == "") {
            this.mensajeComponent.setErrorMsg("Ingrese el motivo de anulacion.");
            document.getElementById("openModalanularModal").click();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.subscription = this.service.anularNegocio(this.negocioParAanular.Id, this.negocioParAanular.TipoNegocioId, this.motivoAnulacion).subscribe(
            (result:any) => {
                this.obteneContratos();
                let obj = JSON.parse(result);
                if (obj.HayError) {
                    this.mensajeComponent.setErrorMsg(obj.Errores[0].Message);
                } else {
                    this.mensajeComponent.setSuccessMsg("El contrato se anulo correctamente.");
                }
                document.getElementById("openModalanularModal").click();
                this.motivoAnulacion = "";
                this.negocioParAanular = null;
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    anular(negocio) {
        console.log(negocio);
        this.negocioParAanular = negocio;
        document.getElementById("openModalanularModal").click();
    }
    editar(negocio) {
        if (negocio.TipoNegocioId == 1) {
            this.navService.navegarSeccion("/crear-contrato/afijar/" + negocio.Id);
        }
        if (negocio.TipoNegocioId == 2) {
            this.navService.navegarSeccion("/crear-contrato/aprecio/" + negocio.Id);
        }
        if (negocio.TipoNegocioId == 3) {
            this.navService.navegarSeccion("/crear-contrato/fijacion/" + negocio.Id);
        }
    }

    onCheckboxChange(e) {

        if (e.target.checked) {
            this.actualizarAutomaticamente = true;
        } else {
            this.actualizarAutomaticamente = false;
        }
    }

    refrescar() {
        console.log("refrescar");
        if (this.actualizarAutomaticamente == true) {
            this.obteneContratos();
        }
    }

}