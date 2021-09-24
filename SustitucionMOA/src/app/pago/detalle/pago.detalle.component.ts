import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { PagoService } from './../pago.service';
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
    selector: 'app-pago-detalle',
    templateUrl: `pago.detalle.component.html`,
    providers: [PagoService]
})
export class PagoDetalleComponent extends BaseComponent implements OnInit, AfterViewInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild("smallSpinner")
    protected spinnerSmallComponent: SpinnerSmallComponent;


    constructor(private route: ActivatedRoute, private router: Router, protected service: PagoService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    tituloArchivoExcel = "ReportePagoDetalle.xls";
    numeroPagoId = "";

    setTabs() {
        this.setMenuSeccionTab("pago", "Detalle");
    }

    ngOnInit() {
        this.setTabs();
        this.navService.setSeccionList([]);
        this.securityService.tienePermisoRedirect("CONSULTAR PAGOS DETALLE");
        this.getData();
    }

    ngAfterViewInit() {
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach((params: Params) => {
            this.numeroPagoId = params['id'];
            this.unsubscribe();
            this.subscription = this.service.getDetalle(this.numeroPagoId).subscribe(
                (result:any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    }else if (result.error != undefined && result.error != "") {
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

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcelDetalle(this.numeroPagoId).subscribe(
            (result:any) => {
                this.spinnerSmallComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
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
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    isExportVisible() {
        this.spinnerSmallComponent.visible;
    }


    isNegative(valor : number) {
        return valor < 0;
    }

    descargaPDF(nroDoc: string) {
        return false;
    }

    showCorredorTotal(mercCorredor: number, ivaCorredor: number) {
        return (mercCorredor != 0 || ivaCorredor != 0); 
    }
}