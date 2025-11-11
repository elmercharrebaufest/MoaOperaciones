import { Component, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';
import { Material } from '../../common/models/material';
import { OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { ConfirmationService } from 'primeng/api';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { FiltroFechaFasComponent } from '../../common/view-child/filtro-fecha-fas/filtro-fecha-fas.component';
import { TipoContrato } from '../../common/models/ordenes-de-carga/obtenerContratosDisponiblesResponse';
import { SendDataService } from '../../consulta/send-data.service';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-ordenes-de-carga.listado',
    templateUrl: './ordenes-de-carga.listado.component.html',
    styleUrls: ['./ordenes-de-carga.listado.component.css']
})
export class OrdenesDeCargaListado extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;
    @ViewChild(FiltroFechaFasComponent)
    protected filtroFechaFasComponent: FiltroFechaFasComponent;


    tipoContrato = TipoContrato;
    listaMateriales: Material[];
    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    corredorCodigo: string = "";
    mailUsuarioSAP: string = "";
    tipoOperacion: string = 'Normal';

    filtroEstado: any = null;
    filtroProducto: any = null
    filtroAlta: any = null;
    filtroCliente: any = null;
    filtroCorredor: any = null;
    filtroPatenteChasis: any = null;

    estadoSelected: string = "Todos";
    estadosSelected: string[] = [];

    datosAux: any[];
    //primerListado: any[];
    listaEnviarASAP: number[] = [];

    productoSelected: string = "Todos";
    listaProductos: any = null;
    private selectUndefinedOptionValue: any;
    filtroTipoContrato?: TipoContrato = null;
    seleccionaTodos: boolean = false;

    esInterno: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS');
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esMesaFas: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA MESA FAS');
    esPuerto: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA PUERTO');

    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    //esCliente: boolean = sessionStorage.getItem("tipoUsuario") === "CLI";
    puedeEnviarASAP: boolean = this.isAuthorized('ENVIAR A SAP');

    descripcionEstadoOrdenCarga: any[];
    entregada: string = "Entregada";

    tipoConrato = TipoContrato;

    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;
    filtroFechaKey: string = 'NGOCFas_Periodo';

    constructor(
        protected service: OrdenesDeCargaService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        private confirmationService: ConfirmationService,
        private sendDataService: SendDataService,
        private route: ActivatedRoute
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {        
        this.corredorCodigo = sessionStorage.getItem("proveedor");
        this.mailUsuarioSAP = sessionStorage.getItem("username");
        if (this.esInterno || this.esComercial || this.esMesaFas || this.esPuerto) {
            this.descripcionEstadoOrdenCarga = [
                { label: "Pendiente", value: "Pendiente" },
                { label: "Confirmado", value: "Confirmado" },
                { label: "Pendiente aprobación crédito", value: "Pendiente aprobación crédito" },
                { label: "Entrega generada", value: "Entrega generada" },
                { label: "Anulada", value: "Anulada" },
                { label: "Vencida", value: "Vencida" },
                { label: "Entrega pendiente", value: "Entrega pendiente" },
                { label: "Anulada por vencimiento", value: "Anulada por vencimiento" },
                { label: "Anulación solicitada", value: "Anulación solicitada" },
                { label: "Edición solicitada", value: "Edición solicitada" },
                { label: "Error de datos", value: "Error de datos" },
                { label: "Contrato vencido", value: "Contrato vencido" },
                { label: "Edición rechazada", value: "Edición rechazada" },
                { label: "Sin Enviar a SAP", value: "Sin Enviar a SAP" },
                { label: "Entrega anulada, pedido pendiente de anulación", value: "Entrega anulada, pedido pendiente de anulación" },
                { label: "Pendiente de compensación", value: "Pendiente de compensación" },
                { label: "Entregada", value: "Entregada" },
            ]
        } else {
            this.descripcionEstadoOrdenCarga = [];
            this.estadosSelected = [];
            this.entregada = "Completada";
        }

        const parentRoute = this.route.parent;
        if (parentRoute && parentRoute.data) {
            parentRoute.data.subscribe((data: any) => {
                if (data && data.tipoOperacion) {
                    this.tipoOperacion = data.tipoOperacion;
                    console.log('Tipo de Operación:', this.tipoOperacion);
                }
            });
        }
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.getListado();
        this.obtenerMateriales();
        this.estadosSelected = this.service.estadosFiltrosSeleccionados;
    }
    setEstadosFiltroFAS() {
        this.service.setEstadosFiltro(this.estadosSelected)
    }
    filtrarListado() {
        //this.primerListado = this.datosAux.filter(x => x.DescripcionEstado != this.entregada);
        if (!this.esTercero) {
            if (this.estadosSelected.length < 1 || this.estadosSelected == null) {
                this.data = this.datosAux;
            } else {
                if (this.estadosSelected) {
                    this.data = this.datosAux.filter(x => this.estadosSelected.indexOf(x.DescripcionEstado) >= 0);
                }
            }
        } else {
            this.data = this.datosAux;
        }
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
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

    getListado(resultMessage?: string) {
        this.spinnerComponent.showIt();
        if (resultMessage != undefined) {
            this.mensajeComponent.setSuccessMsg(resultMessage);
        } else {
            this.mensajeComponent.setMsgsEmpty();
        }
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListado(
                this.filtroFechaFasComponent.fecha_inicio,
                this.filtroFechaFasComponent.fecha_fin,
                this.tipoOperacion).subscribe(
                    result => {
                        this.spinnerComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.data = result;
                            this.datosAux = result;
                            this.filtrarListado();
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
        this.spinnerComponent.hideIt();
        return false; //<-- Prevent Refresh
    }

    seleccionarTodos() {
        this.seleccionaTodos = !this.seleccionaTodos;
        if (this.data && this.seleccionaTodos) {
            this.data.forEach((item) => {
                if (item.NoEstaEnSAP && !this.estaListadoParaSAP(item))
                    this.seleccionarItem(item)
            });
        } else if (!this.seleccionaTodos)
            this.listaEnviarASAP = []
    }

    seleccionarItem(item) {
        if (!this.estaListadoParaSAP(item)) {
            this.listaEnviarASAP.push(item.Id);
        } else
            this.listaEnviarASAP = this.listaEnviarASAP.filter(ordenId => ordenId != item.Id)
    }


    estaListadoParaSAP(item): boolean {
        return this.listaEnviarASAP.indexOf(item.Id) !== -1
    }

    enviarASAP() {
        this.blockUI.start('');
        this.mensajeComponent.setMsgsEmpty();
        this.service.enviarOrdenesASAP(this.listaEnviarASAP).subscribe(
            result => {
                this.blockUI.stop();
                this.listaEnviarASAP = []
                if (result.logout) {
                    this.sessionDataService.logout();
                } else if (result.error) {
                    this.getListado();
                    this.mensajeComponent.setErrorMsg(result.error)
                } else if (result.info) {
                    this.getListado();
                    this.mensajeComponent.setInfoMsg(result.info)
                }
                else {
                    this.getListado(result.data);
                }
            },
        );
    }
    navegarAConsultas(idConsulta: number) {
        this.sendDataService.setDatoIdConsultaOrdenDeCarga(idConsulta);
        this.navService.navegarSeccionParam('consulta', 'mis-consultas')
    }
}
