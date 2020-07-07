import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionNGObservadaService } from './../../liquidacion.service';
import { LiquidacionNGBaseComponent } from './../liquidacion.no-granos.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Component({
    selector: 'my-app',
    templateUrl: `./app/liquidacion/no-granos/observada/liquidacion.no-granos.observada.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionNGObservadaService }]
})
export class LiquidacionNGObservadaComponent extends LiquidacionNGBaseComponent {

    constructor(protected service: LiquidacionNGObservadaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReporteComprobantesObservados.xls";

    setTabs() {
        this.setMenuSeccionTab("comprobante-ngs", "Observados");
    }
}