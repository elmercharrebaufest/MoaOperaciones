import { Component, OnInit, ViewChild } from '@angular/core';
import { FacturasCertificacionFilter } from './facturas-certificacion-filter.model';
import { ReporteFacturasCertificacionesService } from '../reporte-facturas-certificaciones.service';
import { SessionDataService } from '../../common/services/SessionDataService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { TipoPeriodo } from '../../common/enums/TipoPeriodo';
import { FiltroFechaComponent } from '../../common/view-child/filtro-fecha/filtro-fecha.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { FacturaService } from '../../factura/factura.service';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SecurityService } from '../../common/services/SecurityService';
import { Permiso } from '../../common/enums/Permisos';
import * as XLSX from 'xlsx';

@Component({
    selector: 'app-reporte-facturas-certificaciones',
    templateUrl: './reporte-facturas-certificaciones.component.html',
    styleUrls: ['./reporte-facturas-certificaciones.component.css']
})
export class ReporteFacturasCertificacionesComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(FiltroFechaComponent)
    protected filtroFechaComponent!: FiltroFechaComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent!: MensajeComponent;

    public facturasCertificacionFilterModel: FacturasCertificacionFilter;
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimaSemana;
    filtroFechaKey: string = 'RepFacCer_Periodo';
    datosCertificaciones: any[] = [];
    public itemsPerPage!: string;

    // Propiedades para paginación
    p: number = 1; // página actual
    totalItems: number = 0;
    totalPaginas: number = 0;
    ordenActual: string = "DESC";
    columnaActual: string = "FechaDeRegistro";

    esUsuarioInternoMoa: boolean = false;

    constructor(
        private facturaService: FacturaService,
        protected sessionDataService: SessionDataService,
        private reporteFacturasCertificacionesService: ReporteFacturasCertificacionesService,
        protected floatMsgService: FloatMsgService,
        protected securityService: SecurityService) {
        this.facturasCertificacionFilterModel = new FacturasCertificacionFilter();
    }

    ngOnInit() {
        this.esUsuarioInternoMoa = this.securityService.tienePermiso(Permiso.ReporteFacturasCertificaciones);
        this.itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
        this.onChangeFecha();
    }

    // Cuando cambia el filtro de fecha
    public onChangeFecha() {
        if (this.filtroFechaComponent.getFechaIncio() == undefined || this.filtroFechaComponent.getFechaFin() == undefined) {
            this.filtroFechaComponent.setPeriodo(this.filtroFechaPeriodoDefault);
        }

        // Aplicar filtro de fecha
        this.facturasCertificacionFilterModel.periodo = this.filtroFechaComponent.periodo;
        this.facturasCertificacionFilterModel.fechaInicio = this.filtroFechaComponent.getFechaIncio();
        this.facturasCertificacionFilterModel.fechaFin = this.filtroFechaComponent.getFechaFin();

        // Resetear otros filtros (mutuamente excluyentes)
        this.facturasCertificacionFilterModel.ordenDeCompra = null;
        this.facturasCertificacionFilterModel.proveedor = null;

        // Reset paginación
        this.p = 1;
        this.cargarDatos();
    }

    // Cuando cambia el filtro de orden de compra
    public onChangeOrdenCompra(ordenCompra: string) {
        // Aplicar filtro de orden de compra
        this.facturasCertificacionFilterModel.ordenDeCompra = ordenCompra;
        this.facturasCertificacionFilterModel.proveedor = null;

        // Reset paginación
        this.p = 1;
        this.cargarDatos();
    }

    // Cuando cambia el filtro de código de proveedor
    public onChangeCodigoProveedor(proveedor: string) {
        // Aplicar filtro de código de proveedor
        this.facturasCertificacionFilterModel.proveedor = proveedor;
        this.facturasCertificacionFilterModel.ordenDeCompra = null;

        // Reset paginación
        this.p = 1;
        this.cargarDatos();
    }

    public cargarDatos() {
        this.blockUI.start();
        this.reporteFacturasCertificacionesService.GetDatosReporte(
            this.facturasCertificacionFilterModel.periodo,
            this.facturasCertificacionFilterModel.fechaInicio,
            this.facturasCertificacionFilterModel.fechaFin,
            this.p,
            parseInt(this.itemsPerPage),
            this.ordenActual,
            this.columnaActual,
            this.facturasCertificacionFilterModel.ordenDeCompra,
            this.facturasCertificacionFilterModel.proveedor
        ).subscribe(
            (result: any) => {
                this.blockUI.stop();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    this.datosCertificaciones = result.certificaciones;
                    this.totalItems = result.totalItems;
                    this.totalPaginas = result.totalPaginas;
                    this.p = result.paginaActual;
                }
            },
            error => {
                this.blockUI.stop();
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
    }

    // Método para manejar cambios de página
    public pageChanged(page: number): void {
        // Prevenir valores de página inválidos
        if (page < 1) {
            page = 1;
        } else if (this.totalPaginas > 0 && page > this.totalPaginas) {
            page = this.totalPaginas;
        }

        // Solo actualizar y cargar si la página realmente cambió
        if (this.p !== page) {
            this.p = page;
            this.cargarDatos();
        }
    }

    // Método para ordenar resultados
    public ordenar(columna: string): void {
        if (this.columnaActual === columna) {
            // Cambiar dirección del orden si es la misma columna
            this.ordenActual = this.ordenActual === "ASC" ? "DESC" : "ASC";
        } else {
            // Nueva columna para ordenar
            this.columnaActual = columna;
            this.ordenActual = "ASC";
        }
        this.cargarDatos();
    }

    public generatePageArray(): number[] {
        const pages: number[] = [];
        const maxPagesToShow = 5; // Ajusta según necesidad

        let startPage = Math.max(1, this.p - Math.floor(maxPagesToShow / 2));
        let endPage = Math.min(this.totalPaginas, startPage + maxPagesToShow - 1);

        // Ajustar si estamos cerca del final
        if (endPage - startPage + 1 < maxPagesToShow && startPage > 1) {
            startPage = Math.max(1, endPage - maxPagesToShow + 1);
        }

        for (let i = startPage; i <= endPage; i++) {
            pages.push(i);
        }

        return pages;
    }

    obtenerNombreDeArchivo(ruta: string) {
        // Replace all backslashes with forward slashes and then split
        const normalizedPath = ruta.replace(/\\/g, '/');
        const partes = normalizedPath.split('/');
        return partes[partes.length - 1];
    }

    descargarDocumentoAdjunto(archivoId: number) {
        this.blockUI.start("Descargando...");
        this.facturaService.descargarDocumentoAdjunto(archivoId.toString())
            .subscribe(
                (result) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }
                    else {
                        var byteArray = new Uint8Array(result.FileContents);
                        var blob = new Blob([byteArray], {
                            type: "application/octet-stream",
                        });

                        if ((window.navigator as any).msSaveOrOpenBlob) {
                            // IE11
                            (window.navigator as any).msSaveOrOpenBlob(
                                blob,
                                result.FileDownloadName
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = result.FileDownloadName;
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);
                            this.blockUI.stop();
                            return false;
                        }
                        this.blockUI.stop();
                    }
                },
                (error) => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
    }

    exportarListadoAExcel() {
        const workbook = XLSX.utils.book_new();

        const encabezados: string[] = [
            'Razón social',
            'Nro OC',
            'Nro certificación',
            'Importe',
            'Moneda',
            'Usuario',
            'Fecha de registro',
            'Archivo'
        ];
        const datosAExportar = this.datosCertificaciones.map(x => [
            x.RazonSocial,
            x.NRO_OC,
            x.NRO_Certificacion,
            x.Importe.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' }),
            x.Moneda,
            x.Mail,
            (new Date(parseInt(x.FechaDeRegistro.slice(6, -2)))).toLocaleString('es-AR'),
            this.obtenerNombreDeArchivo(x.Archivo.Ruta)]
        );
        const datosExcel = [encabezados, ...datosAExportar];

        const worksheet: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(datosExcel);
        
        datosExcel[0].forEach((_, colInd) => {
            const cellAddress = XLSX.utils.encode_cell({ r: 0, c: colInd });
            if (!worksheet[cellAddress]) {
                worksheet[cellAddress] = {};
            }
            worksheet[cellAddress].s = {
                fill: {
                    patternType: "solid",
                    fgColor: { rgb: "D3D3D3" }
                },
                font: {
                    bold: true
                },
                alignment: {
                    horizontal: "center",
                    vertical: "center"
                }
            };
        });

        const anchosCols = datosExcel[0].map((_, colInd) =>
            Math.max(
                ...datosExcel.map(row => (
                    row[colInd] !== null && row[colInd] !== undefined ? row[colInd].toString().length : 0))
            )
        );

        worksheet["!cols"] = anchosCols.map(ancho => ({ wch: ancho }));

        XLSX.utils.book_append_sheet(workbook, worksheet, 'Facturas');

        const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
        this.guardarComoExcel(excelBuffer, 'Facturas de OCs');
    }
    
    guardarComoExcel(buffer: any, nombreArchivo: string): void {
        const data: Blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8' });
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(data);
        link.download = nombreArchivo + '.xlsx';
        link.click();
    }
}