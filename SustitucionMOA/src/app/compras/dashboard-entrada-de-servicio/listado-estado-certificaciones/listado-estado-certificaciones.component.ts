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
import { ConfirmationService, Message, MessageService, SortEvent } from 'primeng/api';
import { DropdownOption } from '../../../common/view-child/dropdown/dropdown.component';
import * as XLSX from 'xlsx';

export interface estadoCertificacion {
    name: string,
    code: string
}

export interface autoCompleteObject {
    valor: string;
    CodigoProveedor: string;
};
import { certificacionES } from '../components/modal-aprobacion/modalAprobacion.interface';
import { TableCustomSort } from '../tableCustomSort.helper';
import { EntradaServicioCabeceraDto } from '../../../common/models/ordenes-compra/entradaServicioCabecera';

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
    filtroFechaDesde: string;
    filtroFechaHasta: string;
    fullscreen: boolean = false;
    userId: any = '';
    tablaPOAprobaciones: EntradaServicioCabeceraDto[] = [];
    tablaPOSap: EntradaServicioCabeceraDto[] = [];
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
    ordenCompraFiltro: string = "";

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
                { id: 'cFechaAprobacion', header: 'Fecha Aprobada', field: 'FechaAprobacion', type: 'date', sortable: true, required: false, visible: false },
                { id: 'cFechaRechazo', header: 'Fecha Rechazo', field: 'FechaRechazo', type: 'date', sortable: true, required: false, visible: false },
                { id: 'cFecha', header: 'Fecha Creación', field: 'FechaCreacion', type: 'date', sortable: true, required: false, visible: false },
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
                { id: 'DPosicion', header: 'N° Posición', field: 'Posicion', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DItem', header: 'N° Ítem', field: 'Item', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DMaterial', header: 'N° Servicio', field: 'Material', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DTxtBrev', header: 'Descripción', field: 'TxtBrev', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DCtdPedido', header: 'Cant.', field: 'CtdPedido', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DU', header: 'UM', field: 'U', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DT', header: 'Precio Unitario', field: 'T', type: 'string', sortable: false, required: false, visible: true },
                { id: 'DNumeroRemito', header: 'Nro. Remito', field: 'NumeroRemito', type: 'string', sortable: false, required: false, visible: true }
            ]
        }
    ];


    public get columnaVisible_FechaAprobacion(): boolean {
        return this.defaultTablesConfig.find(x => x.name === "Certificaciones").columns.find(x => x.field === "FechaAprobacion").visible;
    }

    public get columnaVisible_FechaRechazo(): boolean {
        return this.defaultTablesConfig.find(x => x.name === "Certificaciones").columns.find(x => x.field === "FechaRechazo").visible;
    }

    public get columnaVisible_FechaCreacion(): boolean {
        return this.defaultTablesConfig.find(x => x.name === "Certificaciones").columns.find(x => x.field === "FechaCreacion").visible;
    }

    private estadoSeleccionado?: string;



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

    sortFunction(event: SortEvent, tableName: string): void {
        TableCustomSort.sortFunction(event, tableName, this.defaultTablesConfig);
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

        // Esperar un momento para que el aux-panel emita las fechas iniciales
        setTimeout(async () => {
            await this.getListarPO(this.filtroFechaDesde, this.filtroFechaHasta);
            this.obtenerESSap(this.proveedor, this.documentoNumero);
        }, 100);
    }

    // Nuevo método para establecer las fechas iniciales basándose en la lógica del aux-panel
    setFechasIniciales(): void {
        const fechaActual = new Date();
        fechaActual.setDate(fechaActual.getDate() - 2);
        this.filtroFechaDesde = fechaActual.toISOString().slice(0, 10);
        this.filtroFechaHasta = new Date().toISOString().slice(0, 10);
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

    getListarPO(fechaDesde: string = "", fechaHasta: string = ""): Promise<void> {
        this.recalculando = true;
        this.tablaPOAprobaciones = [];
        return new Promise<void>((resolve, reject) => {
            const subscription = this.service.ObtenerESLocales(this.isAll, '', fechaDesde, fechaHasta, this.ordenCompraFiltro).subscribe(
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


    obtenerESSap(proveedor: string, documentoNumero: string): Promise<void> {
        this.recalculandoAprobadas = true;
        this.tablaPOSap = [];
        return new Promise<void>((resolve, reject) => {
            const subscription = this.service
                .getByProveedorAsync(
                    this.filtroFechaDesde,
                    this.filtroFechaHasta,
                    proveedor,
                    documentoNumero,
                    this.columnaOrden,
                    this.ordenAscendente,
                    this.pageIndex,
                    this.pageSize,
                    this.isAll,
                    this.ordenCompraFiltro)
                .subscribe(
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

    onFiltroFechaDesdeChanged(fechaDesde: string) {
        this.filtroFechaDesde = fechaDesde;
    }

    onFiltroFechaHastaChanged(fechaHasta: string) {
        this.filtroFechaHasta = fechaHasta;
    }

    getOrdenCompraFiltro(ocIngresada: string) {
        this.ordenCompraFiltro = ocIngresada;
        this.getListarPO();
    }

    toggleRow(rowData: any): void {
        this.selectedRow = this.selectedRow === rowData ? null : rowData;
    }

    verMotivosRechazos(rowData: any): void {
        const formMotivosRechazo = this.formularioMotivosRechazo;
        if (formMotivosRechazo) {
            formMotivosRechazo.controls['destinatario'].patchValue(rowData.Ingresante);
            formMotivosRechazo.controls['proveedor'].patchValue(rowData.Proveedor);
            formMotivosRechazo.controls['descripcion'].patchValue(rowData.Descripcion);
            formMotivosRechazo.controls['montoTotal'].patchValue(rowData.MontoTotal);
            formMotivosRechazo.controls['detalleServicio'].patchValue(rowData.entradaServicioDetalle);
            formMotivosRechazo.controls['numeroCertificacion'].patchValue(rowData.NumeroCertificacion);
            formMotivosRechazo.controls['fechaCertificacion'].patchValue(rowData.FechaCreacion);
            formMotivosRechazo.controls['moneda'].patchValue(rowData.Moneda);
        }
        this.mostrarMotivosRechazos = true;
    }

    async filtrarPorEstado(event: any): Promise<void> {
        this.estadoSeleccionado = event.value.code;

        switch (event.value.code) {
            case 'Aprobada':
                this.tabla.reset();
                this.defaultTablesConfig[0].columns.forEach(col => {
                    col.visible = col.field === 'MotivoRechazo' || col.field === 'Acciones' || col.field === 'Reasignar' || col.field === 'FechaRechazo' || col.field === 'AnuladoPor' ? false : true;
                });
                break;
            case 'Pendiente Aprobación':
                // Remover esta llamada duplicada
                // if (this.tablaPOAprobaciones.length < 1) {
                //     this.getListarPO();
                // }
                this.setColumsByUserProfile(this.tablaPOAprobaciones, this.usuario);
                break;
            case 'Rechazado':
                // Remover esta llamada duplicada
                // if (this.tablaPOAprobaciones.length < 1) {
                //     this.getListarPO();
                // }
                this.defaultTablesConfig[0].columns.forEach(col => {
                    col.visible = col.field === 'Aprobador' || col.field === 'Acciones' || col.field === 'Reasignar' || col.field === 'FechaAprobacion' || col.field === 'AnuladoPor' ? false : true;
                });
                break;
            case 'Anulada':
                // Remover esta llamada duplicada
                // if (this.tablaPOAprobaciones.length < 1) {
                //     this.getListarPO();
                // }
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
                    const msj = { severity: 'success', summary: 'Reasignación exitosa!', detail: 'Se reasignó al nuevo aprobador.' };
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
                Monto: this.formularioMotivosRechazo.get('moneda').value === 'ARP' ? '$ ' + item.MontoCertificar : this.formularioMotivosRechazo.get('moneda').value + ' ' + item.MontoCertificar
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
                    Importe: item.Monto,
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
            message: '¿Está seguro de que desea aprobar esta Entrada de Servicio?',
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
                            this.mostrarModalAprobaciones = false;
                            this.blockUI.stop();
                            this.clearMessage();
                            if (!resp.data || resp.error) {
                                this.messageService.add({ severity: 'error', summary: 'Error', detail: resp.error || 'del servidor, vuelva a intentarlo más tarde.' });
                            }
                            else {
                                const tipoResp = resp.data.Type || '';
                                switch (tipoResp) {
                                    case "I": {
                                        this.messageService.add({ severity: 'success', summary: 'Aprobado', detail: resp.data.Message });
                                        this.updateStateFromPending(numeroCertificacion, 'Aprobar');
                                        this.getFecha("1");
                                        this.obtenerESSap(this.proveedor, this.documentoNumero);
                                        break;
                                    }
                                    case "S":
                                    case "Desync":
                                        this.messageService.add({ severity: 'info', summary: '', detail: resp.data.Message });
                                        break;
                                    case "E": {
                                        let mensajeError: string = !!resp.data.Message && (resp.data.Message.startsWith("Sólo es posible contabilizar en ") || resp.data.Message.startsWith("Contabilice en ")) ?
                                            "El período se encuentra cerrado, por favor contabilice en el periodo actual." : (resp.data.Message || '');
                                        this.messageService.add({ severity: 'warning', summary: '', detail: mensajeError });
                                        break;
                                    }
                                    default: {
                                        this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Por favor inténtelo de nuevo.' });
                                        break;
                                    }
                                }
                            }
                        },
                        errorResp => {
                            console.error(errorResp);
                            this.mostrarModalAprobaciones = false;
                            this.blockUI.stop();
                            this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: 'Por favor inténtelo de nuevo.' });
                            this.clearMessage();
                        }
                    )
                );
            }
        });
    }

    async SeeAll() {
        await this.SeeCommon(true);
    }

    async SeeForProvider() {
        await this.SeeCommon(false);
    }

    private async SeeCommon(isAll: boolean) {
        this.blockUI.start('Cargando...');
        this.isAll = isAll;
        const mockEvent = { value: { code: this.estadoCertificacion.code } };

        if (this.estadoCertificacion.code === 'Aprobada') {
            await this.obtenerESSap(this.proveedor, this.documentoNumero);
            await this.filtrarPorEstado(mockEvent);
            this.blockUI.stop();
            await this.getListarPO(this.filtroFechaDesde, this.filtroFechaHasta); // Pasar fechas
        } else {
            await this.getListarPO(this.filtroFechaDesde, this.filtroFechaHasta); // Pasar fechas
            await this.filtrarPorEstado(mockEvent);
            this.blockUI.stop();
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
        const conservarColumnas = this.estadoSeleccionado === 'Aprobada'; // para este estado, las columnas se seleccionan en otro lado, por lo que no se deben cambiar las columnas mostradas.

        let userRole: string;

        if (pendienteAprobacion.length > 0 && !conservarColumnas) {

            const sf = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Fiscal, user) && this.equalsIgnoreCase(pa.Suplente, user));
            const fai = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
            const ai = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && !this.equalsIgnoreCase(pa.Fiscal, user));
            const fa = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && !this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
            const fi = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
            const a = pendienteAprobacion.filter(pa => this.equalsIgnoreCase(pa.Aprobador, user) && !this.equalsIgnoreCase(pa.Ingresante, user) && !this.equalsIgnoreCase(pa.Fiscal, user));
            const f = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && !this.equalsIgnoreCase(pa.Ingresante, user) && this.equalsIgnoreCase(pa.Fiscal, user));
            const i = pendienteAprobacion.filter(pa => !this.equalsIgnoreCase(pa.Aprobador, user) && this.equalsIgnoreCase(pa.Ingresante, user) && !this.equalsIgnoreCase(pa.Fiscal, user));

            if (sf.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "SF";

            } else if (fai.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "FAI";

            } else if (ai.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                //this.defaultTablesConfig[0].columns.find(x => x.field === 'Reasignar').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "AI";

            } else if (fa.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "FA";

            } else if (fi.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'Acciones').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "FI";

            } else if (a.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                //this.defaultTablesConfig[0].columns.find(x => x.field === 'Reasignar').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "A";

            } else if (f.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "F";

            } else if (i.length > 0) {
                this.defaultTablesConfig[0].columns.forEach((col: any) => {
                    col.visible = true;
                });

                this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                //this.defaultTablesConfig[0].columns.find(x => x.field === 'Reasignar').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'Acciones').visible = false;
                this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                userRole = "I";

            } else {
                if (fai.length === 0 && ai.length === 0 && fa.length === 0 && fi.length === 0 && a.length === 0 && i.length === 0 && f.length === 0) {
                    this.defaultTablesConfig[0].columns.forEach((col: any) => {
                        col.visible = true;
                    });

                    this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'Acciones').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                }

                if (fai.length === 0 && ai.length === 0 && fa.length === 0 && fi.length === 0 && a.length === 0 && i.length === 0 && f.length === 0) {
                    this.defaultTablesConfig[0].columns.forEach((col: any) => {
                        col.visible = true;
                    });

                    this.defaultTablesConfig[0].columns.find(x => x.field === 'MotivoRechazo').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaAprobacion').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'FechaRechazo').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'Acciones').visible = false;
                    this.defaultTablesConfig[0].columns.find(x => x.field === 'AnuladoPor').visible = false;

                }
            }
        }

        return userRole;
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

    async onBuscar() {
        this.collapseExpandedRow();

        if (this.proveedorSeleccionado !== undefined) {
            this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
        }
        else {
            this.proveedor = '';
        }

        await this.getListarPO(this.filtroFechaDesde, this.filtroFechaHasta);
        this.obtenerESSap(this.proveedor, this.documentoNumero);
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
        const coin: string = moneda === 'ARP' ? '$ ' : moneda + ' ';
        const montoFormatted: string = montoAnterior.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

        return `${coin}${montoFormatted}`;
    }

    getPorcentajeAcumulado(cantidadAnterior: number, cantidad: string, porcentajeCertificar: string): string {
        const porcentajeAcumulado: number = ((cantidadAnterior * 100) / parseFloat(cantidad)) + (parseFloat(porcentajeCertificar) * 1);
        return `${parseFloat(porcentajeAcumulado.toFixed(2))}%`;
    }

    getMontoAcumulado(cantidadAnterior: number, monto: number, cantidadAcertificar: string, moneda: string): string {
        const montoAcumulado: number = (cantidadAnterior * monto) + (parseFloat(cantidadAcertificar) * monto);
        const coin: string = moneda === 'ARP' ? '$ ' : moneda + ' ';
        const montoFormatted: string = montoAcumulado.toLocaleString('en-US', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

        return `${coin}${montoFormatted}`;
    }

    refreshDataTable() {
        if (this.estadoCertificacion.code === 'Aprobada' && !this.recalculandoAprobadas) {
            this.obtenerESSap(this.proveedor, this.documentoNumero);
        }
        if ((this.estadoCertificacion.code === 'Aprobada' || this.estadoCertificacion.code === 'Pendiente Aprobación' || this.estadoCertificacion.code === 'Rechazado' || this.estadoCertificacion.code === 'Anulada') && !this.recalculandoAprobadas) {
            // Pasar las fechas actuales al refrescar
            this.getListarPO(this.filtroFechaDesde, this.filtroFechaHasta);
        }
    }


    isAuthorized(permiso: string) {
        return this.securityService.tienePermiso(permiso);
    }

    correrReasignacionManual() {
        this.confirmationService.confirm({
            key: 'reasignacion',
            header: 'Ejecutar proceso de derivación automática',
            message: '¿Está seguro de que desea correr el proceso de derivación manualmente?',
            accept: () => {
                this.reasignacionManual()
            },
            reject: () => {
            }
        });
    }

    reasignacionManual(): void {
        this.blockUI.start();
        this.service.runReasignacion().subscribe(
            (resp: any) => {
                const msj = { severity: 'success', summary: 'Proceso de derivación automática exitoso', detail: '' };
                if (resp.error) {
                    msj.severity = 'error';
                    msj.summary = resp.error
                    msj.detail = '';
                }
                this.messageService.add(msj);
                this.refreshDataTable();
                this.blockUI.stop();
                this.clearMessage();
            }, error => {
                this.messageService.add({ severity: 'error', summary: 'Ha ocurrido un error.', detail: error });
                this.blockUI.stop();
                this.clearMessage();
            }
        );
    }

    EXCEL_TYPE: string = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    EXCEL_EXTENSION: string = '.xlsx';

    obtenerEncabezados(estado: string): string[] {
        const columnas = this.defaultTablesConfig[0].columns;
        let columnasExcluir: string[] = [];
        columnasExcluir = ['cAcciones', 'esAdjuntos', 'cEstado'];

        if (estado === 'Rechazado') {
            columnasExcluir = [...columnasExcluir, 'cAprobador', 'cAnulador', 'cFechaAprobacion'];
        } else if (estado === 'Aprobada') {
            columnasExcluir = [...columnasExcluir, 'cMotivoRechazo', 'cAnulador', 'cFechaRechazo'];
        } else if (estado === 'Anulada') {
            columnasExcluir = [...columnasExcluir, 'cAprobador', 'cMotivoRechazo', 'cFecha', 'cFechaAprobacion', 'cFechaRechazo'];
        } else if (estado === 'Pendiente Aprobación') {
            columnasExcluir = [...columnasExcluir, 'cMotivoRechazo', 'cAnulador', 'cFechaAprobacion', 'cFechaRechazo'];
        }

        return columnas
            .filter(col => !columnasExcluir.includes(col.id))
            .map(col => col.header);
    }

    exportarTablaAExcel() {
        const datos = [...this.tablaPOAprobaciones, ...this.tablaPOSap]
        const datosPorEstado = this.agruparDatosPorEstado(datos);
        this.exportarDatosAExcelFile(datosPorEstado);
    }

    agruparDatosPorEstado(datos: any[]): { [key: string]: any[][] } {
        const datosPorEstado: { [key: string]: any[][] } = {};

        datos.forEach(item => {
            const estado = item.Estado || 'Sin_Estado';
            if (!datosPorEstado[estado]) {
                datosPorEstado[estado] = [];
            }
            let filaDatos: any[] = [];

            let montoTotal = item.Moneda === 'ARP' ? '$ ' + (parseFloat(item.MontoTotal).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            })) : item.Moneda + ' ' + (parseFloat(item.MontoTotal).toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }));

            if (item.Estado === 'Pendiente Aprobación') {
                filaDatos = [
                    item.EntradaServicio,
                    item.FechaCreacion,
                    item.OrdenCompra,
                    item.CUIT,
                    item.Proveedor,
                    item.Descripcion,
                    montoTotal,
                    item.Ingresante,
                    item.Aprobador,
                ];
            }

            if (item.Estado === 'Rechazado') {
                filaDatos = [
                    item.EntradaServicio,
                    item.FechaRechazo,
                    item.FechaCreacion,
                    item.OrdenCompra,
                    item.CUIT,
                    item.Proveedor,
                    item.Descripcion,
                    montoTotal,
                    item.Ingresante,
                    item.MotivoRechazo,
                ];
            }

            if (item.Estado === 'Anulada') {
                filaDatos = [
                    item.EntradaServicio,
                    item.OrdenCompra,
                    item.CUIT,
                    item.Proveedor,
                    item.Descripcion,
                    montoTotal,
                    item.Ingresante,
                    item.AnuladaPor,
                ];
            }

            if (item.Estado === 'Aprobada') {
                filaDatos = [
                    item.EntradaServicio,
                    item.FechaAprobacion,
                    item.FechaCreacion,
                    item.OrdenCompra,
                    item.CUIT,
                    item.Proveedor,
                    item.Descripcion,
                    montoTotal,
                    item.Ingresante,
                    item.Aprobador,
                ];
            }


            datosPorEstado[estado].push(filaDatos);
        });

        return datosPorEstado;
    }

    exportarDatosAExcelFile(datosPorEstado: { [key: string]: any[][] }) {
        const wb: XLSX.WorkBook = XLSX.utils.book_new();

        // Crear una hoja para cada estado
        Object.keys(datosPorEstado).forEach(estado => {
            const encabezados = this.obtenerEncabezados(estado);
            const datos = [encabezados, ...datosPorEstado[estado]];

            const ws: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(datos);

            // Aplicar estilo al encabezado (primera fila)
            const range = XLSX.utils.decode_range(ws['!ref']!);
            const headerColor = { rgb: "D3D3D3" };

            datos[0].forEach((_, colIndex) => {
                const cellAddress = XLSX.utils.encode_cell({ r: 0, c: colIndex });
                if (!ws[cellAddress]) ws[cellAddress] = {};
                ws[cellAddress].s = {
                    fill: {
                        patternType: "solid",
                        fgColor: headerColor
                    },
                    font: {
                        bold: true
                    },
                    alignment: {
                        horizontal: "center",
                        vertical: "center"
                    }
                };
            });

            const colWidths = datos[0].map((_, colIndex) =>
                Math.max(
                    ...datos.map(row => (row[colIndex] !== null && row[colIndex] !== undefined ? row[colIndex].toString().length : 0))
                )
            );

            ws["!cols"] = colWidths.map(width => ({ wch: width }));

            XLSX.utils.book_append_sheet(wb, ws, estado || 'Aprobadas');
        });

        const excelBuffer: any = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
        this.guardarComoExcel(excelBuffer, 'Estados de Certificaciones');
    }


    guardarComoExcel(buffer: any, nombreArchivo: string): void {
        const data: Blob = new Blob([buffer], { type: this.EXCEL_TYPE });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(data);
        link.download = nombreArchivo + this.EXCEL_EXTENSION;
        link.click();
    }

}
