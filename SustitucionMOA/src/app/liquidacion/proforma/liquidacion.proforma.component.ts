import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { LiquidacionService, LiquidacionProformaService } from './../liquidacion.service';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { Seccion } from './../../common/models/seccion';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';

@Component({
    selector: 'app-liquidacion-proforma',
    templateUrl: `liquidacion.proforma.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionProformaService }]
})
export class LiquidacionProformaComponent extends BaseComponent implements OnInit, AfterViewInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("smallSpinner")
    protected spinnerSmallComponent: SpinnerSmallComponent;


    constructor(private route: ActivatedRoute, private router: Router, protected service: LiquidacionProformaService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    tituloArchivoExcel = "ReporteProformaDeLiquidacion.xls";
    tituloArchivoPDF = "ProformaFinal-";
    fijacion = "";
    faltanDatosCalidad: boolean = false;

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Proforma");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.getData();
    }

    ngAfterViewInit() {
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach((params: Params) => {
            this.fijacion = params['id'];
            this.unsubscribe();
            this.subscription = this.service.getDataProforma(this.fijacion).subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                        this.validarDatosCalidad();
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        });
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelProforma(this.fijacion).subscribe(
            (result:any) => {
                this.spinnerSmallComponent.hideIt();
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
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    descargarProforma(){
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.descargarProformaFinal(this.fijacion).subscribe(
            (result:any) => {
                this.spinnerSmallComponent.hideIt();
                if(result.Pdf){
                    var byteArray = new Uint8Array(result.Pdf.data);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + this.fijacion + '.pdf');
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoPDF + this.fijacion + '.pdf';
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
                else{
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                }
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    isExportVisible() {
        this.spinnerSmallComponent.visible;
    }


    isNegative(valor: number) {
        return valor < 0;
    }

    showModalTableResponsive() {
        this.modalService.openModalTableResponsive("Procedencia Flete", [
            { etiqueta: "Vencimiento", valor: 'a' },
            { etiqueta: "Tipo", valor: 'b'},
            { etiqueta: "Comprobante", valor: 'c' },
            { etiqueta: "Producto", valor: 'd' },
            { etiqueta: "Liquidacion", valor: 'e' },
            { etiqueta: "Total", valor: 'f' },
            { etiqueta: "Contrato", valor: 'g' }
        ]);
        return false;
    }

    showModalProcedencia(contrato: string) {
        this.floatMsgService.setMsgsEmpty();
        
        this.unsubscribe();
        this.subscription = this.service.getFleteProcedencia(contrato).subscribe(
            (result:any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    //this.data = result.data;
                    this.modalService.openModalFleteProcedencia("Procedencia Flete (" + contrato + ")", contrato, result.procedenciasFlete);
                }
                return false;
            },
            error => {
                // this.spinnerComponent.hideIt();
                this.floatMsgService.setErrorMsg(error.message);
                return false;
            }

        );
        return false;
    }

    validarDatosCalidad(){
        this.data.salidas.forEach(s => {
            if(s.caracteristica.toUpperCase() == "FALTAN DATOS CALIDAD"){
                this.faltanDatosCalidad = true;
            }
        })
    }
}