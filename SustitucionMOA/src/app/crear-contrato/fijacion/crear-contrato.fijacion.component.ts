import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoFijacionService } from './../crear-contrato.service';
import { ContratoFijacion } from "../../common/models/contratoFijacion";
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Seccion } from '../../common/models/seccion';
import { FijacionesAutomaticas } from '../../common/models/fijacionesAutomaticas';
import { DatePipe, registerLocaleData } from '@angular/common';
declare var $: any;
import localeEsAr from '@angular/common/locales/es-AR';

@Component({
    selector: 'app-crear-contrato-fijacion',
    templateUrl: `crear-contrato.fijacion.component.html`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoFijacionService }]
})
export class CrearContratoFijacionComponent extends CrearContratoBaseComponent {

    contrato: ContratoFijacion = new ContratoFijacion();
    cantMaxima: number;
    cantMinima: number;
    condicionFijacion: string;
    cantMaximaNumber: number;

    ngOnInit() {
        registerLocaleData(localeEsAr, 'es-AR');
        super.ngOnInit();
        this.contrato.TipoNegocioId = 3;
        this.contrato.Id = this.id;
        console.log("inicia el componente")
        if (this.id > 0) {
            this.traerContratoCompleto(this.contrato);
        } else {
            this.negocioHabilitado(this.contrato);
        }
       
    }

    ngAfterViewInit(): void {


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


    }

    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "Fijacion");
    }

    selectEventProveedor(item) {
        //this.contrato.MaterialId = null;
        this.pendientesFijar = [];
        this.pendienteFijar = null;
        this.contrato.ProveedorId = item.Id;
        this.cuitProveedorSeleccionado = item.CUIT;
        console.log("prov: ", item.Id);
        if (item != null && item.Id != null && item.Id > 0) {
            this.obtenerDatosCompraNet(this.contrato, item.Id);
        }
    }

    onChangeSearchProveedor(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.buscarProveedoresConCorredor(term).subscribe(
                (result:any) => {
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

    isVisiblePagoDiferido(): boolean {
        return this.contrato.MonedaId == "ARP  ";
    }
    isVisibleDolarizado(): boolean {
        return this.contrato.MonedaId == "USDM ";
    }
    isVisibleBolsa(): boolean {
        return this.contrato.BoletoId != 3;
    }

    grabarContratoFijacion() {
        console.log(this.contrato);
        if (!this.validarContrato()) {
            return false;
        }
        var hoy = new Date();
        var hoysinhora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
        if (this.contrato.Id == 0) {
            this.contrato.FechaOperacion = hoysinhora;
            this.contrato.Fecha = hoy;
        }

        this.contrato.EstadoId = 9;

        this.contrato.TipoNegocioId = 3;


        this.contrato.StandardDeCalidadId = null;
        this.contrato.PrecioNeto = this.contrato.Precio;

        this.contrato.ObservacionTercero = this.ObservacionTercero;
        if (this.contrato.DolarizadoTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Dolarizado: " + this.ObservacionDolarizadoTercero;
        }
        if (this.contrato.PagoDiferidoTercero == true) {
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Pago Diferido: " + this.contrato.DiasPesificado + "días.";
        }

        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        try {
            this.unsubscribe();
            this.subscription = this.service.grabarContratoFijacion(this.contrato).subscribe(
                (result:any) => {
                    this.blockUI.stop();
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
                                if (element.Message == "Proveedor No Operable por Riesgo Comercial Alto") {
                                    element.Message = "Proveedor no operable, contactese con la mesa comercial";
                                }
                                if (element.Message.indexOf("Precio fuera de Rango") != -1) {
                                    element.Message = "Precio fuera de Rango";
                                }
                                errores = errores + element.Message + " - ";
                            });

                            this.mensajeComponent.setErrorMsg(errores);
                        } else {
                            //this.mensajeComponent.setSuccessMsg("El contrato se genero correctamente.");
                            this.contrato.MaterialId = null;
                            this.contrato.Cantidad = null;
                            this.contrato.CampanaId = null;
                            this.contrato.Precio = null;
                            this.contrato.PrecioNeto = null;
                            this.contrato.MonedaId = null;
                            this.contrato.ContratoSAP = null;
                            document.getElementById("openModalConfirmModal").click();

                        }
                    }
                },
                error => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.blockUI.stop();
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }

    }

    validarContrato() {
        if ((this.contrato.CorredorId != null || this.contrato.CorredorId > 0) && (this.contrato.ComercialId == null)) {
            this.mensajeComponent.setErrorMsg("Debe ingresar la Zona.");
            return false;
        }
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

        if (this.contrato.Pizarra == false && (this.contrato.MonedaId == null || this.contrato.MonedaId == undefined || this.contrato.MonedaId == "")) {
            this.mensajeComponent.setErrorMsg("Debe completar la Moneda.");
            return false;
        }
        if (this.contrato.Cantidad == null || this.contrato.Cantidad == undefined || this.contrato.Cantidad <= 0) {
            this.mensajeComponent.setErrorMsg("Debe completar la Cantidad.");
            return false;
        }
        if ((this.ObservacionDolarizadoTercero == "" || this.ObservacionDolarizadoTercero == undefined) && this.contrato.DolarizadoTercero == true) {
            this.mensajeComponent.setErrorMsg("Debe completar en la observación la fecha de Dolarizado.");
            return false;
        }
        if (this.contrato.DiasPesificado < 7 && this.contrato.PagoDiferidoTercero == true) {
            this.mensajeComponent.setErrorMsg("La cantidad de días de Pago Diferido debe ser mayor o igual a 7.");
            return false;
        }
        if (this.costoFinanciero == "La cantidad de días ingresados supera el maximo permitido.") {
            this.mensajeComponent.setErrorMsg("La cantidad de días de diferimiento ingresados supera el maximo permitido.");
            return false;
        }
        if (this.contrato.Pizarra == true) {
            if (this.condicionFijacion == "07") {
                this.mensajeComponent.setErrorMsg("No se puede crear una fijacion pizarra, con condicion del contrato solo ''MERCADO MOA''.");
                return false;
            }
            if (this.condicionFijacion == "05" && new Date().getHours() >= 13) {
                this.mensajeComponent.setErrorMsg("Fuera de horario para fijacion por pizarra, condicion del contrato solo ''mercado moa''.");
                return false;
            }
        }
        console.log(this.contrato.Cantidad , this.cantMaxima);
        if (this.contrato.Cantidad > this.cantMaximaNumber) {
            this.mensajeComponent.setErrorMsg(`La cantidad supera la maxima de fijacion (${this.cantMaxima}).`);
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

    onChangeMaterial() {
        if (this.contrato.MaterialId != null) {
            this.pendientesFijar = [];
            this.pendienteFijar = null;
            this.contrato.CampanaId = null;
            //this.habilitaciones(this.contrato);
            this.contrato.Precio = 0;
            this.contrato.MonedaId = null;
            this.contrato.Pizarra = false;
        }
    }

    disablePrecio(): boolean {
        return this.contrato.Pizarra == true;
    }

    changeMoneda(event) {
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
                    } else {
                        this.contrato.Precio = item.Precio;
                    }

                }
            }
        } else {
            this.contrato.Precio = 0;
        }
    }

    selectEventContratoId(item: FijacionesAutomaticas) {
        if (item && item.ContratoId) {
            console.log("a fijar", item);
            if (item.Pase == true) {
                this.mensajeComponent.setErrorMsg("Negocio no disponible para Fijar, contactarse con el Comercial.");
                this.contrato.ContratoSAP = "";
                this.pendienteFijarContrato = "";
                return;
            }
            this.mensajeComponent.setErrorMsg("");
            this.pendienteFijar = item;
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
            this.condicionFijacion = item.CondicionFijacionCod;
            let KilosContrato = parseInt(item.KilosContrato.replace(/\./g, ''), 0);

            this.cantMaxima = 30 * KilosContrato / 100;
            if (KilosContrato < 30000) {
                this.cantMaxima = KilosContrato;
            } else if (this.cantMaxima < 30000) {
                this.cantMaxima = 30000;
            }

            this.cantMinima = 30000;
            if (KilosContrato < 30000) {
                this.cantMinima = KilosContrato;
            }
            this.cantMaximaNumber = this.cantMaxima;
            this.cantMaxima = this.numberSeparator(this.cantMaxima, '.');
            this.cantMinima = this.numberSeparator(this.cantMinima, '.');

        }

    }

    numberSeparator(x, y) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, y);
    }

    onChangeSearchContratoId(term: string) {
        //if (term.length > 2) {
        //    this.unsubscribe();
        //    this.subscription = this.service.searchLocalidad(term).subscribe(
        //        (result:any) => {
        //            this.localidades = result;
        //        },
        //        error => {
        //            this.mensajeComponent.setErrorMsg(error.message);
        //        }
        //    );
        //}
    }

    isVisibleContratoDetalle(): boolean {
        var result = !(this.pendienteFijar == null || this.pendienteFijar == "" || this.pendienteFijar == undefined);
        return result;
    }      

    changeDiasDiferido(event) {
        setTimeout(() => {
            this.costoFinanciero = null;

            if (this.contrato.Precio > 0 && this.contrato.PagoDiferidoTercero == true) {
                if ((this.contrato.DiasPesificado == null || this.contrato.DiasPesificado == undefined || this.contrato.DiasPesificado < 7)) {
                    this.costoFinanciero = "La cantidad de días de Pago Diferido debe ser mayor o igual a 7.";
                    return;
                }
                let tasa = 0;
                this.pagosDiferidos.forEach(element => {
                    if (this.contrato.DiasPesificado <= element.CantidadDia && tasa == 0) {
                        tasa = element.Tasa;
                    }
                });
                if (tasa == 0) {
                    this.costoFinanciero = "La cantidad de días ingresados supera el maximo permitido.";
                } else {
                    let precio = Number(this.contrato.Precio.toString().replace(',', '.'));
                    tasa = Number(tasa);
                    let costo = Math.round(precio * (tasa / 100) * (this.contrato.DiasPesificado - 3) / 365 * 2) / 2;
                    let d10 = costo / 10.00;
                    costo = Math.round(d10 * 2) / 2;
                    costo = costo * 10;

                    let precioNeto = precio + costo;
                    
                    //let fecha = new Date();
                    //let diasp = Number(this.contrato.DiasPesificado);
                    //fecha.setDate(fecha.getDate() + diasp);
                    //var datePipe = new DatePipe('es-AR');
                    this.costoFinanciero = "Precio Neto: " + (precioNeto) /*+ "<br> Fecha: " + datePipe.transform(fecha, 'dd/MM/yyyy')*/;
                }
            }

        }, 0);
    }
}