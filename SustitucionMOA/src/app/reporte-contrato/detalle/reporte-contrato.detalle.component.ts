import { Component, EventEmitter, OnInit, Output} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ReporteContratoService } from '../reporte-contrato.service';
import * as XLSX from 'xlsx';
import { DetalleReporteContrato } from '../ReporteContrato.model';
import { OrdenesDeCargaService } from '../../ordenes-de-carga/ordenes-de-carga.service';


@Component({
    selector: 'app-detalle',
    templateUrl: './reporte-contrato.detalle.component.html',
    styleUrls: ['./reporte-contrato.detalle.component.css']
})
export class DetalleComponent extends ListBaseComponent implements OnInit {
    contratoId: string;
    ordenDeCargaId: string=null;
    detalles: DetalleReporteContrato[] = null;
    KilosFacturados = 0;
    KilosEntregados = 0;

    constructor(private route: ActivatedRoute, 
                protected service: ReporteContratoService, 
                protected ordenesDeCargaService: OrdenesDeCargaService, 
                protected navService: NavService, 
                protected sessionDataService: SessionDataService, 
                protected securityService: SecurityService, 
                protected floatMsgService: FloatMsgService, 
                protected modalService: ModalService, 
                private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.service.getContratoSeleccionado().subscribe(data => {
            if (data != null && data >'0'){
                this.getDetalleContrato(data);
                this.contratoId = data;
            }
        });
    }
    ngOnInit() {
        
    }

    getDetalleContrato(contratoId:string) {
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();        
            this.unsubscribe();
            this.subscription = this.service.getDetalleContrato2(contratoId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.detalles = result.data;
                        console.log('this.detalles--->>', this.detalles);
                        this.obtenerKilos();
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );

        
    }

    exportExcelReporteContratoDetalle() {
        this.mensajeComponent.setMsgsEmpty();
        let informacionExportar: any;

        informacionExportar = this.detalles.map(info => {
            return {
                "Fecha Pedido": info.FechaPedido || "-",
                "Fecha Carga": info.FechaCarga || "-",
                "Cantidad Entregada": info.CantidadEntregadaStr || "-",
                "Remito": info.Remito,
                "Factura": info.Factura || "-",
                "Chasis": info.Chasis || "-",
                "Acoplado": info.Acoplado || "-",
                "Chofer": info.Chofer
            }
        });

        if (informacionExportar.length == 0) {
            this.mensajeComponent.setInfoMsg("No existen datos para exportar.");
            return
        }

        this.DownloadJsonData(informacionExportar, "ReporteContratoDetalle");
    }

    DownloadJsonData(JSONData: any, FileTitle: string) {

        //crea la estructura inicial del archivo
        let worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(JSONData);
        let workbook: XLSX.WorkBook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, 'Reporte de contratos detalle');
        //escribe el file para ser descargado
        const excelBuffer: any = XLSX.writeFile(workbook, FileTitle + '.xlsx');
    }

    obtenerKilos() {
        this.KilosEntregados = this.detalles.reduce((prev,curr)=> prev + curr.CantidadEntregada,0)
        this.KilosFacturados = this.detalles.reduce((prev,curr)=> prev + curr.CantidadFactura,0)
    }
    navegarDetalleOrdenCarga(det: DetalleReporteContrato){
        this.service.getOrdenDeCarga(det).subscribe({
            next:(res)=>{
                if(res.error){
                    this.floatMsgService.setErrorMsg(res.error)
                }
                else if(res.info){
                    this.floatMsgService.setInfoMsg(res.info)
                }else{
                    this.goToSeccionParam('/ordenes-de-carga/detalle', res.data.Id.toString());
                }
            }
            ,
        })
    }

}
