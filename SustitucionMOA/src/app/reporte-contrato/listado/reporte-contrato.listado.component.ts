import { Component, OnInit , OnDestroy} from '@angular/core';
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
    //providers: [
    //    ReporteContratoService
    //]

})
export class ReporteContratoListado extends ListBaseComponent implements OnDestroy {

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
    totales: any[];
    KilosPendienteEntrega: string = "";
    KilosTotales: string = "";
    codigoCliente: string = "";
    codigo: any = null;
    codigoProducto: string = "";
    TipoContrato: string = "";
    esFiltro: boolean = false;
    show: boolean = false;
    mostrarPendientes: boolean = false;
     

    ngOnInit() {
        this.getListado();                  
    }
    
    getListado() {
        this.detalle = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getListado(this.filtroFechaComponent.fecha_inicio,
            this.filtroFechaComponent.fecha_fin, this.codigoCliente, this.codigoProducto, this.esFiltro, this.mostrarPendientes, this.TipoContrato).subscribe(
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
                    if (!this.esFiltro) {
                        this.data = result.data.Resultados;
                    }
                    this.show = true;
                    //result.data.Resultados.Detalles.forEach(x => {
                    //    console.log(result.data.Resultados.Detalles);
                    //});

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
        this.esFiltro = false;
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

    setParametros() {
        this.esFiltro = true;
        this.show = false;
        this.getListado();
    }

    //vaciarFiltrosReportes() {
    //    this.filtroCliente = "";
    //    this.filtroContrato = "";
    //    this.filtroProducto = "";
    //    this.filtroTipoContrato = "";
    //}
    setFiltroCliente(cliente: string) {
        if (cliente != "") {
            this.clienteSelected = cliente;
            this.codigo = this.cabecera.find(x => x.NombreCliente == cliente) == undefined ? this.data.find(x => x.NombreCliente == cliente) : this.cabecera.find(x => x.NombreCliente == cliente);
            this.codigoCliente = this.codigo.Cliente;                       
        }
        else {
            this.codigoCliente = "";
        }
        this.setParametros();
    }

    setFiltroTipoContrato(tipoContrato: string) {
        if (tipoContrato != "") {
            this.tipoContratoSelected = tipoContrato;
            this.codigo = this.cabecera.find(x => x.TipoContrato == tipoContrato) == undefined ? this.data.find(x => x.TipoContrato == tipoContrato) : this.cabecera.find(x => x.TipoContrato == tipoContrato);
            this.TipoContrato = this.codigo.TipoContrato;
        }
        else {
            this.TipoContrato = "";
        }
        this.setParametros();
    }

    setFiltroProducto(producto: string) {
        if (producto != "") 
        {
            this.productoSelected = producto
            this.codigo = this.cabecera.find(x => x.DescripcionMaterial == producto) == undefined ? this.data.find(x => x.DescripcionMaterial == producto) : this.cabecera.find(x => x.DescripcionMaterial == producto);
            this.codigoProducto = this.codigo.Producto;        
        }
        else
        {
            this.codigoProducto = "";
        }
        this.setParametros();        
    }

    filtroPendientes() {
        this.mostrarPendientes = true;
        this.setParametros();
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

   
}
