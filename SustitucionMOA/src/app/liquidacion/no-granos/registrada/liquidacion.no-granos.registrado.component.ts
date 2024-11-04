import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionNGRegistradoService } from '../../liquidacion.service';
import { LiquidacionNGBaseComponent } from '../liquidacion.no-granos.component';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { NavService } from '../../../common/services/NavService';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { SecurityService } from '../../../common/services/SecurityService';
import { ModalService } from '../../../common/services/ModalService';
import { TipoPeriodo } from '../../../common/enums/TipoPeriodo';

@Component({
    selector: 'app-liquidacion-no-granos-registrado',
    templateUrl: `liquidacion.no-granos.registrado.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionNGRegistradoService }]
})
export class LiquidacionNGRegistradoComponent extends LiquidacionNGBaseComponent {

    constructor(protected service: LiquidacionNGRegistradoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroEstados: string= "Todos";
    
    tituloArchivo = "ReporteComprobantesRegistrados.xls";

    filtroFechaPeriodoDefault: TipoPeriodo = TipoPeriodo.UltimoMes;
    filtroFechaKey: string = 'NGComprReg_Periodo'

    setTabs() {
        this.setMenuSeccionTab("comprobante-ngs", "Registrados");
    }
}