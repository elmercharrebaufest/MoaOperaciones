import { Component, ViewChild, ElementRef, AfterViewInit, OnInit, ChangeDetectorRef } from '@angular/core';
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
import { Formatter } from '../../../common/formatter/Formatter';
import { MultiSelect } from 'primeng/multiselect';
import { FileModalComponent } from '../file-modal/file-modal.component';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';


@Component({
    selector: 'app-listado-dashboard-certificacion-de-servicios',
    templateUrl: './listado-dashboard-certificacion-de-servicios.component.html',
    styleUrls: ['./listado-dashboard-certificacion-de-servicios.component.css']
})

export class ListadoDashboardCertificacionDeServiciosComponent extends ListBaseComponent implements AfterViewInit, OnInit {

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router,
        private confirmationService: ConfirmationService,
        private location: Location,
        private cdr: ChangeDetectorRef,
        private formBuilder: FormBuilder
    ) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.usuario = sessionStorage.getItem("username");
        this.vendedor = sessionStorage.getItem("proveedor");
    }

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent: FiltroFechaComponent;

    protected locale: any;

    @ViewChild("filtroSolicitantes")
    protected multiSelectSolicitantes: MultiSelect;

    @ViewChild("tabla")
    protected tabla: Table;

    @ViewChild("tablaPosiciones")
    protected tablaPosiciones: Table;

    @ViewChild("tablaItems")
    protected tablaItems: Table;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild("myModal") modal: ModalAltaEntradaDeServicioComponent;

    @ViewChild('fileModal') fileModal: FileModalComponent;


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
    itemIdSelected: any[] = [];
    numeroLineaSelected: Set<string> = new Set();
    itemSelected: any[] = [];
    posicionSelected: any[] = [];
    elementSelected: any[] = [];
    selectedItemId: number | null = null;
    selectedPosicionId: number | null = null;
    ordenCompraIdsMostradas: Set<number> = new Set<number>();
    mensajeError: string = "";
    filaExpandida: any;
    expandedRow: any;
    expandedPositionRow: any;
    expandedItemRow: any;
    proveedorSeleccionado: any;
    proveedorModel: ProveedorModel;
    proveedorList: any[] = new Array();
    btnCertificarHabilitado: boolean = true;

    //#region Variables 
    tablaPO: any[] = [];
    selectedItems: any[][][][] = [];
    cols: any[];
    usuario: string;
    vendedor: string;
    allItems: any[];
    proveedor: string = "";
    showModal: boolean = false;
    showDialog: boolean = false;
    ocFilterApplied: boolean = false;
    recalculando: boolean = false;
    disabledFilter: boolean = false;
    solicitantes: any[] = [];
    selectedSolicitante: any;
    filterSolicitante: boolean = false;
    listadoGeneralSolicitantes: any[] = [];
    filtroSolicitantes: any[] = [];
    fullscreen: boolean = false;
    displayContent: boolean = false;
    isInputActive: boolean = false;
    procesandoCelda: { [key: string]: any[] } = {
        "FechaDocumento": [],
        "DescripcionES": [],
        "Remito": []
    };

    // COLUMNS CONFIG
    userTablesConfig: any[] = [];
    defaultTablesConfig = [
        {
            name: 'Ordenes',
            columns: [
                { id: 'oFecha', header: 'Fecha', field: 'FechaCreacion', type: 'date', sortable: true, required: false, visible: true },
                { id: 'oNro', header: 'Nro de OC', field: 'NumeroOrdenDeCompra', type: 'string', sortable: true, required: true, visible: true },
                { id: 'oProveedor', header: 'Proveedor', field: 'Proveedor', type: 'string', sortable: true, required: false, visible: true },
                { id: 'oRazonSoc', header: 'Razón Social', field: 'NombreProveedor', type: 'string', sortable: false, required: true, visible: true },
                { id: 'oCuit', header: 'CUIT', field: 'Cuit', type: 'string', sortable: false, required: false, visible: true },
                { id: 'oMoneda', header: 'Moneda', field: 'MonedaDescripcion', type: 'string', sortable: false, required: false, visible: true },
                { id: 'oMonto', header: 'Importe', field: 'MontoTotalString', type: 'string', sortable: false, required: false, visible: true }
            ]
        },
        {
            name: 'Posiciones',
            columns: [
                { id: 'pId', header: 'Posición', field: 'NumeroPosicion', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pDescripcion', header: 'Descripción', field: 'Descripcion', type: 'string', sortable: false, required: true, visible: true },
                { id: 'pCantidad', header: 'Cant.', field: 'Cantidad', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pUM', header: 'UM', field: 'UM', type: 'string', sortable: false, required: false, visible: false },
                { id: 'pPrecio', header: 'Prc. Neto', field: 'PrecioUnidadString', type: 'string', sortable: false, required: true, visible: true },
                { id: 'pMoneda', header: 'Moneda', field: 'MonedaDescripcion', type: 'string', sortable: false, required: true, visible: true },
                { id: 'pGrupo', header: 'Grupo Art.', field: 'GrupoArticulos', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pCentro', header: 'Centro', field: 'Centro', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pAlmacen', header: 'Almacén', field: 'Almacen', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pSolped', header: 'NRO_SOLPED', field: 'NumeroSolp', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pContrato', header: 'Contrato', field: 'Contrato', type: 'string', sortable: false, required: false, visible: true },
                { id: 'pSolicitante', header: 'Aprobador', field: 'Solicitante', type: 'string', sortable: false, required: true, visible: true }
            ],
        },
        {
            name: 'Items',
            columns: [
                { id: 'iLinea', header: 'N° Línea', field: 'NumeroLinea', type: 'string', sortable: false, required: false, visible: true },
                { id: 'iNroServicio', header: 'N° Servicio', field: 'ServicioNumero', type: 'string', sortable: false, required: false, visible: true },
                { id: 'iDescripcion', header: 'Txt. Breve', field: 'Descripcion', type: 'string', sortable: false, required: true, visible: true },
                { id: 'iCantidad', header: 'Cant.', field: 'Cantidad', type: 'string', sortable: false, required: true, visible: true },
                { id: 'iUM', header: 'UM', field: 'UM', type: 'string', sortable: false, required: true, visible: true },
                { id: 'iImporte', header: 'Precio Unitario', field: 'ImporteString', type: 'string', sortable: false, required: true, visible: true },
                { id: 'iMonto', header: 'Monto Total', field: null, type: 'custom', sortable: false, required: true, visible: true },
                { id: 'iCantidadReal', header: 'Cant. Anterior', field: 'CantidadReal', type: 'string', sortable: false, required: true, visible: true },
                { id: 'iPorcentaje', header: 'Porc. %', field: 'Porcentaje', type: 'custom', sortable: false, required: true, visible: true },
                // These fields values are calculated in the view. NA: Not applicable
                { id: 'iCantidadACertificar', header: 'Cant. Actual', field: null, type: 'custom', sortable: false, required: true, visible: true },
                { id: 'iPorcentajeACertificar', header: '% a Certificar', field: null, type: 'custom', sortable: false, required: true, visible: true },
                { id: 'iMontoACertificar', header: 'Monto a Certificar', field: null, type: 'custom', sortable: false, required: true, visible: true },
                { id: 'iAcciones', header: 'Acciones', field: null, sortable: false, type: 'custom', required: true, visible: true },

            ]
        },
        {
            name: 'Entradas',
            columns: [
                { id: 'esNro', header: 'NRO_ES', field: 'Id', type: 'custom', sortable: false, required: true, visible: true },
                { id: 'esFechaDoc', header: 'F. Documento (F. de prestación de servicios)', field: 'FechaDocumentoString', type: 'custom', sortable: false, required: true, visible: true },
                { id: 'esFechaContabilización', header: 'F. Contabilización', field: 'FechaContabilizacion', type: 'date', sortable: false, required: true, visible: true },
                { id: 'esReferencia', header: 'Referencia (N° remito)', field: 'Referencia', type: 'custom', sortable: false, required: true, visible: true },
                { id: 'esCantidad', header: 'Cant.', field: 'Cantidad', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esDescripcion', header: 'Desc. ES', field: 'TextoBreve', type: 'custom', sortable: false, required: true, visible: true },
                { id: 'esImporte', header: 'Importe', field: 'ImporteARPUSD', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esAcciones', header: 'Eliminar ES', field: null, type: 'custom', sortable: false, required: true, visible: true },
                { id: 'esAdjuntos', header: 'Adjuntos', field: null, type: 'custom', sortable: false, required: true, visible: true },

            ]
        }
    ];

    formularioResumenCertificacion: FormGroup = this.formBuilder.group({
        descriptions: this.formBuilder.array([])
    });

    periodo: string | undefined | null;

    ngOnInit() {
        this.obtenerConfiguracionDeTablasDelUsuario();

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
        this.navService.setSeccionActive('Ingresar certificación');
        this.navService.navegarSeccion("compras/dashboardCertificacionDeServicios");

        if (sessionStorage.getItem('periodo-certificaciones') != undefined) {
            this.periodo = sessionStorage.getItem('periodo-certificaciones');
        }else{
            this.periodo = '1';
        }

        this.filtroFechaComponent.setPeriodoInitial(this.periodo);
        this.saveConfigurationFilterDates();
        this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicioConfigurado, this.fechaFinConfigurado);
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

    onCheckboxPositionChange(item: any) {

        const itemId = item.PosicionId;
        const numeroLinea = item.NumeroLinea;

        this.itemIdSelected.push(itemId);
        this.numeroLineaSelected.add(numeroLinea);
        this.itemSelected.push(item);

        this.itemSelected.sort((a, b) => a.NumeroLinea > b.NumeroLinea ? 1 : -1);
    };

    seleccionarPosicion(event: any, posicion: any): void {
        if (event.target.checked) {
            const itemsFiltered = posicion.Items.filter(item => this.esItemValidoParaCertificar(item) && Number(item.PorcentajeACertificar) >= 0.001);
            itemsFiltered.forEach((item: any) => {
                if (!this.itemSelected.includes(item)) {
                    item.isSelected = true;
                    item.posicionDescripcion = posicion.Descripcion;
                    this.onCheckboxPositionChange(item);
                }
            });
        }
        else {
            this.clearCheckboxesPositions(posicion);
        }
    }

    clearCheckboxesPositions(positions: any): void {
        const updatedSelectedItems = [...this.itemSelected];
        const updatedSelectedIds = [...this.itemIdSelected];

        this.numeroLineaSelected.clear();

        this.tablaPO.forEach((order: any) => {
            order.Posiciones.forEach((pos: any) => {
                if (positions.Id === pos.Id) {
                    pos.Items.forEach((item: any) => {
                        if (this.itemSelected.some(selectedItem => selectedItem.Id === item.Id)) {
                            item.isSelected = false;
                            const index = updatedSelectedItems.findIndex(selectedItem => selectedItem.Id === item.Id);
                            if (index !== -1) {
                                updatedSelectedItems.splice(index, 1);
                                updatedSelectedIds.splice(index, 1);
                            }
                        }
                    });
                }
            });
        });

        this.itemSelected = updatedSelectedItems;
        this.itemIdSelected = updatedSelectedIds;
    }

    /**
     * Selecciona/Deselecciona el checkbox correspondiente 
     * a un ITEM.
     * @param item
     */
    onCheckboxChange(item: any) {
        const itemId = item.PosicionId;
        const numeroLinea = item.NumeroLinea;
        const posicion = this.obtenerPosicionPorNumero(item.NroOrdenCompra, Number(item.NroPosicion));
        item.posicionDescripcion = posicion.Descripcion;

        if (!item.isSelected) {
            this.itemIdSelected.splice(this.itemIdSelected.indexOf(itemId), 1); 
            this.numeroLineaSelected.delete(numeroLinea);
            this.itemSelected = this.itemSelected.filter((selectedItem: any) => selectedItem.PosicionId !== item.PosicionId || selectedItem.NumeroLinea !== item.NumeroLinea);
        } 
        else {
            this.itemIdSelected.push(itemId);
            if (!this.numeroLineaSelected.has(numeroLinea)) this.numeroLineaSelected.add(numeroLinea);
            item.NroSolP = posicion.NumeroSolp;
            if (!this.itemSelected.includes(item)) this.itemSelected.push(item);
        }

        posicion.isSelected = this.tieneItemsACertificarTodosValidos(posicion) && this.tieneTodosItemsValidosSeleccionados(posicion);
        this.itemSelected.sort((a, b) => a.NumeroLinea > b.NumeroLinea ? 1 : -1);
    }

    loadSolicitantesList(rowData: any) {
       
        if (this.solicitantes !== undefined && this.solicitantes.length > 0) {

            this.solicitantes.length = 0;

            let sol = {
                id: null,
                name: 'Todos',
            }
            this.solicitantes.push(sol);
        }
        else if (this.solicitantes !== undefined) {

            let sol = {
                id: null,
                name: 'Todos',
            }
            this.solicitantes.push(sol);
        }

        rowData.Posiciones.forEach((pos: any) => {

            let sol = {
                id: pos.Solicitante,
                name: pos.Solicitante
            };

            if (this.solicitantes.some(x => x.name.includes(pos.Solicitante))) {
                //skip duplicates
            }
            else {
                this.solicitantes.push(sol);
            }

        });
    }

    loading: boolean = false;


    toggleSolicitanteFilter(): void {
        this.filterSolicitante = !this.filterSolicitante;
    }

    clearCheckboxes(): void {
        this.numeroLineaSelected.clear();
        this.itemSelected = [];
        this.itemIdSelected = [];
        this.tablaPO.forEach((order: any) => {
            order.Posiciones.forEach((pos: any) => {
                pos.isSelected = false;
                pos.Items.forEach((item: any) => {
                    item.isSelected = false;
                });
            });
        });
    }

    //MMSN-574 - Agregar filtros
    onBuscar() {
        this.clearCheckboxes();

        this.disabledFilter = true;
        // MMSN-519: Colapsar fila expandida al activar un filtro.
        this.collapseExpandedRow();
        // MMSN-689: Desactivar filtro de saldo pendiente al activar búsqueda. 
        this.filtrarTablas(false, []);

        // MMS-804: Limpiar Filtro Solicitantes
        this.limpiarFiltroPorSolicitantes();

        if (this.proveedorSeleccionado !== undefined && this.proveedorSeleccionado !== '') {
            this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
        }
        else {
            this.proveedor = '';
        }

        this.expandedPositionRow = false;
        this.saveConfigurationFilterDates();
        this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicioConfigurado, this.fechaFinConfigurado);
        this.tabla.first = 0;
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

    posicionesCompletas: any[] = [];

    getListarPO(proveedor, ordenCompraId, fecha_inicio, fecha_fin) {
        this.getFecha();
        try {
            this.spinnerComponent.showIt();
            this.unsubscribe();
            // this.subscripcionPO = this.service.getByProveedor("2023-01-28", proveedor, "4123001336", this.columnaOrden , this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
            this.subscripcionPO = this.service.getByProveedor(fecha_inicio, fecha_fin, proveedor, ordenCompraId, this.columnaOrden, this.ordenAscendente, this.pageIndex, this.pageSize).subscribe(
                (result: any) => {

                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.tablaPO = result.data;
                        this.obtenerSolicitantes(result.data);
                        this.cargarArrayProcesosSpinners(this.tablaPO);
                        this.length = result.data.length > 0 ? result.data[0].ItemsTotales : result.data.length;
                        this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
                        this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;

                    }
                    if (this.expandedPositionRow) {
                        this.filtrarTablas();
                        //this.tablaPosiciones.toggleRow(this.expandedPositionRow);
                        this.calcularValoresACertificar(this.expandedPositionRow);
                    }

                    this.disabledFilter = false;

                    this.recalculando = false;

                    if (this.spinnerComponent != undefined && this.spinnerComponent.visible) 
                        this.spinnerComponent.hideIt();

                    this.displayContent = true;

                    // Se buscan las posiciones que están al 100%
                    this.posicionesCompletas = [].concat.apply([], this.tablaPO.map(oc => this.calcularPorcentaje(oc)));
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.spinnerComponent.hideIt();
                    this.displayContent = true;
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.spinnerComponent.hideIt()
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    esPosicionCompleta(posicion): boolean {
        let isComplete = this.posicionesCompletas.some(p => p.Id === posicion.Id);
        return isComplete;
    }

    isPendingRelease(oc): boolean {
        return oc.SubjToR != "";
    }

    isNotReceibeMoreMerchandise(posicion): boolean {
        return posicion.NoMoreGR === "X" || posicion.Bloqueada;
    }

    getFecha() {
        var fechaActual = new Date();
        //fechaActual.setDate(fechaActual.getDate() - 2);
        //TODO: cambiar cuando se agregue el filtro de fecha.
        //actualizar
        fechaActual.setMonth(fechaActual.getMonth() - 2);
        this.fechaInicio = fechaActual.toISOString().slice(0, 10);
    }

    deleteES(Id: any, TempId: any, AccountingDate: any) {
        this.confirmationService.confirm({
            message: 'Esta a punto de eliminar la entrada de servicio. <b>¿Desea confirmar?</b>',
            accept: () => { this.deleteById(Id, TempId, AccountingDate); },
            reject: () => { }
        });
    }

    deleteById(Id, TempId, AccountingDate) {
        this.mensajeComponent.setMsgsEmpty();
        this.service.deleteById(Id, TempId, AccountingDate).subscribe((result: any) => {
            if (result.logout == true) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.info != undefined) {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.data != undefined) {
                this.recalculando = true;
                this.disabledFilter = true;
                this.numeroLineaSelected.clear();
                this.itemSelected = [];
                this.itemIdSelected = [];

                //this.floatMsgService.setSuccessMsg("Se ha eliminado la entrada de servicio " + Id);asda
                if (TempId !== null && TempId !== '') {
                    this.floatMsgService.setSuccessMsg("Se ha eliminado la entrada de servicio " + TempId);
                }
                else {
                    this.floatMsgService.setSuccessMsg("Se ha eliminado la entrada de servicio " + Id);
                }
                this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicioConfigurado, this.fechaFinConfigurado);
                setTimeout(() => {
                    let closeBtn = document.getElementsByClassName("alert-success")[0].getElementsByClassName("close")[0] as HTMLElement;
                    closeBtn.click();
                }, 3000);
            }
        });
    }

    openModal() {
        this.searchElement();
        this.createResumenForm();
        this.showModal = this.itemIdSelected.length > 0;
    }
/**
 * construye el formulario de resumen de certificación
 */
    createResumenForm(): void {
        this.resetFormularioResumenCertificacion();
        const descriptionsArray = this.descriptions;
        this.itemSelected.forEach(item => {
            const descriptionForm = this.formBuilder.group({
            description: [item.posicionDescripcion, Validators.required]
            });
            descriptionsArray.push(descriptionForm);
        });
    }

    get descriptions() {
        return this.formularioResumenCertificacion.controls["descriptions"] as FormArray;
    }

    resetFormularioResumenCertificacion(): void {
        this.formularioResumenCertificacion = this.formBuilder.group({
            descriptions: this.formBuilder.array([])
        });
    }

    private fechaInicioConfigurado: any;
    private fechaFinConfigurado: any;

    saveConfigurationFilterDates() {
        if (this.filtroFechaComponent.periodo == '4') {
            this.fechaInicioConfigurado = Formatter.DateToSting(new Date(this.filtroFechaComponent.dtpInput1.nativeElement.value));
            this.fechaFinConfigurado = Formatter.DateToSting(new Date(this.filtroFechaComponent.dtpInput2.nativeElement.value));
        }
        else {
            this.fechaInicioConfigurado = this.filtroFechaComponent.fecha_inicio;
            this.fechaFinConfigurado = this.filtroFechaComponent.fecha_fin;
        }
    }

    actualizarGrilla(event: string) {
        // MMSN-677: Desactivar filtro de saldo pendiente al certificar ES. 
        this.filtrarElementosSinSaldoACertificar(false);
        this.itemIdSelected = [];
        this.itemSelected = [];
        this.numeroLineaSelected.clear();
        this.recalculando = true;
        this.disabledFilter = true;
        this.getListarPO(this.proveedor, this.ordenCompraId, this.fechaInicioConfigurado, this.fechaFinConfigurado);
    }

    /**
     * busca las posiciones que fueron seleccionadas
     * y las guarda en posicionesSelected y elementosSelected 
     */
    searchElement() {
        for (const ordenCompra of this.tablaPO) {

            for (const posicion of ordenCompra.Posiciones) {
                const itemEncontrado = posicion.Items.find(item => item.PosicionId === this.itemSelected[0].PosicionId);

                if (itemEncontrado) {
                    this.elementSelected = ordenCompra;
                    this.posicionSelected = posicion;
                    break;
                }
            }
        }
    }

    onCloseModal(): void {
        this.showModal = false;
    }

    onCloseDialog(): void {
        this.showDialog = false;
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
                        class autoCompleteObject {
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
                                provisional.push(obj);
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

    /**
    * Hace el cambio para habilitar 'Cantidad a certificar'
    * o 'Porcentaje a certificar.' Por requerimiento en
    * MMSN-634 sólo uno de los campos puede ser editable a 
    * la vez.
    * @param index Indice de item sobre el que se aplica la acción.
    */
    habilitarCampoDeValorACertificar(index: number) {
        this.isInputActive = !this.isInputActive;
        let cantidad = document.getElementsByName('cantidad')[index];
        let porcentaje = document.getElementsByName('porcentaje')[index];

        if (cantidad.hasAttribute('disabled')) {
            cantidad.removeAttribute('disabled');
            porcentaje.setAttribute('disabled', 'true');
        }
        else {
            cantidad.setAttribute('disabled', 'true');
            porcentaje.removeAttribute('disabled');
        }
    }


    habilitarTodosCampoDeValorACertificar(): void {
        if (this.expandedPositionRow) { // Verifica si hay una posición expandida
          const posicionExpandida = this.tablaPosiciones.value.find(pos => pos.Id === Number(this.expandedPositionRow)); 
          if (posicionExpandida) { 
            posicionExpandida.Items.forEach(item => {
              const rowIndex = this.tablaItems.value.indexOf(item);
              if (rowIndex !== -1 && (item.CantidadACertificar > 0 || item.PorcentajeACertificar > 0)) {
                this.habilitarCampoDeValorACertificar(rowIndex);
              }
            });
          }
        }
      }

      hayElementosParaCertificar(): boolean {
        if (this.expandedPositionRow) {
          const posicionExpandida = this.tablaPosiciones.value.find(pos => pos.Id === Number(this.expandedPositionRow));
          if (posicionExpandida) {
            return posicionExpandida.Items.some(item => item.CantidadACertificar > 0 || item.PorcentajeACertificar > 0);
          }
        } else {
          return this.tablaPO.some(ordenCompra =>
            ordenCompra.Posiciones.some(posicion =>
              posicion.Items.some(item => 
                (item.CantidadACertificar > 0 || item.PorcentajeACertificar > 0) && 
                (!this.ocFilterApplied || this.tienePorcentajeACertificar(item))
              )
            )
          );
        }
        return false; 
      }


    /**
    * Se llama desde cada posición cuando se expande para calcular los
    * valores a certificar de cada item.
    * @param posicion
    */
    calcularValoresACertificar(posicion: any) {
        this.tablaPO.filter(orders => orders.NumeroOrdenDeCompra === posicion.NroOrdenCompra).forEach(order => {
            order.Posiciones.filter(positions => positions.NumeroPosicion === posicion.NumeroPosicion).forEach(position => {
                position.Items.forEach(item => {
                    if (item.MontoACertificar == undefined) {
                        const monto = item.Importe;
                        const cantidad = item.Cantidad;
                        item.CantidadACertificar = cantidad - item.CantidadReal;
                        item.PorcentajeACertificar = (item.CantidadACertificar * 100) / cantidad;
                        item.MontoACertificar = (item.CantidadACertificar * monto) / cantidad;
                    }
                });
            });
        });
    }

  eliminarFormatoNumeroLocal(value: any): number {
    if (!value) value = 0;
    if (typeof value === 'string') {
      value = parseFloat(value.replace(',', ''));
    }
    return value;
  }

  private calcularMontoACertificar(cantidadACertificar: number, importe: number, cantidad: number): number {
    const montoActualizado = (cantidadACertificar * importe) / cantidad;
    return montoActualizado;
  }

  private calcularPorcentajeACertificar(cantidadACertificar: number, cantidad: number): number {
    const porcentajeACertificar = (cantidadACertificar * 100) / cantidad;
    return porcentajeACertificar;
  }

    actualizarValoresACertificarPorCantidad(item: any): void {
      let cantidadACertificar = item.CantidadACertificar;
      const cantidadDisponible = item.Cantidad - item.CantidadReal;

      if (cantidadACertificar > cantidadDisponible || (cantidadACertificar < 0 && cantidadACertificar != '')) {
        item.CantidadACertificar = cantidadDisponible;
        cantidadACertificar = cantidadDisponible;
      }
      const porcentajeACertificar = this.calcularPorcentajeACertificar(cantidadACertificar, item.Cantidad);
      item.PorcentajeACertificar = Number(porcentajeACertificar.toFixed(3));
      item.MontoACertificar = this.calcularMontoACertificar(cantidadACertificar, item.Importe, item.Cantidad);
    }

    Number = Number;
    timeout: any = null;
    actualizarValoresACertificarPorPorcentaje(item: any, event): void {
        clearTimeout(this.timeout);
        this.timeout = setTimeout(() => {
            if (event.keyCode != 13) {
                const porcentajeDisponible = (100 - item.Porcentaje);
                const cantidadDisponible = item.Cantidad - item.CantidadReal;
                const porcentajeACertificar = item.PorcentajeACertificar;

                if (porcentajeACertificar > porcentajeDisponible || (porcentajeACertificar < 0 && porcentajeACertificar != '')) {
                    item.PorcentajeACertificar = porcentajeDisponible;
                }

                const cantidadACertificar = (item.PorcentajeACertificar * item.Cantidad) / 100;
                item.CantidadACertificar = Number(cantidadACertificar.toFixed(3));
                item.MontoACertificar = this.calcularMontoACertificar(cantidadACertificar, item.Importe, item.Cantidad);

                if (porcentajeACertificar != '' && porcentajeACertificar > 0 &&
                    (item.CantidadACertificar.toFixed(3) === cantidadDisponible.toFixed(3) || Number(item.MontoACertificar.toFixed(2)) < 0.01)) {
                    item.PorcentajeACertificar = porcentajeDisponible;
                    item.CantidadACertificar = cantidadDisponible;
                    item.MontoACertificar = this.calcularMontoACertificar(cantidadACertificar, item.Importe, item.Cantidad);
                }
            }
        }, 500);
    }

    validarMantenerItemSeleccionado(item: any) {
        if (!this.esItemValidoParaCertificar(item) || Number(item.PorcentajeACertificar) < 0.001) {
            this.clearCheckbox(item);
        }
        const posicion = this.obtenerPosicionPorNumero(item.NroOrdenCompra, Number(item.NroPosicion));
        posicion.isSelected = this.tieneItemsACertificarTodosValidos(posicion) && this.tieneTodosItemsValidosSeleccionados(posicion);
    }

    numbersOnly(event): boolean {
        const charCode = (event.which) ? event.which : event.keyCode;
        // Verificar si el valor ingresado ya contiene .
        if (charCode === 46) {
            return event.target.value.indexOf('.') === -1;
        }

        // Verificar que sean un número
        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }

        // Verificar que no se acepten más de 2 decimales
        if (event.target.value.includes('.')) {
            return event.target.value.split('.')[1].length <= 2;
        }
    }

    clearCheckbox(item: any): void {
        this.numeroLineaSelected.clear();
        item.isSelected = false;
        this.itemIdSelected = this.itemIdSelected.filter(obj => { return obj !== item.Id });
        this.itemSelected = this.itemSelected.filter(obj => { return obj !== item });
    }

    isGet100(item: any): boolean {
        return item.Porcentaje === '100';
    }

    selectAllItems(event: any, posicion: any): void {
        if (event.target.checked) {
            const itemsFiltered = posicion.Items.filter((item: any) => this.esItemValidoParaCertificar(item) && Number(item.PorcentajeACertificar) >= 0.001);
            if (itemsFiltered.length > 0) {
                itemsFiltered.forEach((item: any) => {
                    if (!this.itemSelected.includes(item)) {
                        item.isSelected = true;
                        this.onCheckboxChange(item);
                    }
                });
            }
        } else {
            this.clearCheckboxes();
        }
    }

    ///**
    // * Selecciona/Deselecciona toda la posición.
    // * @param item
    // */
    //actionCheckPosition(item: any): void {
    //    let posicion = this.tablaPosiciones.value.find(posicion => posicion.NumeroPosicion === parseInt(item.NroPosicion));
    //    let itemsConSaldoDisponible = posicion.Items.filter(item => Number(item.Porcentaje) < 100);
    //    //posicion.isSelected = !itemsConSaldoDisponible.some(item => !item.isSelected);
    //}


    obtenerOrdenDeCompraPorNumero(nroOrdenDeCompra: number): any {
        return this.tablaPO.find(orden => orden.NumeroOrdenDeCompra === nroOrdenDeCompra, []);
    }

    obtenerPosicionPorNumero(nroOrdenDeCompra: number, nroPosicion: number): any {
        const oc = this.obtenerOrdenDeCompraPorNumero(nroOrdenDeCompra);
        return oc.Posiciones.find(posicion => posicion.NumeroPosicion === nroPosicion);
    }

    ordenEsPendienteDeLiberacion(nroOrdenDeCompra: number): boolean {
        const oc = this.obtenerOrdenDeCompraPorNumero(nroOrdenDeCompra);
        return oc.SubjToR === 'X';
    }

    posicionEsConEntregaFinal(nroOrdenDeCompra: number, nroPosicion: number): boolean {
        const posicion = this.obtenerPosicionPorNumero(nroOrdenDeCompra, nroPosicion);
        return posicion.NoMoreGR === 'X';
    }

    /**
     * Evalúa si mostar o no el checkbox para seleccionar la posición.
     * @param posicion 
     * @returns {boolean}
     */
    mostrarCheckboxDeSeleccionarPosicion(posicion: any): boolean {
        let orderPendienteDeLiberacion = this.ordenEsPendienteDeLiberacion(posicion.NroOrdenCompra);
        let posicionConEntregaFinal = this.posicionEsConEntregaFinal(posicion.NroOrdenCompra, Number(posicion.NumeroPosicion));
        let posicionTieneSaldoACertificar = this.tieneItemsACertificar(posicion);
        let mostrarCheckboxDeSeleccionarPosicion = !orderPendienteDeLiberacion && !posicionConEntregaFinal && posicionTieneSaldoACertificar;
        return mostrarCheckboxDeSeleccionarPosicion;
    }

    /**
     * Evalúa si mostrar o no el checkbox para seleccionar todos los items.
     * @param posicion
     * @returns {boolean}
     */
    mostrarCheckboxDeSeleccionarTodosItems(posicion: any): boolean {
        let orderPendienteDeLiberacion = this.ordenEsPendienteDeLiberacion(posicion.NroOrdenCompra);
        let posicionConEntregaFinal = this.posicionEsConEntregaFinal(posicion.NroOrdenCompra, Number(posicion.NumeroPosicion));
        let posicionTieneItemsACertificar = this.tieneItemsACertificar(posicion);
        const mostrarCheckboxDeSeleccionarTodosItems = !orderPendienteDeLiberacion && !posicionConEntregaFinal && posicionTieneItemsACertificar && !posicion.Bloqueada;
        return mostrarCheckboxDeSeleccionarTodosItems;
    }

    /**
     * Evalúa si el checkbox para seleccionar un item debe o no estar habilitado.
     * @param item
     * @returns {boolean}
     */
    deshabilitarCheckboxDeItem(item: any): boolean {
        let ordenPendienteDeLiberacion = this.ordenEsPendienteDeLiberacion(item.NroOrdenCompra);
        let posicionConEntregaFinal = this.posicionEsConEntregaFinal(item.NroOrdenCompra, Number(item.NroPosicion));
        let itemTienePorcentajeACertificar = this.tienePorcentajeACertificar(item);
        let itemTieneMontoACertificar = this.tieneMontoVálidoACertificar(item);
        let deshabilitarCheckboxDeItem = ordenPendienteDeLiberacion || !itemTienePorcentajeACertificar || !itemTieneMontoACertificar || posicionConEntregaFinal || Number(item.PorcentajeACertificar) < 0.001;
        return deshabilitarCheckboxDeItem;
    }

    /**
     * Activa el filtro de posiciones e items sin saldo 
     * a certificar. Este filtro oculta/muestra items certificados
     * al 100%. Si la posición tiene todos sus items certificados
     * entonces oculta también la posición.
     * @param activate Boolean que activa/desactiva filtro
     */
    filtrarElementosSinSaldoACertificar(activate: boolean): void {
        this.filtrarTablas(activate, this.filtroSolicitantes);
    }

    private filtrarTablas(filtroSaldo?: boolean, filtroSolicitante?: string[]): void {
        if (filtroSaldo != undefined) this.ocFilterApplied = filtroSaldo;
        if (filtroSolicitante != undefined) this.filtroSolicitantes = filtroSolicitante;

        setTimeout(() => {
            let ordenesFiltradas = this.tablaPO;

            if (this.ocFilterApplied) {
                ordenesFiltradas = ordenesFiltradas.reduce((acc, orden) => (orden.Posiciones.some(posicion => this.tieneItemsACertificar(posicion)) ? [...acc, orden] : acc), []);
            }

            if (this.filtroSolicitantes.length > 0) {
                ordenesFiltradas = ordenesFiltradas.reduce((acc, orden) => (this.filtrarPosiciones(orden.Posiciones).length > 0 ? [...acc, orden] : acc), []);
            }

            // Se asigna a tabla.value en lugar de usar tabla.filter porque .filter si no tiene 
            // resultados no muestra el template "emptyMessage".
            this.tabla.value = ordenesFiltradas;

            this.tabla.first = 0;
            setTimeout(() => this.filtrarTablaPosiciones(), 60);
        }, 20); // Esta espera es necesaria para que parezca el elemento en el DOM
    }

    private filtrarPosiciones(posiciones: any[]): any[] {
        let posicionesFiltradas = (posiciones && posiciones.length > 0) ? posiciones : [];
        if (this.ocFilterApplied) {
            posicionesFiltradas = posicionesFiltradas.filter(posicion => this.tieneItemsACertificar(posicion));
        }
        if (this.filtroSolicitantes.length > 0) {
            posicionesFiltradas = posicionesFiltradas.filter(posicion => this.filtroSolicitantes.includes(posicion.Solicitante.toUpperCase()));
        }
        return posicionesFiltradas;
    }

    private filtrarTablaPosiciones(): void {
        if (this.tablaPosiciones) {
            let expandedRowId = Object.keys(this.tabla.expandedRowKeys)[0];

            let posiciones = this.tablaPO.find(orden => orden.NumeroOrdenDeCompra == expandedRowId).Posiciones;
            let posicionesFiltradas = this.filtrarPosiciones(posiciones);

            // Se asigna a tabla.value en lugar de usar tabla.filter porque .filter si no tiene 
            // resultados no muestra el template "emptyMessage".
            this.tablaPosiciones.value = posicionesFiltradas;
            this.tablaPosiciones.first = 0;

            if (this.expandedPositionRow) {
                let expandedRow = posicionesFiltradas.find(posicion => posicion.Id === Number(this.expandedPositionRow));
                if (expandedRow && !this.tablaPosiciones.isRowExpanded(expandedRow)) {
                    this.tablaPosiciones.toggleRow(expandedRow);
                }
                setTimeout(() => this.expandirItem(), 20);
            }
        }
    }

    private expandirItem(): void {
        if (this.tablaItems) {
            let items = this.tablaItems.value;
            if (this.expandedItemRow) {
                let expandedRow = items.find(item => item.NumeroLinea === Number(this.expandedItemRow));
                if (expandedRow && !this.tablaItems.isRowExpanded(expandedRow)) {
                    this.tablaItems.toggleRow(expandedRow);
                }
            }
        }
    }

    /**
     * Used from template
     */
    private setExpanded(table: Table): void {
        let expandedRowId = Object.keys(table.expandedRowKeys)[0] ? Object.keys(table.expandedRowKeys)[0] : undefined;
        if (table.el.nativeElement.id == 'posiciones') this.expandedPositionRow = expandedRowId;
        if (table.el.nativeElement.id == 'items') this.expandedItemRow = expandedRowId;
    }

    tienePorcentajeACertificar(item: any): boolean {
        return Number(item.Porcentaje) < 100;
    }

    tieneMontoVálidoACertificar(item: any): boolean {
        return item.MontoACertificar > 0;
    }

    esItemValidoParaCertificar(item: any): boolean {
        return this.tienePorcentajeACertificar(item) && this.tieneMontoVálidoACertificar(item);
    }

    tieneItemsACertificarTodosValidos(posicion: any): boolean {
        return posicion.Items.filter(item => this.tienePorcentajeACertificar(item)).every(item => this.esItemValidoParaCertificar(item)) && !posicion.Bloqueada;
    }

    tieneTodosItemsValidosSeleccionados(posicion: any): boolean {
        return posicion.Items.filter(item => this.esItemValidoParaCertificar(item)).every(item => item.isSelected);
    }

    /**
     * Valida que la posición tenga al menos un item con
     * porcentaje disponible a certificar.
     * @param posicion
     */
    tieneItemsACertificar(posicion: any) {
        let elementosPorCertificar = posicion.Items.some(item => this.tienePorcentajeACertificar(item));
        return elementosPorCertificar;
    }

    /**
     * Valida que la orden tenga al menos una posición
     * con items con porcentaje disponible a certificar.
     * @param posiciones
     */
    tienePosicionesConItemsACertificar(posiciones: any[]) {
        const tieneItemsACertificar = (posicion) => this.tieneItemsACertificar(posicion);
        let tienePosicionesConItemsACertificar = posiciones.some(tieneItemsACertificar);
        return tienePosicionesConItemsACertificar;
    }

    /**
     * Obtiene configuración de tablas del usuario
     * del localStorage.
     */
    obtenerConfiguracionDeTablasDelUsuario() {
        let colConfig = localStorage.getItem('columnasCertificaciones');
        this.userTablesConfig = [...this.defaultTablesConfig];

        if (colConfig) {
            let visibleCols = colConfig.split(',');

            this.userTablesConfig.forEach(t => {
                t.columns.forEach(c => c.visible = colConfig.includes(c.id));
            });
        }
        else {
            this.guardarConfiguracionDeTablasDeUsuario();
            this.obtenerConfiguracionDeTablasDelUsuario();
        }
    }

    /**
     * Guarda en el localStorage la configuración
     * de tablas del usuario.
     */
    guardarConfiguracionDeTablasDeUsuario() {
        let visibleColumns = this.userTablesConfig.reduce((acc, t) => acc.concat(t.columns.filter(c => c.visible).map(a => a.id)), []);
        localStorage.setItem('columnasCertificaciones', JSON.stringify(visibleColumns));
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
     * Calcula la cantidad de columnas de una tabla
     * para actualizar el colspan de una tabla anidada.
     * @param nombreTabla
     * @returns número de columnas de una tabla.
     */
    cantidadColumnasTabla(nombreTabla: string): number {
        let count = this.defaultTablesConfig.find(table => table.name.toLowerCase() === nombreTabla.toLowerCase()).columns.length + 1;
        return count;
    }
    //ex update
    /**
     * Actualiza la configuración de tablas 
     * cuando el usuario selecciona/deselecciona
     * una columna para mostrar/ocultar.
     * @param tableName
     * @param columnId
     */
    actualizarConfiguarcionDeTablas(tableName: string, columnId: string) {
        let table = this.userTablesConfig.find(t => t.name == tableName);
        if (table != undefined) {
            let column = table.columns.find(c => c.id === columnId);

            if (column != undefined) {
                column.visible = !column.visible;
            }
        }
        this.guardarConfiguracionDeTablasDeUsuario();
    }

    /**
     * Verifica si el array tiene al menos un elemento
     * columna no requerido. Es decir al menos una columna
     * que se pueda ocultar.
     */
    tieneColumnasOpcionales(columnList: any[]): boolean {
        return columnList.some(col => !col.required);
    }

    obtenerSolicitantes(ocs: any): void {
        let solicitantesUnicos = new Set<string>();
        ocs.forEach((oc: any) => {
            oc.Posiciones.forEach((pos: any) => {
                if (pos.Solicitante.length === 0) {
                    pos.Solicitante = '(Vacío)';
                }
                solicitantesUnicos.add(pos.Solicitante.toUpperCase());

            });
        });
        this.listadoGeneralSolicitantes = Array.from(solicitantesUnicos).map(solicitante => ({ label: solicitante, value: solicitante }));
    }

    filtrarPorSolicitantes(event: any): void {
        this.filtrarTablas(this.ocFilterApplied, event.value);
    }

    mostrarPosicionSolicitante(posicion: any): any {
        if (this.filtroSolicitantes.length > 0) {
            return this.filtroSolicitantes.includes(posicion.Solicitante.toUpperCase());
        } else {
            return true;
        }
    }

    limpiarFiltroPorSolicitantes() {
        this.multiSelectSolicitantes.valuesAsString = 'Solicitantes';
        this.multiSelectSolicitantes.value = [];
    }

    calcularPorcentaje(oc): any[] {
        const posicionesCompletas = oc.Posiciones.filter(posicion => {
            this.calcularValoresACertificar(posicion);

            const items = posicion.Items || [];
            const totalItems = items.length;
            const itemsCompletados = items.filter(item => item.Porcentaje === "100").length;

            return itemsCompletados === totalItems;

        });

        return posicionesCompletas.map(posicion => ({
            Id: posicion.Id,
            Posicion: posicion
        }));
    }

    showScrollbar: boolean = false;

    toggleFullscreen() {
        this.fullscreen = !this.fullscreen;
        if (this.fullscreen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'auto';
        }
    }

    isARP = false;

    formatCurrency(columnField: string, rowData: any): string {
        let formattedValue = rowData[columnField];

        if (columnField === "MontoTotalString" || columnField === "PrecioUnidadString") {
                formattedValue = rowData.MonedaDescripcion === 'ARP' ?  '$ '+formattedValue : rowData.MonedaDescripcion+ ' ' +formattedValue;
            }

        return formattedValue;
    }


    calculateAmount(colId: string, rowData: any): string {
        if (colId === 'iMonto') {
            const monto = rowData.Cantidad * rowData.Importe;
            return this.formatAmount(monto, rowData.Moneda);
        } else if (colId === 'iMontoACertificar') {
            const montoACertificar = rowData.CantidadACertificar * rowData.Importe;
            if (isNaN(montoACertificar)) return '';
            return this.formatAmount(montoACertificar, rowData.Moneda);
        }
        return '';
    }

    formatAmount(monto: number, moneda: string): string {
        if (moneda === "ARP") {
            this.isARP = true;
            return `$ ${monto.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        } else {
            this.isARP = false;
            return `${moneda} ${monto.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        }
    }

    formatImport(columna: string, valor: any, rowData: any): string {

        if (columna === 'iImporte' && rowData.MonedaDescripcion === 'ARP') {
            valor = "$ " + valor;
        }
        else if(columna === 'iImporte' && rowData.MonedaDescripcion !== 'ARP'){
            valor =  rowData.MonedaDescripcion +" " + valor;
        }

        return valor;
    }

    formatESImport(columna: string, rowData: any): string {

        let parts = rowData.ImporteARPUSD.split(' ');

        let currency: string;
        let amount: string;

        if (isNaN(parseFloat(parts[0]))) {
            currency = parts[0];
            amount = parts[1];
        } else {
            amount = parts[0];
            currency = parts[1];
        }

        if (amount.includes(',') || amount.includes('.')) {
            if (currency === 'ARP') {
                return '$ ' + amount;
            } else {
                return currency + ' ' + amount;
            }
        } else {
            let valorNumerico = parseFloat(amount);
            let formattedAmount = valorNumerico.toLocaleString('en-US', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            });
    
            if (currency === 'ARP') {
                return '$ ' + formattedAmount;
            } else {
                return currency + ' ' + formattedAmount;
            }
        }
    }

    /**
     * Metodo para cargar un array de booleanos que corresponden a las celdas editables como usuario ingresante.
     * @param pendienteAprobacion 
     * @returns 
     */
    cargarArrayProcesosSpinners(pendienteAprobacion: any[]): void {
        const tieneTemporalId = pendienteAprobacion.some(item =>
            item.Posiciones.some(posicion =>
                posicion.Items.some(item =>
                    item.EntradasServicio.some(entrada =>
                        entrada.TemporalId !== null
                    )
                )
            )
        );
        if (!tieneTemporalId) {
            return;
        }
        pendienteAprobacion.forEach(item => {
            item.Posiciones.forEach(posicion => {
                posicion.Items.forEach(item => {
                    item.EntradasServicio.forEach(entrada => {
                        if (entrada.TemporalId !== null) {
                            this.procesandoCelda["FechaDocumento"][entrada.IdES] = false;
                            this.procesandoCelda["DescripcionES"][entrada.IdES] = false;
                            this.procesandoCelda["Remito"][entrada.IdES] = false;
                        }
                    });
                });
            });
        });
    }    

    /**
     * Metodo que envia la información de las ES editables como ingresante de la misma
     * @param valor 
     * @param columnaEditar 
     * @param id 
     * @param nroOc 
     */
    enviarInformacionIngresante(valor: any, columnaEditar: string, id: number, nroOc: string): void {
        this.procesandoCelda[columnaEditar][id] = true;
        const data = {
          ID: id,
          ColumnaEditar: columnaEditar,
          NuevoValor: valor
        }
        this.service.enviarEdicionIngresante(data).subscribe( 
        resp => {
            this.setearNuevoValorDeCelda(valor, columnaEditar, id, nroOc);
        }, error => {
            console.error(error)
        }
        )
    }

    /**
     * Metodo que setea el dato editado luego de ser enviado al servicio para actualizarlo.
     * Esto actualiza el valor de la celda sin necesidad de cargar la tabla nuevamente.
     * @param valor 
     * @param columnaEditar 
     * @param id 
     * @param nroOc 
     */
    setearNuevoValorDeCelda(valor: any, columnaEditar: string, id: number, nroOc: string): void{
        const ordenCompraIndex = this.tablaPO.findIndex(oc => oc.NumeroOrdenDeCompra === nroOc);
            if (ordenCompraIndex !== -1) {
                const ordenCompra = this.tablaPO[ordenCompraIndex];
                const itemIndex = ordenCompra.Posiciones
                    .flatMap(posicion => posicion.Items)
                    .findIndex(item => item.EntradasServicio.some(es => es.IdES === id));

                if (itemIndex !== -1) {
                    const item = ordenCompra.Posiciones
                        .flatMap(posicion => posicion.Items)[itemIndex];
                    const entradaServicioIndex = item.EntradasServicio.findIndex(es => es.IdES === id);

                    if (entradaServicioIndex !== -1) {
                        const entradaServicio = item.EntradasServicio[entradaServicioIndex];

                        switch (columnaEditar) {
                            case 'DescripcionES':
                                this.tablaPO[ordenCompraIndex].Posiciones
                                    .flatMap(posicion => posicion.Items)[itemIndex].EntradasServicio[entradaServicioIndex].TextoBreve = valor;
                                break;
                            case 'FechaDocumento':
                                const fecha = valor.split('-');
                                valor = fecha[2] + '/' + fecha[1] + '/' + fecha[0];
                                this.tablaPO[ordenCompraIndex].Posiciones
                                    .flatMap(posicion => posicion.Items)[itemIndex].EntradasServicio[entradaServicioIndex].FechaDocumentoString = valor;
                                break;
                            case 'Remito':
                                this.tablaPO[ordenCompraIndex].Posiciones
                                    .flatMap(posicion => posicion.Items)[itemIndex].EntradasServicio[entradaServicioIndex].Referencia = valor;
                                break;
                            default:
                                // Caso no manejado
                                break;
                        }
                        this.procesandoCelda[columnaEditar][id] = false;
                    }
                }
            }
    }

    totalesBloqueadasEliminadas(oc: any): void {
        const allBlockOrDeleted = oc.Posiciones.every(
            pos => pos.Bloqueada
        )

        return allBlockOrDeleted;
    }

    fileTypes: { [key: string]: string } = {
        ".pdf": 'application/pdf',
        ".csv": "text/csv",
        ".msg": "application/vnd.ms-outlook"
    };
    
    loadingRows = new Map<number, boolean>();

    descargarArchivos(rowData: any, index: number) {

        let id = rowData.Id === 0 || rowData.Id == undefined || rowData.Id == null ? rowData.TemporalId : rowData.Id;
        
        this.loadingRows[index] = true;
        this.cdr.detectChanges();
        
        this.service.GetAdjuntosByES(id).subscribe(result => {
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

}
