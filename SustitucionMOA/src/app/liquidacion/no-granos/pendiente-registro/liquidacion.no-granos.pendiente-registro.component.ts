import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionNGPendienteRegistroService } from '../../liquidacion.service';
import { LiquidacionNGBaseComponent } from '../liquidacion.no-granos.component';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { NavService } from '../../../common/services/NavService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { SecurityService } from '../../../common/services/SecurityService';
import { ModalService } from '../../../common/services/ModalService';
import { TipoPeriodo } from '../../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-liquidacion-no-granos-pendiente-registro',
    templateUrl: `liquidacion.no-granos.pendiente-registro.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionNGPendienteRegistroService }]
})
export class LiquidacionNGPendienteRegistroComponent extends LiquidacionNGBaseComponent {

    constructor(protected service: LiquidacionNGPendienteRegistroService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroEstados: string= "Todos";
    tituloArchivo = "ReporteComprobantesPendienteRegistro.xls";
    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimoMes;
    filtroFechaKey: string = 'NGComprPend_Periodo'

    setTabs() {
        this.setMenuSeccionTab("comprobante-ngs", "Pendientes de registro");
    }
}