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
import { EntradaServicioCabeceraDto } from '../../../common/models/ordenes-compra/entradaServicioCabecera';

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
    totalRows: number = 0;
    currentPage: number = 1;
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
    //tablaPOSap: EntradaServicioCabeceraDto[] = [];
    //tablaPOAprobaciones: EntradaServicioCabeceraDto[] = [];
    tablaPO: EntradaServicioCabeceraDto[] = [];
    fechaSeleccionadaAux: string = "1";
    recalculando: boolean = false;
    recalculandoAprobadas: boolean = false;
    filtroFechaDesde: string;
    filtroFechaHasta: string;
    ordenCompraFiltro: string = "";

    first: number = 0;
    lastEvent: any = null;

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
        { name: 'Estado: Rechazadas', code: 'Rechazado' },
        { name: 'Estado: Anuladas', code: 'Anulada' }
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
                { id: 'cAnulador', header: 'Anulado Por', field: 'AnuladoPor', type: 'string', sortable: true, required: false, visible: false },
                { id: 'cMotivoRechazo', header: 'Motivo de rechazo', field: 'MotivoRechazo', type: 'string', sortable: false, required: false, visible: false },
                { id: 'esAdjuntos', header: 'Adjuntos', field: null, type: 'custom', sortable: false, required: true, visible: true },

            ]
        },
        {
            name: 'ESDetalle',
            columns: [
                { id: 'DPosicion', header: 'N° Posición', field: 'Posicion', type: 'string', sortable: false, required: false, visible: true },
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
        this.estadoCertificacion = this.listadoEstadoCertificacion.find(
        x => x.code === 'Pendiente Aprobación') || this.estadoCertificacion;
        this.getFecha('1');
        this.innerWidth = window.innerWidth;
        this.navService.setSeccionActive("Estado certificaciones");
        this.navService.navegarSeccion("compras/listadoEstadoCertificacionesProveedor");
        this.recalculando = true;
        this.recalculandoAprobadas = true;
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

    listarEntradasServicio(fechaDesde: string, fechaHasta: string, proveedor: string, documentoNumero: string): Promise<void> {
        this.recalculando = true;
        this.tablaPO = [];
        return new Promise<void>((resolve, reject) => {
            this.unsubscribe();
            this.subscripcionPO = this.service.ListarEntradaServicio(false, fechaDesde, fechaHasta, this.ordenCompraFiltro,
                proveedor, documentoNumero, this.columnaOrden, this.ordenAscendente, this.pageIndex, this.pageSize, this.estadoCertificacion.code).subscribe(
                    (result: any) => {
                        if (result.logout === true) {
                            this.sessionDataService.logout();
                            reject('Logout required');
                        } else {
                            this.tablaPO = result.data;
                            const valor = result.data.length > 0 ? result.data[0].ItemsTotales : 0;
                            this.totalRows = valor;

                            if (this.estadoCertificacion.code !== 'Aprobada') {
                                this.agregarTipoMonedaEnDetalle(result.data);
                            }

                            this.filtrarColumnasPorEstado(this.estadoCertificacion.code);
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

    toggleRow(rowData: any): void {
        this.selectedRow = this.selectedRow === rowData ? null : rowData;
    }

    isSelectedRow(rowData: any): boolean {
        return this.selectedRow === rowData;
    }

    filtrarColumnasPorEstado(estado: string): void {

        // Determinar qué columnas deben mostrarse u ocultarse según el estado
        const columnasVisibles = this.obtenerColumnasVisiblesSegunEstado(estado);

        // Aplicar los cambios de visibilidad a las columnas
        this.aplicarVisibilidadColumnas(columnasVisibles);
    }

    // Método para determinar qué columnas deben mostrarse u ocultarse según el estado
    private obtenerColumnasVisiblesSegunEstado(estado: string): string[] {
        switch (estado) {
            case 'Aprobada':
                return ['cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cEstado', 'cAprobador', 'cFechaAprobacion', 'esAdjuntos'];
            case 'Pendiente Aprobación':
                return ['DImporteTotal', 'DFechaPres', 'cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cEstado', 'cAprobador', 'esAdjuntos'];
            case 'Rechazado':
                return ['cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cAprobador', 'cEstado', 'cMotivoRechazo', 'cFechaRechazo', 'esAdjuntos'];
            case 'Anulada':
                return ['cFecha', 'cID_ES', 'cDescripción', 'cMontoTotal', 'cOrdenCompra', 'cUsuario', 'cEstado', 'cAprobador', 'cAnulador', 'esAdjuntos']
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

    // showOrHideAuxPanel(): boolean {
    //     const estadoCertificacion = { name: 'Estado: Aprobadas', code: 'Aprobada' };
    //     if (this.estadoCertificacion.code === estadoCertificacion.code) {
    //         return true;
    //     }
    //     return false;
    // }

    onFiltroFechaDesdeChanged(fechaDesde: string) {
        this.filtroFechaDesde = fechaDesde;
    }

    onFiltroFechaHastaChanged(fechaHasta: string) {
        this.filtroFechaHasta = fechaHasta;
    }

    getOrdenCompraFiltro(ocIngresada: string) {
        this.ordenCompraFiltro = ocIngresada;
        //this.getListarPO();
    }

    isMoaIntern(usuario) {
        let userIntern = '@molinos';
        return usuario.includes(userIntern) ? 'Ingresado por MOA' : usuario;
    }

    async onBuscar() {
           this.first = 0;
        this.currentPage = 1;
        this.lastEvent = null;
        this.loadData({
            first: 0,
            rows: this.pageSize
        });
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

        this.service.GetAdjuntosByES(rowData.NumeroCertificacion).subscribe(result => {
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
        this.first = 0;
        this.loadData({
            first: 0,
            rows: this.pageSize
        });
    }

    loadData(event: any) {
        const mismoEvento =
            this.lastEvent &&
            this.lastEvent.first === event.first &&
            this.lastEvent.rows === event.rows &&
            this.lastEvent.sortField === event.sortField &&
            this.lastEvent.sortOrder === event.sortOrder;

        if (mismoEvento) {
            return;
        }

        this.lastEvent = { ...event };

        this.first = event.first;

        this.columnaOrden = event.sortField;
        this.ordenAscendente = event.sortOrder === 1;

        const page = event.first / event.rows;
        const size = event.rows;

        this.currentPage = page + 1;
        this.pageSize = size;

        this.listarEntradasServicio(
            this.filtroFechaDesde,
            this.filtroFechaHasta,
            this.proveedor,
            this.documentoNumero
        );
    }
}