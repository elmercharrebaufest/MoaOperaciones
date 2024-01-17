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

@Component({
  selector: 'app-listado-estado-certificaciones',
  templateUrl: './listado-estado-certificaciones.component.html',
  styleUrls: ['./listado-estado-certificaciones.component.css']
})
export class ListadoEstadoCertificacionesComponent extends ListBaseComponent {
  
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
  documentoNumero: string = "";
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
  selectedRow: any;

  opcionesDropdown = ['Opción 1', 'Opción 2', 'Opción 3'];

  constructor(protected service: ComprasService, protected navService: NavService,
      protected sessionDataService: SessionDataService, protected securityService: SecurityService,
      protected floatMsgService: FloatMsgService, protected modalService: ModalService,
      protected route: ActivatedRoute, protected router: Router,
      private location: Location) {
      super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
      this.usuario = sessionStorage.getItem("username");
  }

  //#region Variables 
  tablaPO: any[];
  cols: any[];
  usuario: string;
  vendedor: string;
  allItems : any[];
  proveedor: string = "";
  
      
    ngOnInit() {
      this.getListarPO(this.proveedor, this.documentoNumero);

      this.navService.setSeccionActive("Estado certificaciones");

      this.navService.navegarSeccion("compras/listadoEstadoCertificaciones");
    }
  
    toggleTable(data: any) {
  
      const index = this.posicionRow.indexOf(data);
      if (index === -1) {
          this.posicionRow.push(data);
          this.isTableExpanded = !this.isTableExpanded;
      } else {
          this.posicionRow.splice(index, 1);
          this.isTableExpanded = false;
      }
  

    }
  
    toggleEntradaServicio() {
      this.isEntradaDeServicioExpanded = !this.isEntradaDeServicioExpanded;
    }
  
    onCheckboxChange(e: any) {
  
    }
  
  
    ngOnDestroy(): void {
        // this.subscripcionPO.unsubscribe();
    }
  
    getListarPO(proveedor, documentoNumero) {
      this.getFecha();
      try {
          this.spinnerComponent.showIt();
          this.unsubscribe();
          // this.subscripcionPO = this.service.getByProveedor("2023-01-28", proveedor, "4123001336", this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
          this.subscripcionPO = this.service.getByProveedorAsync(this.fechaInicio, proveedor, this.documentoNumero, this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
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
    //fechaActual.setDate(fechaActual.getDate() - 2);
    //TODO: cambiar cuando se agregue el filtro de fecha.
    //actualizar
    fechaActual.setMonth(fechaActual.getMonth() - 2);
    this.fechaInicio = fechaActual.toISOString().slice(0, 10);
}

  
    deleteES(item: any) {
      //TODO: lógica para cuando se especifique el borrado de una ES
    }
  
    handlePageEvent(e: any) {
        this.pageSize = e.rows;
        this.pageIndex = e.page + 1;
        this.getListarPO(this.proveedor, this.documentoNumero);
    }

    onOrder(columna: string) {
      if (this.columnaOrden != columna) {
          this.ordenAscendente = false
      } else {
          this.ordenAscendente = this.ordenAscendente == false ? true : false;
      }
      this.columnaOrden = columna;
      this.getListarPO(this.proveedor, this.documentoNumero);
  }

  toggleRow(rowData: any): void {
      this.selectedRow = this.selectedRow === rowData ? null : rowData;
  }

  isSelectedRow(rowData: any): boolean {
      return this.selectedRow === rowData;
  }
}