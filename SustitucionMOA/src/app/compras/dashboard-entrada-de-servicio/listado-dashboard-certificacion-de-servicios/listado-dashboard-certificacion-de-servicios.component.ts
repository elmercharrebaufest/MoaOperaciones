import { Component, ViewChild, ElementRef, AfterViewInit, OnInit } from '@angular/core';
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
        private location: Location) {
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
    itemIdSelected: any[] = [];
    numeroLineaSelected: Set<string> = new Set();
    itemSelected: any[] = [];
    elementSelected: any[] = [];
    selectedItemId: number | null = null;
    selectedPosicionId: number | null = null;
    ordenCompraIdsMostradas: Set<number> = new Set<number>();
    mensajeError: string = "";
    filaExpandida: any;
    expandedRow: any;
    expandedPositionRow: any;
    proveedorSeleccionado: any;
    proveedorModel: ProveedorModel;
    proveedorList: any[] = new Array();
    btnCertificarHabilitado: boolean = true;

    //#region Variables 
    tablaPO: any[];
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
                { id: 'pSolicitante', header: 'Solicitante', field: 'Solicitante', type: 'string', sortable: false, required: true, visible: true }
            ],
        },
        {
            name: 'Items',
            columns: [
                { id: 'iLinea', header: 'N° Línea', field: 'NumeroLinea', type: 'string', sortable: false, required: false, visible: true },
                { id: 'iNroServicio', header: 'N° Servicio', field: 'NumeroServicio', type: 'string', sortable: false, required: false, visible: true },
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
                { id: 'esNro', header: 'NRO_ES', field: 'Id', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esFechaDoc', header: 'F. Documento (F. de prestación de servicios)', field: 'FechaDocumentoString', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esFechaContabilización', header: 'F. Contabilización', field: 'FechaContabilizacion', type: 'date', sortable: false, required: true, visible: true },
                { id: 'esReferencia', header: 'Referencia (N° remito)', field: 'Referencia', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esCantidad', header: 'Cant.', field: 'Cantidad', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esDescripcion', header: 'Desc. ES', field: 'TextoBreve', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esImporte', header: 'Importe ARP/USD', field: 'ImporteARPUSD', type: 'string', sortable: false, required: true, visible: true },
                { id: 'esAcciones', header: 'Eliminar ES', field: null, type: 'custom', sortable: false, required: true, visible: true }
            ]
        }
    ];

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
        this.filtroFechaComponent.setPeriodoInitial('2');
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
        this.actionCheckPosition(item);
        this.itemSelected.sort((a, b) => a.NumeroLinea > b.NumeroLinea ? 1 : -1);
    };

    selectAllPositionsLines(event: any, positions: any): void {
        if (event.target.checked) {
            const itemsFiltered = positions.Items.filter((row: any) => !this.isGet100(row) && row.MontoACertificar != 0);

            itemsFiltered.forEach((item: any) => {
                if (!this.itemSelected.includes(item)) {
                    item.isSelected = true;
                    this.onCheckboxPositionChange(item);
                }
            });
            
        }
        else {
            this.clearCheckboxesPositions(positions);
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

        if (this.itemIdSelected.includes(itemId) && this.numeroLineaSelected.has(numeroLinea)) {

            this.itemIdSelected.splice(this.itemIdSelected.indexOf(itemId), 1);
            this.numeroLineaSelected.delete(numeroLinea);

            this.itemSelected = this.itemSelected.filter((selectedItem: any) =>
                selectedItem.PosicionId !== item.PosicionId || selectedItem.NumeroLinea !== item.NumeroLinea);
        }
        else {
            this.itemIdSelected.push(itemId);
            this.numeroLineaSelected.add(numeroLinea);
            this.itemSelected.push(item);
        }

        this.actionCheckPosition(item);
        this.itemSelected.sort((a, b) => a.NumeroLinea > b.NumeroLinea ? 1 : -1);
    }

    setPositionRow(posiciones: any[]) {
        posiciones.forEach(posicion => this.calcularValoresACertificar(posicion));
        this.mostrarOcultarItemsSinSaldoACertificar();
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
        this.filtrarElementosSinSaldoACertificar(false);

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
                        this.length = result.data.length > 0 ? result.data[0].ItemsTotales : result.data.length;
                        this.pageSize = result.data.length > 0 ? result.data[0].ItemPorPagina : 10;
                        this.pageIndex = result.data.length > 0 ? result.data[0].Pagina : 1;
                    }
                    if (this.expandedPositionRow) {
                        this.calcularValoresACertificar(this.expandedPositionRow);
                        this.mostrarOcultarItemsSinSaldoACertificar();
                        setTimeout(() => {
                            this.recalculando = false;
                            this.tablaPosiciones.toggleRow(this.expandedPositionRow);
                        }, 500)
                    }
                    this.disabledFilter = false;
                    if (this.spinnerComponent) this.spinnerComponent.hideIt();
                    this.displayContent = true;
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

    getFecha() {
        var fechaActual = new Date();
        //fechaActual.setDate(fechaActual.getDate() - 2);
        //TODO: cambiar cuando se agregue el filtro de fecha.
        //actualizar
        fechaActual.setMonth(fechaActual.getMonth() - 2);
        this.fechaInicio = fechaActual.toISOString().slice(0, 10);
    }

    deleteES(Id: any) {
        this.confirmationService.confirm({
            message: 'Esta a punto de eliminar la entrada de servicio. <b>¿Desea confirmar?</b>',
            accept: () => { this.deleteById(Id); },
            reject: () => { }
        });
    }

    deleteById(Id) {
        this.mensajeComponent.setMsgsEmpty();
        this.service.deleteById(Id).subscribe((result: any) => {
            if (result.logout == true) {
                this.sessionDataService.logout();
            } else if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.info != undefined) {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.data != undefined) {
                this.recalculando = true;
                this.disabledFilter = true;
                this.floatMsgService.setSuccessMsg("Se ha eliminado la entrada de servicio " + Id);
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
        this.showModal = this.itemIdSelected.length > 0;
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

   


    /**
    * Se llama desde cada posición cuando se expande para calcular los
    * valores a certificar de cada item.
    * @param posicion
    */
    calcularValoresACertificar(posicion: any) {
        this.tablaPO.filter(orders => orders.NumeroOrdenDeCompra === posicion.NroOrdenCompra).forEach(order => {
            order.Posiciones.filter(positions => positions.NumeroPosicion === posicion.NumeroPosicion).forEach(position => {
                position.Items.forEach(item => {
                    const monto = item.Importe;
                    const cantidad = item.Cantidad;
                    item.CantidadACertificar = cantidad - item.CantidadReal;
                    item.PorcentajeACertificar = (item.CantidadACertificar * 100) / cantidad;
                    item.MontoACertificar = (item.CantidadACertificar * monto) / cantidad;
                });
            });
        });
    }

    calcularMontoACertificar(item: any) {
        const montoActualizado = (item.CantidadACertificar * item.Importe);
        item.MontoACertificar = montoActualizado;
    }

    actualizarValoresACertificarPorCantidad(item: any): void {
        const cantidadACertificar = item.CantidadACertificar;
        const cantidadDisponible = item.Cantidad - item.CantidadReal;

        if (cantidadACertificar > cantidadDisponible || (cantidadACertificar < 0 && cantidadACertificar != '')) {
            item.CantidadACertificar = cantidadDisponible;
        }

        const percentajeACertificar = (item.CantidadACertificar * 100) / item.Cantidad;
        item.PorcentajeACertificar = Number(percentajeACertificar.toFixed(3));
        this.calcularMontoACertificar(item);
    }

    actualizarValoresACertificarPorPorcentaje(item: any): void {
        const porcentajeDisponible = (100 - item.Porcentaje);
        const porcentajeACertificar = item.PorcentajeACertificar;

        if (porcentajeACertificar > porcentajeDisponible || (porcentajeACertificar < 0 && porcentajeACertificar != '')) {
            item.PorcentajeACertificar = porcentajeDisponible;
        }

        const cantidadACertificar = (item.PorcentajeACertificar * item.Cantidad) / 100;
        item.CantidadACertificar = Number(cantidadACertificar.toFixed(3));
        this.calcularMontoACertificar(item);
    }

    validarMantenerItemSeleccionado(item: any) {
        if (item.CantidadACertificar == 0) {
            this.clearCheckbox(item);
        }
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
        this.actionCheckPosition(item);
        this.itemIdSelected = this.itemIdSelected.filter(obj => { return obj !== item.Id });
        this.itemSelected = this.itemSelected.filter(obj => { return obj !== item });
    }

    isGet100(item: any): boolean {
        return item.Porcentaje === '100';
    }

    selectAllItems(event: any, items: any): void {
        if (event.target.checked) {
            const itemsFiltered = items.filter((row: any) => !this.isGet100(row) && row.MontoACertificar != 0);
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

    /**
     * Selecciona/Deselecciona toda la posición.
     * @param item
     */
    actionCheckPosition(item: any): void {
        let posicion = this.tablaPosiciones.value.find(posicion => posicion.NumeroPosicion === parseInt(item.NroPosicion));
        let itemsConSaldoDisponible = posicion.Items.filter(item => item.MontoACertificar > 0);
        posicion.isSelected = !itemsConSaldoDisponible.some(item => !item.isSelected);
    }

    hideCheckboxToAll(items: any): boolean {
        return items.some(item => !this.isGet100(item) || item.MontoACertificar != 0)
    }

    hidePositionCheckboxToAll(items: any): boolean {
        return items.some(element => {
            return !this.isGet100(element) || (((element.Cantidad - element.CantidadReal) * element.Importe) / element.Cantidad) != 0;
        });
    }

    /**
    * Activa el filtro de posiciones e items sin saldo 
    * a certificar. Este filtro oculta/muestra items certificados
    * al 100%. Si la posición tiene todos sus items certificados
    * entonces oculta también la posición.
    * @param activate Boolean que activa/desactiva filtro
    */
    filtrarElementosSinSaldoACertificar(activate: boolean): void {
        this.ocFilterApplied = activate;
        this.mostrarOcultarItemsSinSaldoACertificar();
    }

    /**
     * Oculta/muestra items sin saldo a certificar.
     */
    mostrarOcultarItemsSinSaldoACertificar() {
        setTimeout(() => {
            let entradasServicio = document.querySelectorAll('#entradasServicio')[0];
            let items = document.querySelectorAll('.percentage-green');

            if (this.ocFilterApplied) {
                items.forEach(el => el.closest('tr').classList.add('hidden'));

                if (entradasServicio != undefined) {
                    entradasServicio.classList.add('hidden');
                }
            }
            else {
                items.forEach(el => el.closest('tr').classList.remove('hidden'));

                if (entradasServicio != undefined) {
                    entradasServicio.classList.remove('hidden');
                }
            }
        }, 20); // Esta espera es necesaria para que parezca el elemento en el DOM
    }

    /**
     * Valida que la posición tenga al menos un item con
     * porcentaje disponible a certificar.
     * @param posicion
     */
    tieneItemsACertificar(posicion: any) {
        let elementosPorCertificar = posicion.Items.some(item => item.Porcentaje != '100');
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
     * del sessionStorage.
     */
    obtenerConfiguracionDeTablasDelUsuario() {
        let colConfig = sessionStorage.getItem('columnasCertificaciones');
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
     * Guarda en el session storage la configuración
     * de tablas del usuario.
     */
    guardarConfiguracionDeTablasDeUsuario() {
        let visibleColumns = this.userTablesConfig.reduce((acc, t) => acc.concat(t.columns.filter(c => c.visible).map(a => a.id)), []);
        sessionStorage.setItem('columnasCertificaciones', JSON.stringify(visibleColumns));
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
        let NumeroOrdenesDeCompras = [];
        let posicionesFiltradas = [];
        this.filtroSolicitantes = event.value;
        this.tablaPO.forEach((oc: any) => {
            posicionesFiltradas = oc.Posiciones.filter((pos: any) => event.value.includes(pos.Solicitante.toUpperCase()));
            if (posicionesFiltradas.length > 0) {
                NumeroOrdenesDeCompras.push(oc.NumeroOrdenDeCompra);
            }
        });
        this.tabla.filter(NumeroOrdenesDeCompras, 'NumeroOrdenDeCompra', 'in');
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
        this.filtrarPorSolicitantes(this.multiSelectSolicitantes);
    }

    obtenerCantidadDePosicionesAMostrar(posiciones: any[]): number {
        if (!this.ocFilterApplied && !this.filtroSolicitantes) {
            return posiciones.length;
        }

        let posicionesAMostrar = posiciones;

        if (this.filtroSolicitantes.length > 0) {
            posicionesAMostrar = posicionesAMostrar.filter(posicion => this.filtroSolicitantes.includes(posicion.Solicitante.toUpperCase()));
        }

        if (this.ocFilterApplied) {
            posicionesAMostrar = posicionesAMostrar.filter(posicion => this.tieneItemsACertificar(posicion));
        }

        return posicionesAMostrar.length;
    }
}