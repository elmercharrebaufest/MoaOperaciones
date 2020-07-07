import { Component, OnInit, ViewChild } from '@angular/core';
import { PagoService, PagoEmitidoService } from './../pago.service';
import { PagoComponent } from './../pago.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Component({
    selector: 'my-app',
    templateUrl: `./app/pago/emitido/pago.emitido.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: PagoService, useClass: PagoEmitidoService }]
})
export class PagoEmitidoComponent extends PagoComponent {

    constructor(protected service: PagoEmitidoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReportePagosEmitidos.xls";

    setTabs() {
        this.setMenuSeccionTab("pago", "Emitidos");
    }

    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONSULTAR PAGOS");
    }

    showModalTableResponsive(pago: any) {
        this.modalService.openModalTableResponsive("Pago", [
            { etiqueta: "Fecha de Acreditación", valor: pago.facreditacion },
            { etiqueta: "ID Pago", valor: pago.idPago },
            { etiqueta: "Total Mercadería", valor: pago.totalMercaderiaString },
            { etiqueta: "Total IVA", valor: pago.ivaString },
            { etiqueta: "Retenciones", valor: pago.retencionString },
            { etiqueta: "Neto Acreditado", valor: pago.montoString }
        ]);
        return false;
    }

    isEnProceso(value: string) {
        return value.toLowerCase() == "en proceso";
    }
}