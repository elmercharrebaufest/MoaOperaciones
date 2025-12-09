import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from "@angular/router";
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { precioMoaCompraNet } from '../common/models/precioMoaCompraNet';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { Seccion } from './../common/models/seccion';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { CrearContratoService } from './crear-contrato.service';

declare var $: any;

@Component({
    selector: 'app-contrato',
    template: ``,
    providers: [CrearContratoService]
})
export class CrearContratoBaseComponent extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    @ViewChild("spinnerCampana")
    protected spinnerCampana: SpinnerSmallComponent;
    BolsaId: any;
    bolsaRecomendada: any;

    constructor(
        protected service: CrearContratoService,
        protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerComponent = new SpinnerComponent();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerCampana = new SpinnerSmallComponent();
    }

    crearModificar: string = "Crear";
    id: number = 0;
    contratoEditar: any;
    esCorredorEnDataAgro: boolean = false;
    retirados: boolean = null;
    cuitProveedorSeleccionado: string = "";
    localidad: any;
    localidades = [];
    proveedor: any;
    proveedores = [];
    pendienteFijar: any;
    pendienteFijarContrato: string = "";
    mensajeCambioBolsa: string = "";
    pendientesFijar = [];
    keyword = 'Nombre';
    keyword2 = "RazonSocial";
    keyword3 = "ContratoId";
    autocompleteNotFoundText = "No encontrado";
    mensajeModal = "";

    fechaInicio = new Date().toLocaleDateString('en-GB');
    fechaFin = new Date().toLocaleDateString('en-GB');
    fechafInicio = new Date().toLocaleDateString('en-GB');
    fechafFin = new Date().toLocaleDateString('en-GB');

    bolsasAutomaticas: any = new Array();
    datosContrato: any = new Array();
    materiales: any = new Array();
    monedas: any = new Array();
    monedasTodas: any = new Array();
    destinos: any = new Array();
    campanias: any = new Array();
    sustentables: any = new Array();
    campaniasTodas: any = new Array();
    zona: any = new Array();
    bolsasSelect: any = new Array();
    bolsasConfirma: any = new Array();
    bolsasFisico: any = new Array();
    bolsasCarta: any = new Array();
    condicionVendedor: any = new Array();
    condicionFijacion: any = new Array();
    datosCompraNet: any = null;
    datosPizarra: boolean = null;
    datosPrecioMoa: any = new Array<precioMoaCompraNet>();
    listaPrecios: any = null;
    precioPorNegocioSeleccionado: any = null;
    precioMaiz: boolean = null;
    precioTrigo: boolean = null;
    precioSoja: boolean = null;
    precioGirasol: boolean = null;
    checkPermisos() { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); }
    precioSeleccionado: boolean = false;
    materialSeleccionado: string = "";
    ObservacionPagoDiferidoTercero: string = "";
    ObservacionDolarizadoTercero: string = "";
    ObservacionCalidadTercero: string = "";
    ObservacionSustentableTercero: string = "";
    ObservacionTercero: string = "";
    pagosDiferidos: any = new Array();
    maximoDiasDiferimiento: number = 0;
    costoFinanciero: string;
    placeholderDolarizado: string = this.placeHoldeDolarizado();
    ngOnInit() {

        this.setTabs();
        this.checkPermisos();
        //this.navService.setSeccionList([

        //    new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
        //    new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
        //    new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),
        //    new Seccion('/crear-contrato/alta-masiva', 'crear-contrato', 'Fijacion'),

        //]);
        //this.obteneDatosContrato();
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) {
                this.id = params["id"];
                this.crearModificar = "Modificar";
            };
        });
    }

    placeHoldeDolarizado() {
        let today = new Date(Date.now() + 60 * 24 * 60 * 60 * 1000);
        let dd = String(today.getDate());
        let mm = String(today.getMonth() + 1); //January is 0!
        let yyyy = today.getFullYear();
        let text = dd + '/' + mm + '/' + yyyy;
        return "ej: " + text;
    }

    negocioHabilitado(contrato) {
        this.blockUI.start('');
        this.unsubscribe();
        this.subscription = this.service.traerPrecioMoaMateriales(contrato.TipoNegocioId).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let precio = JSON.parse(result);
                    this.retirados = true;
                    this.listaPrecios = precio;

                    for (var i = 0; i < precio.length; i++) {
                        let preciopornegocio = precio[i].filter(x => x.TipoNegocioId == contrato.TipoNegocioId)
                        var matRetirado = true;
                        if (preciopornegocio.filter(x => x.Retirado == false).length > 0 ||
                            preciopornegocio[0].Pizarra == true) {
                            this.retirados = false;
                            matRetirado = false;
                        }

                        if (preciopornegocio[0].MaterialId == 1) {
                            this.precioMaiz = matRetirado == false;
                            if (matRetirado) {
                                $("#precioMaiz").css("color", "red");
                            } else {
                                $("#precioMaiz").css("color", "#017940");
                            }
                        }

                        if (preciopornegocio[0].MaterialId == 2) {
                            this.precioTrigo = matRetirado == false;
                            if (matRetirado) {
                                $("#precioTrigo").css("color", "red");
                            } else {
                                $("#precioTrigo").css("color", "#017940");
                            }
                        }

                        if (preciopornegocio[0].MaterialId == 3) {
                            this.precioSoja = matRetirado == false;
                            if (matRetirado) {
                                $("#precioSoja").css("color", "red");
                            } else {
                                $("#precioSoja").css("color", "#017940");
                            }
                        }

                        if (preciopornegocio[0].MaterialId == 4 || preciopornegocio[0].MaterialId == 5) {
                            this.precioGirasol = matRetirado == false;
                            if (matRetirado) {
                                $("#precioGirasol").css("color", "red");
                            } else {
                                $("#precioGirasol").css("color", "#017940");
                            }
                        }
                    }

                    if (this.retirados == false || contrato.Id > 0) {
                        this.obteneDatosContrato(contrato);
                        this.blockUI.stop();
                    } else {
                        this.blockUI.stop();
                        this.mensajeComponent.setErrorMsg("Negocio no disponible");
                    }
                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;
    }

    obteneDatosContrato(contrato) {
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato(contrato.TipoNegocioId).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj2 = JSON.parse(result.bolsaAutomatica);
                    this.bolsasAutomaticas = obj2.Data;
                    let obj = JSON.parse(result.DatosContrato);
                    this.datosContrato = obj;

                    //obj.Datos.material.forEach(element => {
                    //    let el = {
                    //        Id: element.MaterialId,
                    //        Descripcion: element.Descripcion
                    //    }
                    //    this.materiales.push(el);
                    //});
                    obj.Datos.Bolsa.forEach(element => {
                        if (element.Id != 8 && element.Id != 9) {
                            let el = {
                                Id: element.Id,
                                Descripcion: element.Descripcion
                            }
                            this.bolsasConfirma.push(el);
                            if (el.Descripcion == "Bs As" || el.Descripcion == "Rosario")
                                this.bolsasFisico.push(el);
                            if (element.Descripcion == "Bs As")
                                this.bolsasCarta.push(el);
                        }

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
                        if (element.Id != 9 && element.Id != 10 && element.Id != 13) {
                            let el = {
                                Id: element.Id,
                                Descripcion: element.Descripcion
                            }
                            this.destinos.push(el);
                        }

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
                        //this.monedas.push(el);
                        this.monedasTodas.push(el);
                    });
                    obj.Datos.Condicion.forEach(element => {
                        if (element.Id == 2 || element.Id == 5 || element.Id == 7) {
                            let el = {
                                Id: element.Id,
                                Descripcion: element.Descripcion
                            }
                            this.condicionFijacion.push(el);
                        }

                    });
                    this.validarDirecto(contrato);
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.blockUI.stop();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    validarDirecto(contrato) {
        this.unsubscribe();
        this.subscription = this.service.validarDirecto().subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    if (obj != null && obj > 0) {
                        contrato.CorredorId = obj;
                        this.esCorredorEnDataAgro = true;
                        this.blockUI.stop();
                        if (this.id > 0) {
                            this.habilitaciones(contrato);
                        }
                        this.navService.setSeccionList([

                            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
                            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
                            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),
                            new Seccion('/crear-contrato/alta-masiva', 'crear-contrato', 'Alta Masiva'),

                        ]);
                    } else {
                        this.obtenerDatosCompraNet(contrato, "");
                        this.esCorredorEnDataAgro = false;
                        this.navService.setSeccionList([

                            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
                            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
                            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),

                        ]);

                        this.blockUI.stop();
                        if (this.id == 0) {
                            this.obtenerDatosCompraNet(contrato, "");
                            this.blockUI.stop();
                        } else {
                            this.habilitaciones(contrato);
                        }
                    }
                    this.blockUI.stop();
                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }

    comparePagosDiferidos(a, b) {
        const bandA = a.CantidadDia;
        const bandB = b.CantidadDia;

        let comparison = 0;
        if (bandA > bandB) {
            comparison = 1;
        } else if (bandA < bandB) {
            comparison = -1;
        }
        return comparison;
    }

    habilitaciones(contrato) {
        this.blockUI.start('');
        this.unsubscribe();
        this.subscription = this.service.habilitaciones(contrato.MaterialId, contrato.TipoNegocioId).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.blockUI.stop();
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
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
                        this.sustentables = new Array();
                        JSON.parse(obj.TraerHabilitarSustentable).forEach(element => {
                            if (element.TipoNegocioId == contrato.TipoNegocioId) {
                                let el = {
                                    Id: element.Id,
                                    Precio: element.Precio,
                                    MonedaId: element.MonedaId,
                                    DesdeVigencia: new Date(parseInt(element.DesdeVigencia.substr(6))),
                                    HastaVigencia: new Date(parseInt(element.HastaVigencia.substr(6))),
                                    TipoNegocio: element.TipoNegocio,
                                    TipoNegocioId: element.TipoNegocioId,
                                    HastaEntrega: new Date(parseInt(element.HastaEntrega.substr(6))),
                                    DesdeEntrega: new Date(parseInt(element.DesdeEntrega.substr(6))),
                                }
                                this.sustentables.push(el);
                            }
                        });
                        this.pagosDiferidos = new Array();
                        if (obj.TraerPagosDiferido) {
                            JSON.parse(obj.TraerPagosDiferido).forEach(element => {
                                let el = {
                                    Id: element.Id,
                                    Descripcion: `${element.CantidadDia} dias $${element.Importe}`,
                                    CantidadDia: element.CantidadDia,
                                    Tasa: element.Tasa
                                }
                                this.pagosDiferidos.push(el);
                                if (this.maximoDiasDiferimiento < element.CantidadDia) {
                                    this.maximoDiasDiferimiento = element.CantidadDia;
                                }
                            });
                            this.pagosDiferidos.sort(this.comparePagosDiferidos);
                            console.log(this.pagosDiferidos)
                        }

                        if (contrato.TipoNegocioId == 3) {
                            this.obtenerFijacionesAutomaticas(contrato.MaterialId, "");
                        } else {
                            this.blockUI.stop();
                        }

                    }
                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }


    obtenerDatosCompraNet(contrato, idProveedorDataAgro) {
        this.blockUI.start('');
        this.unsubscribe();
        this.subscription = this.service.obtenerDatosCompraNet(idProveedorDataAgro).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    this.datosCompraNet = obj;
                    contrato.ClasificacionId = obj.ClasificacionCompraNetId;
                    contrato.BolsaId = obj.BolsaCompraNetId;
                    this.BolsaId = obj.BolsaCompraNetId;
                    contrato.BoletoId = obj.BoletoCompraNetId;
                    if (obj.BoletoCompraNetId == 1) {
                        this.bolsasSelect = this.bolsasConfirma;
                    }
                    if (obj.BoletoCompraNetId == 2) {
                        this.bolsasSelect = this.bolsasFisico;
                    }
                    if (obj.BoletoCompraNetId == 3) {
                        this.bolsasSelect = [];
                    }
                    if (obj.BoletoCompraNetId == 4) {
                        this.bolsasSelect = this.bolsasCarta;
                    }

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
                    contrato.MaterialId = this.materiales[0].Id;                   

                    if (contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 2) {
                        this.validarProveedor(idProveedorDataAgro, contrato);
                    }
                    if (contrato.TipoNegocioId == 3) {
                        this.obtenerFijacionesAutomaticas(contrato.MaterialId, "");
                    }
                    this.SeleccionAutomaticaBolsa(contrato);

                    this.blockUI.stop();
                }

            },
            error => {
                this.blockUI.stop();
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
        return this.datosPizarra == true;
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
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
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
                                Filtro: '<p class="buscar-nomb"><strong>' + con.ContratoId + '</strong> - ' +
                                    (con.Pase? '<strong>A Fijar PASE</strong> - ': '') +
                                    'KG CTO: ' + con.KilosContrato + ' ' +
                                    ' - KGS SIN PRECIO : ' + con.ARecibirSinPrecio + ' - KGS SIN FIJAR : ' + con.RecibidoSinFijar + '' +
                                    ' - KILOS A FIJAR: ' + con.KilosPendiente + ' - KG APLIC: ' + con.KilosAplicados + '' +
                                    ' - Hasta: ' + con.FechaHasta + ' - <strong>' + con.CentroDescripcion + '</strong></p>',
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
                                Pase: con.Pase,
                            }
                        })
                        if (this.contratoEditar != null && this.contratoEditar.DatosFijacion.ContratoId) {
                            let afijar = this.pendientesFijar.filter(a => a.ContratoId == this.contratoEditar.DatosFijacion.ContratoId);
                            if (afijar.length == 1) {
                                console.log(afijar[0]);
                                this.pendienteFijar = afijar[0];
                            }
                            this.contratoEditar = null;
                        } else {
                            //this.pendienteFijarContrato = "";
                        }
                    }
                    this.blockUI.stop();

                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    };

    irACargas() {
        this.navService.navegarSeccion("/reporte/contrato");
    }

    validarProveedor(proveedorId, contrato) {
        this.subscription = this.service.validarProveedor(proveedorId).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = result;
                    if (obj != null) {
                        obj = JSON.parse(obj);
                    }
                    if ((contrato.ClasificacionId == 2) &&
                        ((contrato.PlanCanje == true && obj.Ruca.Acopiador.PlanCanje == "NO") ||
                            (contrato.Consignatario == true && obj.Ruca.Acopiador.Consignatario == "NO") ||
                            (contrato.PlanCanje == false && contrato.Consignatario == false && obj.Ruca.Acopiador.Directo == "NO"))) {
                        this.mensajeModal = "No est\u00E1 habilitado en Ruca";
                        document.getElementById("openModalMensajeModal").click();
                        this.blockUI.stop();
                        return;
                    }

                    if ((contrato.ClasificacionId == 3) &&
                        ((contrato.PlanCanje == true && obj.Ruca.Otros.PlanCanje == "NO") ||
                            (contrato.Consignatario == true && obj.Ruca.Otros.Consignatario == "NO") ||
                            (contrato.PlanCanje == false && contrato.Consignatario == false && obj.Ruca.Otros.Directo == "NO"))) {
                        this.mensajeModal = "No est\u00E1 habilitado en Ruca";
                        document.getElementById("openModalMensajeModal").click();
                        this.blockUI.stop();
                        return;
                    }

                    if (obj.FechaActualizacion == "NO") {
                        this.mensajeModal = "Falta fecha de actualizaci\u00F3n de legajo";
                        document.getElementById("openModalMensajeModal").click();
                        this.blockUI.stop();
                        return;
                    }
                    this.blockUI.stop();
                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }

    preciosDisponibles(MaterialId, TipoNegocioId) {
        if ((MaterialId == 1 && this.precioMaiz != true)
            || (MaterialId == 2 && this.precioTrigo != true)
            || (MaterialId == 3 && this.precioSoja != true)
            || (MaterialId == 4 && this.precioGirasol != true)
        ) {
            this.mensajeModal = "Material no disponible.";
            document.getElementById("openModalMensajeModal").click();
            return;
        }

        let precio = this.listaPrecios;
        this.precioPorNegocioSeleccionado = precio[MaterialId - 1].filter(x => x.TipoNegocioId == TipoNegocioId && (!(x.Precio == 0 && x.MonedaId != "Pizarra") || x.TipoNegocioId == 1));
        for (var i = 0; i < this.precioPorNegocioSeleccionado.length; i++) {
            if (this.precioPorNegocioSeleccionado[i].DesdeEntrega != null && typeof this.precioPorNegocioSeleccionado[i].DesdeEntrega === 'string') {
                this.precioPorNegocioSeleccionado[i].DesdeEntrega = new Date(parseInt(this.precioPorNegocioSeleccionado[i].DesdeEntrega.substr(6)))
            }
            if (this.precioPorNegocioSeleccionado[i].HastaEntrega != null && typeof this.precioPorNegocioSeleccionado[i].HastaEntrega === 'string') {
                this.precioPorNegocioSeleccionado[i].HastaEntrega = new Date(parseInt(this.precioPorNegocioSeleccionado[i].HastaEntrega.substr(6)))
            }

            if (this.precioPorNegocioSeleccionado[i].DesdeFijacion != null && typeof this.precioPorNegocioSeleccionado[i].DesdeFijacion === 'string') {
                this.precioPorNegocioSeleccionado[i].DesdeFijacion = new Date(parseInt(this.precioPorNegocioSeleccionado[i].DesdeFijacion.substr(6)))
            }
            if (this.precioPorNegocioSeleccionado[i].HastaFijacion != null && typeof this.precioPorNegocioSeleccionado[i].HastaFijacion === 'string') {
                this.precioPorNegocioSeleccionado[i].HastaFijacion = new Date(parseInt(this.precioPorNegocioSeleccionado[i].HastaFijacion.substr(6)))
            }
        }
        document.getElementById("openModalPreciosModal").click();

    }

    seleccionarPrecio(precio, contrato) {
        this.monedas = new Array();
        if (precio.TipoNegocioId == 2 || precio.TipoNegocioId == 3) {
            if (precio.MonedaId == "ARP") {
                this.monedas.push({ Id: "ARP  ", Descripcion: "ARP" });
                contrato.MonedaId = "ARP  ";
                if (precio.TipoNegocioId == 2) { contrato.DolarizadoTercero = false; }
            }
            if (precio.MonedaId == "USD") {
                this.monedas.push({ Id: "USDM ", Descripcion: "USD" });
                contrato.MonedaId = "USDM ";
                if (precio.TipoNegocioId == 2) { contrato.PagoDiferidoTercero = false; }
            }

            if (precio.MonedaId == "Pizarra") {
                contrato.Pizarra = true;
                this.datosPizarra = true;
            } else {
                contrato.Pizarra = false;
                this.datosPizarra = false;
            }


            contrato.Precio = precio.Precio
        }
        if (precio.TipoNegocioId == 2 || precio.TipoNegocioId == 1) {
            //entregadesde hasta
            contrato.FechaDesde = precio.DesdeEntrega;
            contrato.FechaHasta = precio.HastaEntrega;
            if (precio.DestinoId == null) {
                contrato.DestinoId = 1;
            } else {
                contrato.DestinoId = precio.DestinoId;
            }
            this.fechaInicio = contrato.FechaDesde.toLocaleDateString('en-GB');
            this.fechaFin = contrato.FechaHasta.toLocaleDateString('en-GB');
            $("#noCursor").val(contrato.FechaDesde.toLocaleDateString("en-GB"));
            $("#noCursor2").val(contrato.FechaHasta.toLocaleDateString("en-GB"));
        }
        if (precio.TipoNegocioId == 1) {
            //fijacion desde hasta
            contrato.DesdeFijacion = precio.DesdeFijacion;
            contrato.HastaFijacion = precio.HastaFijacion;
            this.fechafInicio = contrato.DesdeFijacion.toLocaleDateString('en-GB');
            this.fechafFin = contrato.HastaFijacion.toLocaleDateString('en-GB');
            $("#noCursorf").val(contrato.DesdeFijacion.toLocaleDateString("en-GB"));
            $("#noCursor2f").val(contrato.HastaFijacion.toLocaleDateString("en-GB"));
        }
        this.materiales = new Array();
        this.materiales.push({ Id: precio.MaterialId, Descripcion: precio.Material });
        contrato.MaterialId = precio.MaterialId;
        this.precioSeleccionado = true;
        this.materialSeleccionado = precio.Material;
        if (contrato.Id == 0) {
            var CampaniaIdActual = this.datosContrato.Datos.material.filter(x => x.Descripcion == precio.Material)[0].CampaniaIdActual;
            contrato.CampanaId = CampaniaIdActual;
        }
        this.habilitaciones(contrato);
    }

    isVisibleGrabar() {
        return !this.spinnerComponent.visible.value;
    }
    isMaterialSeleccionado(material: string) {
        return this.materialSeleccionado == "" || this.materialSeleccionado == material;
    }

    traerContratoCompleto(contrato) {
        this.unsubscribe();
        this.subscription = this.service.traerContratoCompleto(contrato.Id, contrato.TipoNegocioId).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                    this.blockUI.stop();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                    this.blockUI.stop();
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                    this.blockUI.stop();
                } else {
                    this.pendientesFijar = [];
                    if (result != "") {
                        let obj = JSON.parse(result);
                        this.contratoEditar = obj;
                        console.log("contratoEditar", obj);
                        this.precioSeleccionado = true;
                        this.materialSeleccionado = obj.Material;
                        contrato.TipoNegocioId = obj.TipoNegocioId;
                        contrato.MaterialId = obj.MaterialId;
                        contrato.EstadoId = obj.EstadoId;
                        contrato.ProvinciaId = obj.ProvinciaId;
                        if (obj.ObservacionTercero != null) {
                            this.ObservacionTercero = obj.ObservacionTercero.split("|")[0];
                            var p = obj.ObservacionTercero.split("|");
                            if (obj.CalidadTercero == true) {
                                var f = p.filter(function (e) { return e.includes("Calidad:") });
                                if (f) {
                                    this.ObservacionCalidadTercero = f[0].split(":")[1].trim();
                                }
                            }
                            if (obj.DolarizadoTercero == true) {
                                var f = p.filter(function (e) { return e.includes("Dolarizado:") });
                                if (f) {
                                    this.ObservacionDolarizadoTercero = f[0].split(":")[1].trim();
                                }
                            }
                            if (obj.SustentableTercero == true) {
                                var n = "";
                                var f = p.filter(function (e) { return e.includes("Sustentable:") });
                                if (f) {
                                    this.ObservacionSustentableTercero = f[0].split(":")[1].trim();
                                }
                            }
                            if (obj.PagoDiferidoTercero == true) {
                                var f = p.filter(function (e) { return e.includes("Pago Diferido:") });
                                if (f) {
                                    this.ObservacionPagoDiferidoTercero = f[0].split(":")[1].trim();
                                }
                            }
                        }
                        contrato.Proveedorid = obj.Proveedorid;
                        contrato.DestinoId = obj.DestinoId;
                        contrato.Cantidad = obj.Cantidad;
                        contrato.MonedaId = obj.MonedaId;
                        contrato.Precio = obj.Precio;
                        contrato.CantidadCamiones = obj.CantidadCamiones;
                        contrato.ComercialId = obj.ComercialId;
                        contrato.Pizarra = obj.Pizarra;
                        contrato.ClasificacionId = obj.ClasificacionId;
                        contrato.LocalidadId = obj.LocalidadId;
                        contrato.Consignatario = obj.Consignatario;
                        contrato.PlanCanje = obj.PlanCanje;
                        contrato.BoletoId = obj.BoletoId;
                        contrato.BolsaId = obj.BolsaId;
                        contrato.EstablecimientoPropio = obj.EstablecimientoPropio;
                        contrato.ContratoVendedor = obj.ContratoVendedor;
                        contrato.ContratoCorredor = obj.ContratoCorredor;
                        contrato.PagoDiferidoTercero = obj.PagoDiferidoTercero;
                        contrato.PagoDiferidoTerceroId = obj.PagoDiferidoTerceroId;
                        if (!obj.PagoDiferidoTerceroId && this.ObservacionPagoDiferidoTercero)
                            contrato.PagoDiferidoTerceroId = -1;
                        if (contrato.PagoDiferidoTerceroId > 0)
                            this.ObservacionPagoDiferidoTercero = "";

                        contrato.DolarizadoTercero = obj.DolarizadoTercero;
                        contrato.CalidadTercero = obj.CalidadTercero;
                        contrato.SustentableTercero = obj.SustentableTercero;
                        contrato.CampanaId = obj.CampanaId;

                        contrato.ProveedorId = obj.ProveedorId;
                        this.proveedor = obj.Proveedor;
                        if (obj.Corredor != null && obj.Corredor != "" && obj.ComercialId != 22) {
                            contrato.ComercialId = 0;
                        } else {
                            contrato.ComercialId = obj.ComercialId;
                        }
                        contrato.Fecha = new Date(parseInt(obj.Fecha.substr(6)));
                        contrato.FechaOperacion = new Date(parseInt(obj.FechaOperacion.substr(6)));

                        if (contrato.TipoNegocioId == 2 || contrato.TipoNegocioId == 1 || contrato.TipoNegocioId == 3) {
                            //entregadesde hasta
                            contrato.FechaDesde = new Date(parseInt(obj.FechaDesde.substr(6)));
                            contrato.FechaHasta = new Date(parseInt(obj.FechaHasta.substr(6)));
                            this.fechaInicio = contrato.FechaDesde.toLocaleDateString('en-GB');
                            this.fechaFin = contrato.FechaHasta.toLocaleDateString('en-GB');
                            $("#noCursor").val(contrato.FechaDesde.toLocaleDateString("en-GB"));
                            $("#noCursor2").val(contrato.FechaHasta.toLocaleDateString("en-GB"));
                        }
                        if (contrato.TipoNegocioId == 1) {
                            //fijacion desde hasta
                            contrato.DesdeFijacion = new Date(parseInt(obj.DesdeFijacion.substr(6)));
                            contrato.HastaFijacion = new Date(parseInt(obj.HastaFijacion.substr(6)));
                            this.fechafInicio = contrato.DesdeFijacion.toLocaleDateString('en-GB');
                            this.fechafFin = contrato.HastaFijacion.toLocaleDateString('en-GB');
                            $("#noCursorf").val(contrato.DesdeFijacion.toLocaleDateString("en-GB"));
                            $("#noCursor2f").val(contrato.HastaFijacion.toLocaleDateString("en-GB"));
                        }

                        this.localidad = obj.Localidad + " (" + obj.Provincia + ")";

                        if (contrato.TipoNegocioId == 3) {
                            contrato.ContratoSAP = obj.DatosFijacion.ContratoId;
                            contrato.Posicion = obj.Posicion;
                            contrato.TrigoEspecial = obj.TrigoEspecial;
                            this.pendienteFijarContrato = obj.DatosFijacion.ContratoId;
                        }

                        if (contrato.MonedaId == "ARP  ") {
                            this.monedas.push({ Id: "ARP  ", Descripcion: "ARP" });
                            contrato.MonedaId = "ARP  ";
                            if (contrato.TipoNegocioId == 2) { contrato.DolarizadoTercero = false; }
                        }
                        if (contrato.MonedaId == "USDM ") {
                            this.monedas.push({ Id: "USDM ", Descripcion: "USD" });
                            contrato.MonedaId = "USDM ";
                            if (contrato.TipoNegocioId == 2) { contrato.PagoDiferidoTercero = false; }
                        }
                        if (obj.BoletoId == 1) {
                            this.bolsasSelect = this.bolsasConfirma;
                        }
                        if (obj.BoletoId == 2) {
                            this.bolsasSelect = this.bolsasFisico;
                        }
                        if (obj.BoletoId == 3) {
                            this.bolsasSelect = [];
                        }
                        if (obj.BoletoId == 4) {
                            this.bolsasSelect = this.bolsasCarta;
                        }

                        if (contrato.Pizarra == true) {
                            this.datosPizarra = true;
                        } else {
                            this.datosPizarra = false;
                        }
                        this.materiales = new Array();
                        this.materiales.push({ Id: obj.MaterialId, Descripcion: obj.Material });
                        this.precioSeleccionado = true;
                        this.cuitProveedorSeleccionado = obj.Cuit;
                        this.negocioHabilitado(contrato);
                    }
                    //Este tengo que borrar
                    //this.blockUI.stop();
                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    };

    SeleccionAutomaticaBolsa(contrato) {
        console.log("SeleccionAutomaticaBolsa", this.bolsasAutomaticas);
        if (contrato.BoletoId == 1) {
            var p = this.bolsasAutomaticas.filter(a => a.DestinoId == contrato.DestinoId && a.ProvinciaId == contrato.ProvinciaId)
            if (p.length == 1) {
                if (p[0].BolsaId != contrato.BolsaId) {
                    this.bolsaRecomendada = p[0].BolsaId;
                    this.mensajeCambioBolsa = p[0].Bolsa;
                    document.getElementById("openModalbolsaModal").click();
                }

            }
        }
    }

    cambiarBolsa(contrato) {
        contrato.BolsaId = this.bolsaRecomendada;
    }

    changeDestino(contrato) {
        console.log("changeDestino");
        console.log(contrato.DestinoId)
        if (this.BolsaId != null && contrato.BoletoId == 1) {
            contrato.BolsaId = this.BolsaId;
        }
        this.SeleccionAutomaticaBolsa(contrato);
    }

    altaMasiva() {
        this.navService.navegarSeccion("/crear-contrato/alta-masiva");
    };

}