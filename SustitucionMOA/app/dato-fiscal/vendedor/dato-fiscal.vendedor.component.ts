import { Component, ViewChild, OnInit } from '@angular/core';
import { DatoFiscalService } from './../dato-fiscal.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { Seccion } from './../../common/models/seccion';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'list',
    templateUrl: `./app/dato-fiscal/vendedor/dato-fiscal.vendedor.component.html?v=${new Date().getTime()}`,
    providers: [DatoFiscalService]
})
export class VendedoresListComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: DatoFiscalService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    orderedByColumn: string = "vendedor";
    orderDirection: number = 1;
    itemsPerPage = 20;
    filtroVendedor: string = "";
    filtroNroVendedor: string = "";

    setTabs() {
        this.setMenuSeccionTab("dato-fiscal", "Mis Vendedores");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR VENDEDORES");
        var secciones = [];
        if (this.isAuthorized("CONSULTAR DATOS FISCALES"))
            secciones.push(new Seccion('/dato-fiscal/situacion-fiscal', 'dato-fiscal', 'Mi Situacion Fiscal'));

        if (this.isAuthorized("CONSULTAR VENDEDORES") && this.isCorredor())
            secciones.push(new Seccion('/dato-fiscal/vendedor', 'dato-fiscal', 'Mis Vendedores'));

        if (this.isAuthorized("CONSULTAR DOCUMENTACION"))
            secciones.push(new Seccion('/dato-fiscal/documentacion', 'dato-fiscal', 'Documentacion'));

        this.navService.setSeccionList(secciones);
        this.getUsuario();
    }

    getUsuario() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.getVendedores("", "").subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data.vendedores;
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
}