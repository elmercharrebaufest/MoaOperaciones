import { Component, OnInit , OnDestroy} from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SelectItem, ConfirmationService } from 'primeng/api';
import { ReporteContratoService } from '../reporte-contrato.service';




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
    clienteSelected: string = "";
    productoSelected: string = "";
    KilosEntregados: string = "";
    data: any[];
    totales: any[];
    KilosPendienteEntrega: string = "";
    KilosTotales: string = "";
  

   

    ngOnInit() {

        this.getListado();
        
        
       
    }


    getListado() {
        this.detalle = null;
        this.mensajeComponent.setMsgsEmpty();

        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getListado(this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin).subscribe(
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
                    this.data = result.data.Resultados;
                    this.totales = result.totales;
                    console.log(result.totales);
                    this.cargarFiltrosContratos(result);
                    this.getTotalKgEntregados();

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

    getColorProducto(contrato) {
        var fila = this.cabecera.find(x => x.Contrato == contrato);
        return fila.ColorProducto;
    }

    isVisible() {
        return this.cabecera && this.cabecera.length != 0;
    }

    cargarFiltrosContratos(result: any) {
        if (result.filtroCliente != undefined) this.filtroCliente = result.filtroCliente.options;
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
        //if (result.filtroTipoContrato != undefined) this.filtroTipoContrato = result.filtroTipoContrato.options;
    }

    setFiltroCliente(cliente: string) {
        this.clienteSelected = cliente;
        //debugger;
        if (cliente != "") {
            this.cabecera = this.data.filter(x => x.NombreCliente == cliente);
            this.getTotalKgEntregados();
          
        }
        else {
            this.cabecera = this.data;
            this.getTotalKgEntregados();
        }
       
      
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
        if (producto != "") {
            this.cabecera = this.data.filter(x => x.DescripcionMaterial == producto);
            this.getTotalKgEntregados();

        }
        else {
            this.cabecera = this.data;
            this.getTotalKgEntregados();
        }
    }

    getTotalKgEntregados() {
        this.KilosEntregados = this.cabecera.map(t => t.KilosEntregados).reduce((acc, value) => acc + value, 0);
        this.KilosPendienteEntrega = this.cabecera.map(t => t.KilosPendienteEntrega).reduce((acc, value) => acc + value, 0);
        this.KilosTotales = this.cabecera.map(t => t.KilosTotales).reduce((acc, value) => acc + value, 0);

        this.subscription = this.service.getTotalFormatter(this.KilosEntregados, this.KilosTotales, this.KilosPendienteEntrega).subscribe(
            (result) => {
                this.KilosEntregados = result.KilosEntregadosView;
                this.KilosPendienteEntrega = result.KilosPendienteEntregaView;
                this.KilosTotales = result.KilosTotalesView;
                console.log(result);
            },
            (error) => {
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
       
    }

   
}
