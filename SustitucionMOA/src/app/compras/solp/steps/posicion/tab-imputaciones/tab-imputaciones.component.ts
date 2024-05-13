import { Component, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../../common/services/ModalService';
import { NavService } from '../../../../../common/services/NavService';
import { SecurityService } from '../../../../../common/services/SecurityService';
import { SessionDataService } from '../../../../../common/services/SessionDataService';
import { ComprasService } from '../../../../compras.service';
import { EnumTipoImputacion } from '../../../../enum-tipo-imputacion';
import { SubPosicionViewModel } from '../tab-subposicion/sub-posicion-view-model';
import { SolpPosicion } from '../../../solp-posicion';

@Component({
  selector: 'tab-imputaciones',
  templateUrl: './tab-imputaciones.component.html',
  styleUrls: ['../../../../compras.component.css']
})
export class TabImputacionesComponent extends ListBaseComponent {

  @Input('posicion')
  protected posicion: SolpPosicion;

  tituloColumnaTipoDeImputacion: string;
  enumTipoImputacion: typeof EnumTipoImputacion = EnumTipoImputacion;
  autocomplete: any[];
  tablaAFiltrar: any;
  listadoPosicionActual = Array<SubPosicionViewModel>();
  camposObligatorios: any[];

  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnChanges() {
    this.actualizarTipoDeImputacion();
    this.camposObligatorios = [
      { campo: 'valorImputacion', esObligatorio: typeof this.posicion === "undefined" ? false : this.posicion.esTipoPosicionMaterial ? true : (this.posicion.valorImputacion || this.posicion.cuentaMayor) },
      { campo: 'cuentaMayor', esObligatorio: typeof this.posicion === "undefined" ? false : this.posicion.esTipoPosicionMaterial ? true : (this.posicion.valorImputacion || this.posicion.cuentaMayor) },
    ];

    this.posicion.validateImputaciones();
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.posicion.validateImputaciones();
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
    return (camposVacios != null && (vacio == undefined || vacio == ""));
  }

    actualizarTipoDeImputacion(): void {
        if (this.posicion.tipoImputacion) {
            switch (this.posicion.tipoImputacion.Codigo) {
                case this.enumTipoImputacion.CentroDeCosto:
                    this.tituloColumnaTipoDeImputacion = "Centro de costo";
                    this.tablaAFiltrar = 'CecoSolpSap';
                    break;
                case this.enumTipoImputacion.OrdenDeOt:
                    this.tituloColumnaTipoDeImputacion = "Nro de OT";
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
        } else {
            this.enumTipoImputacion.CentroDeCosto;
            this.tituloColumnaTipoDeImputacion = "Centro de costo";
            this.tablaAFiltrar = 'CecoSolpSap';
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
                    this.autocomplete = soloDescripcion ? result.map(x => x.Descripcion.trim()) : result;
                }
            },
            error => {
                this.floatMsgService.setErrorMsg(error.message);
                this.spinnerComponent.hideIt();
            }
        );
    }
    catch (e) {
        this.floatMsgService.setErrorMsg(e);
        return false; //<-- Prevent Refresh
    }

    return false; //<-- Prevent Refresh
  }
}
