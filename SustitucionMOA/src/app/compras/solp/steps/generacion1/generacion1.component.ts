import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { ListBaseComponent } from '../../../../common/base-components/list-base-component'
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { NavService } from '../../../../common/services/NavService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { ComprasService } from '../../../compras.service'
import { Solp } from '../../solp';
import { ValidadorPasoSolpService } from '../../../validadorPasoSolpService';
import { EnumPasoSolp } from '../../../enum-paso-solp';

declare var $: any;

@Component({
    selector: 'generacion1',
    templateUrl: `generacion1.component.html`,
    styleUrls: ['../../../compras.component.css'],
})

export class Generacion1Component extends ListBaseComponent  {

    @Input('model')
    protected model: Solp;

    @Input('locale')
    protected locale: any;

    @Output() onEstCompleto = new EventEmitter<any>();

    //validaciones
    formulario: FormGroup;

    nombreDePedido: string = "";
    solpPaso1Result: any;
    fechaEntrega: any;
    horaEntrega: any;
    fechaEditable: boolean;
    hoy: Date = new Date();


    camposObligatorios: any[] = [
        { campo: 'nombreDePedido', esObligatorio: true},
        { campo: 'fiscalContrato', esObligatorio: true},
        { campo: 'mail', esObligatorio: true},
    ];

    constructor(protected service: ComprasService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securityService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private formBuilder: FormBuilder,
        private validadorPasoSolpService : ValidadorPasoSolpService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }


    ngOnInit() {
        this.setTabs();

        if (this.model.mail === undefined) {     //if (!this.model.mail || this.model.mail === undefined) {         
            this.model.mail = sessionStorage.getItem("username");
        };

        //declaro las validaciones para los campos
        this.formulario = this.formBuilder.group({
            nombreDePedido: new FormControl({value : ""}, Validators.compose([Validators.required])),
            fiscalContrato: new FormControl('', Validators.required),
            mail: ['', [Validators.pattern('[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}')]]
        });
        
        this.validadorPasoSolpService.formulario = this.formulario;

        if (this.model.cargoPasoUno) {
            this.validadorPasoSolpService.aplicarValidaciones()
        }
        this.model.cargoPasoUno = true;
    }

    ngOnDestroy()
    {
        super.ngOnDestroy();
        this.onEstCompleto.emit({codigo :EnumPasoSolp.PliegoGeneracion1, esPasoInvalido : this.validadorPasoSolpService.esPasoInvalido()});
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

    mostrarValidacion(campoAValidar, vacio){
        let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
        if(vacio !== undefined) {
            return (camposVacios != null && vacio == 0);
        }
        return true;
    }

    onBlur(control: string)
    {
        this.validadorPasoSolpService.onBlurDirty(control);
    }

    public get configurarFechaCalendar(): boolean {

        if(this.model.trabajoHecho){
            return true;
        }

        if(this.model.trabajoHecho && this.model.urgencia){
            return true;
        }

        if(this.model.trabajoHecho && this.model.adicional){
            return true;
        }
        
        return false;
    }

      
}
