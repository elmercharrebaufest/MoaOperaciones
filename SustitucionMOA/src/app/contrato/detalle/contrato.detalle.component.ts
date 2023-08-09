import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { ContratoBaseComponent } from './../contrato.component';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ContratoService, ContratoFijacionService } from './../contrato.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { Seccion } from './../../common/models/seccion';
import { ModalService } from './../../common/services/ModalService';

@Component({
    selector: 'app-contrato-detalle',
    templateUrl: `contrato.detalle.component.html`,
    providers: [{ provide: ContratoService, useClass: ContratoFijacionService }]
})
export class ContratoDetalleComponent extends BaseComponent implements OnInit, AfterViewInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("spinnerSmallExport")
    protected spinnerSmallExportComponent: SpinnerSmallComponent;

    @ViewChild("spinnerSmallPDF")
    protected spinnerSmallPDFComponent: SpinnerSmallComponent;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        protected service: ContratoService,
        protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService
    ) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any = null;
    tituloArchivoPDF = "BoletoFisico";
    tituloArchivoExcel = "ReporteContratoDetalle.xls";
    numeroContratoId = "";
    filtroCCPP = "";

    setTabs() {
        this.setMenuSeccionTab("contrato", "Detalle");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.navService.setSeccionList([new Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.getData();
    }

    public ngAfterViewInit(): void {
        this.spinnerSmallExportComponent = new SpinnerSmallComponent();
        this.spinnerSmallPDFComponent = new SpinnerSmallComponent();
    }

    getData() {
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.subscribe(params => {
            this.numeroContratoId = params['id'];
            this.unsubscribe();
            this.subscription = this.service.getDetalle(this.numeroContratoId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result;

                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
            //this.navService.setSeccionActive('Fijaciones');
        });
    }

    downloadBoletoFisico() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallPDFComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.downloadBoletoFisico(this.numeroContratoId).subscribe(
            (result: any) => {
                this.spinnerSmallPDFComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.data);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + this.numeroContratoId + ".pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoPDF + this.numeroContratoId + ".pdf"
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.spinnerSmallPDFComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;
    }

    exportarExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallExportComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.numeroContratoId).subscribe(
            (result: any) => {
                this.spinnerSmallExportComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoExcel);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoExcel;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.spinnerSmallExportComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    descargarPDF() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDFCalidad(this.numeroContratoId).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.data);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, "Calidad Contrato(" + this.numeroContratoId + ").pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = "Calidad Contrato(" + this.numeroContratoId + ").pdf"
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
            }
        );
        return false;
    }

    showDataPlus(registro: any) {
        return this.tieneData(registro.kgNetos) || this.tieneData(registro.kgDto) || this.tieneData(registro.kgApli) || this.tieneData(registro.dto);
    }

    tieneData(value: any) {
        return value != undefined && value != 0 && value != "" && value != "0 KG" && value != "0%";
    }

    descargaRetencionesPDF(pago: string) {
        // Descarga PDF TODO
        return false;
    }

    showModalTableBoletosResponsive(Boleto: any) {
        this.modalService.openModalTableResponsive("Oblea Boleto", [
            { etiqueta: "Envío bolsa", valor: Boleto.envioBolsa },
            { etiqueta: "Vuelta Bolsa", valor: Boleto.vueltaBolsa },
            { etiqueta: "Oblea Bolsa", valor: Boleto.obleaBolsa },
            { etiqueta: "Envío Afip", valor: Boleto.envioAfip },
            { etiqueta: "Vuelta Afip", valor: Boleto.vueltaAfip },
            { etiqueta: "REG. Afip", valor: Boleto.obleaAfip },
            { etiqueta: "Oblea Prov. Plan Canje", valor: Boleto.provPlanCanje },
            { etiqueta: "Envío Sellado", valor: Boleto.envioSellado },
            { etiqueta: "Vuelta Sellado", valor: Boleto.vueltaSellado }
        ]);
        return false;
    }

    showModalTablePagosResponsive(Pago: any) {
        this.modalService.openModalTableResponsive("Pago", [
            { etiqueta: "Fecha", valor: Pago.fecha },
            { etiqueta: "ID pago", valor: Pago.idPago },
            { etiqueta: "Brutor", valor: Pago.brutoString },
            { etiqueta: "Iva", valor: Pago.ivaString },
            { etiqueta: "Retenciones", valor: Pago.retencionesString },
            { etiqueta: "Total", valor: Pago.netoString }
        ]);
        return false;
    }

    showModalTableLiquidacionesResponsive(Liquidacion: any) {
        this.modalService.openModalTableResponsive("Liquidación", [
            { etiqueta: "Fecha", valor: Liquidacion.fecha },
            { etiqueta: "Tipo", valor: Liquidacion.tipo },
            { etiqueta: "Comprobante", valor: Liquidacion.comprobante },
            { etiqueta: "Nº Fijación", valor: Liquidacion.pedido },
            { etiqueta: "KG", valor: Liquidacion.cantidadString },
            { etiqueta: "Precio/tn", valor: Liquidacion.precioString },
            { etiqueta: "Total", valor: Liquidacion.totalString }
        ]);
        return false;
    }

    showModalTableLFijacionesResponsive(Fijacion: any) {
        this.modalService.openModalTableResponsive("Fijación", [
            { etiqueta: "Fecha", valor: Fijacion.fecha },
            { etiqueta: "Nº Fijación", valor: Fijacion.nroFija },
            { etiqueta: "Kilos Fijados", valor: Fijacion.kilosFijaString },
            { etiqueta: "Precio/TN", valor: Fijacion.precioString }
        ]);
        return false;
    }

    showModalTableAmpliacionesAnulacionesResponsive(AmpAnul: any) {
        this.modalService.openModalTableResponsive("Ampliación / Anulación", [
            { etiqueta: "Tipo", valor: AmpAnul.tipo },
            { etiqueta: "Fecha", valor: AmpAnul.fecha },
            { etiqueta: "Cantidad", valor: AmpAnul.cantidadString },
            { etiqueta: "Precio/Multa", valor: AmpAnul.importeString }
        ]);
        return false;
    }

    showModalTableAplicacionesResponsive(Aplicacion: any) {
        this.modalService.openModalTableResponsive("Aplicación", [
            { etiqueta: "Fecha", valor: Aplicacion.fecha },
            { etiqueta: "Carta de Porte", valor: Aplicacion.ccpp },
            { etiqueta: "Lugar de Descarga", valor: Aplicacion.descarga },
            { etiqueta: "KG Brutos", valor: Aplicacion.kgBrutosString },
            { etiqueta: "KG Aplicados", valor: Aplicacion.kgNetosString }
        ]);
        return false;
    }

    showModalTableHijosResponsive(Hijo: any) {
        this.modalService.openModalTableResponsive("Hijo", [
            { etiqueta: "Fecha de Concertación", valor: Hijo.fecha },
            { etiqueta: "Contrato Molinos", valor: Hijo.contrMolinos },
            { etiqueta: "Contrato Proveedor", valor: Hijo.contrProve },
            { etiqueta: "Pactado", valor: Hijo.cantidadString },
            { etiqueta: "Precio/tn", valor: Hijo.precioString }
        ]);
        return false;
    }

    isData() {
        return this.data != null;
    }
    tieneCamara(calidad: any): boolean {
        return calidad.certificado != '';
    }
}