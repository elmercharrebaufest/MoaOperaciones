import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
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
import { DestinoFason, setupDaysAndMonths, sumarDias } from '../orden-carga-fason-utils';
import { OrdenesDeCargaFasonService } from '../ordenes-de-carga-fason.service';
import { ApiResponse } from '../../common/models/response';
import { IOrdenesBaseComponent, OrdenesBaseComponent } from '../../common/base-components/ordenes-base-component';
import { Permiso } from '../../common/enums/Permisos';
import { finalize } from 'rxjs/operators';
import { forkJoin } from 'rxjs';
import { Domicilio } from '../../common/models/ordenes-de-carga/domicilio';
import { Planta } from '../../common/models/ordenes-de-carga/planta';

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
        protected seleccionarProveedorService: SeleccionarProveedorService) {
        super(service, navService, securityService, floatMsgService, modalService);
    }

    listaProductos: Material[];
    listaDestinos: DestinoFason[] = [];
    listaClientes: any[];
    listaCorredores: any[];

    clienteSeleccionado: any = "";
    corredorSeleccionado: any;

    patentesChasis: any;
    patentesAcoplados: any;

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
    modificaReventa = this.isAuthorized(Permiso.FasonModificarCampoReventa);

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

    ngOnInit() {
        this.userEmail = sessionStorage.getItem("username");
        this.ordenDeCargaFason.Cantidad = 30000;
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaFasonId = params["id"];
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
                this.cargarClientes(this.CodigoCorredor);
            } else {
                if (this.esCliente()) { //sessionStorage.getItem("tipoUsuario") == "CLI") {
                    this.clienteCodigo = sessionStorage.getItem("proveedor");
                    this.clienteCUIT = sessionStorage.getItem("cuit");
                }
            }
        }

        if (this.ordenDeCargaFasonId > 0) {
            this.obtenerOrdenDeCarga();
        } else {
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
        /*VER ESTA VALIDACION, ACA VALIDA COMO SI FUERA UN CUIT PERO EN EL FRONT DICE QUE PONGA EL DNI/CUIL*/
        if (this.ordenDeCargaFason.CUILChofer == undefined || this.ordenDeCargaFason.CUILChofer.toString().trim().length != 11) {
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
        if (this.ordenDeCargaFason.RazonSocialTransporte == undefined || this.ordenDeCargaFason.RazonSocialTransporte.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese la razón social del transporte.");
            return false;
        }
        if (this.ordenDeCargaFason.CUITTransporte == undefined || this.ordenDeCargaFason.CUITTransporte.toString().trim().length != 11) {
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
        if (!this.ordenDeCargaFason.Destino) {
            // Mostrar un mensaje de error al usuario o hacer algo para indicar que es necesario seleccionar un corredor
            this.mensajeComponent.setInfoMsg("Seleccione un destino.");
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
        this.obtenerDestinos(this.ordenDeCargaFason.Cliente)
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

    getFecha = (meses: number, fecha: Date = new Date()) => {
        fecha.setMonth(fecha.getMonth() - meses);
        var anho = fecha.toLocaleString("default", { year: "numeric" });
        var mes = fecha.toLocaleString("default", { month: "2-digit" });
        var dia = fecha.toLocaleString("default", { day: "2-digit" });
        let stringFecha = anho + '-' + mes + '-' + dia;
        return stringFecha;
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
        console.log("grabar ordenDeCargaFason", this.ordenDeCargaFason);
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
            this.subscriptionDropDowns = this.service.getOrdenDeCargaFason(this.ordenDeCargaFasonId).subscribe(
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


                        var parts = this.ordenDeCargaFason.FechaRetiro.toString().split('/');
                        var year = parts[2].split(" ");
                        this.ordenDeCargaFason.FechaRetiro = new Date(Number(year[0]), Number(parts[1]) - 1, Number(parts[0]));

                        const producto = this.listaProductos.find(producto => producto.MaterialId == this.ordenDeCargaFason.Producto_Id)
                        this.selectProducto(producto);
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

    obtenerDestinos = (Cliente: string) => {
        this.subscription = this.service.getDestino(Cliente).subscribe(
            (result) => {
                this.listaDestinos = result.data;
                if (this.ordenDeCargaFason.LocalidadDescripcion) {
                    let seleccionado = this.listaDestinos.find(a => a.LocalidadDescripcion == this.ordenDeCargaFason.LocalidadDescripcion);
                    if (seleccionado) {
                        this.ordenDeCargaFason.Destino = seleccionado;
                    }
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    obtenerProductos = () => {
        this.subscription = this.service.getMateriales().subscribe(
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
                        console.log("this.CodigoCorredor " + this.CodigoCorredor);
                        if (this.CodigoCorredor != "") {
                            let seleccionado = this.listaCorredores.filter(a => a.CodigoProveedor == this.CodigoCorredor);
                            console.log("seleccionado ", seleccionado);
                            if (seleccionado != null && seleccionado.length > 0) {
                                console.log("seleccionado[0] ", seleccionado[0]);

                                this.corredorSeleccionado = seleccionado[0];
                                this.ordenDeCargaFason.CUITCorredor = this.corredorSeleccionado.CUIT;
                                this.ordenDeCargaFason.CorredorId = this.corredorSeleccionado.Id;
                                console.log("asdasd " + this.corredorSeleccionado.Id, this.corredorSeleccionado);
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

                            this.obtenerDestinos(this.clienteSeleccionado.Id);
                        } else {
                            if (this.ordenDeCargaFason.Cliente != null && this.ordenDeCargaFason.Cliente != "") {
                                this.clienteSeleccionado = this.listaClientes.find(x => x.CodigoProveedor == this.ordenDeCargaFason.Cliente);
                                this.clienteCUIT = this.clienteSeleccionado.CUIT;
                                this.ordenDeCargaFason.CUITCliente = Number(this.clienteCUIT);
                                this.ordenDeCargaFason.Cliente = this.clienteSeleccionado.Id;
                                this.obtenerDestinos(this.clienteSeleccionado.Id);
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

    selectProducto(value: Material) {
        this.ordenDeCargaFason.ProductoSeleccionado = value;
        this.ordenDeCargaFason.Producto_Id = value.MaterialId;
        this.validaCPEDG = this.ordenDeCargaFason.ProductoSeleccionado.ValidaSisaRuca;
        if (!this.ordenDeCargaFasonId) {
            this.ordenDeCargaFason.Reventa = this.modificaReventa && this.validaCPEDG && !this.ordenDeCargaFason.Reventa
        }

        if (!this.validaCPEDG)
            this.setearDefaultEnCPEDG();
        this.setCantidadCambioProducto();
    }
    setCantidadCambioProducto() {
        this.ordenDeCargaFason.Cantidad =
            this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap == CODIGO_PELLET_GIRASOL_INTEGRAL ? CANTIDAD_PELLET_GIRASOL : CANTIDAD_DEFAULT;
    }
    setearDefaultEnCPEDG() {
        this.ordenDeCargaFason.Reventa = false;
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
    resetearPlantasDomicilios() {
        this.plantaSeleccionada = undefined;
        this.domicilioSeleccionado = undefined;
        this.listaPlantas = [];
        this.listaDomicilios = [];
        this.onDomicilioSeleccionadoChanged()
        this.onPlantaSeleccionadaChanged();
    }

    cuitDestinatarioChanged() {
        const cuit = this.ordenDeCargaFason.CUITDestinatario;
        if (!cuit || !this.revisarCUITFormatoValido(cuit)) {
            this.ordenDeCargaFason.RazonSocialDestinatario = undefined;
            return;
        }
        this.validandoCuitDestinatario = true;
        this.ordenDeCargaFason.RazonSocialDestinatario = undefined;
        this.mensajeCuitDestinatario = "";

        this.service.validarExisteCuitScato(cuit).subscribe(
            result => {
                this.validandoCuitDestinatario = false;
                const data = this.manejarErroresApiResponse(result);
                if (!data)
                    return;
                if (!data.Existe) {
                    //this.displayModal = campo;
                }
                else {
                    this.ordenDeCargaFason.RazonSocialDestinatario = data.RazonSocial;
                }
                this.validarSisaDestinatario(cuit)
            })
    }

    cuitDestinoChanged() {
        const cuit = this.ordenDeCargaFason.CUITDestino;
        if (!cuit || !this.revisarCUITFormatoValido(cuit)) {
            this.ordenDeCargaFason.RazonSocialDestino = undefined;
            return;
        }
        this.validandoCuitDestino = true;
        this.ordenDeCargaFason.RazonSocialDestino = undefined;
        this.mensajeCuitDestino = "";

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
                this.validarSisaDestino(cuit)
            })
    }

    validarSisaDestinatario(cuitDestinatario: string) {
        this.validandoCuitDestinatario = true;
        this.service.validarSisaCuit(cuitDestinatario, "", this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap).subscribe(
            result => {
                this.validandoCuitDestinatario = false;
                const esValidoSisa = this.manejarErroresApiResponse(result);
                if (!esValidoSisa) {
                    this.mensajeCuitDestinatario = "El CUIT destinatario no está habilitado en SISA, no podrá cargar la orden hasta regularizar la situación";
                }
                else {
                    this.validarRucaDestinatario(cuitDestinatario);
                }
            }
        )
    }

    validarSisaDestino(cuitDestino: string) {
        this.validandoCuitDestino = true;
        this.service.validarSisaCuit("", cuitDestino, this.ordenDeCargaFason.ProductoSeleccionado.CodigoSap).subscribe(
            result => {
                this.validandoCuitDestino = false;
                const esValidoSisa = this.manejarErroresApiResponse(result);
                if (!esValidoSisa) {
                    this.mensajeCuitDestino = "El CUIT destino no está habilitado en SISA, no podrá cargar la orden hasta regularizar la situación";
                }
                else {
                    this.validarRucaDestino(cuitDestino);
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
                    this.mensajeCuitDestino = "El CUIT destino no posee planta/domicilio en RUCA, no podrá cargar la orden hasta regularizar la situación";
                } else {
                    this.onDestinoIngresado(cuitDestino)
                }
            }
        );
    }
}
