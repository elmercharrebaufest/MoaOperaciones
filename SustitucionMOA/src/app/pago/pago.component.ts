import { Component, OnInit, ViewChild } from '@angular/core';
import { PagoService } from './pago.service';
import { SessionDataService } from './../common/services/SessionDataService';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ModalService } from './../common/services/ModalService';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';



@Component({
    selector: 'app-pago',
    template: ``,
    providers: [PagoService]
})
export class PagoComponent extends ListBaseComponent {

    constructor(protected service: PagoService, protected navService: NavService, protected sessionDateService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDateService, securityService, floatMsgService, modalService);
    }

    filtroID: string = "";
    filtroNroPago: string = "";


    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        this.getData();
    }

    isVisible(): boolean {
        if (this.data && this.data.pagos.length != 0)
            return true;
        else
            return false;
    }

    showModalComprobantes(documento: string, fecha: string, fiscYear: string) {
        this.unsubscribe();
        this.floatMsgService.setMsgsEmpty();
        this.subscription = this.service.getComprobantes(documento, fecha, fiscYear).subscribe(
            (result:any) => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    //this.data = result.data;
                    this.modalService.openModalComprobante("Comprobantes de Pago", result.comprobantes);
                }
            },
            error => {
                // this.spinnerComponent.hideIt();
                this.floatMsgService.setErrorMsg(error.message);
            }

        );
        return false;
    }

    descargaPDF(documento: string, ejercicio: string) {
        let tituloArchivoPDF = "Documento-";
        this.unsubscribe();
        this.mensajeComponent.setMsgsEmpty();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(
            (result:any) => {
                //this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    //this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    //this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    var byteArray = new Uint8Array(result.data);
                    var blob = new Blob([byteArray], { type: 'application/pdf' });
                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, tituloArchivoPDF + documento + ".pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = tituloArchivoPDF + documento + ".pdf"
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            error => {
                //this.spinnerComponent.hideIt();
                //this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;
    }

    protected vaciarFiltros() {
        this.filtroID = "";
        this.filtroNroPago = "";
    }

    isNegative(valor: number) {
        return valor < 0;
    }
}