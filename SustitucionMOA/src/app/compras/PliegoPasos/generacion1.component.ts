import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SolpService } from './../solp.service'
import { Solp } from './../Solp';

declare var $: any;

@Component({
    selector: 'generacion1',
    templateUrl: `generacion1.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class Generacion1Component extends ListBaseComponent {

    @Input('model') 
    protected model:Solp;

    @Input('locale') 
    protected locale:any;


    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }


    nombreDeObra: string = "";
    solpPaso1Result: any;
    fechaEntrega: any;
    horaEntrega: any;

    
    parsearFecha () {
        this.fechaEntrega = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
    if(this.fechaEntrega != '' && this.fechaEntrega != null && this.horaEntrega != '' && this.horaEntrega != null){
        var dateParts = this.fechaEntrega.split("-");
        this.model.fechaDeEntregaDeOfertasFecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2], this.horaEntrega);
        }
        console.log(this.model.fechaDeEntregaDeOfertasFecha, "No funciona");

    }


    setTabs() {
        this.setMenuSeccionTab("Generacion1", "Generacion1");
    }

    ngOnInit() {
        this.setTabs();
        if(!this.model.mail)
            this.model.mail = sessionStorage.getItem("username");
        this.model.horaEntrega = new Date(1,1,1,10,0,0,0);
        this.model.fechaEntrega = new Date(2021,1,1);
    }


}
