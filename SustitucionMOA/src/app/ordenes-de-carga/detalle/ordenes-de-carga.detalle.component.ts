import { DatePipe } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { BaseComponent } from '../../common/base-components/base-component';
import { CorredorContrato } from '../../common/models/ordenes-de-carga/corredorContrato';
import { EstadoOrdenDeCarga } from '../../common/models/ordenes-de-carga/estadoOrdenDeCarga';
import { KILOS_DISPONIBLES_APROBADO, OrdenDeCarga, VOLVER_A_DETALLE_REPORTE } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { TipoContrato } from '../../common/models/ordenes-de-carga/obtenerContratosDisponiblesResponse';
import { Factura } from '../../common/models/ordenes-de-carga/Factura';
import { finalize } from 'rxjs/operators';
import { SendDataService } from '../../consulta/send-data.service';

@Component({
    selector: 'app-ordenes-de-carga.detalle',
    templateUrl: './ordenes-de-carga.detalle.component.html',
    styleUrls: ['./ordenes-de-carga.detalle.component.css', '../listado/ordenes-de-carga.listado.component.css']
})
export class OrdenesDeCargaDetalleComponent extends BaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild('mainStart') mainDiv?: ElementRef<HTMLDivElement>;
    ordenDeCargaId: number = 0;
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    mensajeError: string = "";
    mensajeSeleccionarContrato?: string;
    mensajeSeleccionarFactura?: string;
    mensajeValidacionScato: string = "";
    ordenDeCargaHistorial: any = {};
    estadosVerHistorial: EstadoOrdenDeCarga[] = [
        EstadoOrdenDeCarga.Anulada,
        EstadoOrdenDeCarga.Entregada,
        EstadoOrdenDeCarga.AnuladaPorVencimiento,
        EstadoOrdenDeCarga.EdicionRechazada
    ];
    corredores: Map<number, string>;
    corredorSeleccionado: number;

    contratos: string[] = [];
    facturas: Factura[] = [];
    contratoSeleccionado: string;
    facturaSeleccionada?: Factura = null;

    corredorContratoList: CorredorContrato[] = [];
    corredorContratoSeleccionado: CorredorContrato;

    validaCPEDG = false;

    mostrarBotonContratos: boolean = false;
    mostrarBotonCorredores: boolean = false;
    mostrarBotonNotificarTransporte: boolean = false;
    mostrarBotonVerificarTransporte: boolean = false;
    mostrarBotonVerificarSituacionCrediticia: boolean = false;
    mostrarBotonVerHistorial: boolean = false;
    mostrarBotonAnular: boolean = false;
    mostrarBotonAnularPorVencimiento: boolean = false;
    mostrarBotonActivarOC: boolean = false;
    mostrarBotonForzarCreacionPedido: boolean = false;
    mostrarBotonEditar: boolean = false;
    mostrarBotonSeleccionarFactura: boolean = false;
    mostrarBotonVolverADetalle: boolean = false;
    mostrarBotonVerificarCuitsTercero: boolean = false;

    mostrarListadoInterno: boolean = false;
    mostrarListadoTercero: boolean = false;
    mostrarListadoComercial: boolean = false;
    mostrarListadoMesaFas: boolean = false;
    mostrarListadoPuerto: boolean = false;

    mostrarBotonSolicitarAnulacion: boolean = false;
    mostrarBotonAprobarRechazarAnulacion: boolean = false;
    mostrarBotonEdicionFinalizada: boolean = false;
    mostrarBotonVerificarCompensacion: boolean = false;

    ordenActivaScato: boolean = false;

    // esInterno: boolean = false;
    esInterno: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS');
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esMesaFas: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA MESA FAS');
    esPuerto: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA PUERTO');
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esAnulador: boolean = this.isAuthorized('ANULAR ORDEN DE CARGA');

    navegandoADetalle = false;

    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        private router: Router,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe,
        private sendDataService: SendDataService,
        private confirmationService: ConfirmationService) {
        super(navService, securytiService, floatMsgService, modalService);
        this.mostrarBotonVolverADetalle = !!sessionStorage.getItem(VOLVER_A_DETALLE_REPORTE);
    }

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
            this.cargarDetalle()
        });
    }

    cargarDetalle() {
        if (this.ordenDeCargaId > 0) {
            this.obtenerOrdenDeCarga();
        }
    }

    confirmarSA(Id) {
        this.confirmationService.confirm({
            key: 'confirmarSA',
            message: '¿Desea solicitar anulación?',
            accept: () => {
                this.solicitarAnulacion(Id)
            },
            reject: () => {
            }
        });
    }

    confirmarRSA(Id) {
        this.confirmationService.confirm({
            key: 'confirmarRSA',
            message: '¿Desea rechazar la solicitud de anulación?',
            accept: () => {
                this.rechazarSolicitudAnulacion(Id)
            },
            reject: () => {
            }
        });
    }

    rechazarSolicitudAnulacion(Id) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.rechazarSolicitudAnulacion(Id).subscribe(
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
                        this.navService.navegarSeccion(
                            "/ordenes-de-carga"
                        );
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
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    solicitarAnulacion(Id) {
        try {
            this.mensajeComponent.setMsgsEmpty();
            this.spinnerComponent.showIt();
            this.unsubscribe();
            this.blockUI.start('Registrando solicitud...');

            this.subscription = this.service.solicitarAnulacion(Id).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.navService.navegarSeccion(
                            "/ordenes-de-carga"
                        );
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
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    //Está función va a desaparecer cuando hagamos el refactor de como mostrar los datos de esta pantalla
    verificarListado() {

        this.mostrarListadoTercero = false;
        this.mostrarListadoComercial = false;
        this.mostrarListadoMesaFas = false;
        this.mostrarListadoPuerto = false;


        if (this.esComercial) {
            this.mostrarListadoComercial = true;
            return;
        }

        if (this.esMesaFas) {
            this.mostrarListadoMesaFas = true;
            return;
        }

        if (this.esPuerto) {
            this.mostrarListadoPuerto = true;
            return;
        }

        if (this.esTercero) {
            this.mostrarListadoTercero = true;
            return;
        }
    }

    verificarBotones() {
        this.mostrarBotonContratos = false;
        this.mostrarBotonVerHistorial = false;
        this.mostrarBotonAprobarRechazarAnulacion = false;
        this.mostrarBotonAnularPorVencimiento = false;
        this.mostrarBotonAnular = false;
        this.mostrarBotonActivarOC = false;
        this.mostrarBotonForzarCreacionPedido = false;
        this.mostrarBotonVerificarSituacionCrediticia = false;
        this.mostrarBotonNotificarTransporte = false;
        this.mostrarBotonEditar = false;
        this.mostrarBotonEdicionFinalizada = false;
        this.mostrarBotonSeleccionarFactura = false;
        this.mostrarBotonVerificarCompensacion = false;
        this.mostrarBotonVerificarCuitsTercero = false;

        if (this.estadosVerHistorial.indexOf(this.ordenDeCarga.Estado) >= 0) {
            this.mostrarBotonVerHistorial = true;
            return;
        }

        if (this.esInterno || this.esComercial || this.esMesaFas) {
            this.mostrarBotonVerHistorial = true;
            this.mostrarBotonEditar = true;

            if (this.esAnulador) {
                if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.AnulacionSolicitada || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Confirmado || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.ContratoVencido || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EntregaGenerada || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EntregaPendiente || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.ErrorDeCarga || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Pendiente || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Vencida || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EdicionSolicitada || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion) {
                    this.mostrarBotonAnular = true;
                }
            }

            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EdicionSolicitada) {
                this.mostrarBotonEdicionFinalizada = true;
            }

            if (this.ordenDeCarga.ContratoSeleccionado) {
                if (this.ordenDeCarga.ContratoSeleccionado.KgDisponibles < KILOS_DISPONIBLES_APROBADO ||
                    !this.ordenDeCarga.ContratoSAP) {
                    this.mostrarBotonContratos = true;
                }
            }
            else {
                console.error("Falta cargar contrato seleccionado");
            }

            if (!this.ordenDeCarga.TransporteExiste) {
                this.mostrarBotonNotificarTransporte = true;
                this.mostrarBotonVerificarTransporte = true;
            }

            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito) {
                this.mostrarBotonVerificarSituacionCrediticia = true;
            }


            if (this.ordenDeCarga.ContratoSinCantidadPendiente) {
                this.mostrarBotonForzarCreacionPedido = true;
            }

            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Vencida) {
                this.mostrarBotonAnularPorVencimiento = true;
                if (this.ordenDeCarga.FechaVencimientoAmpliada == false) {
                    this.mostrarBotonActivarOC = true;
                }
            }
            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.AnulacionSolicitada) {
                this.mostrarBotonAprobarRechazarAnulacion = true;
            }
            if (this.ordenDeCarga.TipoContrato === TipoContrato.FacturaAnticipada && !this.ordenDeCarga.NumeroFacturaSeleccionada) {
                this.mostrarBotonSeleccionarFactura = true;
            }
            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.PendienteCompensacion) {
                this.mostrarBotonVerificarCompensacion = true;
            }
            if ((this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Pendiente || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EntregaPendiente) && this.ordenDeCarga.NecesitaVerificarCuitsTerceros) {
                this.mostrarBotonVerificarCuitsTercero = true;
            }
        }
        else {
            if (this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Vencida &&
                this.ordenDeCarga.FechaVencimientoAmpliada == false) {
                    this.mostrarBotonActivarOC = true;
                }
        }
        if (this.esAnulador) {
            this.mostrarBotonAnular = true;
        }
        if (!(this.ordenDeCarga.Estado == EstadoOrdenDeCarga.AnulacionSolicitada || this.ordenDeCarga.Estado == EstadoOrdenDeCarga.EdicionSolicitada)) {
            if ((this.esTercero || ((this.esCliente() || this.esCorredor) && !this.mostrarBotonAnular)) && !(this.ordenDeCarga.Estado == EstadoOrdenDeCarga.Vencida))
                this.mostrarBotonSolicitarAnulacion = true;

            if (this.ordenDeCarga.EdicionRechazada != true)
                this.mostrarBotonEditar = true;
        }
    }

    obtenerOrdenDeCarga() {
        try {
            this.unsubscribe();
            this.blockUI.start('Procesando...');
            this.subscriptionDropDowns = this.service.getOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                        this.ordenDeCarga = result.data;
                        this.validaCPEDG = this.ordenDeCarga.ValidaSisaRuca;
                        this.separarCadenas();
                        this.verificarBotones()
                        if (this.mensajeError || this.ordenDeCarga.DescripcionErrorInterno)
                            this.mensajeComponent.setMsgsEmpty();
                        if (this.ordenDeCarga.MensajeValidacionSAP != "" && this.ordenDeCarga.MensajeValidacionSAP != "OK" && this.esInterno) {
                            this.mensajeComponent.setMsgsEmpty();
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

    separarCadenas() {

        this.ordenDeCarga.OrdenDeCargaCambiosHistorial.forEach(x => {

            const wordRegex = /[A-Z]?[a-z]+|[0-9]+|[A-Z]+(?![a-z])/g;
            const string = x.NombreColumnaCambio;
            var palabra = string.match(wordRegex);
            x.NombreColumnaCambio = palabra.join(" ");
        });
    }

    notificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.notificarTransporte(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.obtenerOrdenDeCarga()
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


    verificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.verificarTransporte(this.ordenDeCargaId).subscribe(
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
                        this.ordenDeCarga.DescripcionErrorInterno = null;
                        if (result.data != "Orden de carga actualizada correctamente") {
                            this.mensajeComponent.setInfoMsg(result.data);
                        } else {
                            this.mensajeComponent.setSuccessMsg(result.data);
                        }
                        this.mostrarBotonNotificarTransporte = false;
                        this.mostrarBotonVerificarTransporte = false;
                        this.obtenerOrdenDeCarga();
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

    verificarSituacionCrediticia() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.verificarSituacionCrediticia(this.ordenDeCargaId).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data.error != undefined && result.data.error != "") {
                        this.mensajeComponent.setErrorMsg(result.data.error);
                    } else if (result.data.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.data.info);
                    } else {
                        this.ordenDeCarga.DescripcionErrorInterno = null;
                        this.mensajeComponent.setSuccessMsg(result.data.Mensaje);
                        this.obtenerOrdenDeCarga();
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
        this.mensajeSeleccionarContrato = null;
        this.spinnerComponent.showIt();
        if (!this.contratoSeleccionado) {
            this.mensajeSeleccionarContrato = "Por favor seleccione un contrato para confirmar.";
            return;
        }
        this.unsubscribe();
        this.blockUI.start('Grabando...');
        try {
            this.subscriptionDropDowns = this.service.seleccionarContrato(this.ordenDeCargaId, this.contratoSeleccionado).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    document.getElementById("closemodalSeleccionarContrato").click();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.data.error != undefined && result.data.error != "") {
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setErrorMsg(result.data.error);
                    } else if (result.data.info != undefined) {
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setInfoMsg(result.data.info);
                    } else {
                        this.ordenDeCarga.ContratoSAP = this.contratoSeleccionado;
                        this.mostrarBotonContratos = false;
                        this.ordenDeCarga.DescripcionErrorInterno = null;
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeComponent.setSuccessMsg(result.data.Mensaje);
                        this.obtenerOrdenDeCarga();
                    }
                },
                error => {
                    this.blockUI.stop();
                    document.getElementById("closemodalSeleccionarContrato").click();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    seleccionarFactura() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.mensajeSeleccionarFactura = null;
        if (!this.facturaSeleccionada) {
            this.mensajeSeleccionarFactura = "Por favor seleccione un número de factura.";
            return;
        }
        this.unsubscribe();
        this.blockUI.start('Grabando...');
        try {
            this.subscriptionDropDowns = this.service.seleccionarFactura(this.ordenDeCargaId, this.facturaSeleccionada).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    document.getElementById("closemodalSeleccionarFactura").click();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.data.error != undefined && result.data.error != "") {
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setErrorMsg(result.data.error);
                    } else if (result.data.info != undefined) {
                        this.obtenerOrdenDeCarga();
                        this.mensajeComponent.setInfoMsg(result.data.info);
                    } else {
                        this.ordenDeCarga.DescripcionErrorInterno = null;
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeComponent.setSuccessMsg(result.data.Mensaje);
                        this.obtenerOrdenDeCarga();
                    }
                },
                error => {
                    this.blockUI.stop();
                    document.getElementById("closemodalSeleccionarFactura").click();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    abrirModalContratos() {
        this.mainDiv.nativeElement.scrollIntoView({ behavior: "smooth", block: "center" });
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('');
        this.unsubscribe();
        try {
            this.subscriptionDropDowns = this.service.obtenerContratos(this.ordenDeCargaId).subscribe(
                result => {
                    this.blockUI.stop();
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
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.blockUI.stop();
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    abrirModalSeleccionarFactura() {
        this.mainDiv.nativeElement.scrollIntoView({ behavior: "smooth", block: "center" });
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('');
        this.unsubscribe();
        const contrato = this.ordenDeCarga.ContratoSAP || this.ordenDeCarga.ContratoIngresado;
        try {
            this.subscriptionDropDowns = this.service.obtenerFacturasDeContrato(contrato).subscribe(
                result => {
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.facturas = result.data;
                        document.getElementById("openSeleccionarFactura").click();

                    }
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.blockUI.stop();
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    abrirModalAnular() {
        this.verificarOrdenActivaScato(this.ordenDeCarga.Id.toString());
        document.getElementById("openAnularOrden").click();
    }
    abrirModalAnularVencimiento() {
        document.getElementById("openAnularOrdenVencimiento").click();
    }
    abrirModalActivarOC() {
        document.getElementById("openModalActivarOC").click();
    }
    abrirModalEdicionFinalizada() {
        this.verificarOrdenActivaScato(this.ordenDeCargaId.toString());
        document.getElementById("openEdicionFinalizada").click();
    }

    anularOrden() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            document.getElementById("closemodalAnularOrden").click();
            this.mainDiv.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'center' })
            this.subscriptionDropDowns = this.service.anular(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.obtenerOrdenDeCarga();
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.obtenerOrdenDeCarga();
                    } else {
                        this.floatMsgService.setSuccessMsg(result.data);

                        this.navService.navegarSeccion(
                            "/ordenes-de-carga"
                        );
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.showIt();
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    anularOrdenPorVencimiento() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            document.getElementById("closemodalAnularOrdenVencimiento").click();
            this.mainDiv.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'center' })
            this.subscriptionDropDowns = this.service.anularPorVencimiento(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.obtenerOrdenDeCarga();
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.obtenerOrdenDeCarga();
                    } else {
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
            document.getElementById("closemodalAnularOrdenVencimiento").click();
            this.mensajeComponent.setErrorMsg(e);
            document.getElementById("closemodalAnularOrdenVencimiento").click();
        }
    }
    edicionFinalizada(Tipo) {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            if (Tipo == "A") {
                console.log(this.ordenDeCargaId);

                this.subscriptionDropDowns = this.service.edicionFinalizada(this.ordenDeCargaId).subscribe(
                    result => {
                        this.spinnerComponent.hideIt();
                        document.getElementById("closemodalEdicionFinalizada").click();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.mensajeComponent.setSuccessMsg(result.data);

                            this.navService.navegarSeccion(
                                "/ordenes-de-carga"
                            );
                        }
                    },
                    error => {
                        document.getElementById("closemodalEdicionFinalizada").click();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
            }
            else {
                this.subscriptionDropDowns = this.service.rechazarSolicitudEdicion(this.ordenDeCargaId).subscribe(
                    result => {
                        this.spinnerComponent.hideIt();
                        document.getElementById("closemodalEdicionFinalizada").click();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.mensajeComponent.setSuccessMsg(result.data);

                            this.navService.navegarSeccion(
                                "/ordenes-de-carga"
                            );
                        }
                    },
                    error => {
                        document.getElementById("closemodalEdicionFinalizada").click();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
            }

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

    abrirModalDetalleHistorialCompras() {
        document.getElementById("openModalDetalleHistorial").click();
    }

    forzarCreacionPedido() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Grabando...');
        try {
            this.subscription = this.service.forzarCreacionOrden(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    document.getElementById("closemodalForzarCreacion").click();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.obtenerOrdenDeCarga();
                    }
                },
                error => {
                    this.blockUI.stop();
                    document.getElementById("closemodalForzarCreacion").click();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    activarOC() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscriptionDropDowns = this.service.activarOC(this.ordenDeCargaId).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    document.getElementById("closemodalActivarOC").click();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);

                        this.navService.navegarSeccion(
                            "/ordenes-de-carga"
                        );
                    }
                },
                error => {
                    document.getElementById("closemodalActivarOC").click();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    verificarCompensacion() {
        this.mensajeComponent.setMsgsEmpty();
        this.mainDiv.nativeElement.scrollIntoView({ behavior: "smooth", block: "center" });
        this.blockUI.start('Procesando ...');
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.service.verificarCompensacion(this.ordenDeCargaId).pipe(
                finalize(() => { this.blockUI.stop(); this.spinnerComponent.hideIt() })
            ).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        return;
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                    this.obtenerOrdenDeCarga()
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.blockUI.stop();
            this.mensajeComponent.setErrorMsg(e);
        }
    }
    navegarADetalle() {
        this.navegandoADetalle = true;
        this.goToSeccion('/reporte-contrato/');
    }

    public extraOnDestroy(): void {
        if (!this.navegandoADetalle)
            sessionStorage.removeItem(VOLVER_A_DETALLE_REPORTE)
    }

    verificarOrdenActivaScato(ordenId: string) {
        this.mensajeValidacionScato = undefined;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.service.validarOrdenActivaScato(ordenId).pipe(
                finalize(() => { this.blockUI.stop(); this.spinnerComponent.hideIt() })
            ).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        return;
                    } else if ((result.error != undefined && result.error != "") || result.info != undefined) {
                        this.mensajeValidacionScato = "No se pudo validar si la orden esta activa en Scato."
                    } else {
                        this.ordenActivaScato = result.data;
                        if (this.ordenActivaScato) {
                            this.mensajeValidacionScato = "Actualmente la orden se encuentra activa en Scato."
                        }
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.mensajeValidacionScato = "No se pudo validar si la orden esta activa en Scato."
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.blockUI.stop();
            this.mensajeValidacionScato = "No se pudo validar si la orden esta activa en Scato."
        }
    }

    verificarCuitsTerceros() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.verificarCuitsTerceros(this.ordenDeCargaId).subscribe(
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
                        this.ordenDeCarga.DescripcionErrorInterno = null;
                        this.obtenerOrdenDeCarga();
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

    nuevaConsultaInterna() {
        this.sendDataService.setData({
            orden: this.ordenDeCarga,
            codSubcategoria: "FAS",
        });
        this.router.navigate(['/consulta/crear-consulta-interna'], {
        });
    }
}
