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
    expandedRows: any[] = [];
    posicionRow: any[] = [];
    isTableExpanded = false;
    isEntradaDeServicioExpanded = false;
    selectedItemIndex: number | null = null;

    expandedRow: any; // Variable para rastrear la fila expandida
    // isTableExpanded: boolean = false;

    mockData = {
        "data": [
          {
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },
          {
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },{
            "OrdenCompraId": 4123001929,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "08-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra",
            "MontoTotal": 256000,
            "Posiciones": [
              {
                "OrdenCompraId": 432520,
                "PosicionId": 0,
                "Descripcion": "Gestion 1",
                "Material": 3001500,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- MAY",
                "Posicion": '001_pos',
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                
                "Items": [
                  {
                    "ItemId": 313210,
                    "PosicionId": 1,
                    "Descripcion": "Pintura oficina",
                    "Cantidad": 1,
                    "PrecioBruto": 78000
                  },
                ]
              },
            ]
          },
          {
            "OrdenCompraId": 4123001930,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L",
            "Fecha": "05-11-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra 02",
            "MontoTotal": 922999,
            "Posiciones": [
              {
                "OrdenCompraId": 45631,
                "PosicionId": 2,
                "Descripcion": "Gestion 2",
                "Posicion": '002_pos',
                "Material": 3001501,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- JUN",
                "T": 1,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46285,
                "Items": [
                  {
                    "ItemId": 20122,
                    "PosicionId": 1,
                    "Descripcion": "Pintura 1",
                    "Cantidad": 1,
                    "PrecioBruto": 87000,

                  }
                ]
              },
            ]
          },
          {
            "OrdenCompraId": 4123001920,
            "ProveedorNombre": "ARROYITO MAQUINARIAS S R L 2",
            "Fecha": "05-09-2023",
            "Descripcion": "Falta Determinar Descripcion de Orden de Compra 03",
            "MontoTotal": 522569,
            "Posiciones": [
              {
                "OrdenCompraId": 45632,
                "PosicionId": 3,
                "Descripcion": "Gestion 3",
                "Posicion": '003_pos',
                "Material": 3001504,
                "TextBrev": "Mejoras y Evolutivos MOAOPERACIONES- ENE",
                "T": 3,
                "FeEntrega": "Ejemplo",
                "NroSolped": 46260,
                "Items": [
                  {
                    "ItemId": 20122,
                    "PosicionId": 1,
                    "Descripcion": "Pintura 1",
                    "Cantidad": 1,
                    "PrecioBruto": 87000,

                  }
                ]
              },
            ]
          }
        ]
      };

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
    allItems : any[];
    proveedor: string = "";

    //#endregion
    
    ngOnInit() {
        
        this.getListarPO(this.proveedor, this.ordenCompraId);

        
        

        // this.tablaPO = this.mockData.data;

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
  
    // toggleRow(rowData: any) {
    //   this.expandedRow = this.expandedRow === rowData ? null : rowData; // Alternar entre expandir y colapsar
    //   this.isTableExpanded = !!this.expandedRow; // Verificar si la tabla está expandida
    // }

  toggleRow(row: any): void {
    const index = this.expandedRows.indexOf(row);
    if (index === -1) {
        this.expandedRows.push(row);
    } else {
        this.expandedRows.splice(index, 1);
    }
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

    // console.log(data.PosicionId, this.posicionRow) 
  }

  toggleEntradaServicio() {
    this.isEntradaDeServicioExpanded = !this.isEntradaDeServicioExpanded;
  }

  onCheckboxChange(e: any) {

  }


  ngOnDestroy(): void {
      // this.subscripcionPO.unsubscribe();
  }

  getListarPO(proveedor, ordenCompraId) {
      this.getFecha();
      try {
          this.spinnerComponent.showIt();
          this.unsubscribe();
          debugger
          // this.subscripcionPO = this.service.getByProveedor("2023-01-28", proveedor, "4123001336", this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
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
      //fechaActual.setDate(fechaActual.getDate() - 2);
      //TODO: cambiar cuando se agregue el filtro de fecha.
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
      this.getListarPO(this.proveedor, this.ordenCompraId);
  }

  deleteES(item: any) {
    //TODO: lógica para cuando se especifique el borrado de una ES
  }

  handlePageEvent(e: any) {
      this.pageSize = e.rows;
      this.pageIndex = e.page + 1;
      this.getListarPO(this.proveedor, this.ordenCompraId);
  }
}