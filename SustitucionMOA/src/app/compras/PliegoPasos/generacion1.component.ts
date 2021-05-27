import { AfterContentChecked, AfterViewChecked, AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SolpService } from './../solp.service'
import { Solp } from './../Solp';
import { SelectItem } from 'primeng/api';
import { IValidadorPasoSolp } from '../IValidadorPasoSolp';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { forEach } from '@angular/router/src/utils/collection';

declare var $: any;

@Component({
    selector: 'generacion1',
    templateUrl: `generacion1.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class Generacion1Component extends ListBaseComponent implements IValidadorPasoSolp, AfterViewInit {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    @Input()
    protected pasoIniciado: boolean;

    //validaciones
    formulario: FormGroup;

    nombreDeObra: string = "";
    solpPaso1Result: any;
    fechaEntrega: any;
    horaEntrega: any;
    hoy: Date = new Date();

    constructor(protected service: SolpService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private formBuilder: FormBuilder) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    ngAfterViewInit(): void {
       
    }

    ngOnInit() {
        this.setTabs();
        if (!this.model.mail)
            this.model.mail = sessionStorage.getItem("username");

        //declaro las validaciones para los campos
        this.formulario = this.formBuilder.group({
            nombreDeObra: new FormControl('', [Validators.required,Validators.minLength(5)]),
            fiscalContrato: new FormControl('', Validators.required),
            mail: new FormControl('', Validators.required)
        });
    }


    esPasoInvalido(): boolean {
        this.aplicarValidaciones();
        return this.formulario.invalid;
    }

    aplicarValidaciones(): void {

        Object.keys(this.formulario.controls).forEach(key => {
            let control = this.formulario.get(key);
            control.markAsDirty();
            control.updateValueAndValidity();
          });


        // if (this.pasoIniciado) {
        //     this.formulario.controls["nombreDeObra"].markAsDirty()
        //     this.formulario.controls["fiscalContrato"].markAsDirty()
        //     this.formulario.controls["mail"].markAsDirty()
        //     this.formulario.controls["nombreDeObra"].updateValueAndValidity();
        //     this.formulario.controls["fiscalContrato"].updateValueAndValidity();
        //     this.formulario.controls["mail"].updateValueAndValidity();
        // }
    }



    parsearFecha() {
        this.fechaEntrega = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
        if (this.fechaEntrega != '' && this.fechaEntrega != null && this.horaEntrega != '' && this.horaEntrega != null) {
            var dateParts = this.fechaEntrega.split("-");
            this.model.fechaDeEntregaDeOfertasFecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2], this.horaEntrega);
        }
        console.log(this.model.fechaDeEntregaDeOfertasFecha, "No funciona");

    }


    setTabs() {
        this.setMenuSeccionTab("Generacion1", "Generacion1");
    }

    mostrarError(nombreCampo: string): boolean {
        if (this.formulario && this.formulario.controls) {
            return (this.formulario.controls[nombreCampo].invalid || (this.formulario.controls[nombreCampo].errors && this.formulario.controls[nombreCampo].errors.required))
                && (this.formulario.controls[nombreCampo].dirty || this.formulario.controls[nombreCampo].touched)
        }

        return false;
    }


}
