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
    
    estadoSelected: string = "Todos";
    productoSelected: string = "";
    listaProductos: any = null;


    esInterno: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA DE TERCEROS');
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esMesaFas: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA MESA FAS');
    esPuerto: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA PUERTO');

    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";

    descripcionEstadoOrdenCarga = [
        { label: "Todos", value: 0 },
        { label: "Pendiente", value: 1 },
        { label: "Confirmado", value: 2 },
        { label: "Pendiente aprobación crédito", value: 3 },
        { label: "Entrega generada", value: 4 },
        { label: "Anulada", value: 5 },
        { label: "Entregada", value: 6 },
        { label: "Vencida", value: 7 },
        { label: "Entrega pendiente", value: 8 },
        { label: "Anulada por vencimiento", value: 9 }
    ] 

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.getListado();
        this.obtenerMateriales();
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

    filtroFechasOrdenCarga() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListadoFiltradoOrdenCarga(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(
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


    

    getListado() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListado().subscribe(
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
