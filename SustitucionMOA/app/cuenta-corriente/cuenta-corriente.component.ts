import { Component } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { Seccion } from './../common/models/Seccion';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { CuentaCorrienteService } from './cuenta-corriente.service';

@Component({
    selector: 'app-cuenta-corriente',
    templateUrl: `./app/cuenta-corriente/cuenta-corriente.component.html?v=${new Date().getTime()}`,
    providers: [CuentaCorrienteService]

})
export class CuentaCorrienteBaseComponent extends ListBaseComponent {

    constructor(protected service: CuentaCorrienteService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReporteCuentasCorrientes.xls";
    tituloArchivoPDF = "Documento"

    filtroNroCteContrato: string = "";
    filtroNroLegal: string = "";

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR CUENTA CORRIENTE"); }

    contrato: string = "";
    pago: string = "";
    retencion: string = "";

    setTabs() {
        this.setMenuSeccionTab("cuenta-corriente", "Cuenta Corriente");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList(
            [
                new Seccion('/cuenta-corriente/simple', 'cuenta-corriente', 'Cuenta Corriente'),
                new Seccion('/cuenta-corriente/agrupada', 'cuenta-corriente', 'Detalle de pagos'),
            ]
        );

        this.orderedByColumn = "orden";
        this.orderDirection = 1;
        this.getData();
    }

    getData() {
        this.data = null;
        this.vaciarFiltros();
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.forEach((params: Params) => {
            this.getInfoParam(params['id']);
            this.unsubscribe();
            this.subscription = this.service.getData(this.filtroFechaComponent.periodo, this.filtroFechaComponent.fecha_inicio, this.filtroFechaComponent.fecha_fin, this.contrato, this.pago, this.retencion).subscribe(
                result => {
                    this.data = null;
                    this.mensajeComponent.setMsgsEmpty();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                        this.cargarFiltrosVariables(result);
                        if (result.data.msj != undefined) {
                            this.mensajeComponent.setInfoMsg(result.data.msj);
                        }
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        });
        return false;
    }

    descargaPDF(documento: string, ejercicio: string) {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        this.subscription = this.service.descargarDocumentoPDF(documento, ejercicio).subscribe(
            result => {
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

    isVisible(): boolean {
        if (this.data && this.data.cuentasCorrientes.length != 0)
            return true;
        else
            return false;
    }

    protected vaciarFiltros() {
        this.filtroNroCteContrato = "";
        this.filtroNroLegal = "";
        this.orderedByColumn = "orden";
        this.orderDirection = 1;
    }

    showModalTableResponsive(cuentaCorriente: any) {
        this.modalService.openModalTableResponsive("Detalle de Movimiento", [
            { etiqueta: "F. Emisión", valor: cuentaCorriente.docDate },
            { etiqueta: "F. Vto.", valor: cuentaCorriente.fecVto },
            { etiqueta: "Nº Cte.", valor: cuentaCorriente.docNo },
            { etiqueta: "Descripción", valor: cuentaCorriente.descripcion },
            { etiqueta: "Contrato", valor: cuentaCorriente.contrato },
            { etiqueta: "Moneda", valor: cuentaCorriente.moneda },
            { etiqueta: "TC", valor: cuentaCorriente.ukursString },
            { etiqueta: "Debe", valor: cuentaCorriente.debeString },
            { etiqueta: "Haber", valor: cuentaCorriente.haberString }
        ]);
        return false;
    }

    getInfoParam(value: string) {
        if (value != null && value != undefined && value != "") {
            if (value.length == 2) {
                this.retencion = value;
            } else {
                try {
                    var valueInt = Number(value);
                    var valueString = valueInt.toString();
                    var cantCeros = 4 - valueString.length;
                    var pagoFormat = "";
                    for (var i = 0; i < cantCeros; i++) {
                        pagoFormat += "0";
                    }
                    pagoFormat += valueString
                    this.pago = pagoFormat;
                } catch{ }
            }
        }
    }
}