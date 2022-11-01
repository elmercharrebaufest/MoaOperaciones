import { trigger, state, style, transition, animate } from '@angular/animations';
import { Component, Input, OnInit } from '@angular/core';
import { EcheqContrato, EcheqDocumento } from '../echeq-contrato.model';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es';
import {ConfirmationService} from 'primeng/api';
import { EcheqGestionComponent } from '../echeq-gestion.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { EcheqApertura } from '../echeq.popup/echeqApertura-model';



@Component({
  selector: 'app-echeq-grilla',
  templateUrl: './echeq-grilla.component.html',
  styleUrls: ['./echeq-grilla.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
  providers: [ConfirmationService]
})
export class GrillaComponent extends EcheqGestionComponent implements OnInit{

  @Input() echeqContratos: Array<EcheqContrato>;
  @BlockUI() blockUI: NgBlockUI;

  documentoSelect: EcheqDocumento;
  public itemsPerPage: string;
  msgs: { severity: string; summary: string; detail: string; }[];
  
  displayAperturarEcheq: boolean = false;
  echeqApertura: EcheqApertura;

  ngOnInit() {
    registerLocaleData(es);
    this.itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
  }

  onClickSelectContrato(contrato: EcheqContrato) {
    if(contrato.selected == true){
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

  onClickSelectDocumento(documento: EcheqDocumento){
    if(documento.selected){
      let contrato = this.echeqContratos.find(contrato => contrato.id == documento.parentId);
      //mandar contratos a sap y db

      if(contrato != null){
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
      header: 'ConfirmationContrato',
     icon: 'pi pi-exclamation-triangle',
      accept: () => {
        //toDo falta back, db y RFC 
        this.checkContrato(contrato, false);
        },
        reject: () => {}
    });
  }

  confirmDocumento(documento: EcheqDocumento) {
    this.confirmationService.confirm({
      key: "confimationDocumento",
      message: 'Esta anulando un pago por Echeq, ¿Desea continuar?',
      header: 'ConfirmationContrato',
     icon: 'pi pi-exclamation-triangle',
      accept: () => {
        debugger
        this.checkDocumento(documento, false);

        //toDo falta back, db y RFC 
        },
        reject: () => {}
    });
  }

  public isProductor(contrato){
    return contrato.clasificacion == "PRODUCTOR";
  }

  public checkContrato(contrato, check){
    if(check == true){
      this.marcarContrato(contrato);
    } else {
      this.desmarcarContrato(contrato);
    }
  }

  public checkDocumento(documento, check){
    documento.selected = check;
    
    let contrato = this.echeqContratos.find(contrato => contrato.id == documento.parentId);
    if(contrato.clasificacion == "PRODUCTOR"){
      documento.selected = contrato.selected;
      return;
    }

    let documentosSelected = contrato.documentos.filter(documento => documento.parentId == contrato.id && documento.selected);
    contrato.selected = documentosSelected.length > 0;

    if(check == false){
      this.desmarcarDocumento(documento);
    }
  }  

  marcarContrato(echeqContrato: EcheqContrato){
    try {
      this.blockUI.start('Grabando...');
      this.echeqService.MarcarContrato(echeqContrato.contrato, echeqContrato.pedido).subscribe(response => {
         
          if (response.logout == true) {
              this.sessionDataService.logout();
          } else if (response.error != undefined && response.error != "") {
              this.floatMsgService.setErrorMsg(response.error);
          } else if (response.info != undefined) {
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
          this.floatMsgService.setErrorMsg(error.message);
      });
      this.blockUI.stop();

    } 
    catch (e) {
        this.floatMsgService.setErrorMsg(e);          
    }  
  }

  desmarcarContrato(echeqContrato: EcheqContrato){
    try {
      this.blockUI.start('Grabando...');
      this.echeqService.DesmarcarContrato(echeqContrato.contrato, echeqContrato.pedido).subscribe(response => {
          if (response.logout == true) {
              this.sessionDataService.logout();
          } else if (response.error != undefined && response.error != "") {
              this.floatMsgService.setErrorMsg(response.error);
          } else if (response.info != undefined) {
              this.floatMsgService.setInfoMsg(response.info);
          } else {
                echeqContrato.selected = false;
                echeqContrato.documentos.forEach(docs => {
                  docs.selected = false;
                });
              this.floatMsgService.setSuccessMsg(response)             
              return response;
        }
              },  
      error => {
          this.floatMsgService.setErrorMsg(error.message);
      });
      this.blockUI.stop();

    } 
    catch (e) {
        this.floatMsgService.setErrorMsg(e);          
    }  
  }

  marcarDocumento(echeqDocumento: EcheqDocumento){
    try {
      this.blockUI.start('Grabando...');
      this.echeqService.MarcarDocumento(echeqDocumento.documento, echeqDocumento.pedido, echeqDocumento.contrato).subscribe(response => {
         
          if (response.logout == true) {
              this.sessionDataService.logout();
          } else if (response.error != undefined && response.error != "") {
              this.floatMsgService.setErrorMsg(response.error);
          } else if (response.info != undefined) {
              this.floatMsgService.setInfoMsg(response.info);
          } else {
              echeqDocumento.selected = true;             
              this.floatMsgService.setSuccessMsg(response)
                           
              return response;
        }
              },  
      error => {
          this.floatMsgService.setErrorMsg(error.message);
      });
      this.blockUI.stop();
    } 
    catch (e) {
        this.blockUI.stop();
        this.floatMsgService.setErrorMsg(e);          
    }  
  }

  desmarcarDocumento(echeqDocumento: EcheqDocumento){
    try {
      this.blockUI.start('Grabando...');
      this.echeqService.DesmarcarDocumento(echeqDocumento.documento, echeqDocumento.pedido, echeqDocumento.contrato).subscribe(response => {
         
          if (response.logout == true) {
              this.sessionDataService.logout();
          } else if (response.error != undefined && response.error != "") {
              this.floatMsgService.setErrorMsg(response.error);
          } else if (response.info != undefined) {
              this.floatMsgService.setInfoMsg(response.info);
          } else {
                echeqDocumento.selected = false;             
                this.floatMsgService.setSuccessMsg(response)             
              return response;
        }
              },  
      error => {
          this.floatMsgService.setErrorMsg(error.message);
      });
      this.blockUI.stop();

    } 
    catch (e) {
        this.floatMsgService.setErrorMsg(e);          
    }  
  }

  showAperturarEcheqDialog(echeqDocumento: EcheqDocumento ) {
    this.documentoSelect = echeqDocumento;
    this.displayAperturarEcheq = true;

    if(echeqDocumento.listaChequesApertura == null){
      echeqDocumento.listaChequesApertura = new Array<EcheqApertura>();
    }

    if(echeqDocumento.listaChequesApertura.length == 0){
      let aforo = {
        ordenCheque: 1,
        importeCheque: echeqDocumento.importeEnPesos * 30 / 100,
        porcentaje: 30
      };
      echeqDocumento.listaChequesApertura.push(aforo);
    }
  }

  cancelarEcheqApertura() {
    this.displayAperturarEcheq = false;
}

 // Se asocian los datos del echeq
 aperturarEcheq($event) {
  this.displayAperturarEcheq = false;
}

}
