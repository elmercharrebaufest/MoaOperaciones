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

    acuerdosPendientes: string = null;
    ngOnInit() {
        this.setMenuSeccionTab("crear-contrato", "Carga de Negocios");
        //this.navService.setSeccionList([
        //    //new Seccion('/crear-contrato/cargarnegocio', 'crear-contrato', 'Seleccione un negcio'),
        //    new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
        //    new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
        //    new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),

        //]);
        //super.ngOnInit();
        this.validarAltaMasiva();
    }

    validarAltaMasiva() {
        this.blockUI.start('');                       
        this.unsubscribe();
        this.subscription = this.service.validarDirecto().subscribe(
            result => {
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.blockUI.stop();
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    let obj = JSON.parse(result);
                    if (obj != null && obj > 0) {
                        this.esCorredorEnDataAgro = true;
                        this.blockUI.stop();                        
                        this.navService.setSeccionList([
                            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
                            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
                            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion'),
                            new Seccion('/crear-contrato/alta-masiva', 'crear-contrato', 'Alta Masiva')
                        ]);
                        this.obtenercontratosAcuerdo();
                    } else {
                        this.esCorredorEnDataAgro = false;
                        this.navService.setSeccionList([
                            new Seccion('/crear-contrato/aprecio', 'crear-contrato', 'A Precio'),
                            new Seccion('/crear-contrato/afijar', 'crear-contrato', 'A Fijar'),
                            new Seccion('/crear-contrato/fijacion', 'crear-contrato', 'Fijacion')
                        ]);
                        this.blockUI.stop();                       
                    }
                }
            },
            error => {
                this.blockUI.stop();
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );

        return false;
    }

    obtenercontratosAcuerdo() {
        try {
            this.unsubscribe();
            this.subscription = this.service.ObteneContratosAcuerdo()
                .subscribe(
                    (result) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                        } else if (result.info != undefined) {
                        } else {
                            let contratos = JSON.parse(result);
                            if (contratos.length == 0) {
                                this.acuerdosPendientes = null;
                            } else {
                                this.acuerdosPendientes = "Tiene contratos acuerdos disponibles para cargar.";
                            }                           
                        }
                    },
                    (error) => {
                    }
                );

        } catch (e) {
        }
    }

}