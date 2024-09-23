import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService, MessageService } from 'primeng/api';
import { CANTIDAD_DEFAULT, CANTIDAD_PELLET_GIRASOL, CODIGO_PELLET_GIRASOL_INTEGRAL, Material, RETIRO_EN_PATAGONIA } from '../../common/models/material';
import { OrdenDeCargaFasonDto } from '../../common/models/ordenes-de-carga-fason/ordenDeCargaFasonDto';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SeleccionarProveedorService } from '../../common/shared-components/seleccionar-proveedor/seleccionar-proveedor.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { setupDaysAndMonths, sumarDias } from '../orden-carga-fason-utils';
import { OrdenesDeCargaFasonService } from '../ordenes-de-carga-fason.service';
import { ApiResponse } from '../../common/models/response';
import { IOrdenesBaseComponent, OrdenesBaseComponent } from '../../common/base-components/ordenes-base-component';
import { Permiso } from '../../common/enums/Permisos';
import { finalize } from 'rxjs/operators';
import { Subscription, forkJoin } from 'rxjs';
import { Domicilio } from '../../common/models/ordenes-de-carga/domicilio';
import { Planta } from '../../common/models/ordenes-de-carga/planta';
import { MSG_ALERTA_CAMION_NO_EXISTE, MSG_ALERTA_NO_ESCALABLE } from '../../common/models/ordenes-de-carga/ValidarCamionResponse';
import { Checkbox } from 'primeng/checkbox';
import { EstadoOrdenDeCargaFason } from '../../common/models/ordenes-de-carga-fason/estadoOrdenDeCargaFason';

@Component({
    selector: 'app-alta',
    templateUrl: './ordenes-de-carga-fason.alta.component.html',
    styleUrls: ['./ordenes-de-carga-fason.alta.component.css'],
    providers: [SeleccionarProveedorService],
})
export class OrdenesDeCargaFasonAltaComponent
    extends OrdenesBaseComponent
    implements OnInit, IOrdenesBaseComponent {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild("escalableCheckbox") escalableCheckbox: Checkbox
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    @ViewChild('messages')
    private messagesContainer?: ElementRef<HTMLDivElement>;

    constructor(protected service: OrdenesDeCargaFasonService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        private confirmationService: ConfirmationService, private route: ActivatedRoute,
        protected seleccionarProveedorService: SeleccionarProveedorService,
        protected msgService: MessageService
    ) {
        super(service, navService, securityService, floatMsgService, modalService);
    }

    listaProductos: Material[];
    listaClientes: any[];
    listaCorredores: any[];

    clienteSeleccionado: any = "";
    corredorSeleccionado: any;
    gestiona: Partial<Record<keyof Pick<OrdenDeCargaFasonDto, 'CUITDestinatario' | 'CUITDestino' | 'CUITIntermediarioFlete'>, boolean>> = {
        CUITDestino: false,
        CUITDestinatario: false, CUITIntermediarioFlete: false
    };

    patentesChasis: any = [];
    patentesAcoplados: any = [];

    clienteCUIT: string = "";
    clienteCodigo: string = "";
    CodigoCorredor: string = "";
    Contrato: string = "";
    Producto: string = "";
    desde: string = "";
    hasta: string = "";
    userEmail: string = "";

    mensajeError: string = "";
    mensajeSuccess: string = "";

    es: any;

    esAdmin: boolean = this.isAuthorized(Permiso.FasonVerOrdenesDeCargaAdmin);
    modificaFleteMOA: boolean = this.isAuthorized(Permiso.FleteMOA);

    ordenDeCargaFason: OrdenDeCargaFasonDto = new OrdenDeCargaFasonDto();
    ordenDeCargaFasonId: number = 0;

    maxDateValue = sumarDias(new Date(), 5)


    hoy: Date = new Date();

    private selectUndefinedOptionValue: any;

    public nombreChofer: string;
    validaCPEDG = false;

    validandoCuitDestinatario: boolean = false;
    validandoCuitDestino: boolean = false;
    mensajeCuitDestinatario: string = "";
    mensajeCuitDestino: string = "";
    editando = false;

    cuitsTransporte: any = [];
    cuilsChofer: any = [];

    subscriptions = new Subscription();
    escalableCNRT?: boolean;
    errorAlValidarEscalable = false;

    get noPuedeEditarCuitsTerceros() {
        return this.ordenDeCargaFason.Id && this.ordenDeCargaFason.Estado == EstadoOrdenDeCargaFason.Entregada;
    }
    generalFormatter(data: any): string {
        return `${data['label']}`;
    }

    generalFormatterAcoplado(data: any): string {
        return `${data['value']}`;
    }
    ngOnInit() {
        this.userEmail = sessionStorage.getItem("username");
        this.ordenDeCargaFason.Cantidad = 30000;
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) { this.ordenDeCargaFasonId = params["id"]; this.editando = true }
            else {
                this.getPatentes();
            }
        });

        this.navService.setSeccionList([]);
        this.obtenerProductos();

        if (this.isAuthorized(Permiso.FasonVerOrdenesDeCargaAdmin)) {
            this.ordenDeCargaFason.CUITCliente = 0;
        }
        this.ordenDeCargaFason.FleteMOA = this.modificaFleteMOA;
        this.es = setupDaysAndMonths();

        if (!this.esAdmin) {
            if (this.isCorredor()) {
                this.CodigoCorredor = sessionStorage.getItem("proveedor");
                this.ordenDeCargaFason.CodigoCorredor = this.CodigoCorredor;
                this.cargarClientes(this.CodigoCorredor);
            } else {
                if (this.esCliente()) {
                    this.cargarClienteDirecto(parseInt(sessionStorage.getItem("proveedorId") || ""))
                }
            }
        }

        if (this.ordenDeCargaFasonId > 0) {
            this.obtenerOrdenDeCarga();
        } else if (this.esAdmin) {
            this.obtenerCorredores();
            this.cargarClientes('');
        }
    }

    aceptar() {
        document.getElementById("botonCerrarModal").click();
        this.redirigirAListado();
    }

    redirigirAListado() {
        this.navService.navegarSeccion(
            "/ordenes-de-carga-fason"
        );
    }

    //Validaciones
    validar() {
        if (this.ordenDeCargaFason.NombreChofer == undefined || this.ordenDeCargaFason.NombreChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el nombre del chofer.");
            return false;
        }
        if (this.ordenDeCargaFason.ApellidoChofer == undefined || this.ordenDeCargaFason.ApellidoChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el apellido del chofer.");
            return false;
        }
        /*VER ESTA VALIDACION, ACA VALIDA COMO SI FUERA UN CUIT PERO EN EL FRONT DICE QUE PONGA EL DNI/CUIL*/
        if (
            this.ordenDeCargaFason.CUILChofer == undefined || this.ordenDeCargaFason.CUILChofer.toString().trim().length != 11
            || this.mensajesOrdenDeCargaFason.CUILChofer
        ) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIL de chofer válido.");
            return false;
        }
        if (this.ordenDeCargaFason.PatenteAcoplado == undefined || this.ordenDeCargaFason.PatenteAcoplado.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese una patente válida.");
            return false;
        }
        if (this.ordenDeCargaFason.PatenteChasis == undefined || this.ordenDeCargaFason.PatenteChasis.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese un número de chasis válido.");
            return false;
        }
        if (this.ordenDeCargaFason.PatenteAcoplado == this.ordenDeCargaFason.PatenteChasis) {
            this.mensajeComponent.setInfoMsg("Los números de patente no pueden ser iguales.");
            return false;
        }
        if (this.ordenDeCargaFason.RazonSocialTransporte == undefined || this.ordenDeCargaFason.RazonSocialTransporte.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese la razón social del transporte.");
            return false;
        }
        if (this.ordenDeCargaFason.CUITTransporte == undefined || this.ordenDeCargaFason.CUITTransporte.toString().trim().length != 11
            || this.mensajesOrdenDeCargaFason.CUITTransporte
        ) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de transporte válido.");
            return false;
        }
        if (this.esAdmin && !this.clienteSeleccionado) {
            this.mensajeComponent.setInfoMsg("Seleccione un cliente.");
            return false;
        }
        if (!this.ordenDeCargaFason.Producto_Id) {
            // Mostrar un mensaje de error al usuario o hacer algo para indicar que es necesario seleccionar un corredor
            this.mensajeComponent.setInfoMsg("Seleccione un producto.");
            return false;
        }
        if (!this.ordenDeCargaFason.CantidadDeViajes && !this.ordenDeCargaFasonId) {
            this.mensajeComponent.setInfoMsg("Ingrese una cantidad de viajes.");
            return false;
        }
        if (this.ordenDeCargaFason.CUITIntermediarioFlete && this.mensajesOrdenDeCargaFason.CUITIntermediarioFlete) {
            this.mensajeComponent.setInfoMsg(this.mensajesOrdenDeCargaFason.CUITIntermediarioFlete);
            return false;
        }
        if (this.validaCPEDG) {
            if (this.validandoCliente || this.validandoCuitDestinatario || this.validandoCuitDestino || Object.keys(this.validando).some(v => this.validando[v])) {
                this.mensajeComponent.setInfoMsg("Hay validaciones pendientes. Intente nuevamente en unos segundos.");
                return false;
            }
            if (!this.ordenDeCargaFason.CUITDestinatario || this.mensajeCuitDestinatario) {
                this.mensajeComponent.setInfoMsg(this.mensajeCuitDestinatario || "Debe ingresar un CUIT de destinatario para este producto.")
                return false;
            }
            if (!this.ordenDeCargaFason.CUITDestino || this.mensajeCuitDestino) {
                this.mensajeComponent.setInfoMsg(this.mensajeCuitDestino || "Debe ingresar un CUIT de destino para este producto.")
                return false;
            }
            if (!this.ordenDeCargaFason.PlantaCodigo) {
                this.mensajeComponent.setInfoMsg(this.mensajeCuitDestino || "Debe seleccionar una plante para este tipo de material.")
                return false;
            }
            if (!this.ordenDeCargaFason.DomicilioTipo || !this.ordenDeCargaFason.DomicilioDescr || !this.ordenDeCargaFason.DomicilioOrden) {
                this.mensajeComponent.setInfoMsg(this.mensajeCuitDestino || "Debe seleccionar un domicilio para este tipo de material.")
                return false;
            }
        } else {
            if (!this.ordenDeCargaFason.DestinoMercaderia || this.ordenDeCargaFason.DestinoMercaderia.length < 5) {
                this.mensajeComponent.setInfoMsg("Ingrese un destino de mercadería.");
                return false;
            }
        }

        return true;
    }

    //Changes
    onCorredorSeleccionado = (proveedor: any) => {
        this.CodigoCorredor = this.corredorSeleccionado.CodigoProveedor;
        this.ordenDeCargaFason.CUITCorredor = this.corredorSeleccionado.CUIT;
        this.ordenDeCargaFason.CorredorId = this.corredorSeleccionado.Id;
        this.ordenDeCargaFason.Corredor = this.corredorSeleccionado.CodigoProveedor;
        this.cargarClientes(this.CodigoCorredor);
    }

    onClienteSeleccionado = () => {
        if (this.clienteSeleccionado) {
            this.clienteCUIT = this.clienteSeleccionado.CUIT;
            this.ordenDeCargaFason.CUITCliente = Number(this.clienteCUIT);
            this.ordenDeCargaFason.Cliente = this.clienteSeleccionado.Id;
        }
        this.getCuilsChofer();
        this.getCuitsTransporte();
        if (this.ordenDeCargaFason.ProductoSeleccionado && this.ordenDeCargaFason.ProductoSeleccionado.ValidaSisaRuca) {
            this.validarSisaCliente();
        }
        if (!this.editando) {
            this.validacionExistenciaPatente$.next()
        }
    }

    //Utils
    ordenarYFiltrarClientes(result: any): any[] {
        result = result.filter((thing, i, arr) => {
            return arr.indexOf(arr.find(t => t.CUIT === thing.CUIT)) === i;
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
            cliente.RazonSocial = cliente.RazonSocial + " (" + cliente.CUIT + ")";
        });
        return result;
    }

    ordenarYFiltrarCorredores(result: any): any[] {
        result = result.filter(
            (thing, i, arr) => arr.findIndex(t => t.CUIT === thing.CUIT) === i
        );

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
            cliente.RazonSocial = cliente.RazonSocial + " (" + cliente.CUIT + ")";
        });

        return result;
    }

    //Servicios
    guardarOrdenDeCargaFason() {
        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Grabando...');
        try {
            if (this.ordenDeCargaFasonId > 0) {
                this.subscription = this.service
                    .editar(this.ordenDeCargaFason)
                    .subscribe(
                        (result) => {
                            this.spinnerComponent.hideIt();
                            this.blockUI.stop();
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
                                this.mensajeComponent.setMsgsEmpty();
                                this.mensajeSuccess = result.data.Mensaje;
                                this.ordenDeCargaFasonId = result.data.IdEntidad;
                                document.getElementById("openModalNotificacion")
                                    .click();
                            }
                        },
                        (error) => {
                            this.spinnerComponent.hideIt();
                            this.mensajeComponent.setErrorMsg(error.message);
                            this.blockUI.stop();
                        }
                    );
            } else {
                this.subscription = this.service
                    .agregar(this.ordenDeCargaFason)
                    .subscribe(
                        (result) => {
                            this.spinnerComponent.hideIt();
                            this.blockUI.stop();
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
                                this.mensajeComponent.setMsgsEmpty();
                                this.mensajeSuccess = result.data.Mensaje;
                                this.ordenDeCargaFasonId = result.data.IdEntidad;
                                this.gestionarAltasCuitTerceros(this.ordenDeCargaFasonId.toString());
                                document.getElementById("openModalNotificacion")
                                    .click();
                            }
                        },
                        (error) => {
                            this.spinnerComponent.hideIt();
                            this.mensajeComponent.setErrorMsg(error.message);
                            this.blockUI.stop();
                        }
                    );
            }
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }


    obtenerOrdenDeCarga() {
        try {
            const obtenerOrdenSubscripcion = this.service.getOrdenDeCargaFason(this.ordenDeCargaFasonId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {

                        this.ordenDeCargaFason = result.data.Response;
                        this.clienteCodigo = result.data.Response.Cliente;
                        this.CodigoCorredor = result.data.Response.Corredor;
                        if (this.ordenDeCargaFason.CorredorId) {
                            this.obtenerCorredores();
                        } else {
                            this.obtenerCorredores();
                            this.cargarClientes('');
                        }

                        const producto = this.listaProductos.find(producto => producto.MaterialId == this.ordenDeCargaFason.Producto_Id)
                        this.selectProducto(producto);
                        if (this.ordenDeCargaFason.CUITDestino)
                            this.onDestinoIngresado(this.ordenDeCargaFason.CUITDestino)

                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
            this.localSubscriptions.add(obtenerOrdenSubscripcion)
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }


    obtenerProductos = () => {
        this.service.getMateriales().subscribe(
            (result) => {
                this.listaProductos = result.data;
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    obtenerCorredores() {
        //this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        try {

            this.service.obtenerCorredores().subscribe(
                result => {
                    if (result.logout == true) {
                        //this.blockUI.stop();
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.CodigoCorredor = '';
                        this.listaClientes = [];
                        this.mensajeComponent.setErrorMsg(result.error);
                        //this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        //this.blockUI.stop();
                    } else {
                        this.listaCorredores = this.ordenarYFiltrarCorredores(result.corredores);
                        if (this.CodigoCorredor != "") {
                            let seleccionado = this.listaCorredores.filter(a => a.CodigoProveedor == this.CodigoCorredor);
                            if (seleccionado != null && seleccionado.length > 0) {

                                this.corredorSeleccionado = seleccionado[0];
                                this.ordenDeCargaFason.CUITCorredor = this.corredorSeleccionado.CUIT;
                                this.ordenDeCargaFason.CorredorId = this.corredorSeleccionado.Id;
                                this.cargarClientes(this.corredorSeleccionado.CodigoProveedor);
                            }
                        }
                        //this.blockUI.stop();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    //this.blockUI.stop();
                }
            );
        } catch (err) {
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }
    }

    cargarClientes = (codigoCorredor: string) => {
        this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.getClientes(codigoCorredor).subscribe(
                result => {
                    if (result.logout == true) {
                        this.blockUI.stop();
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.CodigoCorredor = '';
                        this.listaClientes = [];
                        this.mensajeComponent.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.listaClientes = [];
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        this.listaClientes = this.ordenarYFiltrarClientes(result.clientes);
                        if (this.clienteCUIT != null && this.clienteCUIT.length > 0) {
                            this.clienteSeleccionado = this.listaClientes.find(x => x.CUIT == this.clienteCUIT);
                            this.clienteCUIT = this.clienteSeleccionado.CUIT;
                            this.ordenDeCargaFason.CUITCliente = Number(this.clienteCUIT);
                            this.ordenDeCargaFason.Cliente = this.clienteSeleccionado.Id;

                        } else {
                            if (this.ordenDeCargaFason.Cliente != null && this.ordenDeCargaFason.Cliente != "") {
                                this.clienteSeleccionado = this.listaClientes.find(x => x.CodigoProveedor == this.ordenDeCargaFason.Cliente);
                                this.clienteCUIT = this.clienteSeleccionado.CUIT;
                                this.ordenDeCargaFason.CUITCliente = Number(this.clienteCUIT);
                                this.ordenDeCargaFason.Cliente = this.clienteSeleccionado.Id;
                            }
                        }
                        this.blockUI.stop();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (err) {
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }
    }

    seRetiraEnPatagonia(material: Material): boolean {
        return RETIRO_EN_PATAGONIA.includes(material.CodigoSap);
    }
    validarIntermediarioFlete() {
        this.validando.CUITIntermediarioFlete = true;
        this.mensajesOrdenDeCargaFason.CUITIntermediarioFlete = null;
        this.ordenDeCargaFason.RazonSocialIntermediarioFlete = null;

        let cuitIF = this.ordenDeCargaFason.CUITIntermediarioFlete;
        if (!cuitIF || !this.revisarCUITFormatoValido(cuitIF)) {
            this.validando.CUITIntermediarioFlete = false;
            return;
        }
        this.service.validarIntermediarioFlete(cuitIF).subscribe(resp => {
            let data = this.manejarErroresApiResponse(resp);
            if (data) {
                if (data.EsCuitValido) {
                    if (data.ExisteIntermediario) {
                        this.ordenDeCargaFason.RazonSocialIntermediarioFlete = data.RazonSocial;
                    } else {
                        this.displayModal = 'CUITIntermediarioFlete';
                    }
                } else {
                    this.mensajesOrdenDeCargaFason.CUITIntermediarioFlete = "CUIT Invalido";
                }
            }
            this.validando.CUITIntermediarioFlete = false;
        });
    }
    gestionarAltaCUIT() {
        const campo = this.displayModal;
        const cuit = this.ordenDeCargaFason[campo];
        const razonSocial = this.razonSocialParaGestion;
        this.displayModal = null;
        this.razonSocialParaGestion = "";
        this.ordenDeCargaFason[campo.replace("CUIT", "RazonSocial")] = razonSocial;
        let esIntermediarioFlete = campo === 'CUITIntermediarioFlete';
        this.service.enviarMailGestionarAltaCuit(cuit, razonSocial, esIntermediarioFlete).subscribe(result => {
            if (result.logout) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(`${result.error}. Al intentar gestionar alta CUIT ${campo.replace("CUIT", "")}`);
            } else if (result.info != undefined) {
                this.mensajeComponent.setInfoMsg(`${result.info}. Al intentar gestionar alta CUIT ${campo.replace("CUIT", "")}`);
            } else {
                this.mensajesGestionCuit[campo] = `Se solicitó la gestión del alta para la cuit: ${cuit}`;
            }

        })
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

    scrollAMensaje() {
        this.messagesContainer.nativeElement.scrollIntoView({ behavior: 'smooth' })
    }
    cancelarGestionAltaCUIT() {
        if (this.displayModal === 'CUITIntermediarioFlete') {
            this.ordenDeCargaFason.CUITIntermediarioFlete = undefined;
        }
        this.displayModal = null;
    }

    selectProducto(producto?: Material) {
        if (!this.ordenDeCargaFason.ProductoSeleccionado && !producto) {
            this.validaCPEDG = false;
            this.setearDefaultEnCPEDG();
            return;
        }

        if (producto) {
            this.ordenDeCargaFason.ProductoSeleccionado = producto;
        }
        this.ordenDeCargaFason.Producto_Id = this.ordenDeCargaFason.ProductoSeleccionado.MaterialId;
        this.validaCPEDG = this.ordenDeCargaFason.ProductoSeleccionado.ValidaSisaRuca;

        if (!this.validaCPEDG)
            this.setearDefaultEnCPEDG();
        else
            this.validarSisaCliente();
        this.setCantidadCambioProducto();
    }
    setCantidadCambioProducto() {
        this.ordenDeCargaFason.Cantidad =
            this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap == CODIGO_PELLET_GIRASOL_INTEGRAL ? CANTIDAD_PELLET_GIRASOL : CANTIDAD_DEFAULT;
    }
    setearDefaultEnCPEDG() {
        this.ordenDeCargaFason.RemitenteComercial = false;
        this.ordenDeCargaFason.CUITIntermediarioFlete = null;
        this.ordenDeCargaFason.RazonSocialIntermediarioFlete = null;
        this.validarIntermediarioFlete();
        this.resetearPlantasDomicilios();
    }
    onPlantaSeleccionadaChanged() {
        if (this.plantaSeleccionada) {
            this.ordenDeCargaFason.PlantaCodigo = this.plantaSeleccionada.Codigo.toString();
        }
        else {
            this.ordenDeCargaFason.PlantaCodigo = "";
        }
    }

    onDomicilioSeleccionadoChanged() {
        if (this.domicilioSeleccionado) {
            this.ordenDeCargaFason.DomicilioTipo = this.domicilioSeleccionado.Tipo.toString();
            this.ordenDeCargaFason.DomicilioOrden = this.domicilioSeleccionado.Orden
            this.ordenDeCargaFason.DomicilioDescr = this.domicilioSeleccionado.Descripcion;
        }
        else {
            this.ordenDeCargaFason.DomicilioTipo = "";
            this.ordenDeCargaFason.DomicilioOrden = 0;
            this.ordenDeCargaFason.DomicilioDescr = "";
        }
    }

    definirValorPlanta() {
        if (this.ordenDeCargaFasonId && this.listaPlantas)
            this.plantaSeleccionada = this.listaPlantas.find(planta => planta.Codigo.toString() == this.ordenDeCargaFason.PlantaCodigo);
    }
    definirValorDomicilio() {
        if (this.ordenDeCargaFasonId && this.listaDomicilios)
            this.domicilioSeleccionado = this.listaDomicilios.find(domicilio =>
                domicilio.Descripcion == this.ordenDeCargaFason.DomicilioDescr
                && domicilio.Orden == this.ordenDeCargaFason.DomicilioOrden
                && domicilio.Tipo.toString() == this.ordenDeCargaFason.DomicilioTipo
            );
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
    copiarCuitClienteEn(campo: "CUITDestinatario" | "CUITDestino") {
        this.mensajesGestionCuit[campo] = undefined;
        if (this.ordenDeCargaFason[campo] != this.clienteSeleccionado.CUIT) {
            this.ordenDeCargaFason[campo] = this.clienteSeleccionado.CUIT;
            if (campo === "CUITDestinatario")
                this.validarSisaDestinatario();
            else if (campo === "CUITDestino")
                this.validarSisaDestino();
        }
    }
    resetearPlantasDomicilios() {
        this.plantaSeleccionada = undefined;
        this.domicilioSeleccionado = undefined;
        this.listaPlantas = [];
        this.listaDomicilios = [];
        this.onDomicilioSeleccionadoChanged()
        this.onPlantaSeleccionadaChanged();
    }

    validarCuitDestinatarioExiste() {
        const cuit = this.ordenDeCargaFason.CUITDestinatario;
        this.validandoCuitDestinatario = true;
        this.ordenDeCargaFason.RazonSocialDestinatario = undefined;

        this.service.validarExisteCuitScato(cuit).subscribe(
            result => {
                this.validandoCuitDestinatario = false;
                const data = this.manejarErroresApiResponse(result);
                if (!data)
                    return;
                if (!data.Existe) {
                    this.displayModal = 'CUITDestinatario';
                }
                else {
                    this.ordenDeCargaFason.RazonSocialDestinatario = data.RazonSocial;
                }
            })
    }

    validarCuitDestinoExiste() {
        const cuit = this.ordenDeCargaFason.CUITDestino;

        this.validandoCuitDestino = true;

        this.service.validarExisteCuitScato(cuit).subscribe(
            result => {
                this.validandoCuitDestino = false;
                const data = this.manejarErroresApiResponse(result);
                if (!data)
                    return;
                if (!data.Existe) {
                    this.displayModal = 'CUITDestino';
                }
                else {
                    this.ordenDeCargaFason.RazonSocialDestino = data.RazonSocial;
                }

                this.asignarRemitenteComercial();
            })
    }

    validarSisaDestinatario() {
        const cuit = this.ordenDeCargaFason.CUITDestinatario;
        if (!cuit || !this.revisarCUITFormatoValido(cuit)) {
            this.ordenDeCargaFason.RazonSocialDestinatario = undefined;
            return;
        }
        this.validandoCuitDestinatario = true;
        this.ordenDeCargaFason.RazonSocialDestinatario = undefined;
        this.mensajeCuitDestinatario = "";

        this.service.validarSisaCuit(cuit, "", this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap).subscribe(
            result => {
                this.validandoCuitDestinatario = false;
                const esValidoSisa = this.manejarErroresApiResponse(result);
                if (!esValidoSisa) {
                    this.mensajeCuitDestinatario = "El CUIT destinatario no está habilitado en SISA, no podrá cargar la orden hasta regularizar la situación";
                }
                else {
                    this.validarRucaDestinatario(cuit);
                }
            }
        )
    }

    validarSisaDestino() {
        const cuit = this.ordenDeCargaFason.CUITDestino;
        if (!cuit || !this.revisarCUITFormatoValido(cuit)) {
            this.ordenDeCargaFason.RazonSocialDestino = undefined;
            return;
        }
        this.validandoCuitDestino = true;
        this.ordenDeCargaFason.RazonSocialDestino = undefined;
        this.mensajeCuitDestino = "";

        this.service.validarSisaCuit("", cuit, this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap).subscribe(
            result => {
                this.validandoCuitDestino = false;
                const esValidoSisa = this.manejarErroresApiResponse(result);
                if (!esValidoSisa) {
                    this.resetearPlantasDomicilios();
                    this.mensajeCuitDestino = "El CUIT destino no está habilitado en SISA, no podrá cargar la orden hasta regularizar la situación";
                }
                else {
                    this.validarRucaDestino(cuit);
                }
            }
        )
    }

    validarRucaDestinatario(cuitDestinatario: string) {
        this.validandoCuitDestinatario = true;
        this.service.validarCuitRuca(cuitDestinatario).subscribe(
            result => {
                this.validandoCuitDestinatario = false;
                const esValidoRuca = this.manejarErroresApiResponse(result);
                if (!esValidoRuca) {
                    this.mensajeCuitDestinatario = "El CUIT destinatario no posee planta/domicilio en RUCA, no podrá cargar la orden hasta regularizar la situación";
                } else {
                    this.validarCuitDestinatarioExiste()
                }
            }
        );
    }

    validarRucaDestino(cuitDestino: string) {
        this.validandoCuitDestino = true;
        this.service.validarCuitRuca(cuitDestino).subscribe(
            result => {
                this.validandoCuitDestino = false;
                const esValidoRuca = this.manejarErroresApiResponse(result);
                if (!esValidoRuca) {
                    this.resetearPlantasDomicilios();
                    this.mensajeCuitDestino = "El CUIT destino no posee planta/domicilio en RUCA, no podrá cargar la orden hasta regularizar la situación";
                } else {
                    this.onDestinoIngresado(cuitDestino)
                    this.validarCuitDestinoExiste();
                }
            }
        );
    }
    validarCuitTransporte() {
        const campo = "CUITTransporte";
        const cuit = this.ordenDeCargaFason.CUITTransporte ? this.ordenDeCargaFason.CUITTransporte.toString() : "";
        if (!this.revisarCUITFormatoValido(cuit))
            return;
        if (this.mensajesOrdenDeCargaFason[campo])
            this.floatMsgService.setMsgsEmpty();
        this.validando[campo] = true;
        this.mensajesOrdenDeCargaFason[campo] = null;
        this.service.validarCuitTransporte(cuit).subscribe(result => {
            this.validando[campo] = false;
            let data = this.manejarErroresApiResponse(result);
            if (!data && data != null) {
                this.mensajesOrdenDeCargaFason[campo] = "CUIT transporte inválido - Revisar valor ingresado";
                this.floatMsgService.setInfoMsg("CUIT transporte inválido - Revisar valor ingresado");
            }
        });
    }
    validarCuilChofer() {
        const campo = "CUILChofer";
        const cuit = this.ordenDeCargaFason.CUILChofer ? this.ordenDeCargaFason.CUILChofer.toString() : "";
        if (!this.revisarCUITFormatoValido(cuit))
            return;
        if (this.mensajesOrdenDeCargaFason[campo])
            this.floatMsgService.setMsgsEmpty();
        this.validando[campo] = true;
        this.mensajesOrdenDeCargaFason[campo] = null;
        this.service.validarCuilChofer(cuit).subscribe(result => {
            this.validando[campo] = false;
            let data = this.manejarErroresApiResponse(result);
            if (!data && data != null) {
                this.mensajesOrdenDeCargaFason[campo] = "CUIL Chofer inválido - Revisar valor ingresado";
                this.floatMsgService.setInfoMsg("CUIL Chofer inválido - Revisar valor ingresado");
            }
        });
    }
    cargarClienteDirecto(idCliente: Number) {
        this.service.obtenerProveedor(idCliente).subscribe(resp => {
            let proveedor = this.manejarErroresApiResponse(resp);
            if (proveedor) {
                this.clienteSeleccionado = proveedor;
                this.clienteCodigo = proveedor.CodigoProveedor;
                this.ordenDeCargaFason.CUITCliente = this.clienteSeleccionado.CUIT;
                this.onClienteSeleccionado();
            }
        });
    }
    patenteAcopladoSelected(event: any) {
        if (typeof (event) === "string")
            this.ordenDeCargaFason.PatenteAcoplado = event.toUpperCase();
        else if (event.value)
            this.ordenDeCargaFason.PatenteAcoplado = event.value.toUpperCase();
    }
    patenteChasisSelected(event: any) {
        if (typeof (event) === "string")
            this.ordenDeCargaFason.PatenteChasis = event.toUpperCase();
        else if (event.value)
            this.ordenDeCargaFason.PatenteChasis = event.value.toUpperCase();
        this.validacionExistenciaPatente$.next()
    }
    get patenteAcopladoValida() {
        return this.ordenDeCargaFason.PatenteAcoplado && this.ordenDeCargaFason.PatenteAcoplado.trim().length >= 6
    }
    get patenteChasisValido() {
        return (
            this.ordenDeCargaFason.PatenteChasis &&
            this.ordenDeCargaFason.PatenteChasis.trim().length >= 6 &&
            this.esPatenteValida(this.ordenDeCargaFason.PatenteChasis));
    }

    displayModalEscalable = false;
    decidioEscalable = false;

    validarCNRT() {
        if (!(this.patenteAcopladoValida && this.patenteChasisValido))
            return;
        //Posible check de si está marcado el campo escalable
        this.validarCNRTSubject.next();
    }
    override validarCNRTRequest() {
        this.validando.Escalable = true;
        this.validarCNRTSubscription = this.service
            .verificarCNRT(this.ordenDeCargaFason.PatenteChasis, this.ordenDeCargaFason.PatenteAcoplado)
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
                    if (!this.escalableCNRT && this.ordenDeCargaFason.Escalable) {
                        this.msgService.add(MSG_ALERTA_NO_ESCALABLE);
                        this.setValorEscalable()
                    }
                    this.displayModalEscalable = this.escalableCNRT && !this.ordenDeCargaFason.Escalable && !this.decidioEscalable;
                }
            })
    }
    validarEscalable() {
        if (!this.ordenDeCargaFason.Escalable)
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
        this.ordenDeCargaFason.Escalable = value;
    }
    marcarComoEscalable() {
        this.setValorEscalable(true)
        this.displayModalEscalable = false;
    }
    dejarSinEscalable() {
        this.displayModalEscalable = false;
    }
    public extraOnDestroy(): void {
        this.localSubscriptions.unsubscribe();
    }

    getPatentes() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        try {
            this.service.getPatentes(this.ordenDeCargaFason).subscribe(
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
            this.service.getCuilsChofer(this.ordenDeCargaFason).subscribe(
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
            this.service.getCuitsTransporte(this.ordenDeCargaFason).subscribe(
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
    gestionarAltasCuitTerceros(ordenId: string) {
        this.service.EnviarMailAltaCuitTerceros(this.gestiona["CUITIntermediarioFlete"],
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
    cargarAltaCuit() {
        const campo = this.displayModal;
        const razonSocial = this.razonSocialParaGestion;
        this.razonSocialParaGestion = "";
        this.gestiona[campo] = true;
        this.ordenDeCargaFason[campo.replace("CUIT", "RazonSocial")] = razonSocial;
        this.displayModal = null;
        this.mensajesGestionCuit[campo] = `Se solicitará la gestión del alta para el cuit: ${this.ordenDeCargaFason[campo]}`;
    }

    asignarRemitenteComercial() {
        this.ordenDeCargaFason.RemitenteComercial = this.usaRemitenteComercial;
        if (!this.ordenDeCargaFason.RemitenteComercial) {
            this.floatMsgService.setInfoMsg("Su CUIT no será considerado como remitente comercial.")
        } else {
            this.floatMsgService.setInfoMsg("Su CUIT será considerado como remitente comercial.")
        }
    }

    get usaRemitenteComercial() {
        return this.ordenDeCargaFason.CUITCliente.toString() != this.ordenDeCargaFason.CUITDestino;
    }
    validandoCliente = false;

    validarSisaCliente() {
        const cuit = this.ordenDeCargaFason.CUITCliente.toString();
        if (!cuit || !this.revisarCUITFormatoValido(cuit) || !this.ordenDeCargaFason.ProductoSeleccionado) {
            return;
        }
        this.validandoCliente = true;

        this.service.validarSisaCliente(this.clienteSeleccionado.CodigoProveedor, this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap).subscribe(
            result => {
                this.validandoCliente = false;
                const esValidoSisa = this.manejarErroresApiResponse(result);
                if (!esValidoSisa) {
                    this.ordenDeCargaFason.ProductoSeleccionado = null;
                    this.ordenDeCargaFason.Producto_Id = null;
                    this.selectProducto();
                    this.mensajeComponent.setInfoMsg("El cliente no está habilitado en SISA, no podrá cargar la orden hasta regularizar la situación.");
                }
                else {
                    this.validarRucaCliente(cuit);
                }
            }
        )
    }

    validarRucaCliente(cuitCliente: string) {
        this.validandoCliente = true;
        this.service.validarCuitRuca(cuitCliente).subscribe(
            result => {
                this.validandoCliente = false;
                const esValidoRuca = this.manejarErroresApiResponse(result);
                if (!esValidoRuca) {
                    this.ordenDeCargaFason.ProductoSeleccionado = null;
                    this.ordenDeCargaFason.Producto_Id = null;
                    this.selectProducto();
                    this.mensajeComponent.setInfoMsg("Cliente no está habilitado en RUCA, no podrá cargar la orden hasta regularizar la situación");
                }
            }
        );
    }

    validarExistenciaPatentes() {
        this.validacionExistenciaPatenteSub = this.service
            .validarExistenciaPatentes(
                this.ordenDeCargaFason.PatenteChasis, this.ordenDeCargaFason.CUITCliente)
            .subscribe(res => {
                const notificarExistencia = this.manejarApiResponse(res, this.sessionDataService, this.mensajeComponent)
                if (notificarExistencia) {
                    this.floatMsgService.setInfoMsg("El camión ya fué autorizado por otro cliente.");
                }
            });
    }
    puedeValidarExistenciaPatentes(): boolean {
        return this.patenteChasisValido && !!this.ordenDeCargaFason.CUITCliente
    }
}
