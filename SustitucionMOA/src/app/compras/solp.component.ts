import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { Paso } from '../common/models/paso';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { SessionDataService } from '../common/services/SessionDataService';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { StepperComponent } from '../common/view-child/stepper/stepper.component';

@Component({
    selector: 'app-solp',
    templateUrl: './solp.component.html',
})
export class SolpComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;
    
    pasoActual: Paso;
    pasos:Paso[] = [{
        Codigo: 'PliegoGeneracion1',
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Numero: 1
    },
    {
        Codigo: 'PliegoGeneracion2',
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Numero: 2
    },
    {
        Codigo: 'PliegoEspecificacion',
        Nombre: 'Especificaciones técnicas',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Numero: 3
    },
    {
        Codigo: 'PliegoCotizacion',
        Nombre: 'Cotización y plazo de ejecución',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Numero: 4
    },
    {
        Codigo: 'SolpCabecera',
        Nombre: 'Cabecera',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Numero: 5
    },
    {
        Codigo: 'SolpeSubposiciones',
        Nombre: 'Subposiciones',
        Activo: false,
        Completo: false,
        Iniciado: false,
        Numero: 6
    }];

    ngOnInit() {
        if(this.pasos && this.pasos.length > 0){
            this.pasos[0].Activo = true;
            this.pasos[0].Iniciado = true;

            this.pasoActual = this.pasos[0];
        }
    }

    cambioPaso(paso){
        this.pasos.forEach((p,i) => {
            if(p.Codigo == this.pasoActual.Codigo){
                p.Activo = false;
                p.Iniciado = true;
                //p.Completo = true;
            }else if(p.Codigo == paso.Codigo){
                p.Iniciado = true;
                p.Activo = true;
            }
        });

        this.pasoActual = paso;
    }
}
