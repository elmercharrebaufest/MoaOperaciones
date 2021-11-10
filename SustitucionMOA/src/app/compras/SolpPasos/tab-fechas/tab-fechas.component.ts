import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { Solp } from '../../Solp';
import { ValidadorPasoSolpService } from '../../validadorPasoSolpService';

@Component({
  selector: 'tab-fechas',
  templateUrl: './tab-fechas.component.html',
  styleUrls: ['../../compras.component.css']
})
export class TabFechasComponent extends ListBaseComponent implements OnInit {

  @Input('model')
  protected model: Solp;

  @Input('locale')
  protected locale: any;

  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,
    private validadorPasoSolpService: ValidadorPasoSolpService, private confirmationService: ConfirmationService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);

}

  fechaEntregaServicio: any;
  fechaDeLiberacion: any;
  hoy: Date = new Date();

  camposObligatorios: any[] = [
    { campo: 'fechaEntregaServicio', esObligatorio: false, esFijo: false },
    { campo: 'fechaDeLiberacion', esObligatorio: false, esFijo: false },
    { campo: 'plazoDeEntrega', esObligatorio: true, esFijo: true },
    { campo: 'concluido', esObligatorio: false, esFijo: true },
    { campo: 'indiceFijacion', esObligatorio: false, esFijo: true }
  ];


  @Output() onEstCompleto = new EventEmitter<any>();


  ngOnInit() {
  }

  // mostrarAsterisco(nombreCampo: string) {
  //   return this.camposObligatorios.find(x => x.campo == nombreCampo).esObligatorio ? '*' : '';
  // }

  calcularFechaEntrega() {
    let fechaNueva = new Date(this.model.fechaEntrega);
    fechaNueva.setDate(fechaNueva.getDate() + parseInt(this.model.posicionActual.plazoDeEntrega.toString()));
    this.model.posicionActual.fechaEntregaServicio = fechaNueva;
  }

  mostrarValidacion(campoAValidar, vacio){
    let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
    return (camposVacios != null && vacio == undefined);
  }




}
