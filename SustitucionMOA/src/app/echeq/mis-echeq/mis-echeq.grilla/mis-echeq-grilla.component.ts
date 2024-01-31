import { Component, Input, OnInit } from '@angular/core';
import { EcheqContrato, EcheqDocumento } from '../../gestion/echeq-contrato.model';
import { EcheqGestionComponent } from '../../gestion/echeq-gestion.component';
import { EcheqApertura } from '../../gestion/echeq.popup/echeqApertura-model';
import { EcheqReporte } from '../mis-echeq-reporte.model';
import { MisEcheqFilter } from '../mis-echeq.filtros/mis-echeq-filter.model';

@Component({
  selector: 'app-mis-echeq-grilla',
  templateUrl: './mis-echeq-grilla.component.html',
  styleUrls: ['./mis-echeq-grilla.component.css']
})
export class MisEcheqGrillaComponent extends EcheqGestionComponent implements OnInit {

  @Input() echeqContratos: Array<EcheqContrato>;
  @Input() datosReporte: Array<EcheqReporte>;

  verColumnaMarcadas = false;

  documentoSelect: EcheqDocumento;

  public itemsPerPage: string;
  msgs: { severity: string; summary: string; detail: string; }[];

  ngOnInit() {
    this.itemsPerPage = sessionStorage.getItem("itemsPerPage") ? sessionStorage.getItem("itemsPerPage") : "10";
  }

}
