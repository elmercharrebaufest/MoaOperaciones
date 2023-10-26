import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService } from './liquidacion.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { ModalService } from './../common/services/ModalService';



@Component({
    selector: 'app-liquidacion',
    template: ``,
    providers: [LiquidacionService]
})
export class LiquidacionBaseComponent extends ListBaseComponent {

    constructor(protected service: LiquidacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroProducto: any = null;
    productoSelected: string = "";
    filtroObservacion: any = null;
    filtroVendedor: any = null;
    observacionSelected: string = "";
    filtroComprobanteOContrato: string = "";
    tituloArchivoModal: string = "";
    tituloArchivoPDF = "Documento"
    CodigoProveedorSAP: string = sessionStorage.getItem("proveedor");


    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONSULTAR LIQUIDACIONES");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        sessionStorage.getItem("proveedor");
        if (this.securityService.tienePermiso("INFORMAR LIQUIDACION")) {
            this.navService.setSeccionList([
                new Seccion('/liquidacion/informar', 'liquidacion', 'Informar'),
                new Seccion('/liquidacion/informada', 'liquidacion', 'Informadas'),
                new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'),
                new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'),
                new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas'),
                
               
            ]);
        }
        else {
            this.navService.setSeccionList([
                new Seccion('/liquidacion/informada', 'liquidacion', 'Informadas'),
                new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'),
                new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'),
                new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas'),
  
            ]);
        }
        this.getData();
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
    }


    setFiltroVendedor(vendedor: string) {
        this.observacionSelected = vendedor;
    }

    isVisible(): boolean {
        if ((this.data && this.data.liquidaciones && this.data.liquidaciones.length != 0) || (this.data && this.data.comprobantes && this.data.comprobantes.length != 0))
            return true;
        else
            return false;
    }

    showModal(contrato: string, secuencia: string, comprobante: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.getVinculacion(contrato, secuencia).subscribe(
            (result: any) => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.floatMsgService.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.floatMsgService.setInfoMsg(result.info);
                } else {
                    //this.data = result.data;
                    this.modalService.openModalLiquidacion("Cartas de Portes - Comp.: " + comprobante, result.data.data, contrato, secuencia, comprobante);
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
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(
            (result: any) => {
                this.spinnerComponent.hideIt();
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
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + documento + ".pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoPDF + documento + ".pdf"
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

    descargarComprobanteNGPDF(CodigoProveedorSAP: string, FechaDocumento: string, NumeroLegalDocumento: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarComprobanteNGPDF(CodigoProveedorSAP, FechaDocumento, NumeroLegalDocumento).subscribe(
            (result: any) => {
                this.spinnerComponent.hideIt();
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
                        window.navigator.msSaveOrOpenBlob(blob, this.tituloArchivoPDF + NumeroLegalDocumento + ".pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = this.tituloArchivoPDF + NumeroLegalDocumento + ".pdf"
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

    protected vaciarFiltros() {
        this.filtroProducto = null;
        this.filtroObservacion = null;
        this.filtroComprobanteOContrato = "";
        this.productoSelected = "";
        this.observacionSelected = "";
    }

    protected cargarFiltrosVariables(result: any) {
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
        if (result.filtroVendedor != undefined) this.filtroObservacion = result.filtroObservacion.options;
    }
}
