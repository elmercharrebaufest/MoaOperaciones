import { Component, OnInit, ViewChild } from '@angular/core';
import { CrearContratoBaseComponent } from './../crear-contrato.component';
import { CrearContratoService, CrearContratoAltaMasivaService } from './../crear-contrato.service';
import { ContratoAPrecio } from "../../common/models/contratoAPrecio";
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
    selector: 'app-crear-contrato-alta-masiva',
    templateUrl: `crear-contrato.alta-masiva.component.html`,
    providers: [{ provide: CrearContratoService, useClass: CrearContratoAltaMasivaService }]
})
export class CrearContratoAltaMasivaComponent extends CrearContratoBaseComponent {


    contrato: ContratoAPrecio = new ContratoAPrecio();
    contratoAcuerdo: string;
    archivoNombre: string;
    adjunto: FileList;
    ngOnInit() {
        //super.ngOnInit();
        //this.negocioHabilitado(this.contrato);
    }

    ngAfterViewInit(): void {

    }

    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "Alta Masiva");
    }

    grabarContratoAltaMasiva() {
        console.log(this.contrato);
        if (!this.contratoAcuerdo || this.contratoAcuerdo == "") {
            this.mensajeComponent.setErrorMsg("Ingrese el numero de Contrato Acuerdo.");
            return false;
        }
        if (!this.adjunto) {
            this.mensajeComponent.setErrorMsg("Debe seleccionar el arhcivo para la carga masiva.");
            return false;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Grabando...');
        try {
            this.unsubscribe();
            this.subscription = this.service
                .AltaMasivaAcuerdo(this.adjunto, this.contratoAcuerdo)
                .subscribe(
                    (result) => {
                        this.spinnerSmallComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (
                            result.error != undefined &&
                            result.error != ""
                        ) {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            this.mensajeComponent.setMsgsEmpty();
                            this.mensajeComponent.setSuccessMsg(result.data);
                        }
                    },
                    (error) => {
                        this.spinnerSmallComponent.hideIt();
                        this.mensajeComponent.setErrorMsg(error.message);
                    }
                );

        } catch (e) {
            this.blockUI.stop();
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
        }

    }

    handleFileInput(files: FileList, fileKey: string) {
        this.adjunto = files;
        console.log(files);
        this.archivoNombre = files[0].name;
    }

}