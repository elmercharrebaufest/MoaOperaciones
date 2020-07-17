import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDService, RYDInformeService } from './../ryd.service';
import { RYDBaseComponent } from './../ryd.component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { DataService } from './../../common/services/DataService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-ryd-informe',
    templateUrl: `./app/ryd/informe/informe.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: RYDService, useClass: RYDInformeService }]
})
export class InformeComponent extends RYDBaseComponent {

    @ViewChild(DropdownComponent)
    protected inputBalanzaComponent: DropdownComponent;

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    private spinnerComponent: SpinnerComponent;

    @ViewChild(SpinnerSmallComponent)
    private spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: RYDInformeService, protected navService: NavService, protected dataService: DataService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.inputBalanzaComponent = new DropdownComponent();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    filtroBalanza = Array<any>();
    balanzaSelected = "";
    data: any;
    tituloArchivo = "ReporteInforme.xls";

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR INFORME"); }

    setTabs() {
        this.setMenuSeccionTab("ryd", "Informe");
    }

    ngOnInit() {
        super.ngOnInit();
        this.getFiltros();
    }

    getFiltros() {
        this.mensajeComponent.setMsgsEmpty();
        this.subscriptionDropDowns = this.service.getFiltros().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                    this.filtroBalanza = null
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                    this.filtroBalanza = null
                } else {
                    this.filtroBalanza = result.data;
                }
            },
            error => {
                this.mensajeComponent.setErrorMsg(error.message);
                this.filtroBalanza = null
            }
        );
    }

    setFiltroBalanza(balanza: string) {
        this.balanzaSelected = balanza;
        this.getData();
    }

    getData() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = undefined;
        this.unsubscribe();
        this.subscription = this.service.getInforme(this.balanzaSelected).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.data = result.data;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
            
        );
        return false;
    }

    exportExcel() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.exportExcel(this.balanzaSelected).subscribe(
            result => {
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
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivo);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivo;
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

    isVisible() {
        return this.data != undefined && this.data.pesadas.length != 0;
    }

}