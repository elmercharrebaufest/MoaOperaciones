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
import { EcheqService } from './echeq.service';
import { ActivatedRoute, Router } from '@angular/router';



@Component({
    selector: 'app-echeq',
    template: ``,
    providers: [EcheqService]
})
export class EcheqBaseComponent extends ListBaseComponent {

    constructor(protected service: EcheqService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    checkPermisos() {
        this.securityService.tienePermisoRedirect("VER ECHEQ");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();

        sessionStorage.getItem("proveedor");

        this.navService.setSeccionList([
            new Seccion('/echeq/gestion', 'echeq', 'Gestion'),
            new Seccion('/echeq/mis-echeq', 'echeq', 'Mis Echeq')
        ]);    
    }

   
}
