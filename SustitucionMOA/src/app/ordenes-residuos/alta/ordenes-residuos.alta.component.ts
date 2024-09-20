import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { SeleccionarProveedorService } from "../../common/shared-components/seleccionar-proveedor/seleccionar-proveedor.service";
import { BlockUI, NgBlockUI } from "ng-block-ui";
import { MensajeComponent } from "../../common/view-child/mensaje/mensaje.component";
import { SpinnerComponent } from "../../common/view-child/spinner/spinner.component";
import { OrdenesResiduosService } from "../ordenes-residuos.service";
import { NavService } from "../../common/services/NavService";
import { SecurityService } from "../../common/services/SecurityService";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { BaseComponent } from "../../common/base-components/base-component";
import { Material } from "../../common/models/material";
import { ActivatedRoute, Params } from "@angular/router";
import { OrdenCargaResiduosDto } from "../../common/models/ordenes-residuos/ordenCargaResiduosDto";
import { Permiso } from "../../common/enums/Permisos";
import { ApiResponse } from "../../common/models/response";
import { SessionDataService } from "../../common/services/SessionDataService";
import { Proveedor } from "../../common/models/proveedor";
import { Planta } from "../../common/models/ordenes-residuos/planta";
import { Domicilio } from "../../common/models/ordenes-residuos/domicilio";
import { EstadoOrdenResiduosEnum } from "../../common/models/ordenes-residuos/estadoOrdenResiduos";
import { AutocompleteLocalidadComponent } from "../../common/shared-components/autocomplete-localidad/autocomplete-localidad.component";


@Component({
    selector: 'app-alta',
    templateUrl: './ordenes-residuos.alta.component.html',
    providers: [SeleccionarProveedorService],
    styleUrls: ['./ordenes-residuos.alta.component.css']
})
export class OrdenesResiduosAltaComponent extends BaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;
    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    @ViewChild('messages')
    private messagesContainer?: ElementRef<HTMLDivElement>;
    @ViewChild(AutocompleteLocalidadComponent)
    private autocompleteLocalidadComponent: AutocompleteLocalidadComponent;

    constructor(
        protected service: OrdenesResiduosService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected seleccionarProveedorService: SeleccionarProveedorService,
        protected sessionDataService: SessionDataService,
        protected modalService: ModalService,
        private route: ActivatedRoute) {
        super(navService, securityService, floatMsgService, modalService)
    }

    listaClientes: Proveedor[] = [];
    listaProductos: Material[];
    listaPlantas: Planta[] = [];
    listaDomicilios: Domicilio[] = [];

    ordenResiduos: OrdenCargaResiduosDto = new OrdenCargaResiduosDto();
    esAdmin: boolean = this.isAuthorized(Permiso.ResiduosVerOrdenesDeCargaAdmin);
    codigoProveedorUsuario: string = sessionStorage.getItem("proveedor") || "SINCODIGO";

    patentesChasis: string[] = [];
    patentesAcoplados: string[] = [];
    cuilsChofer: string[] = [];
    cuitsTransporte: string[] = [];

    validaCPEDG: boolean = false;
    procesandoCampo: Partial<Record<keyof OrdenCargaResiduosDto, boolean>> = {};
    mensajeSuccess: string = "";
    localidadDescripcion: string = "";

    get NoPuedeEditarCuitsTerceros(): boolean {
        return (this.ordenResiduos.Id &&
            this.ordenResiduos.Estado.Id == EstadoOrdenResiduosEnum.OrdenEntregada) || false;
    }

    ngOnInit(): void {
        let ordenId: number = 0;
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) {
                ordenId = params["id"];
            }
        });

        this.navService.setSeccionList([]);
        this.obtenerProductos();

        if (ordenId > 0) {
            this.obtenerOrdenDeCargaResiduos(ordenId);
        }
        else {
            this.cargarClientes();
        }
    }


    obtenerProductos = () => {
        this.service.getMateriales().subscribe(
            (resp) => {
                let data = this.manejarErroresApiResponse(resp);
                if (data) {
                    this.listaProductos = data;
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        );
    }

    onProductoSeleccionado() {
        if (!this.ordenResiduos.Producto)
            return;

        this.validaCPEDG = this.ordenResiduos.Producto.ValidaSisaRuca;

        if (!this.validaCPEDG) {
            this.ordenResiduos.Planta = undefined;
            this.ordenResiduos.Domicilio = undefined;
            this.listaPlantas = [];
            this.listaDomicilios = [];
        }
        else {
            this.obtenerPlantas();
            this.obtenerDomicilios();
        }
    }

    cargarClientes() {
        if (this.esAdmin) {
            this.cargarClientesAdmin();
        }
        else {
            if (this.esCliente()) {
                this.cargarClienteDirecto(parseInt(sessionStorage.getItem("proveedorId") || ""));
            }
        }
    }

    cargarClientesAdmin() {
        this.service.getClientesResiduos().subscribe(
            (resp) => {
                let clientes = this.manejarErroresApiResponse(resp);
                if (clientes) {
                    this.listaClientes = this.ordenarYFiltrarProveedores(clientes);
                    if (this.ordenResiduos.Cliente) {
                        this.ordenResiduos.Cliente = this.listaClientes.find((v, i, a) => { return v.Id == this.ordenResiduos.Cliente.Id }) || this.ordenResiduos.Cliente;
                        this.onClienteSeleccionado();
                    }
                }
                else {
                    this.listaClientes = [];
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        )
    }

    cargarClienteDirecto(clienteId: number) {
        if (this.ordenResiduos.Cliente && this.ordenResiduos.Cliente.Id == clienteId) {
            this.listaClientes = [this.ordenResiduos.Cliente];
            this.onClienteSeleccionado();
        }
        else {
            this.service.obtenerProveedor(clienteId).subscribe(
                (resp) => {
                    let proveedor = this.manejarErroresApiResponse(resp);
                    if (proveedor) {
                        this.listaClientes = [proveedor];
                        this.ordenResiduos.Cliente = proveedor;
                        this.onClienteSeleccionado();
                    }
                }
            );
        }
    }

    onClienteSeleccionado() {
        this.obtenerPatentes();
        this.obtenerPlantas();
        this.obtenerDomicilios();
        this.obtenerIdsTransportes();
    }

    ordenarYFiltrarProveedores(proveedores: Proveedor[]): Proveedor[] {
        let filtrados = proveedores.filter(
            (thing, i, arr) => arr.findIndex(t => t.CUIT === thing.CUIT) === i
        );
        filtrados.sort((a, b) => {
            const nameA = a.RazonSocial.toUpperCase();
            const nameB = b.RazonSocial.toUpperCase();
            return nameA < nameB ? -1 : (nameA > nameB ? 1 : 0);
        });
        filtrados.forEach(c => { c.RazonSocial = c.RazonSocial + " (" + c.CUIT + ")" });
        return filtrados;
    }

    onLocalidadSeleccionada(idLocalidad: string) {
        if (idLocalidad) {
            this.ordenResiduos.Localidad.Id = Number(idLocalidad);
        }
    }

    obtenerPatentes() {
        if (!this.ordenResiduos.Cliente.Id) {
            this.patentesAcoplados = [];
            this.patentesChasis = [];
            return;
        }
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.service.getPatentes(this.ordenResiduos.Cliente.Id).subscribe(
            (resp) => {
                let patentes = this.manejarErroresApiResponseFloat(resp);
                if (patentes) {
                    this.patentesAcoplados = patentes.PatentesAcoplado;
                    this.patentesChasis = patentes.PatentesChasis;
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        );
    }

    obtenerPlantas() {
        if (!this.ordenResiduos.Cliente.CUIT || !this.validaCPEDG) {
            this.listaPlantas = [];
            this.ordenResiduos.Planta = undefined;
            return;
        }
        this.unsubscribe();
        this.service.obtenerPlantas(this.ordenResiduos.Cliente.CUIT).subscribe(
            (resp) => {
                let plantas = this.manejarErroresApiResponse(resp);
                if (plantas && plantas.length > 0) {
                    this.listaPlantas = plantas;
                }
                else {
                    this.floatMsgService.setErrorMsg(this.ordenResiduos.Cliente.CUIT + " no está habilitado en RUCA, no podrá cargar la orden hasta regularizar la situación");
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        );
    }

    obtenerDomicilios() {
        if (!this.ordenResiduos.Cliente.CUIT || !this.validaCPEDG) {
            this.listaDomicilios = [];
            this.ordenResiduos.Domicilio = undefined;
            return;
        }
        this.unsubscribe();
        this.service.obtenerDomicilios(this.ordenResiduos.Cliente.CUIT).subscribe(
            (resp) => {
                let domicilios = this.manejarErroresApiResponse(resp);
                if (domicilios && domicilios.length > 0) {
                    this.listaDomicilios = domicilios;
                }
                else {
                    this.floatMsgService.setErrorMsg(this.ordenResiduos.Cliente.CUIT + " no está habilitado en RUCA, no podrá cargar la orden hasta regularizar la situación");
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        );
    }

    onPatenteChasisSeleccionado(event: any) {
        this.ordenResiduos.PatenteChasis = event.toUpperCase();
    }

    onPatenteAcopladoValueChanged(valor: string) {
    }

    onPatenteAcopladoBlur(valor: string) {
        this.obtenerIdsTransportes();
    }

    onPatenteAcopladoSeleccionado(event: any) {
        // this.obtenerIdsTransportes();
        this.ordenResiduos.PatenteAcoplado = event.toUpperCase();
    }

    validarCuilChofer() {
        const cuil = this.ordenResiduos.CUILChofer ? this.ordenResiduos.CUILChofer : "";
        if (!this.esFormatoCuilCuitValido(cuil))
            return;
        this.procesandoCampo.CUILChofer = true;
        this.service.esCuilCuitValido(cuil).subscribe(
            (resp) => {
                let esValido = this.manejarErroresApiResponseFloat(resp);
                if (esValido === false) {
                    this.floatMsgService.setInfoMsg("CUIL chofer inválido - Revisar valor ingresado");
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            },
            () => { this.procesandoCampo.CUILChofer = false; }
        );
    }

    validarCuitTransporte() {
        const cuit = this.ordenResiduos.CUITTransporte ? this.ordenResiduos.CUITTransporte : "";
        if (!this.esFormatoCuilCuitValido(cuit))
            return;
        this.procesandoCampo.CUITTransporte = true;
        this.service.esCuilCuitValido(cuit).subscribe(
            (resp) => {
                let esValido = this.manejarErroresApiResponseFloat(resp);
                if (esValido === false) {
                    this.floatMsgService.setInfoMsg("CUIT transporte inválido - Revisar valor ingresado");
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            },
            () => { this.procesandoCampo.CUITTransporte = false; }
        );
    }

    esFormatoCuilCuitValido(cuit: string): boolean {
        return !!(cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number))
    }

    obtenerIdsTransportes() {
        if (!this.ordenResiduos.Cliente.Id || !this.ordenResiduos.PatenteAcoplado || this.ordenResiduos.PatenteAcoplado.length < 4) {
            this.cuilsChofer = [];
            return;
        }
        this.unsubscribe();
        this.service.obtenerIdsTransportes(this.ordenResiduos.Cliente.Id, this.ordenResiduos.PatenteAcoplado).subscribe(
            (resp) => {
                let ids = this.manejarErroresApiResponseFloat(resp);
                if (ids) {
                    this.cuilsChofer = ids.CuilsChoferes;
                    this.cuitsTransporte = ids.CuitsTransporte;
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        );
    }

    validarOrdenDeCargaResiduos(): boolean {
        if (this.ordenResiduos.NombreChofer == undefined || this.ordenResiduos.NombreChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el nombre del chofer.");
            return false;
        }
        if (this.ordenResiduos.ApellidoChofer == undefined || this.ordenResiduos.ApellidoChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el apellido del chofer.");
            return false;
        }
        if (this.ordenResiduos.CUILChofer == undefined || this.ordenResiduos.CUILChofer.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIL de chofer válido.");
            return false;
        }
        if (this.ordenResiduos.PatenteAcoplado &&
            this.ordenResiduos.PatenteAcoplado.trim().length > 0 &&
            this.ordenResiduos.PatenteAcoplado.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese una patente acoplado válida.");
            return false;
        }
        if (this.ordenResiduos.PatenteChasis == undefined || this.ordenResiduos.PatenteChasis.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese una patente chasis válida.");
            return false;
        }
        if (this.ordenResiduos.PatenteAcoplado == this.ordenResiduos.PatenteChasis) {
            this.mensajeComponent.setInfoMsg("Los números de patente no pueden ser iguales.");
            return false;
        }
        if (this.ordenResiduos.RazonSocialTransporte == undefined || this.ordenResiduos.RazonSocialTransporte.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese la razón social del transporte.");
            return false;
        }
        if (this.ordenResiduos.CUITTransporte == undefined || this.ordenResiduos.CUITTransporte.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de transporte válido.");
            return false;
        }
        if (!this.ordenResiduos.Cliente) {
            this.mensajeComponent.setInfoMsg("Seleccione un cliente.");
            return false;
        }
        if (!this.ordenResiduos.Producto) {
            this.mensajeComponent.setInfoMsg("Seleccione un producto.");
            return false;
        }
        if (!this.ordenResiduos.Localidad && !this.validaCPEDG) {
            this.mensajeComponent.setInfoMsg("Seleccione un destino.");
            return false;
        }
        if (this.validaCPEDG) {
            if (!(this.ordenResiduos.Planta && this.ordenResiduos.Planta.Codigo)) {
                this.mensajeComponent.setInfoMsg("Debe seleccionar una plante para este tipo de material.")
                return false;
            }
            if (!(this.ordenResiduos.Domicilio && this.ordenResiduos.Domicilio.Descripcion)) {
                this.mensajeComponent.setInfoMsg("Debe seleccionar un domicilio para este tipo de material.")
                return false;
            }
        }
        return true;
    }

    guardarOrdenDeCargaResiduos() {
        if (!this.validarOrdenDeCargaResiduos()) {
            return;
        }
        if (this.ordenResiduos.Id > 0) {
            this.grabarOrdenEditada();
        }
        else {
            this.crearNuevaOrden();
        }
    }

    crearNuevaOrden() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.blockUI.start("Grabando...");
        try {
            this.subscription = this.service.crearOrdenResiduos(this.ordenResiduos).subscribe(
                (resp) => {
                    let resData = this.manejarErroresApiResponse(resp);
                    if (resData) {
                        this.mensajeSuccess = resData.Mensaje;
                        this.ordenResiduos.Id = resData.IdOrden;
                        document.getElementById("openModalNotificacion").click();
                    }
                },
                (err) => {
                    this.mensajeComponent.setErrorMsg(err.message);
                },
                () => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                }
            );
        }
        catch (err) {
            this.mensajeComponent.setErrorMsg(err);
        }
    }

    grabarOrdenEditada() {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.blockUI.start("Grabando...");
        try {
            this.subscription = this.service.editarOrdenResiduos(this.ordenResiduos).subscribe(
                (resp) => {
                    let resData = this.manejarErroresApiResponse(resp);
                    if (resData) {
                        this.mensajeSuccess = resData.Mensaje;
                        this.ordenResiduos.Id = resData.IdOrden;
                        document.getElementById("openModalNotificacion").click();
                    }
                },
                (err) => {
                    this.mensajeComponent.setErrorMsg(err.message);
                },
                () => {
                    this.spinnerComponent.hideIt();
                    this.blockUI.stop();
                }
            );
        }
        catch (err) {
            this.mensajeComponent.setErrorMsg(err);
        }
    }

    obtenerOrdenDeCargaResiduos(idOrden: number) {
        this.blockUI.start();
        this.unsubscribe();
        this.service.obtenerOrdenResiduos(idOrden).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.cargarClientes();
                    this.ordenResiduos.Producto = this.listaProductos.find((v, i, a) => v.MaterialId == this.ordenResiduos.Producto.MaterialId) || this.ordenResiduos.Producto;
                    this.ordenResiduos.Almacen = this.ordenResiduos.Producto.Almacenes.find((v, i, a) => v.Id == this.ordenResiduos.Almacen.Id) || this.ordenResiduos.Almacen;
                    if (this.ordenResiduos.Localidad) {
                        this.autocompleteLocalidadComponent.localidad_Id = this.ordenResiduos.Localidad.Id;
                        this.autocompleteLocalidadComponent.getLocalidadById();
                        this.localidadDescripcion = `${this.ordenResiduos.Localidad.Nombre} (${this.ordenResiduos.Localidad.ProvinciaNombre})`;
                    }
                    this.onProductoSeleccionado()
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            },
            () => { this.blockUI.stop(); }
        )
    }

    redirigirAListadoOrdenesResiduos() {
        this.navService.navegarSeccion("/ordenes-residuos");
    }

    cerrarNotificacionYVolver() {
        document.getElementById("botonCerrarModal").click();
        this.redirigirAListadoOrdenesResiduos();
    }


    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        this.mensajeComponent.setMsgsEmpty();
        if (response.logout) {
            this.sessionDataService.logout();
            return null;
        }
        if (response.error) {
            this.mensajeComponent.setErrorMsg(response.error);
            this.scrollAMensaje();
            return null;
        }
        if (response.info) {
            this.mensajeComponent.setInfoMsg(response.info);
            this.scrollAMensaje();
        }
        return response.data || null;
    }

    manejarErroresApiResponseFloat<T>(response: ApiResponse<T>): T | null {
        this.floatMsgService.setMsgsEmpty();
        if (response.logout) {
            this.sessionDataService.logout();
            return null;
        }
        if (response.error) {
            this.floatMsgService.setErrorMsg(response.error);
            return null;
        }
        if (response.info) {
            this.floatMsgService.setInfoMsg(response.info);
        }
        return response.data || null;
    }

    scrollAMensaje() {
        if (this.messagesContainer)
            this.messagesContainer.nativeElement.scrollIntoView({ behavior: 'smooth' });
    }
}
