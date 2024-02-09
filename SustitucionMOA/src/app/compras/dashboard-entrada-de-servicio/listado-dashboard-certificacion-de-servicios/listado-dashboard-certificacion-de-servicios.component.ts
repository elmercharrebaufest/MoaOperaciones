import { Component, ViewChild } from '@angular/core';
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
import { FiltroFechaComponent } from './../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ConfirmationService } from 'primeng/api';
import { MensajeComponent } from '../../../common/view-child/mensaje/mensaje.component';
import { ProveedorModel } from '../../../modelos/proveedor-model';
import { forEach } from '@angular/router/src/utils/collection';



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
      private confirmationService: ConfirmationService,
      private location: Location) {
      super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
      this.usuario = sessionStorage.getItem("username");
      this.vendedor = sessionStorage.getItem("proveedor");
  }

  @ViewChild(FiltroFechaComponent)
  protected filtroFechaComponent: FiltroFechaComponent;

  protected locale: any;

  @ViewChild("tabla")
  protected tabla: Table;

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild("myModal") modal: ModalAltaEntradaDeServicioComponent;

  @BlockUI() blockUI: NgBlockUI;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  nroSolp: string = "";
  ordenAscendente: boolean;
  columnaOrden: string;
  fechaInicio = "";
  fechaFin = "";
  length = 0;
  pageSize: number = 10;
  pageIndex: number = 1;                                        
  @ViewChild('paginator') paginator: Paginator
  subscripcionPO: Subscription
  ordenCompraId: string = "";
  //MMSN-519
  expandedRows: boolean[] = [];
  posicionRow: any[] = [];
  isTableExpanded = false;
  isTableItemsExpanded = false;
  isEntradaDeServicioExpanded = false;
  selectedItemIndex: number | null = null;
  checkSelected = false;
  itemIdSelected: any[]= [];
  numeroLineaSelected: Set<string> = new Set();
  itemSelected: any[] = [];
  elementSelected: any[] = [];
  selectedItemId: number | null = null;
  selectedPosicionId: number | null = null;
  ordenCompraIdsMostradas: Set<number> = new Set<number>();
  mensajeError: string = "";
  filaExpandida: any;
  expandedRow: any;
  proveedorSeleccionado: any;
  proveedorModel: ProveedorModel;
  proveedorList: any[] = new Array();

  //#region Variables 
  tablaPO: any[];
  cols: any[];
  usuario: string;
  vendedor: string;
  allItems : any[];
  proveedor: string = "";
  showModal: boolean = false;

  //Filtros
  ocFilterValues: string[] = [];
  tablaPOCopy: any[] = []; //copia de la tabla original.
    
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
    this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin);
  }

    ngAfterViewInit() {
        this.mensajeComponent = new MensajeComponent();
    }

  validarLoginAzure() {
    throw new Error('Method not implemented.');
  }

  isVisibleError() {
    return this.mensajeError != "";
}

  toggleEntradaServicio() {
    this.isEntradaDeServicioExpanded = !this.isEntradaDeServicioExpanded;
  }

  onCheckboxChange(item: any) {
    const itemId = item.PosicionId;
    const numeroLinea = item.NumeroLinea;
    if (this.itemIdSelected.includes(itemId) && this.numeroLineaSelected.has(numeroLinea)) {
      this.itemIdSelected.splice(this.itemIdSelected.indexOf(itemId), 1);
      this.numeroLineaSelected.delete(numeroLinea);
      
      this.itemSelected = this.itemSelected.filter((selectedItem: any) => 
      selectedItem.PosicionId !== item.PosicionId || selectedItem.NumeroLinea !== item.NumeroLinea);
    } else {
      this.itemIdSelected.push(itemId);
      this.numeroLineaSelected.add(numeroLinea);
      this.itemSelected.push(item);
    }
  }

  clearCheckboxes(): void {
    this.numeroLineaSelected.clear();
    this.itemSelected.forEach(( item: any ) => { item.isSelected = false });
    this.itemIdSelected.splice(0, this.itemIdSelected.length);
    this.itemSelected.splice(0, this.itemSelected.length);
  }
  
  //MMSN-519
  toggleRow(rowData: any) {
    this.clearCheckboxes();
      //MMSN-519 - Al activar un filtro, colapsar filas expandidas.
    this.expandedRow = rowData;
  }

    //MMSN-574 - Agregar filtros
    onBuscar() {
        this.collapseExpanded();

        if (this.proveedorSeleccionado !== undefined && this.proveedorSeleccionado !== '') {
            this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
        }
        else {
            this.proveedor = '';
        }

        let startDate = this.filtroFechaComponent.fecha_inicio;
        let endDate = this.filtroFechaComponent.fecha_fin;

        this.getListarPO(this.proveedor, this.ordenCompraId, startDate, endDate);
    }

    //getOrders(periodo: string, fecha_inicio: string, fecha_fin: string) {
    //    //MMSN-519 - Al activar un filtro, colapsar filas expandidas.
    //    this.collapseExpanded()

    //  this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio);
    //}

    //MMSN-519 - Al activar un filtro, colapsar filas expandida
    collapseExpanded() {
      if (this.expandedRow != undefined) {
        if (this.tabla.isRowExpanded(this.expandedRow)) {
          this.tabla.toggleRow(this.expandedRow);
        }           
      }
    }

    getListarPO(proveedor, ordenCompraId,fecha_inicio, fecha_fin) {
      this.getFecha();
      try {
          this.spinnerComponent.showIt();
          this.unsubscribe();
          // this.subscripcionPO = this.service.getByProveedor("2023-01-28", proveedor, "4123001336", this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
          this.subscripcionPO = this.service.getByProveedor(fecha_inicio, fecha_fin, proveedor, ordenCompraId, this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
                (result:any) => {
                 
                  if (result.logout == true) {
                    this.sessionDataService.logout();
                  } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                  } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                  } else {
                    this.tablaPO = result.data;
                    this.tablaPOCopy = result.data.slice(); // Clon del objeto inicial para revertir los valores al limpiar el filtro.
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
      this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicio, this.fechaFin);
  }

    deleteES(Id: any) {
    this.confirmationService.confirm({
      message: 'Esta a punto de eliminar la entrada de servicio. <b>¿Desea confirmar?</b>',
        accept: () => {
          this.deleteById(Id);
        },
        reject: () => {

        },
      }
    );
  }

 deleteById(Id) {
     this.mensajeComponent.setMsgsEmpty();
    this.service.deleteById(Id).subscribe(
        (result: any) => {          
            if (result.logout == true) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.info != undefined) {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.data != undefined) {
                this.floatMsgService.setSuccessMsg("Se ha eliminado la entrada de servicio " + Id);
                this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin);          
            }
      }
     );
  }

  handlePageEvent(e: any) {
      this.pageSize = e.rows;
      this.pageIndex = e.page + 1;
      this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicio, this.fechaFin);
  }

    openModal() {
      this.searchElement();

      if (this.itemIdSelected.length > 0) {
          this.showModal = true;
      }
      else {
          this.showModal = false;
      }
    }

    actualizarGrilla(event: string) {
        this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin);
    }

    searchElement() {
      for (const ordenCompra of this.tablaPO) {
        for (const posicion of ordenCompra.Posiciones) {
          const itemEncontrado = posicion.Items.find(item => item.PosicionId === this.itemSelected[0].PosicionId);
  
          if (itemEncontrado) {
            this.elementSelected = ordenCompra;
            break;
          }
        }
      }
    }

    onCloseModal() {
        this.showModal = false;

    }

    autocompleteProveedor(event) {
        try {
            this.subscription = this.service.autocompleteProveedor(event.query).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        class autoCompleteObject{
                            valor: string;
                            CodigoProveedor: string;
                        };
                        let provisional: any[] = new Array();

                        result.forEach((element) => {
                            let obj = new autoCompleteObject();
                            obj.valor = element.CodigoProveedor + ' - ' + element.RazonSocial;
                            obj.CodigoProveedor = element.CodigoProveedor;                           
                            if (provisional.some(x => x.valor === obj.valor)) {

                            }
                            else {
                                provisional.push(obj)
                            }                            
                        });
                        this.proveedorList = provisional;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

  // Filtrar orden de compra.
  ocFilters(obj: any): void { 
    if(this.ocFilterValues.length !== 0){
      this.ocFilterValues.forEach((value: string) => {
        switch (value) {
          case "SP":
            this.tablaPO = this.spFilter(obj);
            break;
          default:
            this.tablaPO = this.tablaPOCopy;
            break;
        }
      });
    } else {
      this.tablaPO = this.tablaPOCopy;
    }
  }

  // Filtra ordenes de compra con saldo pendiente.
  spFilter(tablaPO: any): any[] { 
    const nuevoArray = tablaPO.map((orden: any) => {
      const nuevasPosiciones = orden.Posiciones.map((posicion: any) => {
        // Filtrar los items con porcentaje menor a 100 y se pueden modificar si es necesario
        const nuevosItems = posicion.Items.filter((item: any) => parseInt(item.Porcentaje) < 100);
        return { ...posicion, Items: nuevosItems };
      }).filter((posicion: any) => {
        // Verificar si hay alguna posición con items que tengan porcentajes menores a 100
        const tieneItemsMenorA100 = posicion.Items.length > 0;
        return tieneItemsMenorA100;
      });
    
      // Verificar si hay alguna posición con items con porcentaje menor a 100
      const algunaPosicionConItemsMenorA100 = nuevasPosiciones.length > 0;
    
      // Retornar la orden solo si hay alguna posición con items con porcentaje menor a 100
      return algunaPosicionConItemsMenorA100 ? { ...orden, Posiciones: nuevasPosiciones } : null;
    }).filter(Boolean);
    
    // Eliminar las órdenes de compra que tienen todas sus posiciones con items al 100%
    const nuevoArrayFinal = nuevoArray.filter((orden: any) => {
      // Verificar si alguna posición tiene al menos un item con porcentaje menor a 100
      const algunaPosicionConItemsMenorA100 = orden.Posiciones.some((posicion: any) => {
        // Verificar si algún item tiene porcentaje menor a 100
        return posicion.Items.some((item: any) => parseInt(item.Porcentaje) < 100);
      });
    
      // Retornar la orden solo si alguna posición tiene items con porcentaje menor a 100
      return algunaPosicionConItemsMenorA100;
    });

    return nuevoArrayFinal;
  }

}