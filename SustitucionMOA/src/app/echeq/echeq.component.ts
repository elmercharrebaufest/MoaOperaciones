import { Component } from '@angular/core';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { Seccion } from './../common/models/seccion';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { EcheqService } from './echeq.service';

@Component({
    selector: 'app-echeq',
    template: ``,
    providers: [EcheqService]
})
export class EcheqComponent extends ListBaseComponent {

    constructor(protected service: EcheqService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("VER ECHEQ"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();

        this.navService.setSeccionList(
            [
                new Seccion('/echeq/gestion', 'echeq', 'Gestion'),
                new Seccion('/echeq/mis-echeq', 'echeq', 'Mis Echeq'),
           
            ]);

        // this.getData();
    }

}