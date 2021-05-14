import { Component, ElementRef, Input, ViewChild } from '@angular/core';
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

    @Input('model') 
    protected model:Solp;

    @Input('locale') 
    protected locale:any;


  

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }


    solpPaso2Result: any;
    fechaEntrega: any;
    horaEntrega: any;
    visitaDeObraHora: any;
    visitaDeObraFecha: any;
    fechaLimiteHora: any;
    fechaLimiteFecha: any;


    //variables auxiliares de text rich
    posicionDeInicioInsert: number = 0;


    parsearFecha() {
        this.fechaEntrega = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
        if (this.fechaEntrega != '' && this.fechaEntrega != null && this.horaEntrega != '' && this.horaEntrega != null) {
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

        if (this.listaVisitas.length == 0) {
            this.agregarNuevaVisita();
        }

    }


    setTabs() {
        this.setMenuSeccionTab("Generacion2", "Generacion2");
    }

    ngOnInit() {
        this.setTabs();
        this.model.fechaLimiteHora = new Date(1,1,1,10,0,0,0);
        this.model.fechaLimiteFecha = new Date(2021,1,1);
        
    }

    selectionChange(event) {

        if (event.range && this.model.observaciones) {
            this.posicionDeInicioInsert = this.ObtenerPosicionInsert(event.range.index, this.model.observaciones);
        }
    }

    fileChange(file) {
        if (this.posicionDeInicioInsert != undefined) {
            var textoInicial = this.model.observaciones.substring(0, this.posicionDeInicioInsert + 1);
            var textoFinal = this.model.observaciones.substring(this.posicionDeInicioInsert + 1, this.model.observaciones.length);
            this.model.observaciones = textoInicial + '<img src=' + file + '>' + textoFinal;
            this.posicionDeInicioInsert = undefined;
        }
        else {

            this.model.observaciones = this.model.observaciones + '<img src=' + file + '>';
        }
    }

    ObtenerPosicionInsert(posicion: number, texto: string) {
        let contar = false;
        for (var i = 0; i < texto.length; i++) {
            var letra = texto[i];
            if (letra == "<") {
                contar = false;
                continue
            }
            else if (letra == ">") {
                contar = true;
                continue;
            }

            if (contar) {
                posicion--;

            }

            if (posicion == 0)
                return i;
        }
    }

}
