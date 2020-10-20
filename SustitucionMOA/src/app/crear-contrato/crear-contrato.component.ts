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
    localidad: any;
    localidades = [];
    proveedor: any;
    proveedores = [];
    keyword = 'Nombre';
    keyword2 = "RazonSocial";
    autocompleteNotFoundText = "No encontrado";

    datosContrato: any = new Array();
    materiales: any = new Array();
    monedas: any = new Array();
    destinos: any = new Array();
    campanias: any = new Array();
    zona: any = new Array();
    bolsasSelect: any = new Array();
    bolsasConfirma: any = new Array();
    bolsasFisico: any = new Array();
    bolsasCarta: any = new Array();
    condicionVendedor: any = new Array();
    condicionFijacion: any = new Array();
    datosCompraNet: any = null;


    checkPermisos() { this.securityService.tienePermisoRedirect("CREAR CONTRATOS"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/contrato/crear/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/contrato/crear/afijar', 'crear-contrato', 'A Fijar'),
            //new Seccion('/crear-contrato/fijaciones', 'crear-contrato', 'Fijaciones')
        ]);
        //this.obteneDatosContrato();
    }

    obteneDatosContrato(contrato) {
        this.unsubscribe();
        this.subscription = this.service.obteneDatosContrato().subscribe(
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
                    contrato.MonedaId = this.monedas[0].Id;
                    contrato.MaterialId = this.materiales[0].Id;
                    contrato.CampanaId = this.campanias[0].Id;
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

}