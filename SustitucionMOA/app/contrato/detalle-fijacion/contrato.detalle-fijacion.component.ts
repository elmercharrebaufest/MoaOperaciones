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
import { Seccion } from './../../common/models/Seccion';
import { ModalService } from './../../common/services/ModalService';

@Component({
    selector: 'my-app',
    templateUrl: `./app/contrato/detalle-fijacion/contrato.detalle-fijacion.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: ContratoService, useClass: ContratoFijacionService }]
})
export class ContratoDetalleFijacionComponent extends BaseComponent implements OnInit, AfterViewInit {

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
    tituloArchivoExcel = "ReporteFijacionDetalle.xls";
    numeroContratoId = "";
    fijacion = "";

    setTabs() {
        this.setMenuSeccionTab("contrato", "Detalle Fijaciones");
    }

    ngOnInit() {
        this.setTabs();
        this.securityService.tienePermisoRedirect("CONSULTAR CONTRATO DETALLE");
        this.navService.setSeccionList([new Seccion('/contrato/vigente', 'contrato', 'Vigentes'), new Seccion('/contrato/fijacion', 'contrato', 'Fijaciones'), new Seccion('/contrato/ampliacion', 'contrato', 'Ampliaciones'), new Seccion('/contrato/anulacion', 'contrato', 'Anulaciones')]);
        this.navService.setSeccionActive("Detalle");
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
        this.route.params.forEach((params: Params) => {
            this.numeroContratoId = params['id'];
            this.fijacion = params['id2']; 
            this.unsubscribe();
            this.subscription = this.service.getDetalleFijacion(this.numeroContratoId, this.fijacion).subscribe(
                result => {
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

    exportarExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallExportComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalleFijacion(this.numeroContratoId, this.fijacion).subscribe(
            result => {
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

    showDataPlus(registro: any) {
        return this.tieneData(registro.kgNetos) || this.tieneData(registro.kgDto) || this.tieneData(registro.kgApli) || this.tieneData(registro.dto);
    }

    tieneData(value: any) {
        return value != undefined && value != 0 && value != "" && value != "0 KG" && value != "0%";
    }

}