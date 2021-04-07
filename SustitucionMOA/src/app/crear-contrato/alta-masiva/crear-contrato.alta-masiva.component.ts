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
    contratoAcuerdo: number = 0;
    contratosAcuerdo: any = new Array();
    conError: any = new Array();
    sinError: any = new Array();
    archivoNombre: string;
    adjunto: FileList;
    ngOnInit() {
        //super.ngOnInit();
        //this.negocioHabilitado(this.contrato);
        this.obtenercontratosAcuerdo()
    }
    obtenercontratosAcuerdo() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('');
        try {
            this.unsubscribe();
            this.subscription = this.service.ObteneContratosAcuerdo()
                .subscribe(
                    (result) => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.blockUI.stop();
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.blockUI.stop();
                            this.mensajeComponent.setInfoMsg(result.info);
                        } else {
                            let contratos = JSON.parse(result);
                            let lista = new Array();
                            if (contratos.length == 0) {
                                lista.push({ Id: 0, Filtro: "No hay contratos acuerdos disponibles." });
                            } else {
                                lista.push({ Id: 0, Filtro: "Seleccione un Acuerdo de la lista..." });
                                for (var i = 0; i < contratos.length; i++) {
                                    lista.push({ Id: contratos[i].Id, Filtro: contratos[i].Filtro });
                                }
                            }
                            this.contratosAcuerdo = lista;

                            this.mensajeComponent.setMsgsEmpty();
                            this.blockUI.stop();
                            this.spinnerComponent.hideIt();
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

    ngAfterViewInit(): void {

    }

    setTabs() {
        this.setMenuSeccionTab("crear-contrato", "Alta Masiva");
    }

    grabarContratoAltaMasiva() {
        console.log(this.contrato);
        if (!this.contratoAcuerdo || this.contratoAcuerdo == null || this.contratoAcuerdo == 0) {
            this.mensajeComponent.setErrorMsg("Seleccione un Contrato Acuerdo.");
            return false;
        }
        if (!this.adjunto) {
            this.mensajeComponent.setErrorMsg("Debe seleccionar el arhcivo para la carga masiva.");
            return false;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Procesando archivo...');
        try {
            this.sinError = new Array();
            this.conError = new Array();
            this.unsubscribe();
            this.subscription = this.service
                .AltaMasivaAcuerdo(this.adjunto, this.contratoAcuerdo.toString())
                .subscribe(
                    (result) => {
                        //this.spinnerSmallComponent.hideIt();
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (
                            result.error != undefined &&
                            result.error != ""
                        ) {
                            this.mensajeComponent.setErrorMsg(result.error); this.blockUI.stop();
                            this.spinnerComponent.hideIt();
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info); this.blockUI.stop();
                            this.spinnerComponent.hideIt();
                        } else {
                            this.sinError = result.Resume.filter(a => a.HasError == false);
                            this.conError = result.Resume.filter(a => a.HasError == true);
                            this.mensajeComponent.setMsgsEmpty();
                            this.blockUI.stop();
                            this.spinnerComponent.hideIt();

                        }
                    },
                    (error) => {
                        this.blockUI.stop();
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
        this.irACargas
    }

    descargarExcelModeloAltaMasiva() {
        this.subscription = this.service
            .excelModeloAltaMasiva()
            .subscribe(
                (result) => {
                    if (result.error) {

                    } else {
                        var byteArray = new Uint8Array(result.data);
                        var blob = new Blob([byteArray], {
                            type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        });
                        if (window.navigator.msSaveOrOpenBlob) {
                            // IE11
                            window.navigator.msSaveOrOpenBlob(
                                blob,
                                "AltaMasiva.xlsx"
                            );
                        } else {
                            var url = window.URL.createObjectURL(blob);
                            var link = document.createElement("a");
                            document.body.appendChild(link);
                            link.href = url;
                            link.download = "AltaMasiva.xlsx";
                            link.click();
                            setTimeout(function () {
                                window.URL.revokeObjectURL(url);
                            }, 0);

                            return false;
                        }
                    }
                },
                () => {

                }
            );
    }

}