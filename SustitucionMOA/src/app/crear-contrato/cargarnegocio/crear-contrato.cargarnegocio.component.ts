import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoCargarNegocioService } from './../crear-contrato.service';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { Seccion } from '../../common/models/seccion';
import { forEach } from '@angular/router/src/utils/collection';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
declare var $: any;

@Component({
    selector: 'app-crear-contrato-cargarnegocio',
    templateUrl: `crear-contrato.cargarnegocio.component.html`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoCargarNegocioService }]
})
export class CrearContratoCargarNegocioComponent extends CrearContratoBaseComponent {
        
    ngOnInit() {
        this.setMenuSeccionTab("crear-contrato", "Carga de Negocios");
        this.navService.setSeccionList([
            //new Seccion('/crear-contrato/cargarnegocio', 'crear-contrato', 'Seleccione un negcio'),
            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),

        ]);
        //super.ngOnInit();
        //this.contrato.TipoNegocioId = 2;
        //this.negocioHabilitado(this.contrato);
    }

    


}