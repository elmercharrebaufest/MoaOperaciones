import { Component, OnInit, ViewChild, Renderer, ElementRef, AfterViewInit } from '@angular/core';
import { SessionDataService } from './../../common/services/SessionDataService';
//import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
//import { ListBaseComponent } from './../common/base-components/list-base-component'
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SecurityService } from './../../common/services/SecurityService';
import { EcheqService } from '../echeq.service';
import { EcheqBaseComponent } from '../echeq.component';



@Component({
    selector: 'app-echeq-gestion',
    templateUrl: `echeq.gestion.component.html`,
    providers: [{ provide: EcheqService}]
})
export class EcheqGestionComponent extends EcheqBaseComponent {


    constructor(protected service: EcheqService, protected navService: NavService, protected sessionDataService: SessionDataService, protected elementRef: ElementRef, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    } 

    setTabs() {
        this.setMenuSeccionTab("echeq", "Gestion");
    }
   
}