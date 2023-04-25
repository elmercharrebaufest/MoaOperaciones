import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { EmpresaGranosService } from '../../alta-proveedores/empresa-granos/empresa-granos.service';
import { BaseComponent } from '../../common/base-components/base-component';
import { Material } from '../../common/models/material';
import { CuitValidaExistencia, CuitValidaRUCA, CuitValidaSISA, OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
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
import { ContratoOrdenFas } from '../../common/models/ordenes-de-carga/obtenerContratosDisponiblesResponse';
import { finalize } from 'rxjs/operators';

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
    mensajesOrdenDeCarga: Partial<Record<keyof OrdenDeCarga, string>> = {};
    validando: Partial<Record<keyof OrdenDeCarga, boolean>> = {};
    displayModal: Partial<Record<keyof OrdenDeCarga, boolean>> = {};
    validaCPEDG = false;
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
    private selectUndefinedOptionValue: any;

    contratosDisponibles: ContratoOrdenFas[] = [];
    contratoSeleccionado: ContratoOrdenFas;
    listaMateriales: Material[];
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esAdmin: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    listaClientes: any[];
    noEditarCliente: boolean = false;

    puedeEditarContrato: boolean = false;
    resultadoValidacionCorCliConPro: boolean = false;
    mensajeValidacionCorCliConPro: string = "";

    listaCorredores: any[];
    corredorSeleccionado: any;
    public patternPatente = { '0': { pattern: new RegExp('\[a-zA-Z0-9\]') } };

    constructor(protected service: OrdenesDeCargaService, protected usuarioService: UsuarioService, protected navService: NavService, protected seleccionarProveedorService: SeleccionarProveedorService, private route: ActivatedRoute, protected sessionDataService: SessionDataService, protected securytiService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected empresaGranosService: EmpresaGranosService, public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.userEmail = sessionStorage.getItem("username");
        this.desde = this.getFecha(12);
        this.hasta = this.getFecha(0);
        this.ordenDeCarga.Cantidad = 30000;
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
        });
        this.navService.setSeccionList([]);
        this.onCorredorFocusOut('', false);
        this.obtenerMateriales();
        if (this.esCorredor) {
            this.CodigoCorredor = sessionStorage.getItem("proveedor");
            if (this.ordenDeCargaId == 0) {
                this.cargarClientes(this.CodigoCorredor);
            }
        }

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
            this.clienteCodigo = sessionStorage.getItem("proveedor");
            if (this.ordenDeCargaId == 0 && !(this.esComercial || this.esCorredor)) {
                this.cargarContratosDisponibles(this.clienteCodigo);
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
            this.subscription = this.service.getMateriales().subscribe(
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
        if (this.ordenDeCarga.CUITIntermediarioFlete && this.ordenDeCarga.CUITIntermediarioFlete.toString().trim().length != 11) {
            this.mensajeComponent.setInfoMsg("Ingrese un CUIT de intermediario flete válido.");
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
                        this.obtenerMateriales();
                        this.ordenDeCarga = result.data;
                        this.CodigoCorredor = result.data.CodigoCorredor;
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
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Grabando...');
        try {
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
            } else {
                this.subscription = this.service
                    .agregar(this.ordenDeCarga)
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
        } catch (e) {
            console.error(e);
            this.mensajeComponent.setErrorMsg(e);
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
                    },
                    (error) => {
                        console.error(' onCorredorFocusOut: ', error.message);
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
                this.subscription = this.service.getMateriales().subscribe(
                    (result) => {
                        this.listaMateriales = result.data;
                    },
                    (error) => {
                        console.error(error);
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );
            }
        } catch (err) {
            console.error(' onCorredorFocusOut: ', err);
            this.mensajeComponent.setErrorMsg(err);
        }
    }

    cargarClientes = (codigoCorredor: string) => {
        this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        try {

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
                },
                error => {
                    console.error(' cargarClientes: ', error.message);
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (err) {
            console.error(' cargarClientes: ', err);
            this.mensajeComponent.setErrorMsg(err);
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
            }
            else {
                this.clienteCUIT = this.clienteSeleccionado.CUIT;
                pClienteCodigo = this.clienteSeleccionado.CodigoProveedor;
            }
        }
        this.ordenDeCarga.CUITCliente = Number(this.clienteCUIT);
        this.getPatentes();
        this.cargarContratosDisponibles(pClienteCodigo);
    }

    onContratoSeleccionadoChanged = () => {
        if (this.ordenDeCarga.ContratoSeleccionado) {
            this.Contrato = this.ordenDeCarga.ContratoSeleccionado.NumeroContrato;
            this.ordenDeCarga.ContratoIngresado = this.ordenDeCarga.ContratoSeleccionado.NumeroContrato;
            this.ordenDeCarga.Producto_Id = this.ordenDeCarga.ContratoSeleccionado.Producto.MaterialId;
            this.Producto = this.ordenDeCarga.ContratoSeleccionado.Producto.MaterialId.toString();
            // this.validaCPEDG = this.ordenDeCarga.ContratoSeleccionado.Producto.MaterialId > 6;
            let materialSeleccionado = this.listaMateriales.find(mat => mat.MaterialId === this.ordenDeCarga.Producto_Id);
            this.validaCPEDG = (materialSeleccionado != undefined && materialSeleccionado.ValidaSisaRuca);
        }
        else {
            this.Contrato = "";
            this.ordenDeCarga.ContratoIngresado = "";
            this.ordenDeCarga.Producto_Id = this.selectUndefinedOptionValue;
            this.validaCPEDG = false;
        }
        this.validarSisaCorredorCliente()
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


    obtenerCorredores() {
        //this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
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
                            let seleccionado = result.data.vendedores.filter(a => a.idVendedor == this.CodigoCorredor);
                            if (seleccionado != null && seleccionado.length > 0) {
                                this.corredorSeleccionado = seleccionado[0];
                            }
                        }
                        //this.blockUI.stop();
                    }
                },
                error => {
                    console.error(' cargarClientes: ', error.message);
                    this.mensajeComponent.setErrorMsg(error.message);
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
        if (this.ordenDeCarga && this.ordenDeCarga.Id) {
            this.contratosDisponibles = [this.ordenDeCarga.ContratoSeleccionado];
            this.onContratoSeleccionadoChanged();
            return;
        }
        // this.mensajeComponent.setMsgsEmpty();
        this.contratosDisponibles = [];
        this.contratoSeleccionado = null as any;
        try {
            if (pClienteCodigo) { //this.clienteSeleccionado) { //|| this.corredorSeleccionado) {
                //let pClienteCodigo = this.clienteSeleccionado.CodigoProveedor;
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
                            } else
                                if (resp.Info) {
                                    this.mensajeComponent.setInfoMsg(resp.Info);
                                } else {
                                    this.contratosDisponibles = resp.Contratos;
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
        if (!this.revisarCUITFormatoValido(cuit)) return
        this.validando[campo] = true;
        this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = null;
        this.mensajesOrdenDeCarga[campo] = null;
        this.service.validarExisteCuitScato(cuit).pipe(finalize(() => {
            this.validando[campo] = false;
        })).subscribe(
            result => {
                if (result.logout) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);

                } else {
                    if (!result.data.Existe) {
                        this.mensajesOrdenDeCarga[campo] = `La cuit ${cuit} no se encuentra registrada`;
                        this.displayModal[campo] = true;
                    } else {

                        this.ordenDeCarga[campo.replace("CUIT", "RazonSocial")] = result.data.RazonSocial;
                        this.validarSisaCuit(cuit, campo)
                    }
                }
            })
    }
    validarSisaCuit(cuit: string, campo: CuitValidaSISA) {
        this.service.validarSisaCuit(cuit).subscribe(result => {
            if (result.logout) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.info != undefined) {
                this.mensajeComponent.setInfoMsg(result.info);

            } else {
                if (!result.data) {
                    this.mensajesOrdenDeCarga[campo] = `El ${campo.replace("CUIT", "")} no se encuentra habilitado en SISA`;
                } else {
                    if (campo == "CUITDestino") {
                        this.obtenerPlantasYDomicilios();
                    }
                    else if (campo !== "CUITCorredor") {
                        this.validarRuca(cuit, campo)
                    }
                }
            }
        })
    }
    revisarCUITFormatoValido(cuit: string): boolean {
        return cuit && cuit.length == 11 && !Number.isNaN(cuit as unknown as number)
    }
    gestionarAltaCUIT(campo: CuitValidaExistencia) {
        const cuit = this.ordenDeCarga[campo];
        this.mensajesOrdenDeCarga[campo] = `Se solicitó la gestión del alta para la cuit: ${cuit}`;
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
                .subscribe(resp => {
                    if (resp.logout) {
                        this.sessionDataService.logout();
                    } else {
                        if (resp.error) {
                            this.mensajeComponent.setErrorMsg(resp.error);
                        } else {
                            if (resp.info) {
                                this.mensajeComponent.setInfoMsg(resp.info)
                            }
                            let data = resp.data;
                            if (!data.CorredorHabilitadoEnSisa || !data.ClienteHabilitadoEnSisa) {
                                if (!data.CorredorHabilitadoEnSisa && !data.ClienteHabilitadoEnSisa) {
                                    this.mensajeComponent.setErrorMsg("Corredor y Cliente no están habilitados en SISA, no podrá cargar la orden hasta regularizar la situación");
                                }
                                else {
                                    if (!data.CorredorHabilitadoEnSisa) {
                                        this.mensajeComponent.setErrorMsg("Corredor no habilitado en SISA, no podrá cargar la orden hasta regularizar la situación");
                                    } else {
                                        this.mensajeComponent.setErrorMsg("Cliente no habilitado en SISA, no podrá cargar la orden hasta regularizar la situación");
                                    }
                                }
                                this.ordenDeCarga.ContratoSeleccionado = undefined;
                                this.ordenDeCarga.Producto_Id = 0;
                                this.Producto = "";
                            }
                        }
                    }
                    this.blockUI.stop();
                })
        } catch (err) {
            console.error('validarSisaCorredorCliente: ', err);
            this.mensajeComponent.setErrorMsg(err);
            this.blockUI.stop();
        }
    }
    validarRuca(cuit: string, campo: CuitValidaRUCA) { }
    obtenerPlantasYDomicilios() { }
    descripcionIntermediarioFlete = "Texto descriptivo de lo que representa el campo CUIT Intermediario Flete"
    descripcionTransporte = "Texto descriptivo de lo que representa el campo CUIT Transporte"
}


