import { Component, OnInit, ViewChild } from '@angular/core';
import { PagoService, PagoEmitidoNGService } from './../../pago.service';
import { PagoComponent } from './../../pago.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { ModalService } from './../../../common/services/ModalService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { NavService } from './../../../common/services/NavService';
import { SecurityService } from './../../../common/services/SecurityService';
import { Seccion } from './../../../common/models/seccion';
import { TipoPeriodo } from '../../../common/enums/TipoPeriodo';



@Component({
    selector: 'app-pago-no-granos-emitido',
    templateUrl: `pago.no-granos.emitido.component.html`,
    providers: [{ provide: PagoService, useClass: PagoEmitidoNGService }]
})
export class PagoEmitidoNGSComponent extends PagoComponent {

    constructor(
        protected service: PagoEmitidoNGService,
        protected navService: NavService,
        protected sessionDataService: SessionDataService,
        protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService
    )
    {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReportePagosEmitidos.xls";
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimoMes;
    filtroFechaKey: string = 'NGPagList_Periodo';

    setTabs() {
        this.setMenuSeccionTab("pago-ngs", "Emitidos");
    }

    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONSULTAR PAGOS NG");
    }

    showModalTableResponsive(pago: any) {
        this.modalService.openModalTableResponsive("Pago", [
            { etiqueta: "Fecha de Pago", valor: pago.fechaPago },
            { etiqueta: "Número Pago", valor: pago.numeroPago },
            { etiqueta: "Vía de Pago", valor: pago.viaPago },
            { etiqueta: "Monto Comprobantes", valor: pago.totalMercaderiaString },
            { etiqueta: "Retenciones", valor: pago.retencionString },
            { etiqueta: "Neto Acreditado", valor: pago.montoString }
        ]);
        return false;
    }
}