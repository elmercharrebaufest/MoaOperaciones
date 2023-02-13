import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectItem } from 'primeng/api';
import { ListBaseComponent } from '../../../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../../common/services/ModalService';
import { NavService } from '../../../../../common/services/NavService';
import { SecurityService } from '../../../../../common/services/SecurityService';
import { SessionDataService } from '../../../../../common/services/SessionDataService';
import { ComprasService } from '../../../../compras.service';
import { Solp } from '../../../solp';
import { SolpPosicion } from '../../../solp-posicion';

declare var $: any;

@Component({
  selector: 'tab-datos-posicion',
  templateUrl: `tab-datos-posicion.component.html`,
  styleUrls: ['../../../../compras.component.css']
})
export class TabDatosPosicionComponent extends ListBaseComponent{

  @Input('combos')
  protected combos: any;

  @Input('model')
  protected model: Solp;

  @Input('disabled')
  protected disabled: boolean = false;

  @Input('posicion')
  protected posicion: SolpPosicion;

  @Output() onEstCompleto = new EventEmitter<any>();  

  //Declaro las variables
  articuloCompras: SelectItem[];
  solicitanteCompras: SelectItem[];
  grupoCompras: SelectItem[];

  camposObligatorios: any[] = [
    { campo: 'selectGrupoCompras', esObligatorio: true, esFijo: false },
    { campo: 'selectArticuloCompras', esObligatorio: true, esFijo: true },
    { campo: 'selectSolicitanteCompras', esObligatorio: true, esFijo: true },
    { campo: 'necesidadCompras', esObligatorio: false, esFijo: true },
    { campo: 'textoSuministro', esObligatorio: true, esFijo: false },
    { campo: 'motivo', esObligatorio: true, esFijo: false },
    { campo: 'modelo', esObligatorio: true, esFijo: false }
  ];

  camposAValidar: any[] = [
    { campo: 'selectGrupoCompras', servicio: true, materialCatalogado: true, materialSinCatalogar: true },
    { campo: 'selectArticuloCompras', servicio: true, materialCatalogado: true, materialSinCatalogar: true },
    { campo: 'selectSolicitanteCompras', servicio: true, materialCatalogado: true, materialSinCatalogar: true },
    { campo: 'necesidadCompras', servicio: true, materialCatalogado: true, materialSinCatalogar: true },
    { campo: 'textoSuministro', servicio: false, materialCatalogado: true, materialSinCatalogar: true },
    { campo: 'motivo', servicio: false, materialCatalogado: false, materialSinCatalogar: true },
    { campo: 'modelo', servicio: false, materialCatalogado: false, materialSinCatalogar: true }
  ]

  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnChanges() {
    this.grupoCompras = this.combos.GrupoCompras;
    this.articuloCompras = this.combos.GrupoArticulo;

    if (!this.posicion.selectSolicitanteCompras)
    this.posicion.selectSolicitanteCompras = this.model.fiscalContrato;

    //this.validarTabCompleto();
    this.posicion.validateDatosPosicion();
    this.mostrarCamposTipoSolp(this.camposAValidar);
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    //this.validarTabCompleto();
    this.posicion.validateDatosPosicion();
  }

  buscarCombo(event, type) {
    switch (type) {
        case 'GRUPO COMPRAS':
            this.grupoCompras = this.combos.GrupoCompras.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
            break;
        case 'ARTICULO COMPRAS':
            this.articuloCompras = this.combos.GrupoArticulo.filter(x => x.CodigoDescripcion.toLowerCase().includes(event.query.toLowerCase()));
            break;
        default:
            break;
    }
  }

  mostrarValidacion(campoAValidar, vacio){
    let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
    return (camposVacios != null && vacio == undefined);
  }
  
  mostrarCamposTipoSolp(campoAValidar){
    let camposMostrar = this.camposAValidar.find(x => x.campo == campoAValidar);
    if (camposMostrar != null && camposMostrar != undefined) {
      switch (this.posicion.tipoPosicion.Codigo) {
        case "SERVICIO":
          return camposMostrar.servicio;
        case "MATERIALES":
          if (this.posicion.codigoServicio != "" && this.posicion.codigoServicio != undefined) {
            return camposMostrar.materialCatalogado;  
          }
          return camposMostrar.materialSinCatalogar;    
        default:
          return camposMostrar.servicio; 
      }
    }
    return false;
  }

}
