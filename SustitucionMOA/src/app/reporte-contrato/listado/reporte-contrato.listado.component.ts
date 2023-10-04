import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ConfirmationService } from 'primeng/api';
import { ReporteContratoService } from '../reporte-contrato.service';
import * as XLSX from 'xlsx';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ReporteContrato } from '../ReporteContrato.model';
import { Formatter } from '../../common/formatter/Formatter';
import { FiltroFechaReporteComponent } from '../../common/view-child/filtro-fecha-reporte/filtro-fecha-reporte.component';
import { VOLVER_A_DETALLE_REPORTE } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es'



@Component({
    selector: 'app-reporte-contrato.listado',
    templateUrl: './reporte-contrato.listado.component.html',
    styleUrls: ['./reporte-contrato.listado.component.css'],
})
export class ReporteContratoListado extends ListBaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;
    @ViewChild(FiltroFechaReporteComponent)
    protected filtroFechaReporteComponent: FiltroFechaReporteComponent;
    @ViewChild('modalDetalleReporteContrato')
    protected modalDetalleContrato: ElementRef<HTMLDivElement>;


    constructor(protected service: ReporteContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.filtroFechaReporteComponent = new FiltroFechaReporteComponent();
    }
    contratoSeleccionadoId: string = '';
    detalle: any[];
    cabecera: ReporteContrato[];
    filtroCliente: any = null;
    filtroProducto: any = null;
    filtroTipoContrato: any = null;
    filtroContrato: string = "";
    clienteSelected: string = "";
    productoSelected: string = "";
    tipoContratoSelected: string = "";
    KilosEntregados: string = "";
    data: any[];
    KilosPendienteEntrega: string = "";
    KilosTotales: string = "";
    codigoCliente: string = "";
    codigoProducto: string = "";
    TipoContrato: string = "";
    show: boolean = false;
    disabled: boolean = false;
    mostrarPendientes: boolean = true;
    columnaCliente: string = "NombreClienteCUIT";
    columnaProducto: string = "DescripcionMaterial";
    ColumnaTipoContrato: string = "TipoContrato";
    columnaContrato: string = "Contrato";
    esComercial: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA COMERCIALES');
    esMesaFas: boolean = this.isAuthorized('VER ORDENES DE CARGA PARA MESA FAS');
    esAdmin: boolean = this.isAuthorized('VER TODAS ORDENES DE CARGA');
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    esCliente: any = sessionStorage.getItem("tipoUsuario") === "CLI";
    esInterno: boolean = (this.esComercial || this.esMesaFas || this.esAdmin);

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.filtroFechaReporteComponent.setPeriodo('4');
        this.filtroFechaReporteComponent.setFechaIncio(Formatter.DateToSting(new Date(new Date().setDate(new Date().getDate() - 180))));
        this.filtroFechaReporteComponent.setFechaFin(Formatter.DateToSting(new Date()));
        this.mostrarPendientes = true;
        this.getListado();
        registerLocaleData(es)
    }


    getListado() {
        this.detalle = null;
        this.varciarFiltrosReporte();
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('');
        this.unsubscribe();
        this.disabled = true;
        this.cabecera = null;
        this.data = null;
        this.subscription = this.service.getListado(
            this.filtroFechaReporteComponent.fecha_inicio,
            this.filtroFechaReporteComponent.fecha_fin,
            this.mostrarPendientes).subscribe(
                (result: any) => {
                    this.mensajeComponent.setMsgsEmpty();
                    this.blockUI.stop();
                    if (result.logout) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.varciarFiltrosReporte();
                    } else {
                        this.cabecera = result.data.Resultados;
                        this.cargarFiltrosContratos(result);
                        this.getTotalKilogramos();
                        this.data = result.data.Resultados;
                        this.ejecutarFiltro();
                        this.show = true;
                        this.disabled = false;

                        // VALIDA EL CASO CUANDO UN USUARIO ES DIRECTO
                        const listaUsuariosDirectos = this.cabecera.filter(x => x.Corredor == '0050005000');
                        if (listaUsuariosDirectos != null && listaUsuariosDirectos.length > 0) {
                            this.esCliente = true;
                            this.esCorredor = true;
                        }
                        if (!!sessionStorage.getItem(VOLVER_A_DETALLE_REPORTE)) {
                            this.modalDetalleContrato.nativeElement.click();
                            this.verDetalleContrato(this.service.getContratoSeleccionadoValue())
                        }
                    }

                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        return false;
    }
    getListadoFechas() {
        this.show = false;
        this.getListado();
    }

    getColorProducto(contrato): string {
        const fila = this.cabecera.find(x => x.Contrato == contrato);
        return fila.ColorProducto;
    }

    isVisible() {
        return this.show && this.cabecera.length;
    }

    guardarFiltros() {
        const { fecha_inicio, fecha_fin } = this.filtroFechaReporteComponent
        this.service.setFechas(fecha_inicio, fecha_fin);
    }

    verDetalleContrato(numeroContrato: string) {
        this.contratoSeleccionadoId = numeroContrato;
        this.service.setContratoSeleccionado(numeroContrato);
    }

    cargarFiltrosContratos(result: any) {
        if (result.filtroCliente != undefined) this.filtroCliente = result.filtroCliente.options;
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
        if (result.filtroTipoContrato != undefined) this.filtroTipoContrato = result.filtroTipoContrato.options;
    }


    setFiltroCliente(cliente: string) {
        this.clienteSelected = cliente;
        this.ejecutarFiltro();
    }

    setFiltroTipoContrato(tipoContrato: string) {
        this.tipoContratoSelected = tipoContrato;
        this.ejecutarFiltro();
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto
        this.ejecutarFiltro();
    }

    filtroPendientes(event) {
        this.mostrarPendientes = event;
        this.ejecutarFiltro();
    }


    setFiltroContrato(contrato: string) {
        this.filtroContrato = contrato.split(' ').join('');
        this.ejecutarFiltro();
    }

    ejecutarFiltro() {
        this.cabecera = this.data;
        var listaFiltro = [{ "campo": this.clienteSelected, "columna": this.columnaCliente },
        { "campo": this.productoSelected, "columna": this.columnaProducto },
        { "campo": this.tipoContratoSelected, "columna": this.ColumnaTipoContrato },
        { "campo": this.filtroContrato, "columna": this.columnaContrato }];

        for (const a of listaFiltro) {
            if (a.campo == "") {
                continue;
            }
            else {
                this.cabecera = this.cabecera.filter(x => x[a.columna].toUpperCase().indexOf(a.campo.toUpperCase()) >= 0);
            }

        }
        this.cabecera = this.cabecera.filter(x => x.Corredor != "TOTAL");
        if (this.mostrarPendientes) {
            this.cabecera = this.cabecera.filter(x => x.KilosPendienteEntrega > 0);
        }
        this.getTotalKilogramos();
    }

    getTotalKilogramos() {
        this.KilosEntregados = this.cabecera.filter(x => x.Corredor != "TOTAL").map(t => t.KilosEntregados).reduce((acc, value) => acc + value, 0).toString();
        this.KilosPendienteEntrega = this.cabecera.filter(x => x.Corredor != "TOTAL").map(t => t.KilosPendienteEntrega).reduce((acc, value) => acc + value, 0).toString();
        this.KilosTotales = this.cabecera.filter(x => x.Corredor != "TOTAL").map(t => t.KilosTotales).reduce((acc, value) => acc + value, 0).toString();
    }

    exportExcelReporteContrato() {
        this.mensajeComponent.setMsgsEmpty();
        let informacionExportar: any;

        informacionExportar = this.cabecera.map(info => {
            return {
                "Contrato": info.Contrato || "-",
                "Tipo Contrato": info.TipoContrato || "-",
                "Pedido": info.PedidoCliente || "-",
                "Fecha": info.FechaDesde,
                "Cliente": info.NombreCliente || "-",
                "Corredor": info.Corredor || "-",
                "Producto": info.DescripcionMaterial || "-",
                "Kgs Totales": info.KilosTotalesStr,
                "Kgs Entregados": info.KilosEntregadosStr,
                "Kgs Pendiente de entrega": info.KilosPendienteEntregaStr
            }
        });

        if (informacionExportar.length == 0) {
            this.mensajeComponent.setInfoMsg("No existen datos para exportar.");
            return
        }

        this.DownloadJsonData(informacionExportar, "ReporteContrato");
    }

    DownloadJsonData(JSONData: any, FileTitle: string) {

        //crea la estructura inicial del archivo
        let worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(JSONData);
        let workbook: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'Reporte de contratos');
        //escribe el file para ser descargado
        const excelBuffer: any = XLSX.writeFile(workbook, FileTitle + '.xlsx');
    }

    ObtenerContratosFiltro() {
        if (this.filtroContrato == "") this.blockUI.start('');
        this.subscription = this.service.obtenerContratosFiltro(this.filtroFechaReporteComponent.fecha_inicio,
            this.filtroFechaReporteComponent.fecha_fin, this.mostrarPendientes, this.cabecera).subscribe(
                (result: any) => {
                    this.mensajeComponent.setMsgsEmpty();
                    this.blockUI.stop();
                    this.cargarFiltrosContratos(result);
                    if (result.logout) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                        this.varciarFiltrosReporte();
                        this.show = false;
                    } else {
                        this.cabecera = result.data.Resultados;
                        this.getTotalKilogramos();
                        this.show = true;
                    }

                },
                (error) => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);

                }
            );
    }

    varciarFiltrosReporte() {
        this.filtroCliente = null;
        this.filtroProducto = null;
        this.filtroTipoContrato = null;
        this.clienteSelected = "";
        this.productoSelected = "";
        this.tipoContratoSelected = "";
    }

    get verTipoContrato(): boolean {
        return !this.esCorredor;
    }
    get verNroPedidoCliente(): boolean {
        return !this.esCorredor;
    }
    get verCorredor(): boolean {
        return !this.esCorredor;
    }
    get verCliente(): boolean {
        return !this.esCliente
    }
}
