import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { EmpresaGranosService } from '../../alta-proveedores/empresa-granos/empresa-granos.service';
import { BaseComponent } from '../../common/base-components/base-component';
import { Material } from '../../common/models/material';
import { OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { SeleccionarProveedorService } from '../../common/shared-components/seleccionar-proveedor/seleccionar-proveedor.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';
import { NgBlockUI, BlockUI } from 'ng-block-ui';

declare var $: any;

@Component({
    selector: 'app-ordenes-de-carga.alta',
    templateUrl: './ordenes-de-carga.alta.component.html',
    styleUrls: ['./ordenes-de-carga.alta.component.css'],
    providers: [SeleccionarProveedorService],
})

export class OrdenesDeCargaAlta extends BaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    ordenDeCargaId: number = 0;

    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    mensajeError: string = "";
    mensajeSuccess: string = "";
    clienteCUIT: string = "";
    clienteCodigo: string = "";
    CodigoCorredor: string = "";
    Contrato: string = "";
    Producto: string = "";
    patentesChasis: any;
    patentesAcoplados: any;
    private selectUndefinedOptionValue: any;

    listaMateriales: Material[];
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esAdmin: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    listaClientes: any[];
    noEditarCliente : boolean = false;

    puedeEditarContrato: boolean = false;
    resultadoValidacionCorCliConPro: boolean = false;
    mensajeValidacionCorCliConPro: string = "";

    public patternPatente = { '0': { pattern: new RegExp('\[a-zA-Z0-9\]') } };

    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService, 
        protected navService: NavService,
        private spinner: NgxSpinnerService,
        protected seleccionarProveedorService: SeleccionarProveedorService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, 
        protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, 
        protected modalService: ModalService,
        protected empresaGranosService: EmpresaGranosService,
        public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);;
    }

    async ngOnInit() {
        this.ordenDeCarga.Cantidad = 30000;

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
        });

        this.navService.setSeccionList([]);

        console.debug(' tipoUsuario: ', sessionStorage.getItem("tipoUsuario"));
        if (this.esCorredor) {
            this.CodigoCorredor = sessionStorage.getItem("proveedor");
            await this.cargarClientes(this.CodigoCorredor);
        }

        if (this.ordenDeCargaId > 0) {
            this.obtenerOrdenDeCarga();
        } else{
            this.getPatentes();
        }
       
        this.obtenerMateriales();

        if (this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS')) {
            this.ordenDeCarga.CUITCliente = 0;
        }
        
        if (sessionStorage.getItem("tipoUsuario") == "CLI") {
            console.debug(' proveedor: ', sessionStorage.getItem("proveedor"));
            this.onCorredorFocusOut('');
            this.clienteCodigo = sessionStorage.getItem("proveedor");
            if (this.ordenDeCargaId > 0) {
                this.noEditarCliente = true;
            }
        }

        if(this.ordenDeCargaId > 0){
            this.noEditarCliente = true;
        } 
    }

  
    cambioProducto(){
        let productoActual = this.listaMateriales.find(x => x.MaterialId == this.ordenDeCarga.Producto_Id).CodigoSap;       
        if (productoActual == "99709"){
            this.ordenDeCarga.Cantidad = 20000;
        }else{
            this.ordenDeCarga.Cantidad = 30000;
        }
    }

    obtenerMateriales() {
        //Sacamos lo de la lista de campaña, ya que ahora son independientes
        this.subscription = this.service.getMateriales().subscribe(
            (result) => {
                this.listaMateriales = result.data;
                
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    validar() {
        console.debug('validar');
        if (this.esCorredor) {
            if (this.ordenDeCarga.CUITCliente.toString().trim().length != 11) {
                this.mensajeComponent.setInfoMsg("Ingrese un CUIT de cliente válido.");
                return false;
            }
        }

        if (this.ordenDeCarga.NombreChofer.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese el nombre del chofer.");
            return false;
        }

        /*VER ESTA VALIDACION, ACA VALIDA COMO SI FUERA UN CUIT PERO EN EL FRONT DICE QUE PONGA EL DNI/CUIL*/
        if (this.ordenDeCarga.CUITChofer.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIL de chofer válido.");
            return false;
        }

        if (this.ordenDeCarga.PatenteAcoplado.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese una patente válida.");
            return false;
        }

        if (this.ordenDeCarga.ChasisAcoplado.trim().length < 6) {
            this.mensajeComponent.setInfoMsg("Ingrese un número de chasis válido.");
            return false;
        }

        if (this.ordenDeCarga.RazonSocialTransporte.trim().length < 2) {
            this.mensajeComponent.setInfoMsg("Ingrese la razón social del transporte.");
            return false;
        }

        if (this.ordenDeCarga.CUITTransporte.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de transporte válido.");
            return false;
        }

        if (this.ordenDeCargaId == 0) {
            console.debug(' resultadoValidacionCorCliConPro: ', this.resultadoValidacionCorCliConPro);
            if (this.resultadoValidacionCorCliConPro == false) {
                this.mensajeComponent.setErrorMsg(this.mensajeValidacionCorCliConPro);
                return false;
            }
        }

        return true;
    }

    cargaFalsa() {
        this.ordenDeCarga.llenar()
    }

    async obtenerOrdenDeCarga() {       
        try {
            this.subscriptionDropDowns = await this.service.getEditarOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.obtenerMateriales();
                        this.ordenDeCarga = result.data;
                        this.CodigoCorredor = result.data.CodigoCorredor;
                        if (this.esComercial && result.data.ColorSemaforo != "green") {
                            this.puedeEditarContrato = true;
                        }
                        if(result.data.Estado == 3){
                            this.puedeEditarContrato = true;
                        }
                        this.cargarClientes(this.CodigoCorredor);
                        this.clienteCUIT = result.data.CUITCliente;
                        this.clienteCodigo = result.data.CodigoCliente;
                        this.getPatentes();
                        console.debug(' ordenDeCarga: ', this.ordenDeCarga);
                        console.debug(' ordenDeCargaId: ', this.ordenDeCargaId);
                        console.debug(' clienteCUIT: ', this.clienteCUIT);
                        console.debug(' clienteCodigo: ', this.clienteCodigo);
                        console.debug(' CodigoCorredor: ', this.CodigoCorredor);
                        console.debug(' puedeEditarContrato: ', this.puedeEditarContrato);
                        console.debug(' noEditarCliente: ', this.noEditarCliente);
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

    submit() {        
        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Grabando...');
        if (this.ordenDeCargaId > 0) {
            this.subscription = this.service
                .editar(this.ordenDeCarga)
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
        else {
            this.subscription = this.service
                .agregar(this.ordenDeCarga)
                .subscribe(
                    (result) => {
                        this.spinnerComponent.hideIt();
                        this.blockUI.stop();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (
                            result.error != undefined &&
                            result.error != ""
                        ) {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else if (
                            result.data.error != undefined &&
                            result.data.error != ""
                        ) {
                            this.mensajeComponent.setErrorMsg(result.data.error);
                        } else if (result.data.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.data.info);
                        } else {
                            this.mensajeComponent.setMsgsEmpty();

                            this.mensajeSuccess = result.data.Mensaje;

                            this.ordenDeCargaId = result.data.IdEntidad;

                            document
                                .getElementById("openModalNotificacion")
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
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {

                        this.patentesChasis = result.ordenes.map((patente) => {
                            return { label: patente.label, value: patente.label};
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
                        // console.log(this.patentesChasis);
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }      
    }

    PatenteChasisSelected(value: any) {
        this.ordenDeCarga.ChasisAcoplado = value.label;
    }
    PatenteAcopladoSelected(value: any) {
        this.ordenDeCarga.PatenteAcoplado = value.value;
    }

    generalFormatter(data: any): string {
        return `${data['label']}`;
    }

    generalFormatterAcoplado(data: any): string {
        return `${data['value']}`;
    }

    onCorredorSeleccionado = (proveedor: any) => {
        console.debug('onCorredorSeleccionado');
        this.ordenDeCarga.CUITCorredor = proveedor.CUIT;
        this.CodigoCorredor = proveedor.idVendedor;
        this.cargarClientes(proveedor.idVendedor);
    }

    onCorredorFocusOut = (proveedor: any) => {
        console.debug('onCorredorFocusOut');
        console.debug(' proveedor: ', proveedor);
        console.debug(' CodigoCorredor: ', this.CodigoCorredor);
        try{
            if (proveedor == '') {
                this.CodigoCorredor = '';
                this.listaClientes = [];
                //this.spinner.show();
                this.mensajeComponent.setMsgsEmpty();
                this.seleccionarProveedorService.getAllClientsByType(5).subscribe(
                    (result) => {
                        this.spinnerComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.floatMsgService.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.floatMsgService.setInfoMsg(result.info);
                        } else {
                            console.debug('  result: ', result);
                            this.listaClientes = result;
                            this.spinner.hide();
                        }
                    },
                    (error) => {
                        console.error(error.message);
                        this.mensajeComponent.setErrorMsg(error.message);
                        this.spinner.hide();
                    }
                );
            }
        } catch (err) {
            console.error(err.message);
            this.mensajeComponent.setErrorMsg(err);
            this.spinner.hide();
        }
    }

    cargarClientes = async (codigoCorredor: string) => {
        console.debug('cargarClientes');
        console.debug(' codigoCorredor: ', codigoCorredor);
        this.listaClientes = [];
        //this.spinner.show();
        this.mensajeComponent.setMsgsEmpty();
        try { 
            if (this.noEditarCliente) {
                await this.service.visualizarCliente(codigoCorredor).subscribe(
                    result => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.listaClientes = result;
                            this.spinner.hide();
                        }
                    },
                    error => {
                        console.error(error.message);
                        this.mensajeComponent.setErrorMsg(error.message);
                        this.spinner.hide();
                    }
                );
            }
            
        } catch (err) {
            console.error(err.message);
            this.mensajeComponent.setErrorMsg(err);
            this.spinner.hide();
        }
    }

    onClienteSeleccionado = () => {
        console.debug('onClienteSeleccionado');
        console.debug(' clienteCUIT: ', this.clienteCUIT);
        this.ordenDeCarga.CUITCliente = Number(this.clienteCUIT);
        this.getPatentes();
        if (this.clienteCUIT && this.Contrato && this.CodigoCorredor && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, '', this.Contrato, this.CodigoCorredor, this.Producto);
        } else if (this.clienteCUIT && this.Contrato && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, '', this.Contrato, '', this.Producto);
        } else if (this.clienteCodigo && this.Contrato && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, this.clienteCodigo, this.Contrato, '', this.Producto);
        } 
        console.debug(' resultadoValidacionCorCliConPro: ', this.resultadoValidacionCorCliConPro);
    }

    onContratoFocusOut = (contrato: any) => {
        console.debug('onContratoFocusOut');
        console.debug(' contrato: ', contrato);
        this.Contrato = contrato;
        if (this.clienteCUIT && this.Contrato && this.CodigoCorredor && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, '', this.Contrato, this.CodigoCorredor, this.Producto);
        } else if (this.clienteCUIT && this.Contrato && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, '', this.Contrato, '', this.Producto);
        } else if (this.clienteCodigo && this.Contrato && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, this.clienteCodigo, this.Contrato, '', this.Producto);
        } 
        console.debug(' resultadoValidacionCorCliConPro: ', this.resultadoValidacionCorCliConPro);
    }

    onProductoFocusOut = (producto: any) => {
        console.debug('onProductoFocusOut');
        console.debug(' producto: ', producto);
        this.Producto = producto;
        if (this.clienteCUIT && this.Contrato && this.CodigoCorredor && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, '', this.Contrato, this.CodigoCorredor, this.Producto);
        } else if (this.clienteCUIT && this.Contrato && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, '', this.Contrato, '', this.Producto);
        } else if (this.clienteCodigo && this.Contrato && this.Producto) {
            this.validarCorredorClienteContratoProducto(this.clienteCUIT, this.clienteCodigo, this.Contrato, '', this.Producto);
        } 
        console.debug(' resultadoValidacionCorCliConPro: ', this.resultadoValidacionCorCliConPro);
    }
    
    validarCorredorClienteContratoProducto = (clienteCuit: string, clienteCodigo: string, contrato: string, codigoCorredor: string, productoId: string) => {
        console.debug('validarCorredorClienteContratoProducto');
        console.debug(' clienteCuit: ', clienteCuit);
        console.debug(' clienteCodigo: ', clienteCodigo);
        console.debug(' contrato: ', contrato);
        console.debug(' codigoCorredor: ', codigoCorredor);
        console.debug(' productoId: ', productoId);
        //this.spinner.show();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.validarCorredorClienteContratoProducto(clienteCuit, clienteCodigo, contrato, codigoCorredor, productoId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                        // return false;
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        // return false;
                    } else {
                        let resultValidacion = result;
                        this.mensajeValidacionCorCliConPro = "";
                        console.debug(' resultValidacion: ', resultValidacion);
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
                        this.spinner.hide();
                        this.resultadoValidacionCorCliConPro = resultValidacion;
                        // return resultValidacion;
                    }
                },
                error => {
                    console.error(error.message);
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.spinner.hide();
                    // return false;
                }
            );
        } catch (err) {
            console.error(err.message);
            this.mensajeComponent.setErrorMsg(err);
            this.spinner.hide();
            return false;
        }
    }
}
