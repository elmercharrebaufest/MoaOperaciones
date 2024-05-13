import { Component, OnInit, ViewChild } from "@angular/core";
import { BlockUI, NgBlockUI } from "ng-block-ui";
import { ConfirmationService } from "primeng/api";
import { ListBaseComponent } from "../../common/base-components/list-base-component";
import { Permiso } from "../../common/enums/Permisos";
import { Material } from "../../common/models/material";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { NavService } from "../../common/services/NavService";
import { SecurityService } from "../../common/services/SecurityService";
import { SessionDataService } from "../../common/services/SessionDataService";
import { MensajeComponent } from "../../common/view-child/mensaje/mensaje.component";
import { OrdenesResiduosService } from "../ordenes-residuos.service";
import { ApiResponse } from "../../common/models/response";
import { OrdenResiduosFila } from "../../common/models/ordenes-residuos/listarOrdenesResiduosResponse";


@Component({
    selector: 'app-listado',
    templateUrl: './ordenes-residuos.listado.component.html',
    styleUrls: ['./ordenes-residuos.listado.component.css']
})
export class OrdenesResiduosListadoComponent extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    productoSelected: string = "Todos";
    estadosSelected: string[] = [
        "Orden generada",
        "Pendiente",
        "Orden vencida",
        "Orden entregada",
        "Anulada",
        "En proceso"
    ];
    descripcionEstadoOrdenCarga: any[];
    listaProductos: Material[];
    listaClientes: string[] = [];
    filtroCliente: any = null;
    filtroPatente: any = null;
    datosSinFiltrar: OrdenResiduosFila[];
    
    esAdmin: boolean = this.isAuthorized(Permiso.ResiduosVerOrdenesDeCargaAdmin);
    esTercero: boolean = this.isAuthorized(Permiso.ResiduosVerOrdenesDeCarga);

    constructor(
        protected service: OrdenesResiduosService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService) {
        
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit(): void {
        this.setCombosFiltros();
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.obtenerListadoOrdenes();
        this.getProductos();
    }

    setCombosFiltros(): void {
        this.descripcionEstadoOrdenCarga =
            this.esAdmin ?
                [
                    { label: "Orden generada", value: "Orden generada" },
                    { label: "Pendiente", value: "Pendiente" },
                    { label: "Orden vencida", value: "Orden vencida" },
                    { label: "Orden entregada", value: "Orden entregada" },
                    { label: "Anulada", value: "Anulada" }
                ] :
                [
                    { label: "OK", value: "OK" },
                    { label: "Orden vencida", value: "Orden vencida" },
                    { label: "Orden entregada", value: "Orden entregada" },
                    { label: "En proceso", value: "En proceso" }
                ];

        this.estadosSelected =
            this.esAdmin ?
                [
                    "Orden generada",
                    "Pendiente",
                    "Orden vencida",
                    "Orden entregada",
                    "Anulada"
                ]
                : ["OK"];
    }
    
    setFiltroProducto(producto: string): void {
        this.productoSelected = producto;
    }
    
    getProductos(): void {
        this.subscription = this.service.getMateriales().subscribe(
            (resp) => {
                let respData = this.manejarErroresApiResponse(resp);
                if (respData) {
                    this.listaProductos = respData;
                }
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

    filtrarListado(): void {
        if (this.estadosSelected && this.estadosSelected.length >= 1) {
            this.data = this.datosSinFiltrar.filter(x =>
                this.estadosSelected.some(y => y == x.DescripcionEstado));
        }
        else {
            this.data = [];
        }
    }

    obtenerListadoOrdenes(): void {
        this.spinnerComponent.showIt();
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.unsubscribe();
            this.subscription = this.service.obtenerListadoOrdenes(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin)
                .subscribe(resp => {
                    let respData = this.manejarErroresApiResponse(resp);
                    if (respData) {
                        this.datosSinFiltrar = respData.ListaOrdenes;
                        this.filtrarListado();
                    }
                    this.spinnerComponent.hideIt();
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }
    }


    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        if (response.logout) {
            this.sessionDataService.logout();
            return null;
        }
        if (response.error) {
            this.mensajeComponent.setErrorMsg(response.error);
            return null;
        }
        if (response.info) {
            this.mensajeComponent.setInfoMsg(response.info);
        }
        return response.data || null;
    }
}

