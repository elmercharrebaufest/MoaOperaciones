import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../../common/base-components/list-base-component'
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { NavService } from './../../../common/services/NavService';
import { SelectItem } from 'primeng/api';
import { AbstractControl, FormGroup, ValidationErrors } from '@angular/forms';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { EnumPasoSolp } from '../../enum-paso-solp';
import { ComprasService } from '../../compras.service';

declare var $: any;

@Component({
    selector: 'subPosicion',
    templateUrl: `subPosicion.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class SubPosicionComponent extends ListBaseComponent {

    @Input('combos')
    protected combos: any;

  

    @Input('locale')
    protected locale: any;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router, private validadorPasoSolpService: ValidadorPasoSolpService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

    }

    cars: any[];
    setTabs() {
        this.setMenuSeccionTab("Cabecera", "Cabecera");
    }

    ngOnInit() {
        this.setTabs();

        this.cars = [{
            vin: "hola",
            year: "hola",
            brand: "hola",
            color: "hola"
        }]


    }


}
