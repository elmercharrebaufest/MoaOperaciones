import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, SelectItem } from 'primeng/api';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { Solp } from '../../Solp';
import { ValidadorPasoSolpService } from '../../validadorPasoSolpService';


declare var $: any;



@Component({
  selector: 'tab-datos-posicion',
  templateUrl: `tab-datos-posicion.component.html`,
  styleUrls: ['../../compras.component.css']
})
export class TabDatosPosicionComponent extends ListBaseComponent implements OnInit{

  @Input('combos')
  protected combos: any;

  @Input('model')
  protected model: Solp;

  @Input('disabled')
  protected disabled: boolean = false;


  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  //Declaro las variables
  articuloCompras: SelectItem[];
  solicitanteCompras: SelectItem[];
  grupoCompras: SelectItem[];

 


  camposObligatorios: any[] = [
    { campo: 'selectGrupoCompras', esObligatorio: true, esFijo: false },
    { campo: 'selectArticuloCompras', esObligatorio: true, esFijo: true },
    { campo: 'selectSolicitanteCompras', esObligatorio: true, esFijo: true },
    { campo: 'necesidadCompras', esObligatorio: true, esFijo: true },
    { campo: 'textoSuministro', esObligatorio: true, esFijo: true },
    { campo: 'motivo', esObligatorio: true, esFijo: true },
    { campo: 'modelo', esObligatorio: true, esFijo: true }
  ];

  camposAValidar: any[] = [
    { campo: 'selectGrupoCompras', servicio: true, materialCatalogado: true },
    { campo: 'selectArticuloCompras', servicio: true, materialCatalogado: true },
    { campo: 'selectSolicitanteCompras', servicio: true, materialCatalogado: true },
    { campo: 'necesidadCompras', servicio: true, materialCatalogado: true },
    { campo: 'textoSuministro', servicio: false, materialCatalogado: true },
    { campo: 'motivo', servicio: false, materialCatalogado: true },
    { campo: 'modelo', servicio: false, materialCatalogado: true }
  ]

  @Output() onEstCompleto = new EventEmitter<any>();

    
  ngOnInit() {
    this.grupoCompras = this.combos.GrupoCompras;
    this.articuloCompras = this.combos.GrupoArticulo;

    if (!this.model.posicionActual.selectSolicitanteCompras)
    this.model.posicionActual.selectSolicitanteCompras = this.model.fiscalContrato;
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

    if(camposMostrar != null && camposMostrar != undefined){
      switch(this.model.posicionActual.servicio) {
        case 'SERVICIO':
          return camposMostrar.servicio;
        default:
          return camposMostrar.materialCatalogado; 
      }
    }
    return false
  }

}
