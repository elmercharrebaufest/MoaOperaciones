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
import * as uuid from 'uuid';



declare var $: any;

@Component({
    selector: 'generacion2',
    templateUrl: `generacion2.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class Generacion2Component extends ListBaseComponent {

    @Input() model:Solp;


  

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }


    solpPaso2Result: any;
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

    listaVisitas: any[] = [
        {
            id: uuid.v4(),
            visitaDeObraFecha: "",
            visitaDeObraHora: ""
        }
    ];
    
    agregarNuevaVisita() {
        this.listaVisitas.push( 
            {
            id: uuid.v4(),
            visitaDeObraFecha: "",
            visitaDeObraHora: ""
            }
        )

    };

    eliminarVisita(id) {
        this.listaVisitas = this.listaVisitas.filter(x => x.id != id);

        if(this.listaVisitas.length == 0 ){
            this.agregarNuevaVisita();
        }
        
    }


    setTabs() {
        this.setMenuSeccionTab("Generacion2", "Generacion2");
    }

    ngOnInit() {
        this.setTabs();
    }

}
