import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDService } from './ryd.service';
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
//import { PaginationControlsCustomComponent } from './../common/view-child/pagination/pagination.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';



@Component({
    selector: 'app-ryd',
    template: ``,
    providers: [RYDService]
})
export class RYDBaseComponent extends BaseComponent implements OnInit {

    constructor(protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/ryd/carga-pesada', 'ryd', 'Carga de Pesadas'), new Seccion('/ryd/informe', 'ryd', 'Informe'), new Seccion('/ryd/listado-pesadas', 'ryd', 'Listado de Pesadas')]);
    }
}