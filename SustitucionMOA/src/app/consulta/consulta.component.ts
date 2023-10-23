import { Component, OnInit, ViewChild } from '@angular/core';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { ModalService } from './../common/services/ModalService';
import { ConsultaService } from './consulta.service';

@Component({
    selector: 'consulta',
    template: ``,
    providers: [ConsultaService]
})
export class ConsultaBaseComponent extends ListBaseComponent {

    constructor(protected service: ConsultaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() {
        this.securityService.tienePermisoRedirect("CONTACTO MAIL");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas'), new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'),
        new Seccion('/consulta/crear-consulta-interna', 'crear-consulta-interna', 'Nueva Consulta Interna')]);
    }
}