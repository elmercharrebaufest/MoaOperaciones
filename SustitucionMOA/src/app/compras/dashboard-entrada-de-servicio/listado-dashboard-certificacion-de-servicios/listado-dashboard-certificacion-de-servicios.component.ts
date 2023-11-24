import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Table } from 'primeng/table';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { Paginator } from 'primeng/paginator';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';

@Component({
  selector: 'app-listado-dashboard-certificacion-de-servicios',
  templateUrl: './listado-dashboard-certificacion-de-servicios.component.html',
  styleUrls: ['./listado-dashboard-certificacion-de-servicios.component.css']
})
export class ListadoDashboardCertificacionDeServiciosComponent extends ListBaseComponent {

    protected locale: any;

    @ViewChild("tabla")
    protected tabla: Table;

    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    nroSolp: string = "";
    ordenAscendente: boolean;
    columnaOrden: string;
    fechaInicio =  "";
    length = 0;
    pageSize: number = 10;
    pageIndex: number = 1;                                        
    @ViewChild('paginator') paginator: Paginator
    subscripcionPO: Subscription
    itemSelected: any;
    ordenCompraId: string = "";

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");
        this.vendedor = sessionStorage.getItem("proveedor");
    }

    //#region Variables 
    tablaPO: any[];
    cols: any[];
    usuario: string;
    vendedor: string;
    //#endregion

    ngOnInit() {
        this.getListarPO('', '');

        //TO DO: Lógica para mostrar secciones en la pantalla
        // this.loadData();
    
    //     this.checkPermisos();
    //     this.setTabs();
    //     this.securityService.esNoGranosRedirect();

    //     const currentUrl = this.router.url;

    //     if (currentUrl === '/compras/dashboardCertificacionDeServicios') {
    //       this.checkPermisos();
    //     }

    //     this.navService.setSeccionList([new Seccion('/compras/dashboardCertificacionDeServicios', 'compras', 'Ingresar certificación')]);
    //   }
    
    //   checkPermisos() {
    //   }

    //   setTabs() {
    //     this.setMenuSeccionTab("compras", "dashboardCertificacionDeServicios");
    }

    ngOnDestroy(): void {
        this.subscripcionPO.unsubscribe();
    }

    getListarPO(proveedor, ordenCompraId) {
        this.getFecha();
        try {
            this.spinnerComponent.showIt();
            this.unsubscribe();
            this.subscripcionPO = this.service.getByProveedor(this.fechaInicio, proveedor, ordenCompraId, this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
                  (result:any) => {
                    if (result.logout == true) {
                      this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                        this.tablaPO = result.data; 
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

    getFecha() {
        var fechaActual = new Date();
        fechaActual.setDate(fechaActual.getDate() - 2);
        this.fechaInicio = fechaActual.toISOString().slice(0, 10);
    }

    onOrder(columna: string) {
        if (this.columnaOrden != columna) {
            this.ordenAscendente = false
        } else {
            this.ordenAscendente = this.ordenAscendente == false ? true : false;
        }
        this.columnaOrden = columna;
        this.getListarPO('', '');
    }

    handlePageEvent(e: any) {
        this.pageSize = e.rows;
        this.pageIndex = e.page + 1;
        this.getListarPO('', '');
    }
}