import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoService } from './crear-contrato.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/Seccion';
import { ContratoAPrecio } from '../common/models/contratoAPrecio';
import { habilitacionPizarra } from '../common/models/habilitacionPizarra';
import { precioMoaCompraNet } from '../common/models/precioMoaCompraNet'
declare var $: any;

@Component({
    selector: 'app-contrato',
    template: ``,
    providers: [CrearContratoService]
})
export class CrearContratoBaseComponent extends ListBaseComponent {
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: CrearContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerComponent = new SpinnerComponent();
        this.mensajeComponent = new MensajeComponent();

    }
    esCorredorEnDataAgro: boolean = false;
    cuitProveedorSeleccionado: string = "";
    localidad: any;
    localidades = [];
    proveedor: any;
    proveedores = [];
    pendienteFijar: any;
    pendientesFijar = [];
    keyword = 'Nombre';
    keyword2 = "RazonSocial";
    keyword3 = "ContratoId";
    autocompleteNotFoundText = "No encontrado";

    fechaInicio = new Date().toLocaleDateString('en-GB');
    fechaFin = new Date().toLocaleDateString('en-GB');
    fechafInicio = new Date().toLocaleDateString('en-GB');
    fechafFin = new Date().toLocaleDateString('en-GB');

    datosContrato: any = new Array();
    materiales: any = new Array();
    monedas: any = new Array();
    destinos: any = new Array();
    campanias: any = new Array();
    campaniasTodas: any = new Array();
    zona: any = new Array();
    bolsasSelect: any = new Array();
    bolsasConfirma: any = new Array();
    bolsasFisico: any = new Array();
    bolsasCarta: any = new Array();
    condicionVendedor: any = new Array();
    condicionFijacion: any = new Array();
    datosCompraNet: any = null;
    datosPizarra: habilitacionPizarra = null;
    datosPrecioMoa: any = new Array<precioMoaCompraNet>();

    checkPermisos() { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([

            new Seccion('/contrato/crear/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/contrato/crear/afijar', 'crear-contrato', 'A Fijar'),
            new Seccion('/contrato/crear/fijacion', 'crear-contrato', 'Fijacion'),

        ]);
        //this.obteneDatosContrato();
    }

    obteneDatosContrato(contrato) {
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato(contrato.TipoNegocioId).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    this.datosContrato = obj;
                    obj.Datos.material.forEach(element => {
                        let el = {
                            Id: element.MaterialId,
                            Descripcion: element.Descripcion
                        }
                        this.materiales.push(el);
                    });
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
                        this.campaniasTodas.push(el);
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
                    this.validarDirecto(contrato);

                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    validarDirecto(contrato) {
        this.unsubscribe();
        this.subscription = this.service.validarDirecto().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    if (obj != null && obj > 0) {
                        contrato.CorredorId = obj;
                        this.esCorredorEnDataAgro = true;
                    } else {
                        this.obtenerDatosCompraNet(contrato, "");
                        this.esCorredorEnDataAgro = false;
                    }
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }

    habilitaciones(contrato) {
        this.unsubscribe();
        this.subscription = this.service.habilitaciones(contrato.MaterialId, contrato.TipoNegocioId).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = result;
                    if (obj != null) {
                        this.campanias = new Array();
                        JSON.parse(obj.HabilitarCampana).forEach(element => {
                            let el = {
                                Id: element.CampaniaId,
                                Descripcion: element.Campania
                            }
                            this.campanias.push(el);
                        });
                        if (obj.HabilitarPizarra != "") {
                            this.datosPizarra = JSON.parse(obj.HabilitarPizarra);
                        } else {
                            this.datosPizarra = null;
                        }
                        this.datosPrecioMoa = JSON.parse(obj.TraerPrecioMoa);
                        if (contrato.TipoNegocioId == 1) {
                            var leng = this.datosPrecioMoa.length;
                            var items = this.datosPrecioMoa;
                            for (var i = 0; i < leng; i++) {
                                var item = items[i];
                                console.log(item);
                                contrato.FechaDesde = new Date(parseInt(item.DesdeEntrega.substr(6)));
                                contrato.FechaHasta = this.ObtenerFechaHasta(contrato.FechaDesde);
                                this.fechaInicio = contrato.FechaDesde.toLocaleDateString('en-GB');
                                this.fechaFin = contrato.FechaHasta.toLocaleDateString('en-GB');
                                var entregaHasta = new Date(parseInt(item.HastaEntrega.substr(6)));

                                $('.form_datetime_Inicio').datetimepicker('setStartDate', contrato.FechaDesde.toLocaleDateString("en-GB"));
                                $('.form_datetime_Inicio').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));

                                $('.form_datetime_Fin').datetimepicker('setStartDate', contrato.FechaDesde.toLocaleDateString("en-GB"));
                                $('.form_datetime_Fin').datetimepicker('setEndDate', entregaHasta.toLocaleDateString("en-GB"));


                                contrato.DesdeFijacion = new Date(parseInt(item.DesdeFijacion.substr(6)));
                                contrato.HastaFijacion = this.ObtenerFechaHasta(contrato.DesdeFijacion);
                                this.fechafInicio = contrato.DesdeFijacion.toLocaleDateString('en-GB');
                                this.fechafFin = contrato.HastaFijacion.toLocaleDateString('en-GB');
                                var HastaFijacion = new Date(parseInt(item.HastaFijacion.substr(6)));

                                $('.form_datetime_fInicio').datetimepicker('setStartDate', contrato.DesdeFijacion.toLocaleDateString("en-GB"));
                                $('.form_datetime_fInicio').datetimepicker('setEndDate', HastaFijacion.toLocaleDateString("en-GB"));

                                $('.form_datetime_fFin').datetimepicker('setStartDate', contrato.DesdeFijacion.toLocaleDateString("en-GB"));
                                $('.form_datetime_fFin').datetimepicker('setEndDate', HastaFijacion.toLocaleDateString("en-GB"));

                                break;
                            }

                        }

                        if (contrato.TipoNegocioId == 3) {
                            this.obtenerFijacionesAutomaticas(contrato.MaterialId, "");
                        }
                    }
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }


    obtenerDatosCompraNet(contrato, idProveedorDataAgro) {
        this.unsubscribe();
        this.subscription = this.service.obtenerDatosCompraNet(idProveedorDataAgro).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    this.datosCompraNet = obj;
                    contrato.ClasificacionId = obj.ClasificacionCompraNetId;
                    contrato.BolsaId = obj.BolsaCompraNetId;
                    if (obj.BolsaCompraNetId == 1) {
                        this.bolsasSelect = this.bolsasConfirma;
                    }
                    if (obj.BolsaCompraNetId == 2) {
                        this.bolsasSelect = this.bolsasFisico;
                    }
                    if (obj.BolsaCompraNetId == 3) {
                        this.bolsasSelect = [];
                    }
                    if (obj.BolsaCompraNetId == 4) {
                        this.bolsasSelect = this.bolsasCarta;
                    }
                    contrato.BoletoId = obj.BoletoCompraNetId;

                    if (obj.ClasificacionCompraNetId != 1) {
                        contrato.Consignatario = obj.Consignatario;
                        contrato.PlanCanje = obj.PlanCanje;
                    }

                    contrato.LocalidadId = obj.LocalidadId;
                    contrato.ProvinciaId = obj.ProvinciaId;
                    if (obj.ProvinciaId != null) {
                        this.localidad = obj.Localidad + " (" + obj.Provincia + ")";
                    } else {
                        this.localidad = null;
                    }
                    if (obj.ProvinciaId != 1) {
                        contrato.EstablecimientoPropio = null;
                    }
                    //contrato.ClasificacionId = this.datosCompraNet.ComisionPorcentaje;
                    contrato.DestinoId = this.destinos[0].Id;
                    //contrato.MonedaId = this.monedas[0].Id;
                    //contrato.MaterialId = this.materiales[0].Id;
                    //contrato.CampanaId = this.campanias[0].Id;
                }

            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }

    isVisibleProveedor(): boolean {
        return this.esCorredorEnDataAgro == true;
    }

    isVisiblePizarra(): boolean {
        return this.datosPizarra != null;
    }

    ObtenerFechaHasta(fechaBase) {
        var hoy = fechaBase != undefined ? fechaBase : new Date();
        var anio = hoy.getFullYear();
        var mesPost = hoy.getMonth() + 1;
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
        //if (mesPost < 10) {
        //    mesPost = "0" + mesPost.toString();
        //}
        //if (dia < 10) {
        //    dia = "0" + dia.toString();
        //}
        return new Date(anio, mesPost, dia);// dia + '-' + mesPost + '-' + anio;
    }

    obtenerFijacionesAutomaticas(materialId, filtro) {
        this.unsubscribe();
        this.subscription = this.service.obtenerFijacionesAutomaticas(this.esCorredorEnDataAgro, this.cuitProveedorSeleccionado, materialId, filtro).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.pendientesFijar = [];
                    if (result != "") {
                        let obj = JSON.parse(result);
                        //this.pendientesFijar = obj;
                        this.pendientesFijar = obj.map(con => {
                            return {
                                ARecibirSinPrecio: con.ARecibirSinPrecio,
                                Calidad: con.Calidad,
                                Calidades: con.Calidades,
                                Campana: con.Campana,
                                CampanaId: con.CampanaId,
                                Centro: con.Centro,
                                CentroDescripcion: con.CentroDescripcion,
                                ChequeElectronico: con.ChequeElectronico,
                                Color: con.Color,
                                CondicionFijacionCod: con.CondicionFijacionCod,
                                CondicionFijacionDescripcion: con.CondicionFijacionDescripcion,
                                CondicionPagoCod: con.CondicionPagoCod,
                                CondicionPagoDescripcion: con.CondicionPagoDescripcion,
                                ContratoId: con.ContratoId,
                                DesdeEntrega: con.DesdeEntrega,
                                FechaDesde: con.FechaDesde,
                                FechaHasta: con.FechaHasta,
                                Filtro: '<p class="buscar-nomb"><strong>' + con.ContratoId+'</strong> - ' +
                                    'KG CTO: ' + con.KilosContrato+' ' +
                                    ' - KGS SIN PRECIO : ' + con.ARecibirSinPrecio+' - KGS SIN FIJAR : ' + con.RecibidoSinFijar+'' +
                                    ' - KILOS A FIJAR: ' + con.KilosPendiente+' - KG APLIC: ' + con.KilosAplicados+'' +
                                    ' - Hasta: ' + con.FechaHasta+' - <strong>' + con.CentroDescripcion+'</strong></p>',
                                HastaEntrega: con.HastaEntrega,
                                ImporteAPrecio: con.ImporteAPrecio,
                                ImporteSobrePrecio: con.ImporteSobrePrecio,
                                KilosAplicados: con.KilosAplicados,
                                KilosContrato: con.KilosContrato,
                                KilosPendiente: con.KilosPendiente,
                                MonedaAPrecio: con.MonedaAPrecio,
                                MonedaSobrePrecio: con.MonedaSobrePrecio,
                                PagoDiferido: con.PagoDiferido,
                                PorcentajeAPrecio: con.PorcentajeAPrecio,
                                PorcentajeSobrePrecio: con.PorcentajeSobrePrecio,
                                Posicion: con.Posicion,
                                RecibidoSinFijar: con.RecibidoSinFijar,
                            }
                        })
                    }

                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    };

}