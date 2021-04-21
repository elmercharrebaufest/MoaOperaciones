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
    
    pasos:Paso[] = [{
        Codigo: 'PliegoGeneracion1',
        Nombre: 'Generación',
        Activo: false,
        Completo: true,
        Iniciado: true
    },
    {
        Codigo: 'PliegoGeneracion2',
        Nombre: 'Generación',
        Activo: false,
        Completo: false,
        Iniciado: true
    },
    {
        Codigo: 'PliegoEspecificacion',
        Nombre: 'Especificaciones técnicas',
        Activo: false,
        Completo: false,
        Iniciado: true
    },
    {
        Codigo: 'PliegoCotizacion',
        Nombre: 'Cotización y plazo de ejecución',
        Activo: true,
        Completo: true,
        Iniciado: true
    },
    {
        Codigo: 'SolpCabecera',
        Nombre: 'Cabecera',
        Activo: false,
        Completo: true,
        Iniciado: true
    },
    {
        Codigo: 'SolpeSubposiciones',
        Nombre: 'Subposiciones',
        Activo: false,
        Completo: false,
        Iniciado: false
    }];

    ngOnInit() {
        
    }

    
}
