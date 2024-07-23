import { Component, OnInit, ViewChild, HostListener, ChangeDetectorRef, ElementRef } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { Table } from 'primeng/table';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { SpinnerComponent } from '../../../common/view-child/spinner/spinner.component';
import { Paginator } from 'primeng/paginator';
import { Subject, Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { Location } from '@angular/common';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ConfirmationService, Message } from 'primeng/api';

@Component({
  selector: 'app-listado-estado-certificaciones-proveedor',
  templateUrl: './listado-estado-certificaciones-proveedor.component.html',
  styleUrls: ['./listado-estado-certificaciones-proveedor.component.css']
})
export class ListadoEstadoCertificacionesProveedorComponent extends ListBaseComponent implements OnInit {

  protected locale: any;

  @ViewChild("tabla")
  protected tabla: Table;

  private destroy$: Subject<void> = new Subject<void>();

  @BlockUI() blockUI: NgBlockUI;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  nroSolp: string = "";
  ordenAscendente: boolean;
  columnaOrden: string;
  fechaInicio = "";
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
  innerWidth: number;
  tablaPOSap: any[] = [];
  tablaPOAprobaciones: any[] = [];
  fechaSeleccionadaAux: string = "1";
  recalculando: boolean = false;
  recalculandoAprobadas: boolean = false;

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.innerWidth = window.innerWidth;
  }

  estadoCertificacion: any = { name: 'Estado: Pendiente de aprobación', code: 'Pendiente Aprobación' };
  formularioMotivosRechazo: FormGroup | undefined;
  formularioSuplente: FormGroup | undefined;
  mostrarMotivosRechazos: boolean = false;
  mostrarSuplentes: boolean = false;
  motivos = [
    { name: 'Servicio no ejecutado/concluido', code: '1' },
    { name: 'Error en las cantidades certificadas, porcentajes erróneos', code: '2' },
    { name: 'Servicio realizado con resultado distinto al contratado  ', code: '3' },
    { name: 'Falta de presentación de documentación  ', code: '4' },
  ];
  suplentes: any = [
    { name: 'Carlos Duarte', code: '001' }
  ];
  listadoEstadoCertificacion: any = [
    { name: 'Estado: Aprobadas', code: 'Aprobada' },
    { name: 'Estado: Pendiente de aprobación', code: 'Pendiente Aprobación' },
    { name: 'Estado: Rechazadas', code: 'Rechazado' }
  ];
  listadoAreas: any = [];

  //Config tabla

  defaultTablesConfig = [
    {
      name: 'Certificaciones',
      columns: [
        { id: 'cFecha', header: 'Fecha Carga', field: 'FechaCreacion', type: 'date', sortable: true, required: false, visible: true },
        { id: 'cID_ES', header: 'ID-ES', field: 'ID_ES', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cFechaAprobacion', header: 'Fecha Aprobada', field: 'FechaAprobacion', type: 'string', sortable: true, required: false, visible: false },
        { id: 'cFechaRechazo', header: 'Fecha Rechazada', field: 'FechaRechazo', type: 'string', sortable: true, required: false, visible: false },
        { id: 'DFechaPres', header: 'Fecha Prestación', field: 'Fechadeprestación', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cDescripción', header: 'Descripción', field: 'Descripción', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cMontoTotal', header: 'Monto total', field: 'MontoTotal', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cOrdenCompra', header: 'Número OC', field: 'OrdenCompra', type: 'string', sortable: true, required: true, visible: true },
        { id: 'cUsuario', header: 'Usuario', field: 'Usuario', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cAprobador', header: 'Aprobador', field: 'Aprobador', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cEstado', header: 'Estado', field: 'Estado', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cMotivoRechazo', header: 'Motivo de rechazo', field: 'MotivoRechazo', type: 'string', sortable: false, required: false, visible: false },
        { id: 'esAdjuntos', header: 'Adjuntos', field: null, type: 'custom', sortable: false, required: true, visible: true },

      ]
    },
    {
      name: 'ESDetalle',
      columns: [
        { id: 'DTxtBrev', header: 'Descripción', field: 'TxtBrev', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DCtdPedido', header: 'Cantidad Total', field: 'CtdPedido', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DT', header: 'Precio Unitario', field: 'T', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DImporteTotal', header: 'Importe total', field: 'ImporteTotal', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DU', header: 'UM', field: 'U', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DNumeroRemito', header: 'Referencia', field: 'NumeroRemito', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DTextoBreve', header: 'Texto Breve', field: 'TextoBreve', type: 'string', sortable: false, required: false, visible: true },
      ]
    }
  ]

  constructor(protected service: ComprasService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router,
    private location: Location,
    private cdr: ChangeDetectorRef,
    private confirmationService: ConfirmationService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    this.usuario = sessionStorage.getItem("username");
    var fechaActual = new Date();
    fechaActual.setDate(fechaActual.getDate() - 2)
    this.fechaInicio = fechaActual.toISOString().slice(0, 10);
  }

  //#region Variables 
  cols: any[];
  usuario: string;
  vendedor: string;
  allItems: any[];
  proveedor: string = sessionStorage.getItem("proveedor");
  msgs: Message[] = [];

  async ngOnInit() {
    this.getFecha('1');
    this.innerWidth = window.innerWidth;
    this.navService.setSeccionActive("Estado certificaciones");
    this.navService.navegarSeccion("compras/listadoEstadoCertificacionesProveedor");
    this.recalculando = true;
    this.recalculandoAprobadas = true;
    await this.getListarPO(); // No mover.
    this.obtenerESSap(this.proveedor, this.documentoNumero);
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

  getListarPO(): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      this.unsubscribe();
      this.subscripcionPO = this.service.ObtenerESLocales(false, this.proveedor).subscribe(
        (result: any) => {
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
          } else if (result.info != undefined) {
          } else {
            this.agregarTipoMonedaEnDetalle(result.data);
            this.tablaPOAprobaciones = result.data;
            this.length = result.data.length > 0 ? result.data[0].ItemsTotales : result.data.length;
            this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
            this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;
          }
          this.tabla.filter(["Pendiente Aprobación"], "Estado", "in");
          this.recalculando = false;
          resolve();
        }, error => {
          this.floatMsgService.setErrorMsg(error.message);
          this.recalculando = false;
          reject(error);
        }
      );
    });
  }

  displayContent() {
    return !this.spinnerComponent.visible;
  }

  getFecha(rango: string) {
    var fechaActual = new Date();
    switch (rango) {
      case '1':
        fechaActual.setDate(fechaActual.getDate() - 2);
        break;
      case '2':
        fechaActual.setDate(fechaActual.getDate() - 7);
        break;
      case '3':
        fechaActual.setMonth(fechaActual.getMonth() - 1);
        break;
      default:
        fechaActual.setMonth(fechaActual.getMonth() - 1);
        break;
    }
    this.fechaSeleccionadaAux = rango;
    this.fechaInicio = fechaActual.toISOString().slice(0, 10);
  }


  deleteES(item: any) {
    //TODO: lógica para cuando se especifique el borrado de una ES
  }

  async handlePageEvent(e: any) {
    this.pageSize = e.rows;
    this.pageIndex = e.page + 1;
    this.recalculando = true;
    await this.getListarPO(); // No mover.
  }

  async onOrder(columna: string) {
    if (this.columnaOrden != columna) {
      this.ordenAscendente = false
    } else {
      this.ordenAscendente = this.ordenAscendente == false ? true : false;
    }
    this.columnaOrden = columna;
    this.recalculando = true;
    await this.getListarPO(); // No mover.
  }

  toggleRow(rowData: any): void {
    this.selectedRow = this.selectedRow === rowData ? null : rowData;
  }

  isSelectedRow(rowData: any): boolean {
    return this.selectedRow === rowData;
  }

  filtrarPorEstado(event: any): void {
    const estado = event.value.code;

    // Determinar qué columnas deben mostrarse u ocultarse según el estado
    const columnasVisibles = this.obtenerColumnasVisiblesSegunEstado(estado);

    // Aplicar los cambios de visibilidad a las columnas
    this.aplicarVisibilidadColumnas(columnasVisibles);

    // Filtrar la tabla por el estado seleccionado
    this.tabla.filter(estado, 'Estado', 'contains');
  }

  // Método para determinar qué columnas deben mostrarse u ocultarse según el estado
  private obtenerColumnasVisiblesSegunEstado(estado: string): string[] {
    switch (estado) {
      case 'Aprobada':
        return ['cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cEstado', 'cAprobador', 'cFechaAprobacion','esAdjuntos'];
      case 'Pendiente Aprobación':
        return ['DImporteTotal', 'DFechaPres', 'cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cEstado', 'cAprobador','esAdjuntos'];
      case 'Rechazado':
        return ['cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cAprobador', 'cEstado', 'cMotivoRechazo', 'cFechaRechazo','esAdjuntos'];
    }
  }

  // Método para aplicar los cambios de visibilidad a las columnas
  private aplicarVisibilidadColumnas(columnasVisibles: string[]): void {
    this.defaultTablesConfig[0].columns.forEach(col => {
      col.visible = columnasVisibles.includes(col.id);
    });
  }

  verSuplentes(): void {
    this.mostrarSuplentes = true;
  }

  formatearFecha(fecha: string): string {
    setTimeout(() => {
      if (this.estadoCertificacion.code === "Pendiente Aprobación" || this.estadoCertificacion.code === "Rechazado") {
        const nuevoFormatoFecha = fecha.split('/');
        const year = nuevoFormatoFecha[2].split(' ')[0];
        fecha = nuevoFormatoFecha[1] + '/' + nuevoFormatoFecha[0] + '/' + year;
      } else {
        const nuevoFormatoFecha = fecha.split('-');
        fecha = nuevoFormatoFecha[1] + '/' + nuevoFormatoFecha[2] + '/' + nuevoFormatoFecha[0];
      }
    }, 300)
    return fecha;
  }

  showOrHideAuxPanel(): boolean {
    const estadoCertificacion = { name: 'Estado: Aprobadas', code: 'Aprobada' };
    if (this.estadoCertificacion.code === estadoCertificacion.code) {
      return true;
    }
    return false;
  }

  obtenerESSap(proveedor, documentoNumero): void {
    this.service.getByProveedorAsync(this.fechaInicio, proveedor, documentoNumero, this.columnaOrden, this.ordenAscendente, this.pageIndex, this.pageSize, false).subscribe(
      (result: { error: any, data: any }) => {
        this.recalculandoAprobadas = false;
        if (result.error != null) {
          this.floatMsgService.setErrorMsg(result.error);
          return;
        }
        if (result.data.length > 0) {
          this.tablaPOSap = result.data;
        }
        this.tabla.first = 0;

      }, error => {
        this.floatMsgService.setErrorMsg(error.message);
        this.recalculandoAprobadas = false;
      })
  }

  isMoaIntern(usuario){

    let userIntern = '@molinos';
    
    return usuario.includes(userIntern) ? 'Ingresado por MOA' : usuario;
  
  }

  /**
   * filtro de busqueda de las entradas de servicio por rango de fechas.
   */
  onBuscar() {
    this.recalculandoAprobadas = true;
    this.obtenerESSap(this.proveedor, this.documentoNumero);
  }

  agregarTipoMonedaEnDetalle(certificaciones: any[]): void {
    if (certificaciones === undefined || certificaciones === null) {
      return;
    }

    certificaciones.forEach(certificacion => {
      if (certificacion.entradaServicioDetalle) {
        certificacion.entradaServicioDetalle.forEach(detalle => {
          detalle.Moneda = certificacion.Moneda;
        });
      }
    });
  }

  fileTypes: { [key: string]: string } = {
    ".pdf": 'application/pdf',
    ".csv": "text/csv",
    ".msg": "application/vnd.ms-outlook"
};


descargarArchivos(rowData: any) {

    this.service.GetAdjuntosByES(rowData.NumeroCertificacion).subscribe(result => {
        if (result.data.length > 0) {
            
            result.data.forEach((archivo) => {
                this.descargarArchivo(archivo.Adjuntos, archivo.NombreArchivo, archivo.Extension);
            });
        }
        else{
       
          this.confirmationService.confirm({
            message: "<ul>" + "No se encontraron adjuntos a descargar" + "</ul>",
            rejectVisible: false
          });

        }
    });

  }

  descargarArchivo(archivo: ArrayBuffer, nombreArchivo: string, extension: string) {
    const typeExtension = this.fileTypes[extension.toLowerCase()] || "application/octet-stream";
    var byteArray = new Uint8Array(archivo);
    var blob = new Blob([byteArray], { type: typeExtension });

    if (window.navigator.msSaveOrOpenBlob) {
        // IE11
        window.navigator.msSaveOrOpenBlob(blob, nombreArchivo);
    } else {
        var url = window.URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.href = url;
        link.download = nombreArchivo;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
    }
    }

    showScrollbar: boolean = false;
    fullscreen: boolean = false;

    toggleFullscreen() {
        this.fullscreen = !this.fullscreen;
        if (this.fullscreen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'auto';
        }
    }
}