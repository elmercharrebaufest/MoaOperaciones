import { Component, OnInit, ViewChild, HostListener, ChangeDetectorRef, ElementRef, OnDestroy } from '@angular/core';
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
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { DropdownOption } from '../../../common/view-child/dropdown/dropdown.component';

export interface estadoCertificacion {
  name: string,
  code: string
}

export interface autoCompleteObject {
  valor: string;
  CodigoProveedor: string;
};
import { certificacionES } from '../components/modal-aprobacion/modalAprobacion.interface';

@Component({
  selector: 'app-listado-estado-certificaciones',
  templateUrl: './listado-estado-certificaciones.component.html',
  styleUrls: ['./listado-estado-certificaciones.component.css']
})
export class ListadoEstadoCertificacionesComponent extends ListBaseComponent implements OnInit, OnDestroy {
  //#region Variables 
  @ViewChild("tabla") protected tabla: Table;
  @ViewChild('paginator') paginator: Paginator;
  @ViewChild(SpinnerComponent) protected spinnerComponent: SpinnerComponent;
  @BlockUI() blockUI: NgBlockUI;
  @HostListener('window:resize', ['$event'])
  innerWidth: number;
  onResize(event) {
    this.innerWidth = window.innerWidth;
  }

  subscripciones: Subscription[] = [];
  entradaServicioSeleccionada: certificacionES[] = [];
  mostrarModalAprobaciones: boolean = false;
  protected locale: any;
  private destroy$: Subject<void> = new Subject<void>();
  nroSolp: string = "";
  ordenAscendente: boolean = false;
  columnaOrden: string = "FechaCreacion";
  fechaInicio = "";
  length = 0;
  pageSize: number = 10;
  pageIndex: number = 1;
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
  itemSelected: certificacionES[] = [];
  selectedItemId: number | null = null;
  selectedPosicionId: number | null = null;
  ordenCompraIdsMostradas: Set<number> = new Set<number>();
  ordenCompraId: string = "";
  proveedorSeleccionado: autoCompleteObject;
  selectedRow: any;
  estadoCertificacion: estadoCertificacion = { name: 'Estado: Pendiente de aprobación', code: 'Pendiente Aprobación' };
  formularioMotivosRechazo: FormGroup | undefined;
  formularioSuplente: FormGroup | undefined;
  mostrarMotivosRechazos: boolean = false;
  proveedorList: any[] = new Array();
  filtroFechas: Array<DropdownOption> = [
    new DropdownOption("1", "Últimos dos dias"),
    new DropdownOption("2", "Última semana"),
    new DropdownOption("3", "Último mes"),
  ];
  fullscreen: boolean = false;
  userId: any = '';
  tablaPOAprobaciones: any[] = [];
  tablaPOSap: any[] = [];
  cols: any[];
  usuario: string;
  vendedor: string;
  allItems: any[];
  proveedor: string = sessionStorage.getItem("proveedor");
  msgs: Message[] = [];
  havePermission: boolean = false;
  observaciones: string = '';
  isAll: boolean = false; // Permiso para ver todos los registros en la tabla aprobaciones.
  fechaSeleccionadaAux: string = "1";
  recalculando: boolean = false;
  recalculandoAprobadas: boolean = false;

  motivos = [
    { name: 'Servicio no ejecutado/concluido', code: '1' },
    { name: 'Error en las cantidades certificadas, porcentajes erróneos', code: '2' },
    { name: 'Servicio realizado con resultado distinto al contratado', code: '3' },
    { name: 'Falta de presentación de documentación', code: '4' }
  ];
  suplentes: any = [];
  listadoEstadoCertificacion: estadoCertificacion[] = [
    { name: 'Estado: Aprobadas', code: 'Aprobada' },
    { name: 'Estado: Pendientes de aprobación', code: 'Pendiente Aprobación' },
    { name: 'Estado: Rechazadas', code: 'Rechazado' },
    { name: 'Estado: Anuladas', code: 'Anulada' }
  ];
  listadoAreas: any = [];
  defaultTablesConfig = [
    {
      name: 'Certificaciones',
      columns: [
        { id: 'cID_ES', header: 'ID_ES', field: 'ID_ES', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cFechaAprobacion', header: 'Fecha Aprobada', field: 'FechaAprobacion', type: 'string', sortable: true, required: false, visible: false },
        { id: 'cFechaRechazo', header: 'Fecha Rechazo', field: 'FechaRechazo', type: 'string', sortable: true, required: false, visible: false },
        { id: 'cFecha', header: 'Fecha Creación', field: 'FechaCreacion', type: 'date', sortable: true, required: false, visible: true },
        { id: 'cOrdenCompra', header: 'Número OC', field: 'OrdenCompra', type: 'string', sortable: true, required: true, visible: true },
        { id: 'cCuit', header: 'CUIT', field: 'CUIT', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cProveedor', header: 'Proveedor', field: 'Proveedor', type: 'string', sortable: true, required: true, visible: true },
        { id: 'cDescripción', header: 'Descripción', field: 'Descripción', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cMontoTotal', header: 'Monto total', field: 'MontoTotal', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cUsuario', header: 'Usuario', field: 'Usuario', type: 'string', sortable: true, required: false, visible: true },
        { id: 'cEstado', header: 'Estado', field: 'Estado', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cAcciones', header: 'Acciones', field: 'Acciones', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cAprobador', header: 'Aprobador', field: 'Aprobador', type: 'string', sortable: true, required: false, visible: true },
        { id: 'cAnulador', header: 'Anulado Por', field: 'AnuladoPor', type: 'string', sortable: true, required: false, visible: false },
        { id: 'cMotivoRechazo', header: 'Motivo Rechazo', field: 'MotivoRechazo', type: 'string', sortable: false, required: false, visible: false },
        { id: 'esAdjuntos', header: 'Adjuntos', field: null, type: 'custom', sortable: false, required: true, visible: true },

      ]
    },
    {
      name: 'ESDetalle',
      columns: [
        { id: 'DItem', header: 'N° Ítem', field: 'Item', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DPosicion', header: 'N° Posición', field: 'Posicion', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DMaterial', header: 'N° Servicio', field: 'Material', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DTxtBrev', header: 'Descripción', field: 'TxtBrev', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DCtdPedido', header: 'Cant.', field: 'CtdPedido', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DU', header: 'UM', field: 'U', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DT', header: 'Precio Unitario', field: 'T', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DNumeroRemito', header: 'Nro. Remito', field: 'NumeroRemito', type: 'string', sortable: false, required: false, visible: true }
      ]
    }
  ];

  constructor(protected service: ComprasService, protected navService: NavService,
    protected sessionDataService: SessionDataService, protected securityService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router,
    private location: Location,
    private cdr: ChangeDetectorRef,
    private confirmationService: ConfirmationService,
    private messageService: MessageService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    this.usuario = sessionStorage.getItem("username");
    var fechaActual = new Date();
    fechaActual.setDate(fechaActual.getDate() - 2)
    this.fechaInicio = fechaActual.toISOString().slice(0, 10);
  }

  public ngOnDestroy(): void {
    this.subscripciones.forEach(sub => sub.unsubscribe());
  }

  async ngOnInit() {
    this.getFecha('1');
    this.formsCreate();
    this.innerWidth = window.innerWidth;
    this.navService.setSeccionActive("Estado certificaciones");
    this.navService.navegarSeccion("compras/listadoEstadoCertificaciones");

    let permisos = sessionStorage.getItem("permisos");

    if (permisos && permisos.includes("VER TODOS LOS ESTADOS DE ES")) {
      this.havePermission = true;
    }
    this.recalculando = true;
    this.recalculandoAprobadas = true;
    await this.getListarPO(); // No mover.
    this.obtenerESSap(this.proveedor, this.documentoNumero);
  }

  formsCreate(): void {
    this.formularioMotivosRechazo = new FormGroup({
      motivo: new FormControl({ name: null, code: null }, Validators.required),
      destinatario: new FormControl(null),
      detalleCertificacionRechazada: new FormControl(null),
      resumenLineas: new FormControl(null),
      motivoRechazo: new FormControl(null),
      fechaRechazo: new FormControl(null),
      proveedor: new FormControl(null),
      numeroCertificacion: new FormControl(null),
      fechaCertificacion: new FormControl(null),
      descripcion: new FormControl(null),
      importe: new FormControl(null),
      montoTotal: new FormControl(null),
      detalleServicio: new FormControl(null),
      observaciones: new FormControl(null),
      moneda: new FormControl(null)
    });

    this.formularioSuplente = new FormGroup({
      suplente: new FormControl(null),
      nro_es_local: new FormControl(null)
    });
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
      const subscription = this.service.ObtenerESLocales(this.isAll, '').subscribe(
        (result: any) => {
          if (result.logout === true) {
            this.sessionDataService.logout();
            reject('Logout required');
          } else if (result.error !== undefined && result.error !== "") {
          } else if (result.info !== undefined) {
            // Manejo de mensajes informativos, si es necesario
          } else {
            this.agregarTipoMonedaEnDetalle(result.data);
            this.tablaPOAprobaciones = result.data;
            this.userId = this.setColumsByUserProfile(this.tablaPOAprobaciones, this.usuario);
          }
          if (this.estadoCertificacion.code === 'Pendiente Aprobación') {
            this.tabla.filter("Pendiente Aprobación", "Estado", "contains");
          }
          this.recalculando = false;
          resolve();
        },
        error => {
          this.floatMsgService.setErrorMsg(error.message);
          this.recalculando = false;
          reject(error);
        }
      );

      this.subscripciones.push(subscription);
      this.clearMessage();
    });
  }


  obtenerESSap(proveedor, documentoNumero): Promise<void> {
    return new Promise<void>((resolve, reject) => {
      const subscription = this.service.getByProveedorAsync(this.fechaInicio, proveedor, documentoNumero, this.columnaOrden, this.ordenAscendente, this.pageIndex, this.pageSize, this.isAll).subscribe(
        (result: { error: any, data: any }) => {
          this.recalculandoAprobadas = false;
          if (result.error != null) {
            this.floatMsgService.setErrorMsg(result.error);
            return;
          }
          this.tablaPOSap = result.data;
          resolve();
        }, error => {
          this.floatMsgService.setErrorMsg(error.message);
          this.recalculandoAprobadas = false;
          reject(error);
        })
      this.subscripciones.push(subscription);
    });
  }

  clearMessage() {
    setTimeout(() => {
      this.messageService.clear();
    }, 10000)
  }

  displayContent() {
    return !this.spinnerComponent.visible;
  }

  setDateByRange(event: string): void {
    this.getFecha(event);
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

  toggleRow(rowData: any): void {
    this.selectedRow = this.selectedRow === rowData ? null : rowData;
  }

  verMotivosRechazos(rowData: any): void {
    this.formularioMotivosRechazo.controls['destinatario'].patchValue(rowData.Ingresante);
    this.formularioMotivosRechazo.controls['proveedor'].patchValue(rowData.Proveedor);
    this.formularioMotivosRechazo.controls['descripcion'].patchValue(rowData.Descripcion);
    this.formularioMotivosRechazo.controls['montoTotal'].patchValue(rowData.MontoTotal);
    this.formularioMotivosRechazo.controls['detalleServicio'].patchValue(rowData.entradaServicioDetalle);
    this.formularioMotivosRechazo.controls['numeroCertificacion'].patchValue(rowData.NumeroCertificacion);
    this.formularioMotivosRechazo.controls['fechaCertificacion'].patchValue(rowData.FechaCreacion);
    this.formularioMotivosRechazo.controls['moneda'].patchValue(rowData.Moneda);
    this.mostrarMotivosRechazos = true;
  }

  async filtrarPorEstado(event: any): Promise<void> {
    switch (event.value.code) {
      case 'Aprobada':
        this.defaultTablesConfig[0].columns.forEach(col => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Acciones' || col.field === 'Reasignar' || col.field === 'FechaRechazo' || col.field === 'AnuladoPor' ? false : true;
        });
        break;
      case 'Pendiente Aprobación':
        if (this.tablaPOAprobaciones.length < 1) {
          this.recalculando = true;
          this.getListarPO();
        }
        this.setColumsByUserProfile(this.tablaPOAprobaciones, this.usuario);
        break;
      case 'Rechazado':
        if (this.tablaPOAprobaciones.length < 1) {
          this.recalculando = true;
          this.getListarPO();
        }
        this.defaultTablesConfig[0].columns.forEach(col => {
          col.visible = col.field === 'Aprobador' || col.field === 'Acciones' || col.field === 'Reasignar' || col.field === 'FechaAprobacion' || col.field === 'AnuladoPor' ? false : true;
        });
        break;
      case 'Anulada':
        if (this.tablaPOAprobaciones.length < 1) {
          this.recalculando = true;
          this.getListarPO();
        }
        this.defaultTablesConfig[0].columns.forEach(col => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobación' || col.field === 'Acciones' || col.field === 'Reasignar' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' ? false : true;
        });
        break;
    }
    this.tabla.filter(event.value.code, 'Estado', 'contains');
    this.showAllESRows();
  }

  showAllESRows(): void {
    this.tablaPOAprobaciones.forEach(es => {
      const element = document.getElementById(es.EntradaServicio);
      if (element) {
        element.style.display = 'table-row';
      }
    });
  }

  filtrarPorArea(event: any): void {
    let filtrarAreas = [];
    event.value.forEach((area: any) => {
      filtrarAreas.push(area);
    });
    this.tabla.filter(filtrarAreas, 'UsuarioArea', 'contains');
  }

  verSuplentes(suplente: string, nro_es_local: string): void {
    const data = {
      Suplente: suplente,
      NroEsLocal: nro_es_local
    }
    let confirmMessage;

    if (suplente) {
      confirmMessage = {
        acceptLabel: 'Si',
        message: `Está derivando la certificación Nº ` + nro_es_local + ` al siguiente aprobador ` + suplente + `. <b>¿Desea continuar?</b>`,
        accept: () => {
          this.messageService.clear();
          this.blockUI.start('Cargando...');
          this.reasignar(data);
        },
        reject: () => { }
      }
    } else {
      confirmMessage = {
        message: 'No posee suplente asignado',
        acceptLabel: 'Ok',
        rejectVisible: false
      }
    }

    this.confirmationService.confirm(confirmMessage);
  }

  reasignar(data): void {
    this.subscripciones.push(
      this.service.reasignarSuplente(data).subscribe(
        (resp: any) => {
          const msj = { severity: 'success', summary: 'Reasignación exitosa!', detail: 'Se reasigno al nuevo aprobador.' };
          if (resp.error) {
            msj.severity = 'error';
            msj.summary = resp.error
            msj.detail = '';
          }
          this.messageService.add(msj);
          if (!resp.error) {
            this.updateApprover(data.NroEsLocal, resp.data);
          }
          this.blockUI.stop();
          this.clearMessage();
        }, error => {
          this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: error });
          this.blockUI.stop();
          this.clearMessage();
        }
      )
    );
  }

  enviarMotivo() {
    this.messageService.clear();
    this.blockUI.start('Cargando...');
    const data = {
      Destinatario: this.formularioMotivosRechazo.get('destinatario').value,
      MotivoRechazo: this.formularioMotivosRechazo.get('observaciones').value != null ? this.formularioMotivosRechazo.get('motivo').value.name + '. Observación:' + this.formularioMotivosRechazo.get('observaciones').value : this.formularioMotivosRechazo.get('motivo').value.name,
      Proveedor: this.formularioMotivosRechazo.get('proveedor').value,
      NumeroCertificacion: this.formularioMotivosRechazo.get('numeroCertificacion').value,
      FechaCertificacion: this.formularioMotivosRechazo.get('fechaCertificacion').value,
      Descripcion: this.formularioMotivosRechazo.get('descripcion').value,
      MontoTotal: this.formularioMotivosRechazo.get('montoTotal').value,
      DetalleServicio: this.formularioMotivosRechazo.get('detalleServicio').value.map((item: any) => ({
        Descripcion: item.Descripcion,
        Cantidad: item.Cantidad,
        UM: item.UM,
        Porcentaje: item.PorcentajeCertificar,
        Monto: this.formularioMotivosRechazo.get('moneda').value === 'ARP' ? '$ ' + item.MontoCertificar : item.MontoCertificar
      })),
      Moneda: this.formularioMotivosRechazo.get('moneda').value
    }
    this.subscripciones.push(
      this.service.enviarMotivoRechazoES(data).subscribe(
        (resp: any) => {
          if (resp.data.status == "OK") {
            this.messageService.add({ severity: 'success', summary: 'Rechazo exitoso!', detail: 'Estamos refrescando los datos para que puedas ver los cambios.' });
          }
          else {
            this.messageService.add({ severity: 'error', summary: '', detail: resp.data.status });
          }

          this.resetRejectForm();
          this.mostrarMotivosRechazos = false;
          this.updateStateFromPending(data.NumeroCertificacion, 'Rechazar', { es: resp.data.result[0], dateReject: resp.data.Fecha_rechazo_string });
          this.blockUI.stop();
          this.clearMessage();
        }, error => {
          this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.' });
          this.blockUI.stop();
          this.clearMessage();
        }
      )
    );
  }

  ocultandoModal(): void {
    this.resetRejectForm();
  }

  resetRejectForm() {
    this.formularioMotivosRechazo.reset({
      motivo: { name: null, code: null },
      destinatario: null,
      detalleCertificacionRechazada: null,
      resumenLineas: null,
      motivoRechazo: null,
      fechaRechazo: null,
      proveedor: null,
      numeroCertificacion: null,
      fechaCertificacion: null,
      descripcion: null,
      importe: null,
      montoTotal: null,
      detalleServicio: null,
      observaciones: null,
      moneda: null
    });
    this.formularioMotivosRechazo.markAsPristine();
    this.formularioMotivosRechazo.markAsUntouched();
    this.formularioMotivosRechazo.updateValueAndValidity();
  }

  mostrarResumenDeAprobacion(entradaServicio: any): void {
    this.entradaServicioSeleccionada = [
      {
        NroPosicion: entradaServicio.NroPosicion,
        Descripcion: entradaServicio.Descripcion,
        MontoTotalACertificar: Number(entradaServicio.MontoTotal.replace(",", ".")),
        NumeroCertificacion: entradaServicio.NumeroCertificacion,
        Items: entradaServicio.entradaServicioDetalle.map((item: any) => ({
          Cantidad: item.Cantidad,
          CantidadACertificar: Number(item.CantidadCertificar.replace(",", ".")),
          CantidadReal: item.Cantidad,
          Importe: item.MontoCertificar,
          Descripcion: item.TextoBreveServicio,
          Moneda: entradaServicio.Moneda,
          MontoACertificar: item.MontoCertificar,
          NumeroLinea: item.NumeroLinea,
          Porcentaje: Number(item.PorcentajeCertificar),
          PorcentajeACertificar: Number(item.PorcentajeCertificar),
          ServicioNumero: item.CodigoServicio,
          UM: item.UM
        }))
      }
    ];
    this.mostrarModalAprobaciones = true;
  }

  cerrarModalResumen(): void {
    this.mostrarModalAprobaciones = false;
  }

  enviarAprobacion(numeroCertificacion: string): void {
    const moneda: string = this.entradaServicioSeleccionada[0].Items[0].Moneda;
    this.messageService.clear();
    this.confirmationService.confirm({
      message: '¿Esta seguro que desea aprobar esta Entrada de Servicio?',
      header: 'Confirmar Aprobación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: "Sí",
      rejectLabel: "No",
      accept: () => {
        this.blockUI.start('Cargando...');
        this.cerrarModalResumen();
        this.subscripciones.push(
          this.service.enviarAprobacionES(numeroCertificacion, moneda).subscribe(
            (resp) => {
              let mensajeError: string = "";
              if (!resp.data) {
                mensajeError = 'del servidor, vuelva a intentarlo más tarde.'
                this.messageService.add({ severity: 'error', summary: 'Error', detail: mensajeError });
              }
              switch (resp.data.Type) {
                case "I": {
                  this.messageService.add({ severity: 'success', summary: 'Aprobado', detail: resp.data.Message });
                  this.updateStateFromPending(numeroCertificacion, 'Aprobar');
                  this.recalculandoAprobadas = true;
                  this.getFecha("1");
                  this.obtenerESSap(this.proveedor, this.documentoNumero);
                  break;
                }
                case "S":
                case "Desync":
                  this.messageService.add({ severity: 'info', summary: '', detail: resp.data.Message });
                  break;
                case "E": {
                  mensajeError = resp.data.Message.startsWith("Sólo es posible contabilizar en ") ||
                    resp.data.Message.startsWith("Contabilice en ") ?
                    "El período se encuentra cerrado, por favor contabilice en el periodo actual." : resp.data.Message;
                  this.messageService.add({ severity: 'warning', summary: '', detail: mensajeError });
                  break;
                }
                default: {
                  this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.' });
                  break;
                }
              }
              this.mostrarModalAprobaciones = false;
              this.blockUI.stop();
              this.clearMessage();
            }, error => {
              this.mostrarModalAprobaciones = false;
              this.blockUI.stop();
              this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.' });
              this.clearMessage();
            }
          )
        );
      }
    });
  }

  async SeeAll() {
    this.blockUI.start('Cargando...');
    this.isAll = true;
    if (this.estadoCertificacion.code === 'Aprobada') {
      this.recalculandoAprobadas = true;
      await this.obtenerESSap(this.proveedor, this.documentoNumero);
      this.blockUI.stop();
      this.recalculando = true;
      await this.getListarPO();
    } else {
      this.recalculando = true;
      await this.getListarPO();
      this.blockUI.stop();
      this.recalculandoAprobadas = true;
      await this.obtenerESSap(this.proveedor, this.documentoNumero);
    }
  }

  async SeeForProvider() {
    this.blockUI.start('Cargando...');
    this.isAll = false;
    if (this.estadoCertificacion.code === 'Aprobada') {
      this.recalculandoAprobadas = true;
      await this.obtenerESSap(this.proveedor, this.documentoNumero);
      this.blockUI.stop();
      this.recalculando = true;
      await this.getListarPO();
    } else {
      this.recalculando = true;
      await this.getListarPO();
      this.blockUI.stop();
      this.recalculandoAprobadas = true;
      await this.obtenerESSap(this.proveedor, this.documentoNumero);
    }
  }

  /**
   * Metodo para definir el perfil de usuario en la tabla de datos con el filtro Pendiente Aprobación.
   * FAI: Fiscal, Aprobador, Ingresante
   * AI: Aprobador, Ingresante
   * FA: Fiscal, Aprobador
   * FI: Fiscal, Ingresante
   * A: Aprobador
   * I: Ingresante
   * F: Fiscal
   * @param entradasDeServicio 
   * @param user 
   * @returns 
   */
  setColumsByUserProfile(entradasDeServicio: any, user: any): string {
    const pendienteAprobacion = entradasDeServicio.filter(pa => pa.Estado === 'Pendiente Aprobación');
    if (pendienteAprobacion.length > 0) {
      const fai = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
      if (fai.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'AnuladoPor' ? false : true;
        });
        return "FAI";
      }
      const ai = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && !this.equalsIgnoreCase(pa.Fiscal, user));
      if (ai.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'Reasignar' || col.field === 'AnuladoPor' ? false : true;
        });
        return "AI";
      }
      const fa = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && !this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
      if (fa.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'AnuladoPor' ? false : true;
        });
        return "FA";
      }
      const fi = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
      if (fi.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'Acciones' || col.field === 'AnuladoPor' ? false : true;
        });
        return "FI";
      }
      const a = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && !this.equalsIgnoreCase(pa.Ingresante, user) && !this.equalsIgnoreCase(pa.Fiscal, user));
      if (a.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'Reasignar' || col.field === 'AnuladoPor' ? false : true;
        });
        return "A";
      }
      const i = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && !this.equalsIgnoreCase(pa.Fiscal, user));
      if (i.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'Reasignar' || col.field === 'Acciones' || col.field === 'AnuladoPor' ? false : true;
        });
        return "I";
      }
      const f = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && !this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
      if (f.length > 0) {
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'FechaAprobacion' || col.field === 'FechaRechazo' || col.field === 'Acciones' || col.field === 'AnuladoPor' ? false : true;
        });
        return "F";
      }
    }
  }

  equalsIgnoreCase(str1: string, str2: string): boolean {
    let areEqual = false;
    if (str1 && str2) {
      const normalized1 = this.eliminarAcentos(str1);
      const normalized2 = this.eliminarAcentos(str2);
      areEqual = normalized1.toLocaleLowerCase() === normalized2.toLocaleLowerCase();
    }
    return areEqual;
  }

  eliminarAcentos(string) {
    const conAcento = 'áàãâäéèêëíìîïóòõôöúùûüçÁÀÃÂÄÉÈÊËÍÌÎÏÓÒÕÖÔÚÙÛÜÇ';
    const sinAcento = 'aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC';
    const normalizedStr = string
      .split('')
      .map(char => {
        const charIdx = conAcento.indexOf(char)
        if (charIdx !== -1) {
          return sinAcento[charIdx]
        }
        return char
      })
      .join('')
    return normalizedStr;
  }

  /**
   * Hacer la pantalla fullscreen.
   */
  toggleFullscreen() {
    this.fullscreen = !this.fullscreen;
    if (this.fullscreen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'auto';
    }
  }

  /**
     * Muestra/Oculta un panel según nombre de clase
     * que lo identifica.
     * Sólo un panel puede estar activo a la vez.
     * @param className
     */
  togglePanel(className: string): void {
    let panels = document.getElementsByClassName('aux-panel') as HTMLCollectionOf<HTMLElement>;
    Array.from(panels).forEach(panel => {
      if (panel.id === className && panel.classList.contains('hidden')) {
        panel.classList.remove('hidden');
      }
      else {
        panel.classList.add('hidden');
      }
    });
  }

  /**
     * Colapsa la fila expandida.
     */
  collapseExpandedRow() {
    let elementExpanded = document.querySelector('.pi-chevron-down') as HTMLElement;
    if (elementExpanded != null) {
      elementExpanded.click();
    }
  }

  /**
   * filtro de busqueda de las entradas de servicio por rango de fechas.
   */
  onBuscar() {
    // MMSN-519: Colapsar fila expandida al activar un filtro.
    this.collapseExpandedRow();
    this.recalculandoAprobadas = true;

    if (this.proveedorSeleccionado !== undefined) {
      this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
    }
    else {
      this.proveedor = '';
    }

    this.obtenerESSap(this.proveedor, this.documentoNumero);
  }

  /**
   * metodo para mostrar u ocultar el panel auxiliar de filtro por fecha.
   * @returns boolen
   */
  showOrHideAuxPanel(): boolean {
    const estadoCertificacion: estadoCertificacion = { name: 'Estado: Aprobadas', code: 'Aprobada' };
    if (this.estadoCertificacion.code === estadoCertificacion.code) {
      return true;
    }
    return false;
  }

  /**
   * Metodo para actualizar el aprobador en la lista de la tabla al ejecutar la accion de reasignar.
   * @param es Nro entrada de servicio
   */
  updateApprover(es: string, esUpdated: any): void {
    this.tablaPOAprobaciones.filter(e => e.EntradaServicio === es)
      .forEach(x => {
        x.Suplente = esUpdated.newSubstitute;
        x.Aprobador = esUpdated.newApprover;
      })
  }

  /**
   * Metodo para actualizar la tabla luego de ejecutar una acción sin llamar al servicio que lista los registros.
   * @param es Nro entrada de servicio
   * @param action accion ejecutada
   * @param esRejected entrada de servicio rechaza solo para los casos de rechazo.
   */
  updateStateFromPending(es: string, action: string, esRejected?: any): void {
    if (action === 'Aprobar') {
      const indices = this.tablaPOAprobaciones
        .map((ap, index) => ap.EntradaServicio === es ? index : -1)
        .filter(index => index !== -1);

      // Eliminar los elementos desde el final hacia el inicio para evitar problemas de reindexación
      indices.reverse().forEach(index => {
        this.tablaPOAprobaciones.splice(index, 1);
      });
    } else {
      this.tablaPOAprobaciones.filter(ap => ap.EntradaServicio === es).forEach(x => {
        x.Estado = esRejected.es.Estado_certificacion;
        x.MotivoRechazo = esRejected.es.Motivo_rechazo;
        x.FechaRechazo = esRejected.dateReject;
      });
    }
    document.getElementById(es).style.display = 'none';
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

  loadingRows = new Map<number, boolean>();

  descargarArchivos(rowData: any, index: number) {

    this.loadingRows[index] = true;
    this.cdr.detectChanges();

    this.service.GetAdjuntosByES(rowData.EntradaServicio.toString()).subscribe(result => {
      if (result.data.length > 0) {

        result.data.forEach((archivo) => {
          this.descargarArchivo(archivo.Adjuntos, archivo.NombreArchivo, archivo.Extension);
        });
      }
      else {

        this.confirmationService.confirm({
          message: "<ul>" + "No se encontraron adjuntos a descargar" + "</ul>",
          rejectVisible: false
        });

      }

      this.loadingRows[index] = false;
      this.cdr.detectChanges();
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

  getPorcentajeAnterior(cantidadAnterior: number, cantidad: string): string {
    const porcentajeAnterior: number = (cantidadAnterior * 100) / parseFloat(cantidad);
    return `${parseFloat(porcentajeAnterior.toFixed(2))}%`;
  }

  getMontoAnterior(cantidadAnterior: number, monto: number, moneda: string): string {
    const montoAnterior: number = cantidadAnterior * monto;
    const coin: string = moneda === 'ARP' ? '$ ' : '';
    const montoFormatted: string = montoAnterior.toLocaleString('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });

    return `${coin}${montoFormatted}`;
  }

  getPorcentajeAcumulado(cantidadAnterior: number, cantidad: string, porcentajeCertificar: string): string {
    const porcentajeAcumulado: number = ( (cantidadAnterior * 100) / parseFloat(cantidad)) + (parseFloat(porcentajeCertificar) * 1);
    return `${parseFloat(porcentajeAcumulado.toFixed(2))}%`;
  }

  getMontoAcumulado(cantidadAnterior: number, monto: number, cantidadAcertificar: string, moneda: string): string {
    const montoAcumulado: number = (cantidadAnterior * monto) + (parseFloat(cantidadAcertificar) * monto);
    const coin: string = moneda === 'ARP' ? '$ ' : '';
    const montoFormatted: string = montoAcumulado.toLocaleString('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });

    return `${coin}${montoFormatted}`;
  }


}