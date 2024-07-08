import { Component, EventEmitter, OnInit, Output } from '@angular/core';
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
import { Subscription } from 'rxjs';
import { VOLVER_A_DETALLE_REPORTE } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es'

@Component({
    selector: 'app-detalle',
    templateUrl: './reporte-contrato.detalle.component.html',
    styleUrls: ['./reporte-contrato.detalle.component.css']
})
export class DetalleComponent extends ListBaseComponent implements OnInit {
    @Output() navegarDetalle: EventEmitter<void> = new EventEmitter();
    contratoId: string;
    ordenDeCargaId: string = null;
    detalles: DetalleReporteContrato[] = null;
    KilosFacturados = 0;
    KilosEntregados = 0;
    localSubscriptions = new Subscription();

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
        this.localSubscriptions.add(
            this.service.getContratoSeleccionado().subscribe(data => {
                if (data != null && data > '0') {
                    this.getDetalleContrato(data);
                    this.contratoId = data;
                }
            })
        )

    }
    ngOnInit() {
        registerLocaleData(es)
    }

    getDetalleContrato(contratoId: string) {
        this.data = null;
        this.detalles = null;
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
                    this.obtenerKilos();
                    if (!!sessionStorage.getItem(VOLVER_A_DETALLE_REPORTE))
                        sessionStorage.removeItem(VOLVER_A_DETALLE_REPORTE)
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
                "ID": info.OrdenCargaId || "",
                "Fecha de Carga": info.FechaCarga || "-",
                "Cant. Entregada": info.CantidadEntregadaStr || "-",
                "CTG/Remito": info.Remito || "-",
                "CPE": info.CPE || "-",
                "Cant. Facturada": info.CantidadFacturaStr || "-",
                "Factura": info.FacturaLegal || "-",
                "Chasis": info.Chasis || "-",
                "Acoplado": info.Acoplado || "-",
                "Chofer": info.Chofer || "-"
            }
        });

        if (informacionExportar.length == 0) {
            this.mensajeComponent.setInfoMsg("No existen datos para exportar.");
            return
        }

        //Se agrega fila de totales.
        let totales = {
            "ID": "TOTAL",
            "Fecha de Carga": "",
            "Cant. Entregada": this.KilosEntregados.toLocaleString('es-ES') + " KG",
            "CTG/Remito": "",
            "CPE": "",
            "Cant. Facturada": this.KilosFacturados.toLocaleString('es-ES') + " KG",
            "Factura": "",
            "Chasis": "",
            "Acoplado": "",
            "Chofer": ""
        }

        informacionExportar.push(totales);

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
        this.KilosEntregados = this.detalles.reduce((prev, curr) => prev + curr.CantidadEntregada, 0)
        this.KilosFacturados = this.detalles.reduce((prev, curr) => prev + curr.CantidadFactura, 0)
    }
    navegarDetalleOrdenCarga(det: DetalleReporteContrato) {
        this.service.getOrdenDeCarga(det).subscribe({
            next: ({ info, error, data }) => {
                if (error) {
                    this.floatMsgService.setErrorMsg(error)
                }
                else if (info) {
                    this.floatMsgService.setInfoMsg(info)
                } else {
                    this.navegarDetalle.emit();
                    sessionStorage.setItem(VOLVER_A_DETALLE_REPORTE, 'true')
                    this.goToSeccionParam('/ordenes-de-carga/detalle', data.Id.toString());
                }
            }
            ,
        })
    }
    public extraOnDestroy(): void {
        this.localSubscriptions.unsubscribe()
    }
}
