import { Component, ViewChild, OnInit } from '@angular/core';
import { AltaEmpresaService } from './../alta-empresa.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import { Seccion } from './../../common/models/seccion';



@Component({
    selector: 'app-alta-empresa-list',
    templateUrl: `./app/alta-empresa/list/alta-empresa.list.component.html?v=${new Date().getTime()}`,
    providers: [AltaEmpresaService]
})
export class AltaEmpresaListComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: AltaEmpresaService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
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
        this.setMenuSeccionTab('alta-empresa', 'Listado Empresas');
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("ABM EMPRESAS");
        this.navService.setSeccionList([new Seccion('/alta-empresa/list', 'alta-empresa', 'Listado Empresas')]);
        this.getEmpresa();
    }

    getEmpresa() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        try {
            this.unsubscribe();
            this.subscription = this.service.getEmpresas().subscribe(
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
    
    cambiarEstado(empresaId: number, estadoId: number, observacion: string) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.setEstadoAprobacion(empresaId,estadoId,observacion).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getEmpresa();
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

    showModalEmpresaCambiarEstado(empresa: any, estadoid: number) {
        var title = "";
        if (estadoid == 0) {
            title = "Aceptar ";
        } else if (estadoid == 5) {
            title = "Rechazar ";
        } else if (estadoid == 4) {
            title = "Pedir Documentacion a ";
        }
        this.modalService.openModalEmpresaCambiarEstado(title + "Empresa", [
            { etiqueta: "ID", valor: empresa.id },
            { etiqueta: "Usuario", valor: empresa.usuario },
            { etiqueta: "Vendedor", valor: empresa.vendedor }
        ], estadoid);
        return false;
    }
}