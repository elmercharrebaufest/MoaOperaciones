import { Component, Input, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { Table } from 'primeng/table';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Paginator } from 'primeng/paginator';
import { ComprasService } from '../../compras.service';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { Solp } from '../../solp/solp';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { NavService } from '../../../common/services/NavService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { SecurityService } from '../../../common/services/SecurityService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { Subscription } from 'rxjs';

declare var $: any;


@Component({
    selector: 'app-listado-dashboard-comprador',
    templateUrl: `listado-dashboard-comprador.component.html`,
    styleUrls: ['../../compras.component.css',
        './listado-dashboard-comprador.component.css']

})
export class ListadoDashboardCompradorComponent extends ListBaseComponent {

    protected locale: any;

    @ViewChild("tabla")
    protected tabla: Table;

    @BlockUI() blockUI: NgBlockUI;

    @Input('model')
    protected model: Solp;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    nroSolp: string = "";
    sap: boolean = false;
    mantenimiento: boolean = false;
    web: boolean = false;
    orden: string;
    columnaOrden: string;
    length = 0;
    pageSize: number = 10;
    pageIndex: number = 1;
    @ViewChild('paginator') paginator: Paginator
    subscripcionSolp: Subscription

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");

    }
    //#region Variables 
    display: boolean = false;
    tablaSolp: any[];
    tablaSolpCopy: any[];
    cols: any[];
    serviciosDashboard: any = "Servicios"
    solp: Solp = new Solp();
    usuario: string;// = "Prueba";
    checkedFilterSap = false;
    checkedFilterMantenimiento = false;
    checkedFilterWeb = false;
    verTodas: boolean = this.isAuthorized('VER TODAS SOLPS');
    //#endregion

    showDialog() {
        this.display = true;
    }
    ngOnInit() {
        this.getListarSolp();
    }

    ngOnDestroy(): void {
        this.subscripcionSolp.unsubscribe();
    }

    getListarSolp() {
        try {
            this.spinnerComponent.showIt();
            this.subscripcionSolp = this.service.observableListaSolp.subscribe(
                (result: any) => {
                    
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.tablaSolp = result.data;
                        this.length = result.data.length > 0 ? result.data[0].ItemsTotales : result.data.length;
                        this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
                        this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;
                      
                    }
                    this.spinnerComponent.hideIt()
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt()
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerComponent.hideIt()
            return false; //<-- Prevent Refresh
        }
   
        return false; //<-- Prevent Refresh
    }


    listarSolp() {
        this.spinnerComponent.showIt();
        this.service.getListarSolpCompras(this.pageIndex, this.pageSize, this.orden, this.columnaOrden, this.nroSolp);
  
    }

    onOrder(columna: string) {
        if (this.columnaOrden != columna) {
            this.orden = "DESC"
        } else {
            this.orden = this.orden == "DESC" ? "ASC" : "DESC";
        }
        this.columnaOrden = columna;
        this.listarSolp();
    }

    handlePageEvent(e: any) {
        this.pageSize = e.rows;
        this.pageIndex = e.page + 1;       
        this.listarSolp();      

    }

}

