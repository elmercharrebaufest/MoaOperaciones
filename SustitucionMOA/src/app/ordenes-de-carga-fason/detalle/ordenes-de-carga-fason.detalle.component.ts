import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { OrdenDeCargaFasonDto } from '../../common/models/ordenes-de-carga-fason/ordenDeCargaFasonDto';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { OrdenesDeCargaFasonService } from '../ordenes-de-carga-fason.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { EstadoOrdenDeCargaFason } from '../../common/models/ordenes-de-carga-fason/estadoOrdenDeCargaFason';
import { Rol } from '../../common/enums/Roles';
import { Permiso } from '../../common/enums/Permisos';

export interface BotonesDetalleFason {
    editar: boolean;
    verificarTransporte: boolean;
    aprobarRechazarEdicion: boolean;
    aprobarRechazarAnulacion: boolean;
    solicitarAnulacion: boolean;
    anular: boolean;
    verificarCuitsTercero: boolean;
}


@Component({
    selector: 'app-detalle',
    templateUrl: './ordenes-de-carga-fason.detalle.component.html',
    styleUrls: ['./ordenes-de-carga-fason.detalle.component.css', '../listado/ordenes-de-carga-fason.listado.component.css']
})
export class OrdenesDeCargaFasonDetalleComponent extends ListBaseComponent implements OnInit {

    @BlockUI() blockUI: NgBlockUI;

    ordenDeCargaFason: OrdenDeCargaFasonDto = new OrdenDeCargaFasonDto();
    ordenDeCargaFasonId: number = 0;

    botones: BotonesDetalleFason = {} as BotonesDetalleFason;
    mensajeError: string = "";

    corredores: Map<number, string>;
    corredorSeleccionado: number;

    esTercero: boolean = this.isAuthorized(Permiso.FasonVerOrdenesDeCarga);
    esAdmin: boolean = this.isAuthorized(Permiso.FasonVerOrdenesDeCargaAdmin);
    esClienteFason = !this.esAdmin && this.esTercero;
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === Rol.Corredor;

    estadoOrdenDeCargaFason = EstadoOrdenDeCargaFason;

    validaCPEDG = false;

    estadosPermitenEdicion = [
        EstadoOrdenDeCargaFason.Generada,
        EstadoOrdenDeCargaFason.PendienteContabilizacion,
        EstadoOrdenDeCargaFason.Pendiente,
        EstadoOrdenDeCargaFason.PendienteCompensacion,
        EstadoOrdenDeCargaFason.Vencida,
    ];
    estadosPermitenSolicitarAnulacion = [
        EstadoOrdenDeCargaFason.Generada,
        EstadoOrdenDeCargaFason.Pendiente,
        EstadoOrdenDeCargaFason.Vencida,
    ];

    constructor(private route: ActivatedRoute, protected service: OrdenesDeCargaFasonService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe,
        private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaFasonId = params["id"];
        });

        this.navService.setSeccionList([]);
        this.obtenerOrdenDeCargaFason();

    }

    verificarBotones() {
        this.botones = {} as BotonesDetalleFason;

        this.botones.verificarTransporte = this.esAdmin && !this.ordenDeCargaFason.TransporteExiste;

        this.botones.editar = this.estadosPermitenEdicion.includes(this.ordenDeCargaFason.Estado);

        this.botones.aprobarRechazarAnulacion = this.esAdmin && this.ordenDeCargaFason.Estado == EstadoOrdenDeCargaFason.AnulacionSolicitada;

        this.botones.aprobarRechazarEdicion = this.esAdmin && this.ordenDeCargaFason.Estado == EstadoOrdenDeCargaFason.EdicionSolicitada;

        this.botones.solicitarAnulacion = this.esClienteFason && this.estadosPermitenSolicitarAnulacion.includes(this.ordenDeCargaFason.Estado)

        this.botones.anular = this.esAdmin && this.ordenDeCargaFason.Estado != EstadoOrdenDeCargaFason.Entregada;

        this.botones.verificarCuitsTercero = this.esAdmin && this.ordenDeCargaFason.NecesitaVerificarCuitsTerceros;
    }


    obtenerOrdenDeCargaFason() {
        try {
            this.unsubscribe();
            this.subscriptionDropDowns = this.service.getOrdenDeCargaFason(this.ordenDeCargaFasonId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                        this.ordenDeCargaFason = result.data.Response;
                        this.validaCPEDG = this.ordenDeCargaFason.ValidaSisaRuca;
                        this.verificarBotones();
                    }
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
        }
    }


    editarOrden() {
        this.goToSeccion('/ordenes-de-carga-fason/alta/' + this.ordenDeCargaFason.Id);
    }

    verificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.verificarTransporte(this.ordenDeCargaFason.Id).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data) {
                        this.ordenDeCargaFason = result.data;
                    } else {
                        this.obtenerOrdenDeCargaFason();
                    }
                    this.verificarBotones();

                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    confirmarSolicitudAnulacion() {
        this.confirmationService.confirm({
            key: 'confirmarSA',
            message: '¿Desea solicitar anulación?',
            accept: () => {
                this.solicitarAnulacion()
            },
            reject: () => {
            }
        });
    }
    confirmarAnulacion() {
        this.confirmationService.confirm({
            key: 'confirmarAnular',
            message: '¿Desea anular la orden?',
            accept: () => {
                this.anularOrden()
            },
            reject: () => {
            }
        });
    }
    confirmarRechazarSolicitudAnulacion(aprobado: boolean) {
        this.confirmationService.confirm({
            key: 'confirmarRSA',
            message: `¿Desea ${aprobado ? 'aprobar' : 'rechazar'} la solicitud de anulación?`,
            accept: () => {
                this.solicitudAnulacion(aprobado)
            },
            reject: () => {
            }
        });
    }
    confirmarRechazarSolicitudEdicion(aprobado: boolean) {
        this.confirmationService.confirm({
            key: 'confirmarSolicitudEdicion',
            message: `¿Desea ${aprobado ? 'aprobar' : 'rechazar'} la solicitud de anulación?`,
            accept: () => {
                this.solicitudAnulacion(aprobado)
            },
            reject: () => {
            }
        });
    }

    solicitarAnulacion() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.service.solicitarAnulacion(this.ordenDeCargaFason.Id).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data) {
                        this.ordenDeCargaFason = result.data;
                    } else {
                        this.obtenerOrdenDeCargaFason();
                    }
                    this.verificarBotones();
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    solicitudAnulacion(aprobado: boolean) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.service.actualizarSolicitudAnulacion(this.ordenDeCargaFason.Id, aprobado).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data) {
                        this.ordenDeCargaFason = result.data;
                    } else {
                        this.obtenerOrdenDeCargaFason();
                    }
                    this.verificarBotones();
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    solicitudEdicion(aprobado: boolean) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.service.actualizarSolicitudEdicion(this.ordenDeCargaFason.Id, aprobado).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data) {
                        this.ordenDeCargaFason = result.data;
                    } else {
                        this.obtenerOrdenDeCargaFason();
                    }
                    this.verificarBotones();
                },
                error => {
                    this.blockUI.stop();
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
        this.blockUI.start('Procesando...');
        try {
            this.service.anularOrden(this.ordenDeCargaFason.Id).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data) {
                        this.ordenDeCargaFason = result.data;
                    } else {
                        this.obtenerOrdenDeCargaFason();
                    }
                    this.verificarBotones();
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    verificarCuitsTerceros() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.verificarCuitsTerceros(this.ordenDeCargaFasonId).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        this.obtenerOrdenDeCargaFason();
                    }
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
}
