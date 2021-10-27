import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { BaseComponent } from '../../common/base-components/base-component';
import { CorredorContrato } from '../../common/models/ordenes-de-carga/corredorContrato';
import { EstadoOrdenDeCarga } from '../../common/models/ordenes-de-carga/estadoOrdenDeCarga';
import { OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';

@Component({
    selector: 'app-ordenes-de-carga.detalle',
    templateUrl: './ordenes-de-carga.detalle.component.html',
    styleUrls: ['./ordenes-de-carga.detalle.component.css', '../listado/ordenes-de-carga.listado.component.css']
})
export class OrdenesDeCargaDetalleComponent extends BaseComponent implements OnInit {

    ordenDeCargaId: number = 0;
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    mensajeError: string = "";

    corredores: Map<number, string>;
    corredorSeleccionado: number;

    contratos: string[] = [];
    contratoSeleccionado: string;

    pedidos: string[] = [];
    pedidoSeleccionado: string;

    corredorContratoList: CorredorContrato[] = [];
    corredorContratoSeleccionado: CorredorContrato;

    mostrarBotonContratos: boolean = false;
    mostrarBotonPedidos: boolean = false;
    mostrarBotonCorredores: boolean = false;
    mostrarBotonNotificarTransporte: boolean = false;
    mostrarBotonVerificarTransporte: boolean = false;
    mostrarBotonVerificarSituacionCrediticia: boolean = false;
    mostrarBotonAnular: boolean = false;
    mostrarBotonForzarCreacionPedido: boolean = false;


    // esInterno: boolean = false;
    esInterno: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS');
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esMesaFas: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA MESA FAS');
    esPuerto: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA PUERTO');
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";

    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);

        // this.esInterno = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
        });

        this.navService.setSeccionList([]);

        if (this.ordenDeCargaId > 0) {
            this.obtenerOrdenDeCarga();
        }
    }

    verificarBotones() {
        if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Anulada) {
            return;
        }

        if (this.esInterno) {

            if (this.ordenDeCarga.NumeroPedido === "-") {
                this.mostrarBotonPedidos = true
            }

            if (this.ordenDeCarga.ContratoSAP === "-") {
                this.mostrarBotonContratos = true;
                this.mostrarBotonPedidos = false;
            }

            if (!this.ordenDeCarga.TransporteExiste) {
                this.mostrarBotonNotificarTransporte = true;
                this.mostrarBotonVerificarTransporte = true;
            }

            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito) {
                this.mostrarBotonVerificarSituacionCrediticia = true;
            }

            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Pendiente) {
                this.mostrarBotonAnular = true;
            }

            if (this.ordenDeCarga.ContratoSinCantidadPendiente) {
                this.mostrarBotonForzarCreacionPedido = true;
            }
        }
    }

    obtenerOrdenDeCarga() {
        try {
            this.subscriptionDropDowns = this.service.getOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                        this.ordenDeCarga = result.data;
                        this.verificarBotones()
                        if (this.ordenDeCarga.MensajeValidacionSAP != "") {
                            this.mensajeComponent.setInfoMsg(this.ordenDeCarga.MensajeValidacionSAP)
                        }
                    }
                },
                error => {

                }
            );
        } catch (e) {
        }
    }


    notificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.notificarTransporte(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }


    verificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.verificarTransporte(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        // this.mostrarBotonNotificarTransporte = false;
                        // this.mostrarBotonVerificarTransporte = false;
                        this.obtenerOrdenDeCarga();

                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    verificarSituacionCrediticia() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.verificarSituacionCrediticia(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setSuccessMsg(result.data);

                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    abrirModalCorredores() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.obtenerCorredores(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.corredores = result.data;
                        document.getElementById("openSeleccionarCorredor").click();

                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    seleccionarContrato() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscriptionDropDowns = this.service.seleccionarContrato(this.ordenDeCargaId, this.contratoSeleccionado).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCarga.ContratoSAP = this.contratoSeleccionado;
                        this.mostrarBotonContratos = false;
                        document.getElementById("closemodalSeleccionarContrato").click();
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    abrirModalContratos() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.obtenerContratos(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.contratos = result.data;
                        document.getElementById("openSeleccionarContrato").click();

                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    abrirModalAnular() {
        document.getElementById("openAnularOrden").click();
    }

    abrirModalPedidos() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.obtenerPedidos(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.pedidos = result.data;
                        document.getElementById("openSeleccionarPedido").click();

                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    seleccionarPedido() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscriptionDropDowns = this.service.seleccionarPedido(this.ordenDeCargaId, this.pedidoSeleccionado).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCarga.NumeroPedido = this.pedidoSeleccionado;
                        this.mostrarBotonPedidos = false;
                        document.getElementById("closemodalSeleccionarPedido").click();
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    anularOrden() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscriptionDropDowns = this.service.anular(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        document.getElementById("closemodalAnularOrden").click();
                        this.mensajeComponent.setSuccessMsg(result.data);

                        this.navService.navegarSeccion(
                            "/ordenes-de-carga"
                        );
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    editarOrden() {
        this.goToSeccion('/ordenes-de-carga/alta/' + this.ordenDeCarga.Id);
    }


    abrirModalForzarCreacion() {
        document.getElementById("openForzarCreacion").click();
    }

    forzarCreacionPedido() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.forzarCreacionOrden(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        document.getElementById("closemodalForzarCreacion").click();
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
}
