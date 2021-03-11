import { Component, OnInit, ViewChild } from '@angular/core';
import { VentaSustentableService } from './venta-sustentable.service';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';



@Component({
    selector: 'venta-sustentable',
    template: ``,
    providers: [VentaSustentableService]
})
export class VentaSustentableBaseComponent extends BaseComponent implements OnInit {

    constructor(protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
    }
}