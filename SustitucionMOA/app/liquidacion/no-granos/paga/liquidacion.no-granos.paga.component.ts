import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService, LiquidacionNGPagaService } from './../../liquidacion.service';
import { LiquidacionNGBaseComponent } from './../liquidacion.no-granos.component';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Component({
    selector: 'app-liquidacion-no-granos-paga',
    templateUrl: `./app/liquidacion/no-granos/paga/liquidacion.no-granos.paga.component.html?v=${new Date().getTime()}`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionNGPagaService }]
})
export class LiquidacionNGPagaComponent extends LiquidacionNGBaseComponent {

    constructor(protected service: LiquidacionNGPagaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    tituloArchivo = "ReporteComprobantesPagos.xls";

    setTabs() {
        this.setMenuSeccionTab("comprobante-ngs", "Pagos");
    }

}