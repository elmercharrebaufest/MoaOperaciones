import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionNGAprobadaService } from './../../liquidacion.service';
import { LiquidacionNGBaseComponent } from './../liquidacion.no-granos.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';



@Component({
    selector: 'app-liquidacion-no-granos-aprobada',
    templateUrl: `./app/liquidacion/no-granos/aprobada/liquidacion.no-granos.aprobada.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionNGAprobadaService }]
})
export class LiquidacionNGAprobadaComponent extends LiquidacionNGBaseComponent {

    constructor(protected service: LiquidacionNGAprobadaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReporteComprobantesAprobados.xls";

    setTabs() {
        this.setMenuSeccionTab("comprobante-ngs", "Aprobados");
    }

    showModal() { return false; }

}