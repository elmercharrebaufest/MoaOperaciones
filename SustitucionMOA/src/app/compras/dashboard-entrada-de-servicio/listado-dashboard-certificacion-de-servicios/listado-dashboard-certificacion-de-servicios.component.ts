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
import { Seccion } from '../../../common/models/seccion';
import { Location } from '@angular/common';
import { ModalAltaEntradaDeServicioComponent } from '../modal-alta-entrada-de-servicio/modal-alta-entrada-de-servicio.component';
import { throwError as observableThrowError, Observable } from 'rxjs';
//import { FiltroFechaComponent } from './../../common/view-child/filtro-fecha/filtro-fecha.component';
import { FiltroFechaComponent } from './../../../common/view-child/filtro-fecha/filtro-fecha.component';

@Component({
  selector: 'app-listado-dashboard-certificacion-de-servicios',
  templateUrl: './listado-dashboard-certificacion-de-servicios.component.html',
  styleUrls: ['./listado-dashboard-certificacion-de-servicios.component.css']
})
export class ListadoDashboardCertificacionDeServiciosComponent extends ListBaseComponent {

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router,
        private location: Location) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");
        this.vendedor = sessionStorage.getItem("proveedor");
        this.filtroFechaComponent = new FiltroFechaComponent();
    }

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    protected locale: any;

    @ViewChild("tabla")
    protected tabla: Table;

    @ViewChild("myModal") modal: ModalAltaEntradaDeServicioComponent;

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
    ordenCompraId: string = "";
    expandedRows: any[] = [];
    posicionRow: any[] = [];
    isTableExpanded = false;
    isTableItemsExpanded = false;
    isEntradaDeServicioExpanded = false;
    selectedItemIndex: number | null = null;
    checkSelected = false;
    itemIdSelected: Set<string> = new Set();
    itemSelected: any[] = [];
    selectedItemId: number | null = null;
    selectedPosicionId: number | null = null;
    ordenCompraIdsMostradas: Set<number> = new Set<number>();

    expandedRow: any; 

 
    
    //#region Variables 
    tablaPO: any[];
    cols: any[];
    usuario: string;
    vendedor: string;
    allItems : any[];
    proveedor: string = "";
    showModal: boolean = false;

    
    ngOnInit() {       
    
      this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');


    if (this.location.path() === '/compras/dashboardCertificacionDeServicios') {
      this.navService.navegarSeccion("/compras/dashboardCertificacionDeServicios");
    }
    else {
        this.validarLoginAzure();
    }

    this.navService.setSeccionList(
      [
          new Seccion('compras/dashboardCertificacionDeServicios', 'Compras', 'Ingresar certificación'),
          new Seccion('compras/listadoEstadoCertificaciones', 'Compras', 'Estado certificaciones')
      ]
    );

        this.navService.navegarSeccion("compras/dashboardCertificacionDeServicios");
        this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio);

    }

  validarLoginAzure() {
    throw new Error('Method not implemented.');
  }
    //#endregion

  toggleRow(posicion: any) {
    this.ordenCompraIdsMostradas.clear();

    posicion.forEach((item: any) => {
      if (!this.ordenCompraIdsMostradas.has(item.OrdenCompraId)) {
        this.ordenCompraIdsMostradas.add(item.OrdenCompraId);
      } else {
      }
    });
  }

  toggleTable(posicionId: number) {
    if (this.selectedPosicionId === posicionId) {
        this.selectedPosicionId = null;
        this.selectedItemId = null;
        this.isTableExpanded = false;
        this.isTableItemsExpanded = false;
    } else {
        this.selectedPosicionId = posicionId;
        this.isTableExpanded = true;
        this.isTableItemsExpanded = false;
        this.selectedItemId = null;
    }
  }

  toggleTableItems(itemId: number) {
    if (this.selectedItemId === itemId) {
        this.selectedItemId = null;
        this.isTableItemsExpanded = false;
    } else {
        this.selectedItemId = itemId;
        this.isTableItemsExpanded = true;
    }
  }

  toggleEntradaServicio() {
    this.isEntradaDeServicioExpanded = !this.isEntradaDeServicioExpanded;
  }

  onCheckboxChange(item: any) {
    const itemId = item.PosicionId;
    const numeroLinea = item.NumeroLinea;
  
    if (this.itemIdSelected.has(itemId && numeroLinea)) {
      this.itemIdSelected.delete(itemId);
  
      this.itemSelected = this.itemSelected.filter((selectedItem: any) => 
      selectedItem.PosicionId !== item.PosicionId || selectedItem.NumeroLinea !== item.NumeroLinea);
    } else {
      this.itemIdSelected.add(itemId);
      this.itemSelected.push(item);
    }
  }

    getOrders(periodo: string, fecha_inicio: string, fecha_fin: string){
      this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio);
    }

    //onBuscar() {
      
    //  this.getListarPO(this.proveedor, this.ordenCompraId,  this.filtroFechaComponent.fecha_inicio);
    //}

   


    getListarPO(proveedor, ordenCompraId,fecha_inicio) {
      this.getFecha();
      try {
          this.spinnerComponent.showIt();
          this.unsubscribe();
          // this.subscripcionPO = this.service.getByProveedor("2023-01-28", proveedor, "4123001336", this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
          this.subscripcionPO = this.service.getByProveedor(fecha_inicio, proveedor, ordenCompraId, this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
                (result:any) => {
                 
                  if (result.logout == true) {
                    this.sessionDataService.logout();
                  } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                  } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
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
      //fechaActual.setDate(fechaActual.getDate() - 2);
      //TODO: cambiar cuando se agregue el filtro de fecha.
      //actualizar
      fechaActual.setMonth(fechaActual.getMonth() - 2);
      this.fechaInicio = fechaActual.toISOString().slice(0, 10);
  }

  onOrder(columna: string) {
      if (this.columnaOrden != columna) {
          this.ordenAscendente = false
      } else {
          this.ordenAscendente = this.ordenAscendente == false ? true : false;
      }
      this.columnaOrden = columna;
      this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicio);
  }

  deleteES(item: any) {
    //TODO: lógica para cuando se especifique el borrado de una ES
  }

  handlePageEvent(e: any) {
      this.pageSize = e.rows;
      this.pageIndex = e.page + 1;
      this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicio);
  }

  openModal() {
    if (this.itemIdSelected.size > 0) {
      this.showModal = true;
    }
    else {
      this.showModal = false;
    }
  }

  onCloseModal() {
    this.showModal = false;
  }
}