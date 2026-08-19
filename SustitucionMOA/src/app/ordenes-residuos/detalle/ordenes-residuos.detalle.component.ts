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
    puedeEditar: boolean;
    puedeVerificarTransporte: boolean;
    camionHaIngresadoAPlanta: boolean = false;
    
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
                    if (orden.DestinoMercaderia) {
                        orden.DestinoMercaderia.DescripcionCompleta = `${orden.DestinoMercaderia.LocalidadDescripcion} (${orden.DestinoMercaderia.ProvinciaDescripcion})`;
                    }
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
        if (this.esAdmin) {
            this.goToSeccion('/ordenes-residuos/alta/' + this.ordenResiduos.Id);
        }
        else {
            this.service.camionOrdenEstaEnPlanta(this.ordenResiduos.Id).subscribe(
                (resp) => {
                    const camionEstaEnPlanta = this.manejarErroresApiResponse(resp);
                    if (camionEstaEnPlanta === true) {
                        this.floatMsgService.setErrorMsg("La orden no se puede editar por estar el camión en planta");
                        this.camionHaIngresadoAPlanta = true;
                        this.puedeEditar = false;
                    }
                    else {
                        this.goToSeccion('/ordenes-residuos/alta/' + this.ordenResiduos.Id);
                    }
                },
                (err) => { this.mensajeComponent.setErrorMsg(err.message) }
            );
        }
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
        let msj = "¿Desea anular la orden?";
        this.service.camionOrdenEstaEnPlanta(this.ordenResiduos.Id).subscribe(
            (resp) => {
                const camionEstaEnPlanta = this.manejarErroresApiResponse(resp);
                if (camionEstaEnPlanta === true) {
                    this.camionHaIngresadoAPlanta = true;
                    this.puedeEditar = false;
                    msj = "La orden no se puede anular por estar activa en Scato.";
                }
                // else {
                //     this.goToSeccion('/ordenes-residuos/alta/' + this.ordenResiduos.Id);
                // }
                this.confirmationService.confirm({
                    key: 'confirmarAnular',
                    message: msj,
                    accept: () => {
                        this.anularOrden();
                    },
                    reject: () => { }
                });
            },
            (err) => { this.mensajeComponent.setErrorMsg(err.message) }
        );
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
                else {
                    this.puedeAnular = false;
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

        this.puedeEditar = !this.camionHaIngresadoAPlanta && estadosPermitenEdicion.includes(this.ordenResiduos.Estado.Id);

        this.puedeAnular = !this.camionHaIngresadoAPlanta && estadosPermitenAnulacion.includes(this.ordenResiduos.Estado.Id);
        
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
