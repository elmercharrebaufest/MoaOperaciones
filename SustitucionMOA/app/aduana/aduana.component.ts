import { Component, OnInit, ViewChild } from '@angular/core';
import { AduanaService } from './aduana.service';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';

@Component({
    selector: 'my-app',
    template: ``,
    providers: [ AduanaService ]
})
export class AduanaBaseComponent extends BaseComponent implements OnInit {


    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }

    checkPermisos() { }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([
            new Seccion('/aduana/pesada-online', 'aduana', 'Pesada Online'),
            new Seccion('/aduana/pesada-historica', 'aduana', 'Pesada Histórica'),
            new Seccion('/aduana/informacion-meteorologica', 'aduana', 'Inf. Meteorológica'),
            new Seccion('/aduana/camara-muelle', 'aduana', 'Cámaras Muelle'),
            new Seccion('/aduana/camara-consolidacion', 'aduana', 'Cámaras Consolidación')]);
    }
}