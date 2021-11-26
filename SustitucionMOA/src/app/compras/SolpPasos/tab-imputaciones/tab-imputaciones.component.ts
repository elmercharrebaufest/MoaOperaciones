import { Component, Input, OnInit } from '@angular/core';
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
import { EnumTipoImputacion } from '../../enum-tipo-imputacion';
import { SubPosicionViewModel } from '../../PliegoPasos/solapaSubposiciones/subPosicionViewModel';
import { PosicionSolp, Solp } from '../../Solp';
import { ValidadorPasoSolpService } from '../../validadorPasoSolpService';

@Component({
  selector: 'tab-imputaciones',
  templateUrl: './tab-imputaciones.component.html',
  styleUrls: ['../../compras.component.css']
})
export class TabImputacionesComponent extends ListBaseComponent implements OnInit {

  @Input('posicion')
  protected posicion: SubPosicionViewModel;

  @Input('model')
  protected model: Solp;

  tituloColumnaTipoDeImputacion: string;
  enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
  autocomplete: any[];
  tablaAFiltrar: any;
  listadoPosicionActul = Array<SubPosicionViewModel>();


  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, private formBuilder: FormBuilder, protected router: Router,
    private validadorPasoSolpService: ValidadorPasoSolpService, private confirmationService: ConfirmationService) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
}

camposObligatorios: any[] = [
  { campo: 'centroDeCosto', esObligatorio: false, esFijo: true },
  { campo: 'ordenDeOt', esObligatorio: false, esFijo: true },
  { campo: 'ordenDeInversion', esObligatorio: false, esFijo: true },
  { campo: 'siniestroBeneficio', esObligatorio: false, esFijo: true },
  {  campo: 'tipoImputacion', esObligatorio: true, esFijo: true },
  { campo: 'cuentaMayor', esObligatorio: true, esFijo: true }
];

  ngOnInit() {
    this.actualizarTipoDeImputacion();
  }

  validarConNoNulo(subposicion: any, valor: any, campoAValidar: string){    
    if(subposicion.tareaSubcontratar === ""){
        return false
    }  
    
    return valor == null || 
        ((this.camposObligatorios.find(x => x.campo == campoAValidar).esObligatorio) 
        && valor.toString().length == 0);
  }


  mostrarValidacion(campoAValidar, vacio){
    let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
    return (camposVacios != null && vacio == undefined);
  }

  actualizarTipoDeImputacion(): void {
    debugger
    switch (this.model.posicionActual.tipoImputacion) {
        case this.enumTipoImputacion.CentroDeCosto:
            this.tituloColumnaTipoDeImputacion = "Centro de costo";
            this.tablaAFiltrar = 'CecoSolpSap';
            break;
        case this.enumTipoImputacion.OrdenDeOt:
            this.tituloColumnaTipoDeImputacion = "Orden de OT";
            this.tablaAFiltrar = 'OrdenSolpSap';
            break;
        case this.enumTipoImputacion.OrdenInversion:
            this.tituloColumnaTipoDeImputacion = "Orden de inversión"
            this.tablaAFiltrar = 'OrdenSolpSap';
            break;
        case this.enumTipoImputacion.Siniestro:
            this.tituloColumnaTipoDeImputacion = "Siniestro / Centro de beneficio"
            this.tablaAFiltrar = 'CentroBeneficio';
            break;
    }
  }

autocompleteSap(event, tablaAFiltrar, soloDescripcion = false) {
  try {
      this.subscription = this.service.autocompleteSap(tablaAFiltrar || this.tablaAFiltrar, event.query.toLowerCase()).subscribe(
          (result: any) => {
              if (result.logout == true) {
                  this.sessionDataService.logout();
              } else if (result.error != undefined && result.error != "") {
                  this.floatMsgService.setErrorMsg(result.error);
              } else if (result.info != undefined) {
                  this.floatMsgService.setInfoMsg(result.info);
              } else {
                  console.log(result);
                  this.autocomplete = soloDescripcion ? result.map(x => x.Descripcion.trim()) : result;
              }
          },
          error => {
              this.floatMsgService.setErrorMsg(error.message);
              this.spinnerComponent.hideIt();
          }
      );
  } catch (e) {
      this.floatMsgService.setErrorMsg(e);
      return false; //<-- Prevent Refresh
  }

  return false; //<-- Prevent Refresh
}





}
