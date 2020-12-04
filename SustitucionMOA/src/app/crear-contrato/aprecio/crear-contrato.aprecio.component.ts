import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAPrecioService } from './../crear-contrato.service';
import { ContratoAPrecio } from "../../common/models/contratoAPrecio";
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Seccion } from '../../common/models/seccion';
declare var $: any;

@Component({
    selector: 'app-crear-contrato-aprecio',
    templateUrl: `crear-contrato.aprecio.component.html`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoAPrecioService }]
})
export class CrearContratoAPrecioComponent extends CrearContratoBaseComponent {

    contrato: ContratoAPrecio = new ContratoAPrecio();
    fechaInicio = new Date().toLocaleDateString('en-GB');
    fechaFin = new Date().toLocaleDateString('en-GB');

    ngOnInit() {
        super.ngOnInit();
        console.log("inicia el componente")
        this.obteneDatosContrato(this.contrato);

    }

    ngAfterViewInit(): void {


        var hoy = new Date();
        var hoysinhora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
        var hoymesqueviene = this.service.ObtenerFechaHasta(hoysinhora);
        
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
                if (ev.date.valueOf() ) {
                    var fechaBase =new Date(ev.date.valueOf());
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


    }

    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "A Precio");
    }

    selectEventLocalidad(item) {
        this.contrato.LocalidadId = item.LocalidadId;
        this.contrato.ProvinciaId = item.ProvinciaId;
        if (item.ProvinciaId != 1) {
            this.contrato.EstablecimientoPropio = null;
        }
    }

    onChangeSearchLocalidad(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                result => {
                    this.localidades = result;
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        }
    }

    onBoletoSelected() {
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
    }

    changePagoDiferido(event) {
        this.contrato.DolarizadoTercero = false;
    }

    changeDolarizado(event) {
        this.contrato.PagoDiferidoTercero = false;
    }

    changeMoneda(event) {
        this.contrato.PagoDiferidoTercero = false;
        this.contrato.DolarizadoTercero = false;
    }
    isVisiblePagoDiferido(): boolean {
        return this.contrato.MonedaId == "ARP  ";
    }
    isVisibleDolarizado(): boolean {
        return this.contrato.MonedaId == "USDM ";
    }
    isVisibleBolsa(): boolean {
        return this.contrato.BoletoId != 3;
    }

    isVisibleEstablecimiento(): boolean {
        return this.contrato.ProvinciaId == 1;
    }

    grabarContratoAPrecio() {
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
        this.contrato.FechaOperacion = hoysinhora;
        this.contrato.Fecha = hoy;

        this.contrato.Id = 0;
        this.contrato.EstadoId = 9;

        this.contrato.ProvinciaId;
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

        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        console.log(this.contrato);
        try {
            this.unsubscribe();
            this.subscription = this.service.grabarContratoAPrecio(this.contrato).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        let obj = JSON.parse(result);

                        if (obj.HayError) {
                            let errores = "";
                            obj.Errores.forEach(element => {
                                errores = errores + element.Message + " - ";
                            });

                            this.mensajeComponent.setErrorMsg(errores);
                        } else {
                            this.mensajeComponent.setSuccessMsg("El contrato se genero correctamente.");
                        }
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }

    }

    validarContrato() {
        if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.DolarizadoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar  la observacion si Dolarizado.");
            return false;
        }
        if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.PagoDiferidoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar  la observacion si Pago Diferido.");
            return false;
        }
        if ((this.contrato.ObservacionTercero == "" || this.contrato.ObservacionTercero == undefined) && this.contrato.CalidadTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observacion la calidad.");
            return false;
        }
        return true;
    }

    changePlanCanje(event) {
        this.contrato.Consignatario = false;
    }

    changeConsignatario(event) {
        this.contrato.PlanCanje = false;
    }

    changeClasificacion(event) {
        if (this.contrato.ClasificacionId == 1) {
            this.contrato.PlanCanje = false;
            this.contrato.Consignatario = false;
        }
    }
    isNotProductor(event) {
        if (this.contrato.ClasificacionId == 1) {
            return false;
        } else {
            return true;
        }
    }

}