import { Component } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAPrecioService } from './../crear-contrato.service';
import { ContratoAPrecio } from "../../common/models/contratoAPrecio";
import localeEsAr from '@angular/common/locales/es-AR';
import { DatePipe, registerLocaleData } from '@angular/common';

declare var $: any;

@Component({
    selector: 'app-crear-contrato-aprecio',
    templateUrl: `crear-contrato.aprecio.component.html`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoAPrecioService }]
})
export class CrearContratoAPrecioComponent extends CrearContratoBaseComponent {


    contrato: ContratoAPrecio = new ContratoAPrecio();

    ngOnInit() {
        registerLocaleData(localeEsAr, 'es-AR');

        super.ngOnInit();
        this.contrato.TipoNegocioId = 2;
        this.contrato.Id = this.id;
        console.log("id", this.id);
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
                //todayBtn: 1,
                autoclose: 1,
                todayHighlight: 1,
                startView: 2,
                forceParse: 0,
                showMeridian: 1,
                pickTime: false,
                minView: 2,
                maxView: 4,
                //startDate: new Date(),
                //endDate: new Date(),
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


    }

    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "A Precio");
    }

    selectEventProveedor(item) {
        this.contrato.ProveedorId = item.Id;
        console.log("prov: ", item.Id);
        if (item != null && item.Id != null && item.Id > 0) {
            this.obtenerDatosCompraNet(this.contrato, item.Id);
        }
    }

    onChangeSearchProveedor(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.buscarProveedoresConCorredor(term).subscribe(
                (result: any) => {
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

    selectEventLocalidad(item) {
        console.log("selectEventLocalidad");
        this.contrato.LocalidadId = item.LocalidadId;
        this.contrato.ProvinciaId = item.ProvinciaId;
        if (item.ProvinciaId != 1) {
            this.contrato.EstablecimientoPropio = null;
        }
        if (this.BolsaId != null && this.contrato.BoletoId == 1) {
            this.contrato.BolsaId = this.BolsaId;
        }
        this.SeleccionAutomaticaBolsa(this.contrato);
    }

    onChangeSearchLocalidad(term: string) {
        if (term.length > 2) {
            this.unsubscribe();
            this.subscription = this.service.searchLocalidad(term).subscribe(
                (result: any) => {
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
        this.SeleccionAutomaticaBolsa(this.contrato);
    }

    changePagoDiferido(event) {
        this.contrato.DolarizadoTercero = false;
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

    changeDolarizado(event) {
        this.contrato.PagoDiferidoTercero = false;
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
        return this.contrato.ProvinciaId == 1 && this.contrato.ClasificacionId == 1;
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
            this.contrato.ObservacionTercero = this.contrato.ObservacionTercero + "| Pago Diferido: " + this.contrato.DiasPesificado + " días.";
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
            this.subscription = this.service.grabarContratoAPrecio(this.contrato).subscribe(
                (result: any) => {
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
        if (this.contrato.DiasPesificado < 7 && this.contrato.PagoDiferidoTercero == true) {
            this.mensajeComponent.setErrorMsg("La cantidad de días de Pago Diferido debe ser mayor o igual a 7.");
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
        if (this.costoFinanciero == "La cantidad de días ingresados supera el maximo permitido.") {
            this.mensajeComponent.setErrorMsg("La cantidad de días de diferimiento ingresados supera el maximo permitido.");
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
        } else {
            this.contrato.EstablecimientoPropio = null;
        }
    }
    isNotProductor(event) {
        if (this.contrato.ClasificacionId == 1) {
            return false;
        } else {
            return true;
        }
    }

    onChangeMaterial() {
        if (this.contrato.MaterialId != null) {
            this.contrato.CampanaId = null;
            this.habilitaciones(this.contrato);
            this.contrato.Precio = 0;
            this.contrato.MonedaId = null;
            this.contrato.Pizarra = false;
            this.contrato.CalidadTercero = false;
            this.contrato.SustentableTercero = false;

        }
    }

    disablePrecio(): boolean {
        return this.contrato.Pizarra == true;
    }

    changeMoneda(event) {
        console.log("asdsad");
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
                    } else {
                        this.contrato.Precio = item.Precio;
                        this.contrato.FechaDesde = new Date(item.DesdeEntrega);
                        this.contrato.FechaHasta = this.ObtenerFechaHasta(this.contrato.FechaDesde);
                        this.fechaInicio = this.contrato.FechaDesde.toLocaleDateString('en-GB');
                        this.fechaFin = this.contrato.FechaHasta.toLocaleDateString('en-GB');
                    }
                }
            }
        } else {
            this.contrato.Precio = 0;
        }
    }
    configuraciones() {

    }
    isSoja(): boolean {
        return this.contrato.MaterialId == 3;
    }

    isSustentableHabilitado(): boolean {
        let dateParts = $("#noCursor2").val().split("/");
        let entrega = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
        let sustentables = this.sustentables.filter(a => a.TipoNegocioId == this.contrato.TipoNegocioId && a.DesdeEntrega <= entrega && a.HastaEntrega >= entrega);
        if (sustentables.length == 1 && this.contrato.SustentableTercero == true && this.isSoja()) {
            this.contrato.ImporteSustentable = sustentables[0].Precio;
            this.contrato.MonedaSustentableId = sustentables[0].MonedaId;
            this.contrato.Sustentable = true;
            this.ObservacionSustentableTercero = sustentables[0].Precio + " " + sustentables[0].MonedaId
        }
        return sustentables.length == 1 && this.isSoja();
    }

    changeSustentable(event) {

        if (this.contrato.SustentableTercero == true) {
            let dateParts = $("#noCursor2").val().split("/");
            let entrega = new Date(parseInt(dateParts[2]), parseInt(dateParts[1]) - 1, parseInt(dateParts[0]));
            let sustentables = this.sustentables.filter(a => a.TipoNegocioId == this.contrato.TipoNegocioId && a.DesdeEntrega <= entrega && a.HastaEntrega >= entrega);
            if (sustentables.length == 1) {
                this.contrato.ImporteSustentable = sustentables[0].Precio;
                this.contrato.MonedaSustentableId = sustentables[0].MonedaId;
                this.contrato.Sustentable = true;
                this.ObservacionSustentableTercero = sustentables[0].Precio + " " + sustentables[0].MonedaId
            } else {
                this.contrato.ImporteSustentable = null;
                this.contrato.MonedaSustentableId = null;
                this.contrato.Sustentable = false;
                this.contrato.SustentableTercero = false;
                console.log("no esta haibltiado sustentable.")
            }
        } else {
            this.contrato.ImporteSustentable = null;
            this.contrato.MonedaSustentableId = null;
            this.contrato.Sustentable = false;
        }
    }

}