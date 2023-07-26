import { trigger, state, style, transition, animate } from '@angular/animations';
import { Component, Input, OnInit} from '@angular/core';
import { EcheqContrato, EcheqDocumento } from '../echeq-contrato.model';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es';
import { ConfirmationService } from 'primeng/api';
import { EcheqGestionComponent } from '../echeq-gestion.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { EcheqApertura } from '../echeq.popup/echeqApertura-model';
import { BehaviorSubject } from 'rxjs';




@Component({
    selector: 'app-echeq-grilla',
    templateUrl: './echeq-grilla.component.html',
    styleUrls: ['./echeq-grilla.component.css'],
    animations: [
        trigger('detailExpand', [
            state('collapsed', style({ height: '0px', minHeight: '0' })),
            state('expanded', style({ height: '*' })),
            transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
        ]),
    ],
    providers: [ConfirmationService]
})
export class GrillaComponent extends EcheqGestionComponent implements OnInit {
    popupVisible = new BehaviorSubject<boolean>(false);
    clasificacionesDeshabilitadas = [""];
    @Input() echeqContratos: Array<EcheqContrato>;
    @BlockUI() blockUI: NgBlockUI;
    esNuevoEcheq: boolean = false;

    documentoSelect: EcheqDocumento;
    public itemsPerPage: string;
    msgs: { severity: string; summary: string; detail: string; }[];

    echeqApertura: EcheqApertura;
    //listaChequesAux: EcheqApertura[];

    aforoConf: number;
    cantidadEcheq: number;
    ngOnInit() {
        registerLocaleData(es);
        this.itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
        this.configuracionEcheq();
    }

    onClickSelectContrato(contrato: EcheqContrato) {
        if (contrato.selected) {
            contrato.expanded = true;
            this.marcarContrato(contrato);
        } else {
            contrato.selected = true;
            this.confirmContrato(contrato);
        }

        if (contrato) {
            contrato.documentos.forEach(docs => {
                docs.selected = contrato.selected;
            });
        }
    }

    onClickSelectDocumento(documento: EcheqDocumento) {
        let contrato = this.echeqContratos.find(contrato => contrato.id == documento.parentId);

        if (this.isProductor(contrato)) {
            return
        }

        if (documento.selected) {
            //mandar contratos a sap y db

            if (contrato != null) {
                contrato.selected = true;
            }

            this.marcarDocumento(documento);
        } else {
            documento.selected = true;
            this.confirmDocumento(documento);
        }


    }

    confirmContrato(contrato: EcheqContrato) {
        this.confirmationService.confirm({
            key: "confimationContrato",
            message: 'El contrato posee liquidaciones con pagos por Echeq, ¿Desea anular todos sus pagos por ECheq?',
            header: 'Deshacer e-cheq',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                //toDo falta back, db y RFC 
                this.checkContrato(contrato, false);
            },
            reject: () => { }
        });
    }

    confirmDocumento(documento: EcheqDocumento) {
        this.confirmationService.confirm({
            key: "confimationDocumento",
            message: 'Esta anulando un pago por Echeq, ¿Desea continuar?',
            header: 'Deshacer e-cheq',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.checkDocumento(documento, false);
                //toDo falta back, db y RFC 
            },
            reject: () => { }
        });
    }

    public isProductor(contrato) {
        return contrato.clasificacion == "PRODUCTOR";
    }

    public checkContrato(contrato, check) {
        if (check) {
            this.marcarContrato(contrato);
        } else {
            this.desmarcarContrato(contrato);
        }
    }

    public checkDocumento(documento, check) {
        documento.selected = check;

        let contrato = this.echeqContratos.find(contrato => contrato.id == documento.parentId);
        if (contrato.clasificacion == "PRODUCTOR") {
            documento.selected = contrato.selected;
            return;
        }

        let documentosSelected = contrato.documentos.filter(documento => documento.parentId == contrato.id && documento.selected);
        contrato.selected = documentosSelected.length > 0;

        if (!check) {
            this.desmarcarDocumento(documento);
        }
    }

    marcarContrato(echeqContrato: EcheqContrato) {
        let contratoSeleccionado = echeqContrato.contrato;
        try {
            this.blockUI.start('Grabando...');
            this.echeqService.MarcarContrato(echeqContrato.contrato, echeqContrato.pedido).subscribe(response => {
                this.blockUI.stop();

                if (response.logout == true) {
                    this.sessionDataService.logout();
                } else if (response.error != undefined && response.error != "") {
                    this.errorMarcarDesmarcarContrato(contratoSeleccionado, true);
                    this.floatMsgService.setErrorMsg(response.error);
                } else if (response.info != undefined) {
                    this.errorMarcarDesmarcarContrato(contratoSeleccionado, true);
                    this.floatMsgService.setInfoMsg(response.info);
                } else {
                    echeqContrato.selected = true;
                    echeqContrato.documentos.forEach(docs => {
                        docs.selected = true;
                    });
                    this.floatMsgService.setSuccessMsg(response);
                }
            },
                error => {
                    this.errorMarcarDesmarcarContrato(contratoSeleccionado, true);
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                });

        }
        catch (e) {
            this.errorMarcarDesmarcarContrato(contratoSeleccionado, true);
            this.floatMsgService.setErrorMsg(e);
        }
    }

    desmarcarContrato(echeqContrato: EcheqContrato) {
        let contratoSeleccionado = echeqContrato.contrato;
        try {
            this.blockUI.start('Grabando...');
            this.echeqService.DesmarcarContrato(echeqContrato.contrato, echeqContrato.pedido).subscribe(response => {
                this.blockUI.stop();
                if (response.logout == true) {
                    this.sessionDataService.logout();
                } else if (response.error != undefined && response.error != "") {
                    this.errorMarcarDesmarcarContrato(contratoSeleccionado, false);
                    this.floatMsgService.setErrorMsg(response.error);
                } else if (response.info != undefined) {
                    this.errorMarcarDesmarcarContrato(contratoSeleccionado, false);
                    this.floatMsgService.setInfoMsg(response.info);
                } else {
                    echeqContrato.selected = false;
                    echeqContrato.documentos.forEach(docs => {
                        docs.selected = false;
                        docs.listaChequesApertura = [];
                    });
                    this.floatMsgService.setSuccessMsg(response)
                    return response;
                }
            },
                error => {
                    this.errorMarcarDesmarcarContrato(contratoSeleccionado, false);
                    this.floatMsgService.setErrorMsg(error.message);
                    this.blockUI.stop();
                });

        }
        catch (e) {
            this.errorMarcarDesmarcarContrato(contratoSeleccionado, false);
            this.floatMsgService.setErrorMsg(e);
        }
    }

    marcarDocumento(echeqDocumento: EcheqDocumento) {
        let parentId = echeqDocumento.parentId;
        let numeroCOE = echeqDocumento.numeroCOE;
        try {
            this.blockUI.start('Grabando...');
            this.echeqService.MarcarDocumento(echeqDocumento.documento, echeqDocumento.pedido, echeqDocumento.contrato).subscribe(response => {
                this.blockUI.stop();
                if (response.logout == true) {
                    this.sessionDataService.logout();
                } else if (response.error != undefined && response.error != "") {
                    this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, true);
                    this.floatMsgService.setErrorMsg(response.error);
                } else if (response.info != undefined) {
                    this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, true);
                    this.floatMsgService.setInfoMsg(response.info);
                } else {
                    echeqDocumento.selected = true;
                    this.floatMsgService.setSuccessMsg(response)

                    return response;
                }
            },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                    this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, true);
                });
        }
        catch (e) {
            this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, true);
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
        }
    }

    desmarcarDocumento(echeqDocumento: EcheqDocumento) {
        let parentId = echeqDocumento.parentId;
        let numeroCOE = echeqDocumento.numeroCOE;
        try {
            this.blockUI.start('Grabando...');
            this.echeqService.DesmarcarDocumento(echeqDocumento.documento, echeqDocumento.pedido, echeqDocumento.contrato).subscribe(response => {

                if (response.logout == true) {
                    this.sessionDataService.logout();
                } else if (response.error != undefined && response.error != "") {
                    this.floatMsgService.setErrorMsg(response.error);
                    this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, false);
                } else if (response.info != undefined) {
                    this.floatMsgService.setInfoMsg(response.info);
                    this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, false);
                } else {
                    echeqDocumento.selected = false;
                    echeqDocumento.listaChequesApertura = [];
                    this.floatMsgService.setSuccessMsg(response)
                    return response;
                }
            },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                    this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, false);
                });
            this.blockUI.stop();

        }
        catch (e) {
            this.errorMarcaDesmarcaDocumento(parentId, numeroCOE, false);
            this.floatMsgService.setErrorMsg(e);
        }
    }

    showAperturarEcheqDialog(echeqDocumento: EcheqDocumento, esNuevo: boolean) {
        this.documentoSelect = echeqDocumento;
        if (echeqDocumento.listaChequesApertura == null) {
            echeqDocumento.listaChequesApertura = new Array<EcheqApertura>();
        }
        this.esNuevoEcheq = esNuevo;
        this.popupVisible.next(true)
    }

    cancelarEcheqApertura() {
        this.documentoSelect = new EcheqDocumento(null, null);
        this.popupVisible.next(false)
    }

    // Se asocian los datos del echeq
    aperturarEcheq($event) {
        this.agregarApertura();
        this.documentoSelect = new EcheqDocumento(null, null);
        this.popupVisible.next(false)
    }

    configuracionEcheq() {
        try {
            this.subscription = this.service.ConfiguracionEcheq().subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.aforoConf = Number(result.find(x => x.Code == "EcheqAforo").Value);
                        this.cantidadEcheq = Number(result.find(x => x.Code == "EcheqLimiteCantidadAperturas").Value);
                        this.spinnerComponent.hideIt();
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                });
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
        }
    }

    agregarApertura() {
        this.blockUI.start('Grabando...')
        let contratoSeleccionado: string = this.documentoSelect.contrato;
        let numeroCOESeleccionado: string = this.documentoSelect.numeroCOE;

        try {
            this.subscription = this.service.AgregarApertura(this.documentoSelect.documento, this.documentoSelect.pedido, this.documentoSelect.contrato, this.documentoSelect.listaChequesApertura).subscribe(
                (result: any) => {
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                        this.errorAperturaDocumento(contratoSeleccionado, numeroCOESeleccionado);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                        this.errorAperturaDocumento(contratoSeleccionado, numeroCOESeleccionado);
                    } else {
                        this.floatMsgService.setSuccessMsg(result);
                        this.spinnerComponent.hideIt();
                    }
                },
                error => {
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(error.message);
                    this.errorAperturaDocumento(contratoSeleccionado, numeroCOESeleccionado);
                }
            );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            this.errorAperturaDocumento(contratoSeleccionado, numeroCOESeleccionado);
        }
    }

    mask(valor) {
        return valor.replace(/(0)*/, '')
    }
    private errorMarcarDesmarcarContrato(contrato: string, esMarcar: boolean) {
        let contratoSeleccionado = this.echeqContratos.find(x => x.contrato == contrato);
        if (contratoSeleccionado!=null){
            contratoSeleccionado.selected = esMarcar? false : true;
            if(contratoSeleccionado.documentos !=null && contratoSeleccionado.documentos.length){
                contratoSeleccionado.documentos.forEach(item=>{
                    this.errorMarcaDesmarcaDocumento(item.parentId, item.numeroCOE, esMarcar);
                });
            }
        }
    }
    private errorMarcaDesmarcaDocumento(parentId: string ,numeroCOE: string , esMarcar:boolean){
        let contrato = this.echeqContratos.find(contrato => contrato.id == parentId);
        contrato.selected = esMarcar? false: true;
        if (contrato.documentos.length > 0){
            let documentos = contrato.documentos.find(documento => documento.numeroCOE == numeroCOE);
            documentos.selected = esMarcar? false: true;
        }
    }
    private errorAperturaDocumento(contratoSeleccionado: string ,numeroCOESeleccionado: string ){
        if (this.esNuevoEcheq){
            let contratos = this.echeqContratos.filter(x=> x.contrato == contratoSeleccionado);
            if (contratos!=null && contratos.length > 0) {
                let documentosContrato = contratos[0].documentos;
                let documentos = documentosContrato.filter(x=> x.numeroCOE == numeroCOESeleccionado);
                if (documentos!=null && documentos.length > 0)
                    documentos[0].listaChequesApertura = [];
            }
        }
    }
    expandirRow(pagoPendiente) {
        if (pagoPendiente.expanded) pagoPendiente.expanded = false;
        else {
            this.echeqContratos.forEach(x => x.expanded = false);
            pagoPendiente.expanded = !pagoPendiente.expanded;
        }
    }
}
