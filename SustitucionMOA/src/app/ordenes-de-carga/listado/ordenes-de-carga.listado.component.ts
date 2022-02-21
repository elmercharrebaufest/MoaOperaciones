import { Component, OnInit } from '@angular/core';
import { CartaPorteService } from '../../carta-porte/carta-porte2.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';
import { Material } from '../../common/models/material';
import { OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { Formatter } from '../../common/formatter/Formatter';
import { SelectItem } from 'primeng/api';

@Component({
    selector: 'app-ordenes-de-carga.listado',
    templateUrl: './ordenes-de-carga.listado.component.html',
    styleUrls: ['./ordenes-de-carga.listado.component.css']
})
export class OrdenesDeCargaListado extends ListBaseComponent {

    constructor(protected service: OrdenesDeCargaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    listaMateriales: Material[];
    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();

    filtroEstado: any = null;
    filtroProducto: any = null;
    filtroAlta: any = null;
    filtroCliente: any = null;

    estadoSelected: string = "Todos";
    estadosSelected: string[] = [
        "Pendiente",
        "Confirmado",
        "Pendiente aprobación crédito",
        "Entrega generada",
        "Anulada",
        "Vencida",
        "Entrega pendiente",
        "Anulada por vencimiento",
        "Error de datos"
    ];

    estadosSelectedTercero: string[] = [
        "Pendiente de carga",
        "Anulada",
        "Vencida",
        "Sin estado",
        "Listo para retirar",
        "Completada"  
    ];



    datosAux: any[];

    productoSelected: string = "Todos";
    listaProductos: any = null;
    private selectUndefinedOptionValue: any;
    pedidoAnticipado: number = 0;

    esInterno: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS');
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esMesaFas: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA MESA FAS');
    esPuerto: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA PUERTO');

    
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";

    descripcionEstadoOrdenCarga: SelectItem[] =  [
        { label: "Vencida", value: "Vencida" },
        { label: "Pendiente de carga", value: "Pendiente de carga" },
        { label: "Listo para retirar", value: "Listo para retirar" },
        { label: "Completada", value: "Completada" },
        { label: "Anulada", value: "Anulada" },
        { label: "Sin estado", value: "Sin estado" }
    ]



    ngOnInit() {
        if (this.esInterno || this.esComercial || this.esMesaFas || this.esPuerto) {
            this.descripcionEstadoOrdenCarga = [
            { label: "Pendiente", value: "Pendiente" },
            { label: "Confirmado", value: "Confirmado" },
            { label: "Pendiente aprobación crédito", value: "Pendiente aprobación crédito" },
            { label: "Entrega generada", value: "Entrega generada" },
            { label: "Anulada", value: "Anulada" },
            { label: "Entregada", value: "Entregada" },
            { label: "Vencida", value: "Vencida" },
            { label: "Entrega pendiente", value: "Entrega pendiente" },
            { label: "Anulada por vencimiento", value: "Anulada por vencimiento" },
            { label: "Error de datos", value: "Error de datos" }
        ]}

        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.getListado();
        this.obtenerMateriales();
    }

    filtrarListado(){
        if (this.esInterno || this.esComercial || this.esMesaFas || this.esPuerto) {
            if(this.estadosSelected.length < 1 || this.estadosSelected == null){
                this.data = this.datosAux;
            } else {
                this.data = this.datosAux.filter(x => this.estadosSelected.indexOf(x.DescripcionEstado) >= 0);
            }
        } else {
            if(this.estadosSelectedTercero.length < 1 || this.estadosSelectedTercero == null){
                this.data = this.datosAux;
            } else {
                this.data = this.datosAux.filter(x => this.estadosSelectedTercero.indexOf(x.DescripcionEstado) >= 0);
            }
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

    getListado() {
        this.mensajeComponent.setMsgsEmpty();

        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListado(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                        this.datosAux = result.data;
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

        return false; //<-- Prevent Refresh
    }
}
