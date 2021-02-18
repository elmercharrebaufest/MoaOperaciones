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
    styleUrls: ['./ordenes-de-carga.detalle.component.css']
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

    contratos: Map<number, string>;
    contratoSeleccionado: number;

    corredorContratoList: CorredorContrato[] = [];
    corredorContratoSeleccionado: CorredorContrato;

    mostrarBotonContratos: boolean = false;
    mostrarBotonCorredores: boolean = false;
    mostrarBotonNotificarTransporte: boolean = false;
    mostrarBotonVerificarTransporte: boolean = false;
    mostrarBotonVerificarSituacionCrediticia: boolean = false;
    mostrarBotonAnular: boolean = false;

    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);
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

        if (this.ordenDeCarga.ContratoSAP === "" || this.ordenDeCarga.Corredor === "") {
            this.mostrarBotonContratos = true;
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


    abrirModalCorredoresYContratos() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        try {
            this.subscriptionDropDowns = this.service.obtenerContratosYCorredores(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.corredorContratoList = result.data;
                        document.getElementById("openSeleccionarContratoYCorredor").click();

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

    seleccionarContratoYCorredor() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        console.log(this.corredorSeleccionado);
        console.log(this.corredorContratoSeleccionado);
        try {
            this.subscriptionDropDowns = this.service.seleccionarCorredorContrato(this.ordenDeCargaId, this.corredorContratoSeleccionado).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        document.getElementById("closeModalSeleccionarCorredor").click();
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
                        console.log(result.data)
                        this.contratos = result.data;
                        console.log(this.contratos)
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

}
