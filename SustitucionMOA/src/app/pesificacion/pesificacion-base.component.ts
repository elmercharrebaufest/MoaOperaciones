import { Component } from '@angular/core';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { Seccion } from './../common/models/seccion';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { PesificacionService } from './pesificacion.service';

@Component({
    selector: 'app-pesificacion-base',
    template: ``,
})
export class PesificacionBaseComponent extends ListBaseComponent {

    constructor(protected service: PesificacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.setTabs();
        this.navService.setSeccionList(
            [
                new Seccion('/pesificacion', 'pesificacion', 'Carga'),
                new Seccion('/pesificacion/listado', 'pesificacion', 'Listado'),
            ]
        );
    }
}