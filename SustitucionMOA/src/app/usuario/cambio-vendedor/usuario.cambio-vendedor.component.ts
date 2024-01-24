import { Component, ViewChild, OnInit } from '@angular/core';
import { UsuarioService } from './../usuario.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import { BuscadorService } from '../../common/shared-components/buscador/buscador.service';

@Component({
    selector: 'app-usuario-cambio-vendedor',
    templateUrl: `usuario.cambio-vendedor.component.html`,
    providers: [UsuarioService]
})
export class UsuarioCambioVendedorComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected readonly buscadorService: BuscadorService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    orderedByColumn: string = "id";
    orderDirection: number = 1;
    itemsPerPage = 20;
    filtroUsuarioVendedor: string = "";

    setTabs() {
        this.setMenuSeccionTab('usuario', 'Cambio Vendedor');
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("SELECCIONAR VENDEDOR");
        this.navService.setSeccionList([]);
        this.getVendedores();
    }

    getVendedores() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getVendedores().subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data.usuarios;
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

    isVisible() {
        return this.data && this.data.length != 0;
    }

    orderColumnBy(column: string) {
        if (column == this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }

    seleccionarVendedor(vendedor: string, descripcion: string) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.buscadorService.limpiarBuscador();
        try {
            this.unsubscribe();
            this.subscription = this.service.seleccionarVendedor(vendedor, descripcion).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        sessionStorage.setItem("proveedor", result.vendedor);
                        this.sessionDataService.setProveedor(result.vendedor);
                        sessionStorage.setItem("nombre", result.descripcion);
                        this.sessionDataService.setNombre(result.descripcion);
                        sessionStorage.setItem("noticias", JSON.stringify(result.noticias));
                        this.sessionDataService.setNoticias(result.noticias);
                        sessionStorage.setItem("esCodigoCorredor", result.esCodigoCorredor);
                        this.sessionDataService.setEsCodigoCorredor(result.esCodigoCorredor);

                    }
                },
                error => {
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