import { trigger, state, style, transition, animate } from '@angular/animations';
import { Component, Input, OnInit } from '@angular/core';
import { EcheqContrato, EcheqDocumento } from '../echeq-contrato.model';
import { registerLocaleData } from '@angular/common';
import es from '@angular/common/locales/es';



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
})
export class GrillaComponent implements OnInit{

  @Input() echeqContratos: Array<EcheqContrato>;

  public itemsPerPage: string;


  ngOnInit() {
    registerLocaleData(es);
    this.itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
  }

  onClickSelectContrato(contrato: EcheqContrato) {
    if(contrato.selected == true){
      contrato.expanded = true;
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

      if(contrato != null){
        contrato.selected = true;
      }
    } else {
      let contrato = this.echeqContratos.find(contrato => contrato.id == documento.parentId);

      if(contrato != null){
        let documentosSelected = contrato.documentos.filter(documento => documento.parentId == contrato.id && documento.selected);
        contrato.selected = documentosSelected.length > 0;
      }
    }
  }

}
