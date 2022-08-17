import { Component, OnInit, OnDestroy } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SelectItem, ConfirmationService } from 'primeng/api';
import { ReporteContratoService } from '../reporte-contrato.service';
import { formatDate } from '@angular/common';
import * as XLSX from 'xlsx';





@Component({
    selector: 'app-reporte-contrato.listado',
    templateUrl: './reporte-contrato.listado.component.html',
    styleUrls: ['./reporte-contrato.listado.component.css'],
   

})
export class ReporteContratoListado extends ListBaseComponent implements OnInit, OnDestroy {

    constructor(protected service: ReporteContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    detalle: any[];
    cabecera: any[];
    filtroCliente: any = null;
    filtroProducto: any = null;
    filtroTipoContrato: any = null;
    filtroContrato = "";
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
    mostrarPendientes: boolean = false;
    columnaCliente: string = "NombreCliente";
    columnaProducto: string = "DescripcionMaterial";
    ColumnaTipoContrato: string = "TipoContrato";



    ngOnInit() {
        this.getListado();
    }

    getListado() {
        this.detalle = null;
        this.varciarFiltrosReporte();
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getListado(this.filtroFechaComponent.fecha_inicio,
            this.filtroFechaComponent.fecha_fin, this.mostrarPendientes).subscribe(
                (result: any) => {
                    this.mensajeComponent.setMsgsEmpty();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.cabecera = result.data.Resultados;
                        this.cargarFiltrosContratos(result);
                        this.getTotalKilogramos();
                        this.data = result.data.Resultados;
                        this.show = true;

                    }

                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        return false;
    }
    getListadoFechas() {
        this.show = false;
        this.getListado();
    }

    getColorProducto(contrato) {
        var fila = this.cabecera.find(x => x.Contrato == contrato);
        return fila.ColorProducto;
    }

    isVisible() {
        return this.show;
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

    filtroPendientes() {
        this.getListado();
    }

    ejecutarFiltro() {
        this.cabecera = this.data;
        var listaFiltro = [{ "campo": this.clienteSelected, "columna": this.columnaCliente },
        { "campo": this.productoSelected, "columna": this.columnaProducto },
        { "campo": this.tipoContratoSelected, "columna": this.ColumnaTipoContrato }];

        for (const a of listaFiltro) {
            if (a.campo == "") {
                continue;
            }
            else {
                this.cabecera = this.cabecera.filter(x => x[a.columna].toUpperCase().indexOf(a.campo.toUpperCase()) >= 0);


            }

        }
        this.cabecera = this.cabecera.filter(x => x.Corredor != "TOTAL");
        this.ObtenerContratosFiltro();
    }

    getTotalKilogramos() {
        this.KilosEntregados = this.cabecera.map(t => t.KilosEntregados).reduce((acc, value) => acc + value, 0);
        this.KilosPendienteEntrega = this.cabecera.map(t => t.KilosPendienteEntrega).reduce((acc, value) => acc + value, 0);
        this.KilosTotales = this.cabecera.map(t => t.KilosTotales).reduce((acc, value) => acc + value, 0);

        this.subscription = this.service.getTotalFormatter(this.KilosEntregados, this.KilosTotales, this.KilosPendienteEntrega).subscribe(
            (result) => {
                this.KilosEntregados = result.KilosEntregadosView;
                this.KilosPendienteEntrega = result.KilosPendienteEntregaView;
                this.KilosTotales = result.KilosTotalesView;
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );

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
        this.subscription = this.service.obtenerContratosFiltro(this.filtroFechaComponent.fecha_inicio,
            this.filtroFechaComponent.fecha_fin, this.mostrarPendientes, this.cabecera).subscribe(
                (result: any) => {
                    this.mensajeComponent.setMsgsEmpty();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.cabecera = result.data.Resultados;
                        this.cargarFiltrosContratos(result);
                        this.getTotalKilogramos();
                        this.show = true;

                    }

                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }

    varciarFiltrosReporte() {
        this.filtroCliente == null;
        this.filtroContrato == null;
        this.filtroProducto == null;
        this.filtroTipoContrato == null;
        this.clienteSelected == "";
        this.productoSelected = "";
        this.tipoContratoSelected = "";
    }
  
}
