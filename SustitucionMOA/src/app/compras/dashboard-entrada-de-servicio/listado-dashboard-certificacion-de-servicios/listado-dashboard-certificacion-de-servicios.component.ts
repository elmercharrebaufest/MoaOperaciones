import { Component, ViewChild, ElementRef } from '@angular/core';
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
        this.navService.setSeccionActive('Ingresar certificación');
        this.navService.navegarSeccion("compras/dashboardCertificacionDeServicios");
        this.filtroFechaComponent.setPeriodoInitial('2');
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

    onCheckboxChange(item: any) {
        const itemId = item.PosicionId;
        const numeroLinea = item.NumeroLinea;

        if (this.itemIdSelected.includes(itemId) && this.numeroLineaSelected.has(numeroLinea)) {
            this.tablaPO.forEach(order => {
                order.Posiciones.forEach((pos: any) => {
                    pos.isSelected = false;
                });
            });

            this.itemIdSelected.splice(this.itemIdSelected.indexOf(itemId), 1);
            this.numeroLineaSelected.delete(numeroLinea);

            this.itemSelected = this.itemSelected.filter((selectedItem: any) =>
                selectedItem.PosicionId !== item.PosicionId || selectedItem.NumeroLinea !== item.NumeroLinea);
        }
        else {
            this.itemIdSelected.push(itemId);
            this.numeroLineaSelected.add(numeroLinea);
            this.itemSelected.push(item);
            this.actionCheckPosition(item);
        }
        this.itemSelected.sort((a, b) => a.NumeroLinea > b.NumeroLinea ? 1 : -1);
    }

    setPositionRow(posicion: any) {
        this.clearCheckboxes();
        this.expandedPositionRow = posicion;
        this.calcularValoresACertificar(posicion);
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
        this.disabledFilter = true;
        // MMSN-519: Colapsar fila expandida al activar un filtro.
        this.collapseExpandedRow();  
        // MMSN-689: Desactivar filtro de saldo pendiente al activar búsqueda. 
        this.filtrarElementosSinSaldoACertificar(false);

        if (this.proveedorSeleccionado !== undefined && this.proveedorSeleccionado !== '') {
            this.proveedor = this.proveedorSeleccionado.CodigoProveedor;
        }
        else {
            this.proveedor = '';
        }

        let startDate = '';
        let endDate = '';

        if (this.filtroFechaComponent.periodo == '4') {
            startDate = Formatter.DateToSting(new Date(this.filtroFechaComponent.dtpInput1.nativeElement.value));
            endDate = Formatter.DateToSting(new Date(this.filtroFechaComponent.dtpInput2.nativeElement.value));
        }
        else {
            startDate = this.filtroFechaComponent.fecha_inicio;
            endDate = this.filtroFechaComponent.fecha_fin;
        }

        this.expandedPositionRow = false;
        this.getListarPO(this.proveedor, this.ordenCompraId, startDate, endDate);
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
                    this.spinnerComponent.hideIt();
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
                    this.getListarPO(this.proveedor, this.ordenCompraId, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin);  
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

    actualizarGrilla(event: string) {
        // MMSN-677: Desactivar filtro de saldo pendiente al certificar ES. 
        this.filtrarElementosSinSaldoACertificar(false);
        this.itemIdSelected = [];
        this.itemSelected = [];
        this.numeroLineaSelected.clear();
        this.recalculando = true;
        this.disabledFilter = true;
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
        const montoActualizado = (item.CantidadACertificar * item.Importe) / item.Cantidad;
        item.MontoACertificar = montoActualizado;
    }

    actualizarValoresACertificarPorCantidad(item: any) {
        const cantidadACertificar = item.CantidadACertificar;
        const cantidadDisponible = item.Cantidad - item.CantidadReal;

        if (cantidadACertificar === null || cantidadACertificar === '' || cantidadACertificar === 0) {
            this.clearCheckbox(item);
        }

        if (cantidadACertificar > cantidadDisponible || (cantidadACertificar < 0 && cantidadACertificar != '')) {
            item.CantidadACertificar = cantidadDisponible;
        }

        item.PorcentajeACertificar = (item.CantidadACertificar * 100) / item.Cantidad;
        this.calcularMontoACertificar(item);
    }

    actualizarValoresACertificarPorPorcentaje(item: any) {
        const porcentajeDisponible = (100 - item.Porcentaje);
        const porcentajeACertificar = item.PorcentajeACertificar;

        if (porcentajeACertificar === null || porcentajeACertificar === '' || porcentajeACertificar === 0) {
            this.clearCheckbox(item);
        }

        if (porcentajeACertificar > porcentajeDisponible || (porcentajeACertificar < 0 && porcentajeACertificar != '')) {
            item.PorcentajeACertificar = porcentajeDisponible;
        }

        item.CantidadACertificar = (item.PorcentajeACertificar * item.Cantidad) / 100;
        this.calcularMontoACertificar(item);
    }

    numbersOnly(event): boolean {
        const charCode = (event.which) ? event.which : event.keyCode;
        return !(charCode > 31 && (charCode < 48 || charCode > 57)) || charCode === 46;
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

    actionCheckPosition(item: any): void {
        let itemsFiltered = [];
        this.tablaPO.filter(orders => orders.NumeroOrdenDeCompra === item.NroOrdenCompra).forEach(order => {
            order.Posiciones.filter(positions => positions.NumeroPosicion === parseInt(item.NroPosicion)).forEach(pos => {
                pos.Items.forEach((i) => {
                    if(!this.isGet100(i) || i.MontoACertificar != 0){
                        itemsFiltered.push(i);
                        pos.isSelected = itemsFiltered.every((everyItem) => everyItem.isSelected ? true : false);
                    }
                });
            })
        })
    }

    hideCheckboxToAll(items: any): boolean {
        return items.some(item => !this.isGet100(item) || item.MontoACertificar != 0)
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

}