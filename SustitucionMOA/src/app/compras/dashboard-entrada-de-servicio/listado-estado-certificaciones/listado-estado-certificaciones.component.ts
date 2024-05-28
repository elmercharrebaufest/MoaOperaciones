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

@Component({
  selector: 'app-listado-estado-certificaciones',
  templateUrl: './listado-estado-certificaciones.component.html',
  styleUrls: ['./listado-estado-certificaciones.component.css']
})
export class ListadoEstadoCertificacionesComponent extends ListBaseComponent implements OnInit, OnDestroy {
  //#region Variables 
  subscripciones: Subscription[] = [];
  protected locale: any;
  @ViewChild("tabla") protected tabla: Table;
  @ViewChild("elementToToggle") protected elementToToggle: ElementRef<HTMLDivElement>;
  private destroy$: Subject<void> = new Subject<void>();
  @BlockUI() blockUI: NgBlockUI;
  @ViewChild(SpinnerComponent) protected spinnerComponent: SpinnerComponent;
  nroSolp: string = "";
  ordenAscendente: boolean = false;
  columnaOrden: string = "FechaCreacion";
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
  innerWidth: number;
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
    { name: 'Servicio realizado con resultado distinto al contratado', code: '3' },
    { name: 'Falta de presentación de documentación', code: '4' }
  ];
  suplentes: any = [];
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
        { id: 'cID_ES', header: 'ID_ES', field: 'ID_ES', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cFecha', header: 'Fecha', field: 'FechaCreacion', type: 'date', sortable: true, required: false, visible: true },
        { id: 'cOrdenCompra', header: 'Número de OC', field: 'OrdenCompra', type: 'string', sortable: true, required: true, visible: true },
        { id: 'cCuit', header: 'CUIT', field: 'CUIT', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cProveedor', header: 'Proveedor', field: 'Proveedor', type: 'string', sortable: true, required: true, visible: true },
        { id: 'cDescripción', header: 'Descripción', field: 'Descripción', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cMontoTotal', header: 'Monto total', field: 'MontoTotal', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cIngresante', header: 'Ingresante', field: 'Ingresante', type: 'string', sortable: true, required: false, visible: true },
        { id: 'cUsuario', header: 'Usuario', field: 'Usuario', type: 'string', sortable: true, required: false, visible: true },
        { id: 'cEstado', header: 'Estado', field: 'Estado', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cAcciones', header: 'Acciones', field: 'Acciones', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cReasignar', header: 'Reasignar', field: 'Reasignar', type: 'string', sortable: false, required: false, visible: true },
        { id: 'cAprobador', header: 'Aprobador', field: 'Aprobador', type: 'string', sortable: true, required: false, visible: true },
        { id: 'cMotivoRechazo', header: 'Motivo de rechazo', field: 'MotivoRechazo', type: 'string', sortable: false, required: false, visible: false },
      ]
    },
    {
      name: 'ESDetalle',
      columns: [
        { id: 'DPosicion', header: 'N° de Ítem', field: 'Posicion', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DMaterial', header: 'N° de Servicio', field: 'Material', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DTxtBrev', header: 'Descripción', field: 'TxtBrev', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DCtdPedido', header: 'Cantidad', field: 'CtdPedido', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DU', header: 'UM', field: 'U', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DT', header: 'Monto', field: 'T', type: 'string', sortable: false, required: false, visible: true },
        //{ id: 'DCantidadReal', header: 'Cantidad Real', field: 'CantidadReal', type: 'string', sortable: false, required: false, visible: true },
        //{ id: 'DPorcentaje', header: 'PORC. %', field: 'Porcentaje', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DCantidadCertificar', header: 'Cantidad a certificar', field: 'CantidadCertificar', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DPorcentajeCertificar', header: 'Porcentaje a certificar', field: 'PorcentajeCertificar', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DMontoCertificar', header: 'Monto a certificar', field: 'MontoCertificar', type: 'string', sortable: false, required: false, visible: true },
        { id: 'DNumeroRemito', header: 'Nro. Remito', field: 'NumeroRemito', type: 'string', sortable: false, required: false, visible: true }
      ]
    }
  ];
  userId: any = '';
  tablaPO: any[] = [];
  cols: any[];
  usuario: string;
  vendedor: string;
  allItems : any[];
  proveedor: string = sessionStorage.getItem("proveedor");
  msgs: Message[] = [];
  havePermision: boolean = false;
  observaciones: string = '';

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
  }

  public ngOnDestroy(): void {
    this.subscripciones.forEach(sub => sub.unsubscribe());
  }
      
  ngOnInit() {

    this.getListarPO(this.proveedor, this.documentoNumero);

    this.formsCreate();

    this.innerWidth = window.innerWidth;

    this.navService.setSeccionActive("Estado certificaciones");

    this.navService.navegarSeccion("compras/listadoEstadoCertificaciones");
    
    let permisos = sessionStorage.getItem("permisos");
    
    if(permisos && permisos.includes("VER TODOS LOS ESTADOS DE ES"))
    {
      this.havePermision = true;
    }
  }


  showContainerTable(): void {
    this.spinnerComponent.hideIt();
    if (this.elementToToggle) {
      this.elementToToggle.nativeElement.style.display = 'block';
    }
  }

  hideContainerTable(): void {
    this.spinnerComponent.showIt();
    if (this.elementToToggle) {
      this.elementToToggle.nativeElement.style.display = 'none';
    }
  }

  formsCreate(): void {
    this.formularioMotivosRechazo = new FormGroup({
      motivo: new FormControl({name: null, code: null}, Validators.required),
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
      observaciones: new FormControl(null)
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

    getListarPO(proveedor, documentoNumero) {
      this.getFecha();
      this.hideContainerTable();
      this.subscripciones.push(
        this.service.getByProveedorAsync(this.fechaInicio, proveedor, documentoNumero, this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize, this.isAll).subscribe(
        (result:any) => {
          if (result.logout == true) {
            this.sessionDataService.logout();
          } else if (result.error != undefined && result.error != "") {
          } else if (result.info != undefined) {
          } else {
            this.tablaPO = result.data; 
            this.setColumsByUserProfile(this.tablaPO, this.usuario);
            this.length = result.data.length > 0 ? result.data[0].ItemsTotales : result.data.length;
            this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
            this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;
          }
          this.tabla.filter(["Pendiente Aprobación"], "Estado", "in");
          this.showContainerTable();
        }, error => {
          this.floatMsgService.setErrorMsg(error.message);
          this.showContainerTable();
        })
      );
      this.clear();
    }

  clear() {
    setTimeout(() => {
      this.messageService.clear();
    }, 10000)
  }

  displayContent() {
    return !this.spinnerComponent.visible;
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

  verMotivosRechazos(rowData: any): void {
    this.formularioMotivosRechazo.controls['destinatario'].patchValue(rowData.Usuario);
    this.formularioMotivosRechazo.controls['proveedor'].patchValue(rowData.Proveedor);
    this.formularioMotivosRechazo.controls['descripcion'].patchValue(rowData.Descripcion);
    this.formularioMotivosRechazo.controls['importe'].patchValue(rowData.Importe);
    this.formularioMotivosRechazo.controls['montoTotal'].patchValue(rowData.MontoTotal);
    this.formularioMotivosRechazo.controls['detalleServicio'].patchValue(rowData.entradaServicioDetalle);
    this.formularioMotivosRechazo.controls['numeroCertificacion'].patchValue(rowData.NumeroCertificacion);
    this.formularioMotivosRechazo.controls['fechaCertificacion'].patchValue(rowData.FechaCreacion);
    this.mostrarMotivosRechazos = true;
  }

  filtrarPorEstado(event: any): void {
    switch (event.value.code) {
      case 'Aprobada':
        this.defaultTablesConfig[0].columns.forEach(col => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Acciones' || col.field === 'Reasignar'? false : true;
        });
        break;
      case 'Pendiente Aprobación':
        this.setColumsByUserProfile(this.tablaPO, this.usuario);
        break;
      case 'Rechazado':
        this.defaultTablesConfig[0].columns.forEach(col => {
          col.visible = col.field === 'Aprobador' || col.field === 'Acciones' || col.field === 'Reasignar'? false : true;
        });
        break;
    }
    this.tabla.filter(event.value.code, 'Estado', 'contains');
  }

  filtrarPorArea(event: any): void {
    let filtrarAreas = [];
    event.value.forEach((area: any) => {
      filtrarAreas.push(area);
    });
    this.tabla.filter(filtrarAreas, 'UsuarioArea', 'contains');
  }

  verSuplentes(suplente: string, nro_es_local: string): void {
    this.formularioSuplente.controls['suplente'].patchValue(suplente);
    this.formularioSuplente.controls['nro_es_local'].patchValue(nro_es_local);
    this.mostrarSuplentes = true;
  }

  reasignar():void {
    const data = {
      Suplente: this.formularioSuplente.get('suplente').value,
      NroEsLocal: this.formularioSuplente.get('nro_es_local').value
    }
    this.subscripciones.push(
      this.service.reasignarSuplente(data).subscribe(
        (resp: any) => {
          this.getListarPO(this.proveedor, this.documentoNumero);
          this.mostrarSuplentes = false;
          this.messageService.add({severity: 'success', summary: 'Reasignación exitosa!', detail: 'Estamos refrescando los datos para que puedas ver los cambios.'});
        }, error => {
          this.messageService.add({severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.'});
        }
      )
    );
  }

  enviarMotivo() {
    const data = {
      Destinatario: this.formularioMotivosRechazo.get('destinatario').value,
      MotivoRechazo: this.formularioMotivosRechazo.get('observaciones').value != null ? this.formularioMotivosRechazo.get('motivo').value.name + '. Observación:' + this.formularioMotivosRechazo.get('observaciones').value : this.formularioMotivosRechazo.get('motivo').value.name,
      Proveedor: this.formularioMotivosRechazo.get('proveedor').value,
      NumeroCertificacion: this.formularioMotivosRechazo.get('numeroCertificacion').value,
      FechaCertificacion: this.formularioMotivosRechazo.get('fechaCertificacion').value,
      Descripcion: this.formularioMotivosRechazo.get('descripcion').value,
      Importe: this.formularioMotivosRechazo.get('importe').value,
      MontoTotal: this.formularioMotivosRechazo.get('montoTotal').value,
      DetalleServicio: this.formularioMotivosRechazo.get('detalleServicio').value.map((item: any) => ({
        Descripcion: item.Descripcion,
        Cantidad: item.Cantidad,
        UM: item.UM,
        Porcentaje: item.Porcentaje,
        Monto: item.Monto
      }))
    }
    this.subscripciones.push(
      this.service.enviarMotivoRechazoES(data).subscribe(
        (resp: any) => {
          this.resetForm();
          this.getListarPO(this.proveedor, this.documentoNumero);
          this.mostrarMotivosRechazos = false;
          this.messageService.add({severity: 'success', summary: 'Rechazo exitoso!', detail: 'Estamos refrescando los datos para que puedas ver los cambios.'});
        }, error => {
          this.messageService.add({severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.'});
        }
      )
    );
  }

  ocultandoModal(): void{
    this.resetForm();
  }

  resetForm() {
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
      observaciones: null
    });
    this.formularioMotivosRechazo.markAsPristine();
    this.formularioMotivosRechazo.markAsUntouched();
    this.formularioMotivosRechazo.updateValueAndValidity();
  }

  enviarAprobacion(nro_es_local: string): any {
    this.clear();
    this.confirmationService.confirm({
      message: '¿Esta seguro que desea aprobar esta Entrada de Servicio?',
      header: 'Confirmar Aprobación',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.hideContainerTable();
        this.subscripciones.push(
          this.service.enviarAprobacionES(nro_es_local).subscribe(
            resp => {
              let mensajeError: string = "";
              if (!resp.data) {
                mensajeError = 'del servidor, vuelva a intentarlo más tarde.'
                this.messageService.add({severity:'error', summary:'Error', detail: mensajeError});
                this.showContainerTable();
              }
              switch (resp.data.Type) {
                case "I": {
                  this.getListarPO(this.proveedor, this.documentoNumero);
                  this.messageService.add({severity: 'success', summary: 'Aprobado', detail: resp.data.Message});
                  break;
                }
                case "S": {
                  this.messageService.add({severity: 'info', summary: '', detail: resp.data.Message});
                  this.showContainerTable();
                  break;
                }
                case "E": {
                  mensajeError = resp.data.Message.startsWith("Sólo es posible contabilizar en ") ||
                    resp.data.Message.startsWith("Contabilice en ") ?
                    "El período se encuentra cerrado, por favor contabilice en el periodo actual." : resp.data.Message;
                  this.messageService.add({severity: 'warning', summary: '', detail: mensajeError});
                  this.showContainerTable();
                  break;
                }
                default: {
                  this.messageService.add({severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.'});
                  this.showContainerTable();
                  break;
                }
              }
            }, error => {
              this.messageService.add({severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Hemos dectectado un error por favor intentelo de nuevo.'});
              this.showContainerTable();
            }
          )
        );
      }
    });
  }

  isAll: boolean = false;

  SeeAll(){
    this.isAll = true;
    this.getListarPO("", this.documentoNumero);
    
  }

  SeeForProvider(){
    this.isAll = false;
    this.getListarPO(this.proveedor, this.documentoNumero);
  }

  setColumsByUserProfile(entradasDeServicio: any, user: any): void {
    const pendienteAprobacion = entradasDeServicio.filter(pa => pa.Estado === 'Pendiente Aprobación');
    if(pendienteAprobacion.length > 0){
      const fai = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador,user) && this.equalsIgnoreCase(pa.Ingresante,user) && this.equalsIgnoreCase(pa.Fiscal,user));
      if(fai.length > 0){
        this.userId = "FAI";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' ? false : true;
        });
        return;
      }
      const ai = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador,user) && this.equalsIgnoreCase(pa.Ingresante,user) && !this.equalsIgnoreCase(pa.Fiscal,user));
      if(ai.length > 0){
        this.userId = "AI";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Reasignar' ? false : true;
        });
        return;
      }
      const fa = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador,user) && !this.equalsIgnoreCase(pa.Ingresante,user) && this.equalsIgnoreCase(pa.Fiscal,user));
      if (fa.length > 0) {
        this.userId = "FA";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' ? false : true;
        });
          return;
        }
      const fi = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador,user) && this.equalsIgnoreCase(pa.Ingresante,user) && this.equalsIgnoreCase(pa.Fiscal,user));
      if(fi.length > 0){
        this.userId = "FI";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Acciones' ? false : true;
        });
        return;
      }
      const a = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador,user) && !this.equalsIgnoreCase(pa.Ingresante,user) && !this.equalsIgnoreCase(pa.Fiscal,user));
      if(a.length > 0){
        this.userId = "A";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Reasignar' || col.field === 'Aprobador' ? false : true;
        });
        return;
      }
      const i = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador,user) && this.equalsIgnoreCase(pa.Ingresante,user) && !this.equalsIgnoreCase(pa.Fiscal,user));
      if(i.length > 0){
        this.userId = "I";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Reasignar' || col.field === 'Acciones' || col.field === 'Ingresante' ? false : true;
        });
        return;
      }
      const f = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador,user) && !this.equalsIgnoreCase(pa.Ingresante,user) && this.equalsIgnoreCase(pa.Fiscal,user));
      if(f.length > 0){
        this.userId = "F";
        this.defaultTablesConfig[0].columns.forEach((col: any) => {
          col.visible = col.field === 'MotivoRechazo' || col.field === 'Acciones' || col.field === 'Usuario' ? false : true;
        });
        return;
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
}