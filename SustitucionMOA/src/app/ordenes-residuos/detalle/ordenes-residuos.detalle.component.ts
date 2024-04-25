import { Component, OnInit, ViewChild } from "@angular/core";
import { BaseComponent } from "../../common/base-components/base-component";
import { BlockUI, NgBlockUI } from "ng-block-ui";
import { OrdenCargaResiduosDto } from "../../common/models/ordenes-residuos/ordenCargaResiduosDto";
import { Permiso } from "../../common/enums/Permisos";
import { OrdenesResiduosService } from "../ordenes-residuos.service";
import { NavService } from "../../common/services/NavService";
import { SecurityService } from "../../common/services/SecurityService";
import { FloatMsgService } from "../../common/services/FloatMsgService";
import { ModalService } from "../../common/services/ModalService";
import { ActivatedRoute, Params } from "@angular/router";
import { MensajeComponent } from "../../common/view-child/mensaje/mensaje.component";
import { SpinnerComponent } from "../../common/view-child/spinner/spinner.component";
import { ApiResponse } from "../../common/models/response";
import { SessionDataService } from "../../common/services/SessionDataService";
import { EstadoOrdenResiduosEnum } from "../../common/models/ordenes-residuos/estadoOrdenResiduos";
import { ConfirmationService } from "primeng/api";

@Component({
    selector: 'app-detalle',
    templateUrl: './ordenes-residuos.detalle.component.html',
    styleUrls: ['./ordenes-residuos.detalle.component.css', '../listado/ordenes-residuos.listado.component.css']
})
export class OrdenesResiduosDetalleComponent extends BaseComponent implements OnInit {

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(
        private route: ActivatedRoute,
        protected service: OrdenesResiduosService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService,
        private confirmationService: ConfirmationService) {
            super(navService, securityService, floatMsgService, modalService)
    }
        
    ordenResiduos: OrdenCargaResiduosDto = new OrdenCargaResiduosDto();
    esAdmin: boolean = this.isAuthorized(Permiso.ResiduosVerOrdenesDeCargaAdmin);
    esClienteResiduos = !this.esAdmin && this.isAuthorized(Permiso.ResiduosVerOrdenesDeCarga);
    
    validaCPEDG: boolean = false;
    estadoOrdenEntregada: EstadoOrdenResiduosEnum.OrdenEntregada;

    puedeAnular: boolean;
    puedeSolicitarAnulacion: boolean;
    puedeEditar: boolean;
    puedeResolverSolicitudAnulacion: boolean;
    puedeResolverSolicitudEdicion: boolean;
    puedeVerificarTransporte: boolean;
    
    ngOnInit() {
        let idOrden: number = 0;
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) { idOrden = params["id"]; }
        });

        this.navService.setSeccionList([]);
        if (idOrden) { this.obtenerOrdenDeCargaResiduos(idOrden); }
    }

    obtenerOrdenDeCargaResiduos(idOrden: number) {
        this.unsubscribe();
        this.subscriptionDropDowns = this.service.obtenerOrdenResiduos(idOrden).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.validaCPEDG = orden.Producto.ValidaSisaRuca;
                    this.verificarBotones();
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            }
        )
        this.ordenResiduos = new OrdenCargaResiduosDto();
        this.ordenResiduos.Id = idOrden;
    }

    editarOrdenResiduos() {
        this.goToSeccion('/ordenes-residuos/alta/' + this.ordenResiduos.Id);
    }

    verificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.blockUI.start("Procesando...");
        this.subscriptionDropDowns = this.service.verificarTransporte(this.ordenResiduos.Id).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.verificarBotones();
                }
            },
            (err) => {
                this.mensajeComponent.setErrorMsg(err.message);
            },
            () => { this.blockUI.stop(); }
        );
    }
    
    confirmarAnulacion() {
        this.confirmationService.confirm({
            key: 'confirmarAnular',
            message: '¿Desea anular la orden?',
            accept: () => {
                this.anularOrden();
            },
            reject: () => {}
        });
    }

    anularOrden() {
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('Procesando...');
        this.unsubscribe();
        this.service.anularOrdenResiduos(this.ordenResiduos.Id).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.verificarBotones();
                }
            },
            (err) => { this.mensajeComponent.setErrorMsg(err.message); },
            () => { this.blockUI.stop(); }
        );
    }

    confirmarSolicitudAnulacion() {
        this.confirmationService.confirm({
            key: 'confirmarSA',
            message: '¿Desea solicitar anulación?',
            accept: () => { this.solicitarAnulacion() },
            reject: () => {}
        });
    }

    solicitarAnulacion() {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.blockUI.start("Procesando...");
        this.service.solicitarAnulacion(this.ordenResiduos.Id).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.verificarBotones();
                }
            },
            (err) => { this.mensajeComponent.setErrorMsg(err.message); },
            () => { this.blockUI.stop(); }
        );
    }
    
    confirmarRechazarSolicitudAnulacion(aprobarSolicitud: boolean) {
        this.confirmationService.confirm({
            key: 'confirmarRSA',
            message: `¿Desea ${aprobarSolicitud ? "aprobar" : "rechazar"} la solicitud de anulación?`,
            accept: () => { this.resolverSolicitudAnulacion(aprobarSolicitud) },
            reject: () => {}
        });
    }

    resolverSolicitudAnulacion(aprobarSolicitud: boolean) {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        this.service.resolverSolicitudAnulacion(this.ordenResiduos.Id, aprobarSolicitud).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.verificarBotones();
                }
            },
            (err) => { this.mensajeComponent.setErrorMsg(err.message); },
            () => { this.blockUI.stop(); }
        );
    }

    confirmarRechazarSolicitudEdicion(aprobarSolicitud: boolean) {
        this.confirmationService.confirm({
            key: 'confirmarSolicitudEdicion',
            message: `¿Desea ${aprobarSolicitud ? 'aprobar' : 'rechazar'} la solicitud de anulación?`,
            accept: () => { this.resolverSolicitudEdicion(aprobarSolicitud) },
            reject: () => {}
        });
    }

    resolverSolicitudEdicion(aprobarSolicitud: boolean) {
        this.mensajeComponent.setMsgsEmpty();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        this.service.resolverSolicitudEdicion(this.ordenResiduos.Id, aprobarSolicitud).subscribe(
            (resp) => {
                let orden = this.manejarErroresApiResponse(resp);
                if (orden) {
                    this.ordenResiduos = orden;
                    this.verificarBotones();
                }
            },
            (err) => { this.mensajeComponent.setErrorMsg(err.message); },
            () => { this.blockUI.stop(); }
        );
    }
    
    verificarBotones() {
        let estadosPermitenEdicion = [
            EstadoOrdenResiduosEnum.OrdenGenerada,
            EstadoOrdenResiduosEnum.Pendiente,
            EstadoOrdenResiduosEnum.OrdenVencida];

        let estadosPermitenAnulacion = [
            EstadoOrdenResiduosEnum.OrdenGenerada,
            EstadoOrdenResiduosEnum.Pendiente,
            EstadoOrdenResiduosEnum.OrdenVencida];

        this.puedeEditar = estadosPermitenEdicion.includes(this.ordenResiduos.Estado.Id);

        this.puedeAnular = this.esAdmin && estadosPermitenAnulacion.includes(this.ordenResiduos.Estado.Id);
        
        this.puedeSolicitarAnulacion = this.esClienteResiduos && estadosPermitenAnulacion.includes(this.ordenResiduos.Estado.Id);

        this.puedeResolverSolicitudAnulacion = this.esAdmin && this.ordenResiduos.Estado.Id == EstadoOrdenResiduosEnum.AnulacionSolicitada;

        this.puedeResolverSolicitudEdicion = this.esAdmin && this.ordenResiduos.Estado.Id == EstadoOrdenResiduosEnum.EdicionSolicitada;

        this.puedeVerificarTransporte = this.esAdmin && this.ordenResiduos.Estado.Id == EstadoOrdenResiduosEnum.Pendiente;
    }

    manejarErroresApiResponse<T>(response: ApiResponse<T>): T | null {
        this.mensajeComponent.setMsgsEmpty();
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
