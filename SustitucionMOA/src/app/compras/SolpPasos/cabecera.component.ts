import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SolpService } from './../solp.service'
import { PosicionSolp, Solp } from './../Solp';
import { SelectItem } from 'primeng/api';
// import {FormBuilder, FormGroup, FormControl,Validators } from '@angular/forms';

declare var $: any;

@Component({
    selector: 'cabecera',
    templateUrl: `cabecera.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class CabeceraComponent extends ListBaseComponent {

    @Input('model') 
    protected model:Solp;

    @Input('locale') 
    protected locale:any;
    

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    // Declaro las variables
    claseDocumento: SelectItem[];
    centroEntrega: SelectItem[];
    monedaCompras: SelectItem[];
    articuloCompras: SelectItem[];
    solicitanteCompras: SelectItem[];
    grupoCompras: SelectItem[];
    almacenEntrega: SelectItem[];
    posiciones: SelectItem[];
    resultadoProveedores: string[]; 
    fechaEntregaServicio: any;
    fechaDeLiberacion: any;
    hoy: Date = new Date();
    selectPosicion: any; 
    // formulario5: FormGroup;

    
    // Funcion que crea el chips y setea el evento
    onKeyUp(event: KeyboardEvent, texts: string[]) {
      if (event.key == "Enter") {
       let tokenInput = event.srcElement as any;
       if (tokenInput.value) {
        texts.push(tokenInput.value);
        tokenInput.value = "";
       }
      }
    }  

    // Funcion que hace la lista para el autocomplete
    search(event){
        let query = event.query;
        this.resultadoProveedores = [];
    }


    setTabs() {
        this.setMenuSeccionTab("Cabecera", "Cabecera");
    }

    ngOnInit() {
        this.setTabs();
        this.claseDocumento = [
                    { label: "ZSP1 - Mantenimiento mecánico", value: "zsp1" },
                    { label: "ZSP2 - Mantenimiento electrico", value: "zsp2" }
        ];     

        this.centroEntrega = [
            { label: "Centro 1", value: "C1" },
            { label: "Centro 2", value: "C2" }
        ];

        this.almacenEntrega = [
            { label: "Almacen 1", value: "A1" },
            { label: "Almacen 2", value: "A2" }
        ];

        this.grupoCompras = [
            { label: "Materiales", value: "Mat" },
            { label: "Servicios", value: "Ser" }
        ];

        this.solicitanteCompras = [
            { label: "Mariano", value: "Mar" },
            { label: "Alberto", value: "Alb" }
        ];

        this.articuloCompras = [
            { label: "Piedra", value: "Pie" },
            { label: "Papel", value: "Pap" }
        ];

        this.monedaCompras = [
            { label: "ARS", value: "$" },
            { label: "DOL", value: "U$" }
        ];

    // // }
        // //declaro las validaciones para los campos
        // this.formulario5 = this.formBuilder.group({​​​​​​​​
        // textoGenerico: new FormControl ('', Validators.required)
        // }​​​​​​​​);
    }


}
