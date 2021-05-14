import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { FleteService } from './flete.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { Seccion } from './../common/models/seccion';



@Component({
    selector: 'app-flete',
    template: ``,
    providers: [FleteService]
})
export class FleteBaseComponent extends ListBaseComponent implements OnDestroy {

    constructor(protected service: FleteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroProducto: any = null;
    filtroCCPP: string = "";
    filtroProforma: string = "";
    filtroNroLegal: string = "";
    productoSelected: string = "";
    modalServiceSusbcription: any;

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR FLETE"); }

    ngOnInit() {

        if (this.modalServiceSusbcription == undefined) {
            this.modalServiceSusbcription = this.modalService.dataGuardarFlete.subscribe(
                dataGuardarFlete => {
                    this.guardarDatosProforma(dataGuardarFlete);
                });

        }
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/flete/a-facturar', 'flete', 'Viajes A Facturar'), new Seccion('/flete/pendiente', 'flete', 'Viajes Pendientes'), new Seccion('/flete/facturado', 'flete', 'Viajes Facturados')]);
        this.getData();
    }

    setFiltroProducto(producto: string) {
        this.productoSelected = producto;
    }

    descargarPDF(proforma: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.exportPDF(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin, proforma).subscribe(
            result => {
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
                        window.navigator.msSaveOrOpenBlob(blob, "Proforma(" + proforma + ").pdf");
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = "Proforma(" + proforma + ").pdf"
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


    showModalCargaDatos(proforma: string) {
        this.floatMsgService.setMsgsEmpty();
        let viajeProforma = this.data.viajes.filter((viaje: any) => this.filterByProforma(viaje, proforma))[0];
        if (viajeProforma != undefined) {
            this.modalService.openModalFlete("Datos Proforma", viajeProforma);
        } else {
            this.floatMsgService.setErrorMsg("El valor del número de proforma no es válido");
        }
        return false;
    }

    guardarDatosProforma(viajeProforma: any) {
        this.unsubscribe();
        let file = viajeProforma.pdf;
        viajeProforma.pdf = null;
        this.subscription = this.service.guardarDatosProforma(viajeProforma, file).subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.modalService.setErrorMsjModal(result.error);
                } else if (result.info != undefined) {
                    this.modalService.setErrorMsjModal(result.error);
                } else {
                    this.modalService.setSuccessMsjModal("Los datos se han guardado correctamente");
                }
            },
            error => {
                this.modalService.setErrorMsjModal(error.message);
            }
        );
        return false;
    }

    filterByProforma(obj: any, proforma: string) {
        return obj.proforma == proforma
    }

    protected vaciarFiltros() {
        this.filtroProducto = null;
        this.filtroCCPP = "";
        this.filtroProforma = "";
        this.filtroNroLegal = "";
        this.productoSelected = "";
    }

    protected cargarFiltrosVariables(result: any) {
        if (result.filtroProducto != undefined) this.filtroProducto = result.filtroProducto.options;
    }

    isVisible() {
        return this.data && this.data.viajes && this.data.viajes.length != 0;
    }

    public ngOnDestroy() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        if (this.modalServiceSusbcription != undefined) {
            this.modalServiceSusbcription.unsubscribe();
        }
    }
}