import { Component, OnInit, ViewChild } from '@angular/core';
import { LiquidacionService } from './../liquidacion.service';
import { LiquidacionBaseComponent } from './../liquidacion.component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { Seccion } from './../../common/models/seccion';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-liquidacion-no-granos',
    template: ``,
    providers: [LiquidacionService]
})
export class LiquidacionNGBaseComponent extends LiquidacionBaseComponent {

    constructor(protected service: LiquidacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONSULTAR LIQUIDACIONES NG");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/comprobante-ngs/registrado', 'comprobante-ngs', 'Registrados') , new Seccion('/comprobante-ngs/pendiente-registro', 'comprobante-ngs', 'Pendientes de registro')/*, new Seccion('/comprobante-ngs/paga', 'comprobante-ngs', 'Pagos')*/]);
        this.getData();
    }
}