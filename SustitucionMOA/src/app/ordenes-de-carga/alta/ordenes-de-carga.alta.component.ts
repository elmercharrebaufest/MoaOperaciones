import { DatePipe } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { EmpresaGranosService } from '../../alta-proveedores/empresa-granos/empresa-granos.service';
import { BaseComponent } from '../../common/base-components/base-component';
import { Material } from '../../common/models/material';
import { CuitValidaExistencia, CuitValidaRUCA, CuitValidaSISA, KILOS_DISPONIBLES_APROBADO, OrdenDeCarga, SIN_KILOS_DISPONIBLES } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { SeleccionarProveedorService } from '../../common/shared-components/seleccionar-proveedor/seleccionar-proveedor.service';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { ContratoOrdenFas, TipoContrato } from '../../common/models/ordenes-de-carga/obtenerContratosDisponiblesResponse';
import { Planta } from '../../common/models/ordenes-de-carga/planta';
import { Domicilio } from '../../common/models/ordenes-de-carga/domicilio';
import { debounceTime, finalize, take } from 'rxjs/operators';
import { ApiResponse } from '../../common/models/response';
import { Factura, newFactura } from '../../common/models/ordenes-de-carga/Factura';
import { IOrdenesBaseComponent } from '../../common/base-components/ordenes-base-component';
import { EstadoOrdenDeCarga } from '../../common/models/ordenes-de-carga/estadoOrdenDeCarga';
import { Subject, Subscription, forkJoin } from 'rxjs';
import { MessageService } from 'primeng/api';
import { Checkbox } from 'primeng/checkbox';
import { MSG_ALERTA_CAMION_NO_EXISTE, MSG_ALERTA_NO_ESCALABLE } from '../../common/models/ordenes-de-carga/ValidarCamionResponse';
import { Permiso } from '../../common/enums/Permisos';

@Component({
    selector: 'app-ordenes-de-carga.alta',
    templateUrl: './ordenes-de-carga.alta.component.html',
    styleUrls: ['./ordenes-de-carga.alta.component.css'],
    providers: [SeleccionarProveedorService],
})
export class OrdenesDeCargaAlta extends BaseComponent implements OnInit, IOrdenesBaseComponent {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild("escalableCheckbox") escalableCheckbox: Checkbox
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('messages')
    private messagesContainer?: ElementRef<HTMLDivElement>;

    ordenDeCargaId: number = 0;

    validarCNRTSubject = new Subject();
    validarCNRTSubscription?: Subscription;
    subscriptions = new Subscription();
    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    mensajesOrdenDeCarga: Partial<Record<keyof OrdenDeCarga, string>> = {};
    mensajesGestionCuit: Partial<Record<keyof Pick<OrdenDeCarga, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'>, string>> = {};
    gestiona: Partial<Record<keyof Pick<OrdenDeCarga, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'>, boolean>> = {
        CUITDestino: false,
        CUITDestinatario: false, CUITIntermediarioFlete: false
    };
    validando: Partial<Record<keyof OrdenDeCarga, boolean>> = {};
    displayModal: keyof Pick<OrdenDeCarga, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'> | null;
    validaCPEDG = false;
    editando = false;
    focusRazonSocialParaGestion = true;
    mensajeError: string = "";
    mensajeSuccess: string = "";
    clienteCUIT: string = "";
    clienteSeleccionado: any = "";
    clienteCodigo: string = "";
    CodigoCorredor: string = "";
    Contrato: string = "";
    Producto: string = "";
    desde: string = "";
    hasta: string = "";
    userEmail: string = "";
    patentesChasis: any;
    patentesAcoplados: any;
    razonSocialParaGestion = "";

    private selectUndefinedOptionValue: any;

    contratosDisponibles: ContratoOrdenFas[] = [];
    facturasDisponibles: Factura[] = [];
    facturaSeleccionada: Factura = {} as Factura;
    contratoSeleccionado: ContratoOrdenFas;
    tipoContrato = TipoContrato;
    listaMateriales: Material[];
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esComercial: boolean = this.isAuthorized(Permiso.FasVerOrdenesComerciales);
    esAdmin: boolean = this.isAuthorized(Permiso.FasVerTodasOrdenes);
    esInterno: boolean = this.esComercial || this.esCorredor;
    listaClientes: any[];
    noEditarCliente: boolean = false;
    estadosNoPuedeEditarCuitsTercero = [
        EstadoOrdenDeCarga.EntregaGenerada,
        EstadoOrdenDeCarga.Entregada,
        EstadoOrdenDeCarga.EntregaPendiente,
    ];

    puedeEditarContrato: boolean = false;
    resultadoValidacionCorCliConPro: boolean = false;
    mensajeValidacionCorCliConPro: string = "";

    listaCorredores: any[];
    corredorSeleccionado: any;
    public patternPatente = { '0': { pattern: new RegExp('\[a-zA-Z0-9\]') } };

    listaPlantas: Planta[];
    plantaSeleccionada?: Planta;
    listaDomicilios?: Domicilio[];
    domicilioSeleccionado?: Domicilio;

    cuilsChofer: any;
    cuitsTransporte: any;

    loadingCorredores: boolean = false;
    loadingClientes: boolean = false;

    intermediarioFleteCuitFormatoValido: boolean = true;
    escalableCNRT?: boolean;
    errorAlValidarEscalable = false;
    ordenActivaScato: boolean = false;
    mensajeValidacionScato: string = "";

    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService,
        protected navService: NavService,
        protected seleccionarProveedorService: SeleccionarProveedorService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService,
        protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        protected empresaGranosService: EmpresaGranosService,
        public datepipe: DatePipe,
        protected msgService: MessageService
    ) {
        super(navService, securytiService, floatMsgService, modalService);

        this.subscriptions.add(
            this.validarCNRTSubject.pipe(debounceTime(500)).subscribe(_ =>
                this.validarCNRTRequest()
            ))
    }

    get noPuedeEditarCuitsTerceros() {
        return this.ordenDeCarga.Id &&
            (this.estadosNoPuedeEditarCuitsTercero.includes(this.ordenDeCarga.Estado) ||
                (this.ordenDeCarga.TipoContrato != TipoContrato.FacturaAnticipada && this.ordenDeCarga.NumeroPedido)
            );
    }

    ngOnInit() {
        this.userEmail = sessionStorage.getItem("username");
        this.desde = this.getFecha(12);
        this.hasta = this.getFecha(0);
        this.ordenDeCarga.Cantidad = 0;

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
        });
        this.navService.setSeccionList([]);

        this.obtenerMateriales();
        if (this.esComercial) {
            this.onCorredorFocusOut('', false);
        }
        else if (this.esCorredor) {
            this.CodigoCorredor = sessionStorage.getItem("proveedor");
            if (this.ordenDeCargaId == 0) {
                this.cargarClientes(this.CodigoCorredor);
            }
        }

        this.editando = this.ordenDeCargaId > 0;
        this.obtenerCorredores();

        if (this.ordenDeCargaId > 0) {
            this.obtenerOrdenDeCarga();
            this.noEditarCliente = true;
        } else {
            this.getPatentes();
        }
        if (this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS')) {
            this.ordenDeCarga.CUITCliente = 0;
        }
        if (this.esCliente()) {
            const esInterno = this.esComercial || this.esCorredor;
            if (!esInterno) {
                // this.clienteSeleccionado = {
                //     id: sessionStorage.getItem("proveedorId"),
                //     CUIT: sessionStorage.getItem("cuit"),
                //     CodigoProveedor: sessionStorage.getItem("proveedor")
                // }
                // this.clienteCodigo = this.clienteSeleccionado.CodigoProveedor;
                // this.ordenDeCarga.CUITCliente = this.clienteSeleccionado.CUIT;
                this.cargarClienteDirecto(parseInt(sessionStorage.getItem("proveedorId") || ""))
                    .then(() => {
                        if (this.ordenDeCargaId == 0) {
                            this.cargarContratosDisponibles(this.clienteCodigo);
                        }
                    });
            }
        }
    }

    cambioProducto() {
        let productoActual = this.listaMateriales.find(x => x.MaterialId == this.ordenDeCarga.Producto_Id).CodigoSap;
        if (productoActual == "99709") {
            this.ordenDeCarga.Cantidad = 20000;
        } else {
            this.ordenDeCarga.Cantidad = 30000;
        }
    }

    obtenerMateriales() {
        //Sacamos lo de la lista de campaña, ya que ahora son independientes
        try {
            this.service.getMateriales().subscribe(
                (result) => {
                    this.listaMateriales = result.data;
                },
                (error) => {
                    console.error(error);
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            console.error(e);
            this.mensajeComponent.setErrorMsg(e);
        }
    }


    validar() {
        if (!this.ordenDeCarga.NombreChofer || this.ordenDeCarga.NombreChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el nombre del chofer.");
            return false;
        }
        /*VER ESTA VALIDACION, ACA VALIDA COMO SI FUERA UN CUIT PERO EN EL FRONT DICE QUE PONGA EL DNI/CUIL*/
        if (!this.ordenDeCarga.CUITChofer || this.ordenDeCarga.CUITChofer.toString().trim().length != 11 || this.mensajesOrdenDeCarga.CUITChofer || this.validando.CUITChofer) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIL de chofer válido.");
            return false;
        }
        if (!this.patenteAcopladoValida) {
            this.mensajeComponent.setInfoMsg("Ingrese una patente válida.");
            return false;
        }
        if (!this.chasisAcopladoValido) {
            this.mensajeComponent.setInfoMsg("Ingrese un número de chasis válido.");
            return false;
        }
        if (this.ordenDeCarga.ChasisAcoplado == this.ordenDeCarga.PatenteAcoplado) {
            this.mensajeComponent.setInfoMsg("Las patentes de chásis y acoplado no pueden ser iguales.");
            return false;
        }
        if (!this.ordenDeCarga.RazonSocialTransporte || this.ordenDeCarga.RazonSocialTransporte.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese la razón social del transporte.");
            return false;
        }
        if (!this.ordenDeCarga.CUITTransporte || this.ordenDeCarga.CUITTransporte.toString().trim().length != 11 || this.mensajesOrdenDeCarga.CUITTransporte || this.validando.CUITTransporte) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de transporte válido.");
            return false;
        }
        if (this.ordenDeCarga.CUITIntermediarioFlete && this.ordenDeCarga.CUITIntermediarioFlete.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de intermediario flete válido.");
            return false;
        }

        if (this.validaCPEDG) {
            if (this.ordenDeCarga.CUITDestinatario && !this.revisarCUITFormatoValido(this.ordenDeCarga.CUITDestinatario)) {
                this.mensajeComponent.setInfoMsg("Ingrese un CUIT de Destinatario válido.");
                return false;
            }

            if (!(this.ordenDeCarga.DomicilioDescr && this.ordenDeCarga.DomicilioTipo && this.ordenDeCarga.DomicilioOrden)) {
                this.mensajeComponent.setInfoMsg("Seleccione un domicilio.");
                return false;
            }
            if (!this.ordenDeCarga.PlantaCodigo) {
                this.mensajeComponent.setInfoMsg("Seleccione una planta.");
                return false;
            }
            const tieneMensajes = Object.keys(this.mensajesOrdenDeCarga).some(key => this.mensajesOrdenDeCarga[key])
            if (tieneMensajes) {
                this.mensajeComponent.setInfoMsg("Hay campos que no son válidos.");
                return false;
            }
            const estaValidando = Object.keys(this.validando).some(key => this.validando[key]);
            if (estaValidando) {
                this.mensajeComponent.setInfoMsg("Hay campos que todavía se están validando");
                return false;
            }
        }
        else {
            if (!this.ordenDeCarga.DestinoMercaderia || this.ordenDeCarga.DestinoMercaderia.length < 5) {
                this.mensajeComponent.setInfoMsg("Ingrese un destino de mercadería.");
                return false;
            }
        }

        if (!this.ordenDeCarga.Producto_Id) {
            this.mensajeComponent.setInfoMsg("Seleccione un contrato.");
            return false;
        }

        if (this.ordenDeCarga.ContratoSeleccionado.TipoContrato === TipoContrato.FacturaAnticipada) {
            if (!this.ordenDeCarga.NumeroFactura) {
                this.mensajeComponent.setInfoMsg("Debe seleccionar un número de factura para este tipo de contrato.");
                return false;
            }
            if (this.mensajesOrdenDeCarga.NumeroFacturaSeleccionada) {
                this.mensajeComponent.setInfoMsg(this.mensajesOrdenDeCarga.NumeroFacturaSeleccionada);
                return false;
            }
        }

        if (this.mensajesOrdenDeCarga.ContratoSeleccionado) {
            this.mensajeComponent.setInfoMsg(this.mensajesOrdenDeCarga.ContratoSeleccionado)
            return false;
        }

        return true;
    }

    obtenerOrdenDeCarga() {
        try {
            this.subscriptionDropDowns = this.service.getEditarOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(result.error);
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        console.info(result.info);
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCarga = result.data;
                        this.CodigoCorredor = result.data.Corredor || result.data.CodigoCorredor;
                        if (this.esComercial && result.data.ColorSemaforo != "green") {
                            this.puedeEditarContrato = true;
                        }
                        if (result.data.Estado == 3) {
                            this.puedeEditarContrato = true;
                        }
                        this.clienteCUIT = result.data.CUITCliente;
                        this.clienteCodigo = result.data.CodigoCliente;
                        if (this.CodigoCorredor) {
                            this.cargarClientes(this.CodigoCorredor);
                        } else {
                            this.onCorredorFocusOut('', true);
                        }
                        this.getPatentes();
                        this.cargarContratosDisponibles(result.data.CodigoCliente);
                        if (this.ordenDeCarga.CUITDestino)
                            this.onDestinoIngresado(this.ordenDeCarga.CUITDestino)
                    }
                },
                error => {
                    console.error(error);
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            console.error(e);
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    submit() {
        if (!this.Contrato) {
            this.ordenDeCarga.Producto_Id = this.selectUndefinedOptionValue;
        }

        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            this.messagesContainer.nativeElement.scrollIntoView({ behavior: 'smooth' })
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();

        try {
            if (this.ordenDeCargaId > 0) {
                if (this.esInterno) {
                    this.verificarOrdenActivaScato(this.ordenDeCargaId.toString());
                    this.abrirModalEdicionInterno();
                } else {
                    this.editarOrden();
                }
            } else {
                this.agregarOrden();
            }
        } catch (e) {
            console.error(e);
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    agregarOrden() {
        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        this.subscription = this.service
            .agregar(this.ordenDeCarga).pipe(take(1))
            .subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(result.error);
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        console.info(result.info);
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data.error != undefined && result.data.error != "") {
                        console.error(result.error);
                        this.mensajeComponent.setErrorMsg(result.data.error);
                    } else if (result.data.info != undefined) {
                        console.error(result.error);
                        this.mensajeComponent.setInfoMsg(result.data.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeSuccess = result.data.Mensaje;
                        this.ordenDeCargaId = result.data.IdEntidad;
                        this.gestionarAltasCuitTerceros(this.ordenDeCargaId.toString());
                        document.getElementById("openModalNotificacion")
                            .click();
                    }
                },
                (error) => {
                    console.error(error);
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
    }

    editarOrden() {

        document.getElementById("closemodalEditarOrden").click();

        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        this.subscription = this.service
            .editar(this.ordenDeCarga).pipe(take(1))
            .subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(result.error);
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        console.info(result.info);
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else if (result.data.error != undefined && result.data.error != "") {
                        console.error(result.data.error);
                        this.mensajeComponent.setErrorMsg(result.data.error);
                    } else if (result.data.info != undefined) {
                        console.info(result.data.info);
                        this.mensajeComponent.setInfoMsg(result.data.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        this.mensajeSuccess = result.data.Mensaje;
                        this.ordenDeCargaId = result.data.IdEntidad;

                        this.gestionarAltasCuitTerceros(this.ordenDeCargaId.toString());

                        document.getElementById("openModalNotificacion")
                            .click();
                    }
                },
                (error) => {
                    console.error(error);
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
    }

    aceptar() {
        document.getElementById("botonCerrarModal").click();
        this.redirigirADetalles();
    }

    redirigirADetalles() {
        this.goToSeccionParam('/ordenes-de-carga/detalle/', this.ordenDeCargaId.toString())
    }

    redirigirAListado() {
        this.navService.navegarSeccion(
            "/ordenes-de-carga"
        );
    }

    getPatentes() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.subscription = this.service.getPatentes(this.ordenDeCarga).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(' getPatentes: ', result.error);
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        console.info(' getPatentes: ', result.info);
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.patentesChasis = result.ordenes.map((patente) => {
                            return { label: patente.label, value: patente.label };
                        })

                        var flags = [], output = [], l = this.patentesChasis.length, i;
                        for (i = 0; i < l; i++) {
                            if (flags[this.patentesChasis[i].value]) continue;
                            flags[this.patentesChasis[i].value] = true;
                            output.push(this.patentesChasis[i]);
                        }
                        this.patentesChasis = output;

                        this.patentesAcoplados = result.ordenes.map((patente) => {
                            return { label: patente.value, value: patente.value };
                        })

                        flags = [], output = [], l = this.patentesAcoplados.length, i;
                        for (i = 0; i < l; i++) {
                            if (flags[this.patentesAcoplados[i].value]) continue;
                            flags[this.patentesAcoplados[i].value] = true;
                            output.push(this.patentesAcoplados[i]);
                        }
                        this.patentesAcoplados = output;
                    }
                },
                error => {
                    console.error(' getPatentes: ', error.message);
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (err) {
            console.error(' getPatentes: ', err);
            this.floatMsgService.setErrorMsg(err);
            return false; //<-- Prevent Refresh
        }
    }

    getCuilsChofer() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.subscription = this.service.getCuilsChofer(this.ordenDeCarga).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(' getCuilsChofer: ', result.error);
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        console.info(' getCuilsChofer: ', result.info);
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.cuilsChofer = result.cuils.map((cuil) => {
                            return { label: cuil.label, value: cuil.value };
                        })
                        this.getCuitsTransporte();
                    }
                },
                error => {
                    console.error(' getCuilsChofer: ', error.message);
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (err) {
            console.error(' getCuilsChofer: ', err);
            this.floatMsgService.setErrorMsg(err);
            return false; //<-- Prevent Refresh
        }
    }

    getCuitsTransporte() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.subscription = this.service.getCuitsTransporte(this.ordenDeCarga).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(' getCuitsTransporte: ', result.error);
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        console.info(' getCuitsTransporte: ', result.info);
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.cuitsTransporte = result.cuits.map((cuit) => {
                            return { label: cuit.label, value: cuit.value };
                        })
                    }
                },
                error => {
                    console.error(' getCuitsTransporte: ', error.message);
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (err) {
            console.error(' getCuitsTransporte: ', err);
            this.floatMsgService.setErrorMsg(err);
            return false; //<-- Prevent Refresh
        }
    }

    chasisAcopladoSelected(event: string | { value: string }) {
        if (typeof (event) === "string")
            this.ordenDeCarga.ChasisAcoplado = event.toUpperCase();
        else if (event.value)
            this.ordenDeCarga.ChasisAcoplado = event.value.toUpperCase();
    }
    patenteAcopladoSelected(event: string | { value: string }) {
        if (typeof (event) === "string")
            this.ordenDeCarga.PatenteAcoplado = event.toUpperCase();
        else if (event.value)
            this.ordenDeCarga.PatenteAcoplado = event.value.toUpperCase();
    }
    cuitChoferSelected(value: any) {
        this.ordenDeCarga.CUITChofer = value.value;
    }
    cuitTransporteSelected(value: any) {
        this.ordenDeCarga.CUITTransporte = value.value;
    }

    generalFormatter(data: any): string {
        return `${data['label']}`;
    }

    generalFormatterAcoplado(data: any): string {
        return `${data['value']}`;
    }

    getFecha = (meses: number, fecha: Date = new Date()) => {
        fecha.setMonth(fecha.getMonth() - meses);
        var anho = fecha.toLocaleString("default", { year: "numeric" });
        var mes = fecha.toLocaleString("default", { month: "2-digit" });
        var dia = fecha.toLocaleString("default", { day: "2-digit" });
        let stringFecha = anho + '-' + mes + '-' + dia;
        return stringFecha;
    }

    onCorredorSeleccionado = (proveedor: any) => {
        this.CodigoCorredor = this.corredorSeleccionado.idVendedor;
        this.obtenerCorredor(this.CodigoCorredor);

    }

    onCorredorFocusOut = (proveedor: any, edit: boolean) => {
        try {
            if (proveedor == '') {
                this.CodigoCorredor = '';
                this.ordenDeCarga.CUITCorredor = undefined;
                this.listaClientes = [];
                if (edit != true) {
                    this.clienteCUIT = this.selectUndefinedOptionValue;
                }
                this.clienteSeleccionado = null;
                this.mensajeComponent.setMsgsEmpty();
                this.loadingClientes = true;
                this.seleccionarProveedorService.getAllClientsByType(5).subscribe(
                    (result) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            console.error(' onCorredorFocusOut: ', result.error);
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            console.info(' onCorredorFocusOut: ', result.info);
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            this.listaClientes = this.ordenarYFiltrarClientes(result);
                            if (this.clienteCUIT != null && this.clienteCUIT.length > 0) {
                                this.clienteSeleccionado = this.listaClientes.find(x => x.CUIT == this.clienteCUIT);
                            }
                        }
                        this.loadingClientes = false;
                    },
                    (error) => {
                        console.error(' onCorredorFocusOut: ', error.message);
                        this.mensajeComponent.setErrorMsg(error.message);
                        this.loadingClientes = false;
                    }
                );
            }
        } catch (err) {
            console.error(' onCorredorFocusOut: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.loadingClientes = false;
        }
    }

    cargarClientes = (codigoCorredor: string) => {
        this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.loadingClientes = true;
            this.service.visualizarCliente(codigoCorredor, this.desde, this.hasta).subscribe(
                result => {
                    if (result.logout == true) {
                        this.blockUI.stop();
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(' cargarClientes: ', result.error);
                        this.CodigoCorredor = '';
                        this.listaClientes = [];
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.blockUI.stop();
                        this.onCorredorFocusOut('', false);
                    } else if (result.info != undefined) {
                        console.info(' cargarClientes; ', result.info);
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        this.listaClientes = this.ordenarYFiltrarClientes(result.Clientes);
                        if (this.clienteCUIT != null && this.clienteCUIT.length > 0) {
                            this.clienteSeleccionado = this.listaClientes.find(x => x.CUIT == this.clienteCUIT);
                        }
                        this.blockUI.stop();
                    }
                    this.loadingClientes = false;
                },
                error => {
                    console.error(' cargarClientes: ', error.message);
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.loadingClientes = false;
                    this.blockUI.stop();
                }
            );
        } catch (err) {
            console.error(' cargarClientes: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.loadingClientes = false;
            this.blockUI.stop();
        }
    }

    onClienteSeleccionado = () => {
        this.mensajeComponent.setMsgsEmpty();
        let pClienteCodigo: string = "";
        if (this.clienteSeleccionado) {
            if (this.clienteSeleccionado.Id <= 0) {
                // El cliente está relacionado al corredor en SAP, pero no existe en la BD
                this.mensajeComponent.setErrorMsg("El cliente seleccionado no existe en la web, por favor gestionar su alta");
                this.clienteCUIT = "";
                pClienteCodigo = "";
                this.scrollAMensaje();
            }
            else {
                this.clienteCUIT = this.clienteSeleccionado.CUIT;
                pClienteCodigo = this.clienteSeleccionado.CodigoProveedor;
            }
        }
        this.ordenDeCarga.CUITCliente = Number(this.clienteCUIT);
        this.getPatentes();
        this.cargarContratosDisponibles(pClienteCodigo);
        this.setearDefaultEnCPEDG();
    }

    onContratoSeleccionadoChanged = () => {
        this.facturasDisponibles = [];
        this.ordenDeCarga.NumeroFactura = null;
        this.facturaSeleccionada = null;
        if (this.ordenDeCarga.ContratoSeleccionado) {
            this.Contrato = this.ordenDeCarga.ContratoSeleccionado.NumeroContrato;
            this.ordenDeCarga.ContratoIngresado = this.ordenDeCarga.ContratoSeleccionado.NumeroContrato;
            this.ordenDeCarga.Producto_Id = this.ordenDeCarga.ContratoSeleccionado.Producto.MaterialId;
            this.Producto = this.ordenDeCarga.ContratoSeleccionado.Producto.MaterialId.toString();
            this.ordenDeCarga.TipoContrato = this.ordenDeCarga.ContratoSeleccionado.TipoContrato;
            let materialSeleccionado = this.listaMateriales.find(mat => mat.MaterialId === this.ordenDeCarga.Producto_Id);
            this.cambioProducto();
            this.validaCPEDG = (materialSeleccionado != undefined && materialSeleccionado.ValidaSisaRuca);
            this.validarKilosDisponibles()
            if (this.ordenDeCarga.ContratoSeleccionado.TipoContrato === TipoContrato.FacturaAnticipada)
                this.obtenerFacturas()
            this.validarSisaCorredorCliente()
        }
        else {
            this.Contrato = "";
            this.ordenDeCarga.ContratoIngresado = "";
            this.ordenDeCarga.Producto_Id = this.selectUndefinedOptionValue;
            this.validaCPEDG = false;
        }
        this.ordenDeCarga.Reventa = this.validaCPEDG && this.clienteSeleccionado.EsRevendedor && !this.ordenDeCarga.Reventa;
    }

    onPatenteSeleccionada() {
        this.getCuilsChofer();
    }

    validarCorredorClienteContratoProducto = (
        clienteCuit: string,
        clienteCodigo: string,
        contrato: string,
        codigoCorredor: string,
        productoId: string) => {
        this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.validarCorredorClienteContratoProducto(clienteCuit, clienteCodigo, contrato, codigoCorredor, this.userEmail, this.desde, this.hasta, productoId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        console.error(' validarCorredorClienteContratoProducto: ', result.error);
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        console.info(' validarCorredorClienteContratoProducto: ', result.info);
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        let resultValidacion = result.ResultValidation;
                        this.mensajeValidacionCorCliConPro = "";
                        if (resultValidacion == false) {
                            if (codigoCorredor) {
                                this.mensajeValidacionCorCliConPro = "Hubo un error en la validación corredor - cliente - contrato - producto";
                                console.error(this.mensajeValidacionCorCliConPro);
                                this.mensajeComponent.setErrorMsg(this.mensajeValidacionCorCliConPro);
                            }
                            else {
                                this.mensajeValidacionCorCliConPro = "Hubo un error en la validación cliente - contrato - producto";
                                console.error(this.mensajeValidacionCorCliConPro);
                                this.mensajeComponent.setErrorMsg(this.mensajeValidacionCorCliConPro);
                            }
                        }
                        this.resultadoValidacionCorCliConPro = resultValidacion;
                        this.blockUI.stop();
                    }
                },
                error => {
                    console.error(' validarCorredorClienteContratoProducto: ', error.message);
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (err) {
            console.error(' validarCorredorClienteContratoProducto: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }
    }

    ordenarYFiltrarClientes(result: any): any[] {
        result = result.filter((thing, i, arr) => {
            return arr.indexOf(arr.find(t => t.CodigoProveedor === thing.CodigoProveedor)) === i;
        });
        result.sort((a, b) => {
            const nameA = a.RazonSocial.toUpperCase(); // ignore upper and lowercase
            const nameB = b.RazonSocial.toUpperCase(); // ignore upper and lowercase
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        });
        result.forEach((cliente: any) => {
            if (cliente.CUIT) {
                // Clientes que existen en BD
                cliente.RazonSocial = cliente.RazonSocial + " (" + cliente.CUIT + ")";
            }
            else {
                // Clientes que existen en SAP pero no en BD
                cliente.RazonSocial = cliente.RazonSocial + " (" + cliente.CodigoProveedor + ")";
            }
        });
        return result;
    }

    ordenarYFiltrarCorredores(result: any): any[] {

        result = result.filter(
            (thing, i, arr) => arr.findIndex(t => t.idVendedor === thing.idVendedor) === i
        );

        result.sort((a, b) => {
            const nameA = a.descVendedor.toUpperCase(); // ignore upper and lowercase
            const nameB = b.descVendedor.toUpperCase(); // ignore upper and lowercase
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        });
        result.forEach((cliente: any) => {
            cliente.descVendedor = cliente.descVendedor + " (" + (cliente.cuit || '') + ")";
        });

        return result;
    }


    obtenerCorredor(CodigoCorredor: string) {
        try {
            this.seleccionarProveedorService.obtenerProveedorPorCodigo(CodigoCorredor).subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCarga.CUITCorredor = result.CUIT;
                        this.cargarClientes(this.corredorSeleccionado.idVendedor);
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (err) {
            this.mensajeComponent.setErrorMsg(err);
        }
    }

    gestionarAltasCuitTerceros(ordenId: string) {
        this.subscription = this.service.EnviarMailAltaCuitTerceros(this.gestiona["CUITIntermediarioFlete"],
            this.gestiona["CUITDestino"], this.gestiona["CUITDestinatario"], ordenId).subscribe(result => {
                if (result.logout) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(`${result.error}. Al intentar gestionar el alta de cuits`);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(`${result.info}. Al intentar gestionar alta de cuits`);
                } else {
                }
            }
            );
    }

    obtenerCorredores() {
        //this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        this.loadingCorredores = true;
        try {

            this.seleccionarProveedorService.getVendedores("", "", 4).subscribe(
                result => {
                    if (result.logout == true) {
                        //this.blockUI.stop();
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.CodigoCorredor = '';
                        this.listaClientes = [];
                        this.mensajeComponent.setErrorMsg(result.error);
                        //this.blockUI.stop();
                        this.onCorredorFocusOut('', false);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        //this.blockUI.stop();
                    } else {
                        this.listaCorredores = this.ordenarYFiltrarCorredores(result.data.vendedores);
                        if (this.CodigoCorredor != "") {
                            let seleccionado = result.data.vendedores.find(a => a.idVendedor == this.CodigoCorredor);
                            if (seleccionado)
                                this.corredorSeleccionado = seleccionado;

                        }
                        //this.blockUI.stop();
                    }
                    this.loadingCorredores = false;
                },
                error => {
                    console.error(' cargarClientes: ', error.message);
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.loadingCorredores = false;
                    //this.blockUI.stop();
                }
            );
        } catch (err) {
            console.error(' cargarClientes: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }

    }

    cargarContratosDisponibles(pClienteCodigo: string) {
        if (this.ordenDeCarga && this.ordenDeCarga.ContratoSeleccionado && this.ordenDeCarga.Id) {
            const { NumeroContrato, Producto, TipoContrato } = this.ordenDeCarga.ContratoSeleccionado;
            this.ordenDeCarga.ContratoSeleccionado = new ContratoOrdenFas(NumeroContrato, TipoContrato, Producto)
            this.contratosDisponibles = [this.ordenDeCarga.ContratoSeleccionado];
            this.onContratoSeleccionadoChanged();
            return;
        }
        this.contratosDisponibles = [];
        this.contratoSeleccionado = null as any;
        try {
            if (pClienteCodigo) {
                this.mensajeComponent.setMsgsEmpty();
                let pCorredorCodigo = this.corredorSeleccionado ?
                    this.corredorSeleccionado.idVendedor : "";

                this.blockUI.start('');
                this.service
                    .obtenerContratosDisponibles(pClienteCodigo, pCorredorCodigo, this.desde, this.hasta)
                    .subscribe(resp => {
                        if (resp.Logout) {
                            this.sessionDataService.logout();
                        } else
                            if (resp.Error) {
                                this.mensajeComponent.setErrorMsg(resp.Error);
                                this.messagesContainer.nativeElement.scrollIntoView({ behavior: 'smooth' })
                                this.reiniciarProducto();
                            } else
                                if (resp.Info) {
                                    this.mensajeComponent.setInfoMsg(resp.Info);
                                    this.reiniciarProducto();
                                } else {
                                    this.contratosDisponibles = resp.Contratos.map(c => {
                                        return new ContratoOrdenFas(c.NumeroContrato, c.TipoContrato, c.Producto, c.KgDisponibles);
                                    });
                                    this.ordenDeCarga.ContratoSeleccionado =
                                        this.ordenDeCarga.ContratoIngresado ?
                                            this.contratosDisponibles.find(c => c.NumeroContrato == this.ordenDeCarga.ContratoIngresado)
                                            : undefined;
                                    this.onContratoSeleccionadoChanged();
                                }
                        this.blockUI.stop();
                    })
            }
        } catch (err) {
            console.error('cargarContratosDisponibles: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }
    }

    validarExisteCUIT(campo: CuitValidaExistencia) {
        const cuit = this.ordenDeCarga[campo]
        if (!cuit || !this.revisarCUITFormatoValido(cuit)) {
            this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = undefined;
            return;
        }
        this.validando[campo] = true;
        this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = null;
        this.mensajesOrdenDeCarga[campo] = null;

        if (!this.ordenDeCarga.CUITDestinatario && campo === 'CUITDestino')
            this.copiarCuitEnDestinatario();

        this.service.validarExisteCuitScato(cuit).subscribe(
            result => {
                this.validando[campo] = false;
                const data = this.manejarErroresApiResponse(result);
                if (!data)
                    return;
                if (!data.Existe)
                    this.displayModal = campo;
                else {
                    this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = data.RazonSocial;
                    this.gestiona[campo] = false;
                }

            })
    }

    validarSisaCuit(cuit: string, campo: CuitValidaSISA) {
        if (!cuit || !this.revisarCUITFormatoValido(cuit)) {
            this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = undefined;
            return;
        }
        this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = null;
        this.mensajesOrdenDeCarga[campo] = null;
        this.validando[campo] = true;
        this.gestiona[campo] = false;

        if (!this.ordenDeCarga.CUITDestinatario && campo === 'CUITDestino')
            this.copiarCuitEnDestinatario();

        this.service.validarSisaCuit(cuit, campo).subscribe(result => {
            this.validando[campo] = false;
            const validandoDestino = campo == "CUITDestino";
            if (validandoDestino)
                this.resetearPlantasDomicilios()
            if (result.logout) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.info != undefined) {
                this.mensajeComponent.setInfoMsg(result.info);

            } else {
                if (!result.data) {
                    this.mensajesOrdenDeCarga[campo] = `El ${campo.replace("CUIT", "")} no se encuentra habilitado en SISA, no podrá cargar la orden hasta regularizar la situación.`;
                } else {
                    if (validandoDestino) {
                        this.onDestinoIngresado(cuit);
                    }
                    if (campo !== "CUITCorredor") {
                        this.validarRuca(cuit, campo)
                    }
                }
            }
        })
    }

    onDestinoIngresado(cuitDestino: string) {
        if (cuitDestino) {
            this.validando.CUITDestino = true;
            forkJoin([
                this.service
                    .obtenerPlantasDestino(cuitDestino),
                this.service
                    .obtenerDomiciliosDestino(cuitDestino)
            ]).pipe(finalize(() => this.validando.CUITDestino = false)).subscribe(([respPlantas, respDomicilios]) => {
                this.manejarRespuestaDomicilio(respDomicilios, cuitDestino)
                this.manejarRespuestaPlanta(respPlantas, cuitDestino)
            })
        }
        else {
            this.listaPlantas = [];
            this.listaDomicilios = [];
            this.plantaSeleccionada = undefined;
            this.domicilioSeleccionado = undefined;
        }
    }
    manejarRespuestaDomicilio(resp: ApiResponse<Domicilio[]>, cuitDestino: string) {
        let domicilios: Domicilio[] | null;
        if (domicilios = this.manejarErroresApiResponse(resp)) {
            if (domicilios.length > 0) {
                this.listaDomicilios = domicilios;
                this.definirValorDomicilio();
                return;
            }
            this.mensajeComponent.setErrorMsg(cuitDestino + " no está habilitado en RUCA, no podrá cargar la orden hasta regularizar la situación")
            this.scrollAMensaje()
        }
    }
    manejarRespuestaPlanta(resp: ApiResponse<Planta[]>, cuitDestino: string) {
        let plantas: Planta[] | null;
        if (plantas = this.manejarErroresApiResponse(resp)) {
            if (plantas.length > 0) {
                this.listaPlantas = plantas;
                this.definirValorPlanta();
                return;
            }
            this.mensajeComponent.setErrorMsg(cuitDestino + " no está habilitado en RUCA, no podrá cargar la orden hasta regularizar la situación")
            this.scrollAMensaje()
        }
    }
    revisarCUITFormatoValido(cuit: string): boolean {
        return cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number)
    }

    cargarAltaCuit() {
        const campo = this.displayModal;
        const razonSocial = this.razonSocialParaGestion;
        this.razonSocialParaGestion = "";
        this.gestiona[campo] = true;
        this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = razonSocial;
        this.displayModal = null;
        this.mensajesGestionCuit[campo] = `Se solicitará la gestión del alta para el cuit: ${this.ordenDeCarga[campo]}`;
    }

    cancelarGestionAltaCUIT() {
        this.ordenDeCarga[this.displayModal] = undefined
        this.gestiona[this.displayModal] = false;
        this.displayModal = null;
        this.mensajesGestionCuit[this.displayModal] = '';
    }

    validarSisaCorredorCliente() {

        if (!this.ordenDeCarga.ContratoSeleccionado || !this.validaCPEDG) {
            this.mensajeComponent.setMsgsEmpty();
            return;
        }
        let corredorCodigo = this.corredorSeleccionado ? this.corredorSeleccionado.idVendedor : "";
        let clienteCodigo = this.clienteSeleccionado ? this.clienteSeleccionado.CodigoProveedor : "";
        try {
            this.blockUI.start('');
            this.service
                .validarSisaCorredorCliente(corredorCodigo, clienteCodigo)
                .subscribe(svcRes => {
                    let resp = this.manejarErroresApiResponse(svcRes);
                    if (resp && (!resp.CorredorHabilitadoEnSisa || !resp.ClienteHabilitadoEnSisa)) {
                        let msj =
                            (!resp.CorredorHabilitadoEnSisa && !resp.ClienteHabilitadoEnSisa) ?
                                "Corredor y Cliente no están habilitados en SISA, no podrá cargar la orden hasta regularizar la situación" :
                                (!resp.CorredorHabilitadoEnSisa ?
                                    "Corredor no habilitado en SISA, no podrá cargar la orden hasta regularizar la situación" :
                                    "Cliente no habilitado en SISA, no podrá cargar la orden hasta regularizar la situación");
                        this.mensajeComponent.setErrorMsg(msj);
                        this.reiniciarProducto();
                        this.scrollAMensaje();
                    } else if (resp) {
                        this.validarRuca(this.ordenDeCarga.CUITCliente.toString(), "CUITCliente");
                    }
                    this.blockUI.stop();
                })
        } catch (err) {
            this.ordenDeCarga.Cantidad = 0;
            console.error('validarSisaCorredorCliente: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }
    }

    onPlantaSeleccionadaChanged() {
        if (this.plantaSeleccionada) {
            this.ordenDeCarga.PlantaCodigo = this.plantaSeleccionada.Codigo.toString();
        }
        else {
            this.ordenDeCarga.PlantaCodigo = "";
        }
    }

    onDomicilioSeleccionadoChanged() {
        if (this.domicilioSeleccionado) {
            this.ordenDeCarga.DomicilioTipo = this.domicilioSeleccionado.Tipo.toString();
            this.ordenDeCarga.DomicilioOrden = this.domicilioSeleccionado.Orden
            this.ordenDeCarga.DomicilioDescr = this.domicilioSeleccionado.Descripcion;
        }
        else {
            this.ordenDeCarga.DomicilioTipo = "";
            this.ordenDeCarga.DomicilioOrden = 0;
            this.ordenDeCarga.DomicilioDescr = "";
        }
    }

    validarRuca(cuit: string, campo: CuitValidaRUCA) {
        this.validando[campo] = true;
        this.mensajesOrdenDeCarga[campo] = null;
        this.service.validarCuitRuca(cuit).subscribe(result => {
            this.validando[campo] = false;
            let data = this.manejarErroresApiResponse(result);
            if (!data) {
                this.mensajesOrdenDeCarga[campo] = `${campo.replace("CUIT", "")}  no está habilitado en RUCA, no podrá cargar la orden hasta regularizar la situación`;

                if (campo === "CUITCliente")
                    this.reiniciarProducto();

            } else {
                if (campo !== "CUITCliente")
                    this.validarExisteCUIT(campo);
            }
        });
    }

    validarIntermediarioFlete() {
        this.validando.CUITIntermediarioFlete = true;
        this.intermediarioFleteCuitFormatoValido = true;
        this.ordenDeCarga.RazonSocialIntermediarioFlete = undefined;
        this.gestiona['CUITIntermediarioFlete'] = false;
        this.mensajesGestionCuit['CUITIntermediarioFlete'] = '';

        let cuitIF = this.ordenDeCarga.CUITIntermediarioFlete;
        if (!cuitIF || !this.revisarCUITFormatoValido(cuitIF)) {
            this.validando.CUITIntermediarioFlete = false;
            return;
        }
        this.service.validarIntermediarioFlete(cuitIF).subscribe(resp => {
            let data = this.manejarErroresApiResponse(resp);
            if (data) {
                if (data.EsCuitValido) {
                    if (data.ExisteIntermediario) {
                        this.ordenDeCarga.RazonSocialIntermediarioFlete = data.RazonSocial;
                        this.mensajesGestionCuit['CUITIntermediarioFlete'] = undefined;
                    } else {
                        this.displayModal = 'CUITIntermediarioFlete';
                    }
                } else {
                    this.intermediarioFleteCuitFormatoValido = false;
                }
            }
            this.validando.CUITIntermediarioFlete = false;
        });
    }

    reiniciarProducto() {
        this.ordenDeCarga.ContratoSeleccionado = undefined;
        this.ordenDeCarga.Producto_Id = 0;
        this.Producto = "";
        this.validaCPEDG = false;
    }

    copiarCuitEnDestinatario() {
        this.mensajesGestionCuit['CUITDestinatario'] = undefined;
        if (this.ordenDeCarga.CUITDestinatario != this.clienteSeleccionado.CUIT) {
            this.ordenDeCarga.CUITDestinatario = this.clienteSeleccionado.CUIT;
            this.validarSisaCuit(this.ordenDeCarga.CUITDestinatario, 'CUITDestinatario');
        }
    }

    copiarCuitEnDestino() {
        this.mensajesGestionCuit['CUITDestino'] = undefined;
        if (this.ordenDeCarga.CUITDestino != this.clienteSeleccionado.CUIT) {
            this.ordenDeCarga.CUITDestino = this.clienteSeleccionado.CUIT;
            this.validarSisaCuit(this.ordenDeCarga.CUITDestino, 'CUITDestino');
        }
    }

    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        if (response.logout) {
            this.sessionDataService.logout();
            return null;
        }
        if (response.error) {
            this.mensajeComponent.setErrorMsg(response.error);
            this.scrollAMensaje()
            return null;
        }
        if (response.info) {
            this.mensajeComponent.setInfoMsg(response.info)
        }
        return response.data;
    }
    validarCuilChofer() {
        const campo = "CUITChofer";
        const cuit = this.ordenDeCarga.CUITChofer ? this.ordenDeCarga.CUITChofer.toString() : "";
        if (!this.revisarCUITFormatoValido(cuit))
            return;
        if (this.mensajesOrdenDeCarga[campo])
            this.floatMsgService.setMsgsEmpty();
        this.validando[campo] = true;
        this.mensajesOrdenDeCarga[campo] = null;
        this.service.validarCuilChofer(cuit).subscribe(result => {
            this.validando[campo] = false;
            let data = this.manejarErroresApiResponse(result);
            if (!data && data != null) {
                this.mensajesOrdenDeCarga[campo] = "CUIL Chofer inválido – Revisar valor ingresado";
                this.floatMsgService.setInfoMsg("CUIL Chofer inválido – Revisar valor ingresado");
            }
        });
    }
    validarCuitTransporte() {
        const campo = "CUITTransporte";
        const cuit = this.ordenDeCarga.CUITTransporte ? this.ordenDeCarga.CUITTransporte.toString() : "";
        if (!this.revisarCUITFormatoValido(cuit))
            return;
        if (this.mensajesOrdenDeCarga[campo])
            this.floatMsgService.setMsgsEmpty();
        this.validando[campo] = true;
        this.mensajesOrdenDeCarga[campo] = null;
        this.service.validarCuitTransporte(cuit).subscribe(result => {
            this.validando[campo] = false;
            let data = this.manejarErroresApiResponse(result);
            if (!data && data != null) {
                this.mensajesOrdenDeCarga[campo] = "CUIT transporte inválido – Revisar valor ingresado";
                this.floatMsgService.setInfoMsg("CUIT transporte inválido – Revisar valor ingresado");
            }
        });
    }
    definirValorPlanta() {
        if (this.editando && this.listaPlantas)
            this.plantaSeleccionada = this.listaPlantas.find(planta => planta.Codigo.toString() == this.ordenDeCarga.PlantaCodigo);
    }
    definirValorDomicilio() {
        if (this.editando && this.listaDomicilios)
            this.domicilioSeleccionado = this.listaDomicilios.find(domicilio =>
                domicilio.Descripcion == this.ordenDeCarga.DomicilioDescr
                && domicilio.Orden == this.ordenDeCarga.DomicilioOrden
                && domicilio.Tipo.toString() == this.ordenDeCarga.DomicilioTipo
            );
    }

    descripcionIntermediarioFlete = "Llenar en caso que el transporte lo haga un tercero"
    descripcionTransporte = "CUIT transportista MOA"
    ttCopiarCuit = "Copiar CUIT del Cliente"

    scrollAMensaje() {
        this.messagesContainer.nativeElement.scrollIntoView({ behavior: 'smooth' })
    }

    setearDefaultEnCPEDG() {
        this.ordenDeCarga.Reventa = false;
        this.ordenDeCarga.CUITDestinatario = undefined;
        this.validarSisaCuit(this.clienteSeleccionado.cuit, 'CUITDestinatario');
        this.ordenDeCarga.CUITDestino = undefined;
        this.validarSisaCuit(this.clienteSeleccionado.cuit, 'CUITDestino');
        this.ordenDeCarga.CUITIntermediarioFlete = undefined;
        this.validarIntermediarioFlete();
        this.resetearPlantasDomicilios();
    }

    resetearPlantasDomicilios() {
        this.plantaSeleccionada = undefined;
        this.domicilioSeleccionado = undefined;
        this.listaPlantas = [];
        this.listaDomicilios = [];
        this.onDomicilioSeleccionadoChanged()
        this.onPlantaSeleccionadaChanged();
    }

    obtenerFacturas() {
        const numeroContrato = this.ordenDeCarga.ContratoSeleccionado.NumeroContrato;

        this.service.obtenerFacturasDeContrato(numeroContrato).subscribe(result => {
            let data = this.manejarErroresApiResponse(result);
            if (data instanceof Array) {
                if (!data.length) {
                    this.floatMsgService.setInfoMsg("No hay ninguna factura generada para este contrato.");
                    this.contratoSeleccionado = null;
                    this.ordenDeCarga.ContratoSeleccionado = null;
                    this.Contrato = "";
                    this.ordenDeCarga.ContratoIngresado = "";
                    this.ordenDeCarga.Producto_Id = this.selectUndefinedOptionValue;
                    this.validaCPEDG = false;
                }
                this.facturasDisponibles = data.map(newFactura)
                if (this.ordenDeCargaId) {
                    const numeroFacturaOrden = this.ordenDeCarga.NumeroFacturaSeleccionada || this.ordenDeCarga.NumeroFactura;
                    this.facturaSeleccionada = this.facturasDisponibles.find(factura => factura.NumeroFactura == numeroFacturaOrden)
                }
            }
        });
    }
    setNumeroFactura(value: Factura) {
        const { NumeroFactura, NumeroPedido } = value;
        this.ordenDeCarga.NumeroFactura = NumeroFactura;
        this.ordenDeCarga.NumeroPedidoIngresado = NumeroPedido;
        this.validarKilosDisponiblesPedido();
    }
    validarKilosDisponibles() {
        this.mensajesOrdenDeCarga.ContratoSeleccionado = null;
        const esInterno = (this.esComercial || this.esAdmin);

        if (this.ordenDeCargaId == 0 && !esInterno) {
            this.floatMsgService.setMsgsEmpty();
            if (this.ordenDeCarga.ContratoSeleccionado) {
                const { KgDisponibles } = this.ordenDeCarga.ContratoSeleccionado;
                if (KgDisponibles >= KILOS_DISPONIBLES_APROBADO)
                    return;

                const mensaje = "Contrato sin Kilos disponibles.";

                if (KgDisponibles <= SIN_KILOS_DISPONIBLES)
                    this.mensajesOrdenDeCarga.ContratoSeleccionado = mensaje;

                this.floatMsgService.setInfoMsg(mensaje)
            }
        }
    }
    validarKilosDisponiblesPedido() {
        this.mensajesOrdenDeCarga.NumeroFacturaSeleccionada = null;
        const esInterno = (this.esComercial || this.esAdmin);

        if (this.ordenDeCargaId == 0 && !esInterno) {
            this.floatMsgService.setMsgsEmpty();
            if (this.facturaSeleccionada) {
                const { KgDisponibles } = this.facturaSeleccionada;
                if (KgDisponibles >= KILOS_DISPONIBLES_APROBADO)
                    return;
                const mensaje = "Factura sin Kilos disponibles.";

                if (KgDisponibles <= SIN_KILOS_DISPONIBLES)
                    this.mensajesOrdenDeCarga.NumeroFacturaSeleccionada = mensaje;

                this.floatMsgService.setInfoMsg(mensaje)
            }
        }
    }

    cargarClienteDirecto(idCliente: Number): Promise<void> {
        return new Promise((resolve, reject) => {
            this.service.obtenerProveedor(idCliente).subscribe(resp => {
                let proveedor = this.manejarErroresApiResponse(resp);
                if (proveedor) {
                    this.clienteSeleccionado = proveedor;
                    this.clienteCodigo = proveedor.CodigoProveedor;
                    this.ordenDeCarga.CUITCliente = this.clienteSeleccionado.CUIT;
                    resolve();
                }
            });
        });
    }
    get patenteAcopladoValida() {
        return this.ordenDeCarga.PatenteAcoplado && this.ordenDeCarga.PatenteAcoplado.trim().length >= 6
    }
    get chasisAcopladoValido() {
        return this.ordenDeCarga.ChasisAcoplado && this.ordenDeCarga.ChasisAcoplado.trim().length >= 6
    }

    displayModalEscalable = false;
    decidioEscalable = false;

    validarCNRT() {
        if (!(this.patenteAcopladoValida && this.chasisAcopladoValido))
            return;
        //Posible check de si está marcado el campo escalable
        this.validarCNRTSubject.next();
    }
    validarCNRTRequest() {
        this.validando.Escalable = true;
        this.validarCNRTSubscription = this.service
            .verificarCNRT(this.ordenDeCarga.ChasisAcoplado, this.ordenDeCarga.PatenteAcoplado)
            .subscribe(res => {
                this.validando.Escalable = false;
                const validezCNRTResponse = this.manejarErroresApiResponse(res)
                this.errorAlValidarEscalable = !!(res.error || res.info);
                if (this.errorAlValidarEscalable || !validezCNRTResponse)
                    return;
                if (!validezCNRTResponse.ExisteCamion) {
                    this.msgService.add(MSG_ALERTA_CAMION_NO_EXISTE);
                }
                else {
                    this.escalableCNRT = validezCNRTResponse.EsCamionEscalable;
                    if (!this.escalableCNRT && this.ordenDeCarga.Escalable) {
                        this.msgService.add(MSG_ALERTA_NO_ESCALABLE);
                        this.setValorEscalable()
                    }
                    this.displayModalEscalable = this.escalableCNRT && !this.ordenDeCarga.Escalable && !this.decidioEscalable;
                }
            })
    }
    validarEscalable() {
        if (!this.ordenDeCarga.Escalable)
            return;

        if (!this.escalableCNRT && this.escalableCNRT !== undefined) {
            this.msgService.add(MSG_ALERTA_NO_ESCALABLE);
            this.setValorEscalable()
        }
        else if (this.escalableCNRT === undefined)
            this.validarCNRT();
    }
    setValorEscalable(value = false) {
        this.escalableCheckbox.writeValue(value)
        this.ordenDeCarga.Escalable = value;
    }
    marcarComoEscalable() {
        this.setValorEscalable(true)
        this.displayModalEscalable = false;
    }
    dejarSinEscalable() {
        this.displayModalEscalable = false;
    }
    public extraOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    verificarOrdenActivaScato(ordenId: string) {
        try {
            this.service.validarOrdenActivaScato(ordenId).pipe(
                finalize(() => { })
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
                        } else {
                            this.mensajeValidacionScato = undefined;
                        }
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeValidacionScato = "No se pudo validar si la orden esta activa en Scato."
        }
    }

    abrirModalEdicionInterno() {
        document.getElementById("openEdicionOrdenInterno").click();
    }
}
