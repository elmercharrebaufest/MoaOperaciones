import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ListBaseComponent } from '../../../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../../common/services/ModalService';
import { NavService } from '../../../../../common/services/NavService';
import { SecurityService } from '../../../../../common/services/SecurityService';
import { SessionDataService } from '../../../../../common/services/SessionDataService';
import { ComprasService } from '../../../../compras.service';
import { Solp } from '../../../solp';
import { SolpPosicion } from '../../../solp-posicion';

@Component({
  selector: 'tab-direccion-entrega',
  templateUrl: './tab-direccion-entrega.component.html',
  styleUrls: ['../../../../compras.component.css']
})
export class TabDireccionEntregaComponent extends ListBaseComponent {

  @Input('model')
  protected model: Solp;

  @Input('posicion')
  protected posicion: SolpPosicion;

  @Input('combos')
  protected combos: any;
  
  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  provinciaEntrega: any[];
  centroOriginal: any;


  camposObligatorios: any[] = [
    { campo: 'nombreEntrega', esObligatorio: true, esFijo: true },
    { campo: 'codigoPostalEntrega', esObligatorio: true, esFijo: true },
    { campo: 'calleEntrega', esObligatorio: true, esFijo: true },
    { campo: 'paisEntrega', esObligatorio: false, esFijo: true },
    { campo: 'numeroEntrega', esObligatorio: false, esFijo: true },
  ];

  ngOnChanges() {
    this.provinciaEntrega = this.combos.Provincia;
    this.posicion.validateDireccionEntrega();
  }

  ngOnInit(): void {
     this.centroOriginal = {
       nombreEntrega: this.posicion.nombreEntrega,
       codigoPostalEntrega: this.posicion.codigoPostalEntrega,
       calleEntrega: this.posicion.calleEntrega,
       paisEntrega: this.posicion.paisEntrega,
       numeroEntrega: this.posicion.numeroEntrega
    }
  }

  public ngOnDestroy(): void {
    this.posicion.validateDireccionEntrega();
  }

  mostrarValidacion(campoAValidar, vacio){
    let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
    return (camposVacios != null && vacio == "");
  }

  buscarCombo(event, type) {
    switch (type) {
        case 'PROVINCIA':
            this.provinciaEntrega = this.combos.Provincia.filter(x => x.ProvinciaId.toLowerCase().includes(event.query.toLowerCase()));
            break;
        default:
            break;
    }
  }

}




