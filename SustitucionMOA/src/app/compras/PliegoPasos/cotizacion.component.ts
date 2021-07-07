import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { ComprasService } from '../compras.service'
import { Solp } from './../Solp';
import { WeekDay } from '@angular/common';
import { FormBuilder, FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { ValidadorPasoSolpService } from '../validadorPasoSolpService';
import { EnumPasoSolp } from '../enum-paso-solp';


declare var $: any;

@Component({
    selector: 'cotizacion',
    templateUrl: `cotizacion.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class CotizacionComponent extends ListBaseComponent {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    //validaciones
    formularioCotizacion: FormGroup;

    @Output() onEstCompleto = new EventEmitter<any>();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService,
        protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router
        , private formBuilder: FormBuilder,
        private validadorPasoSolpService : ValidadorPasoSolpService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);


    }

   

    mostrarError(nombreCampo: string): boolean {
        if (this.formularioCotizacion && this.formularioCotizacion.controls) {
            return (this.formularioCotizacion.controls[nombreCampo].invalid || (this.formularioCotizacion.controls[nombreCampo].errors && this.formularioCotizacion.controls[nombreCampo].errors.required))
                && (this.formularioCotizacion.controls[nombreCampo].dirty || this.formularioCotizacion.controls[nombreCampo].touched)
        }

        return false;
    }

    validatorDias(control: AbstractControl): { [key: string]: boolean } | null {
        let diasNoSeleccionados = 0;
        for (let index = 0; index < control.value.length; index++) {
            const dia =  control.value[index];
            if (!dia.selected) {
                diasNoSeleccionados = diasNoSeleccionados + 1;
            }
        }

        if (control.value.length == diasNoSeleccionados) {
            return { 'requerid': true };
        }

        return null;
    }

    ejecucion: number;
    comienzoJornadaLaboral: Date;
    terminoJornadaLaboral: Date;


    setTabs() {
        this.setMenuSeccionTab("Cotizacion", "Cotizacion");
    }

    ngOnInit() {
        this.setTabs();

        //declaro las validaciones para los campos
        this.formularioCotizacion = this.formBuilder.group({
            ejecucion: new FormControl('', [Validators.required]),
            comienzoJornadaLaboral: new FormControl('', Validators.required),
            terminoJornadaLaboral: new FormControl('', Validators.required),
            dias: new FormControl(this.model.jornadaLaboralDias, [Validators.required,this.validatorDias])
        });

        this.validadorPasoSolpService.formulario = this.formularioCotizacion;
        if (this.model.cargoPasoCuatro) {
            this.validadorPasoSolpService.aplicarValidaciones();
        }

        this.model.cargoPasoCuatro = true;

    }

    
    ngOnDestroy()
    {
        super.ngOnDestroy();
        this.onEstCompleto.emit({codigo :EnumPasoSolp.PliegoCotizacion, esPasoInvalido : this.validadorPasoSolpService.esPasoInvalido()});
    }


}
