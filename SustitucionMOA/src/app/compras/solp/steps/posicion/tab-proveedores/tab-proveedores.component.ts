import { Component, Input } from '@angular/core';
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
  selector: 'tab-proveedores',
  templateUrl: './tab-proveedores.component.html',
  styleUrls: ['./tab-proveedores.component.css']
})
export class TabProveedoresComponent extends ListBaseComponent  {

  @Input('model')
  protected model: Solp;

  @Input('posicion')
  protected posicion: SolpPosicion;

  @Input('rowIndex')
  protected rowIndex: any;

  resultadoProveedores: string[];
  proveedoresAutocomplete: any;  

  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }

  ngOnInit() {
    this.posicion = this.model.posiciones[this.rowIndex];
  }

  public ngOnDestroy(): void {
      //this.model.posiciones.find(x => x.indice == this.posicion.indice) == this.posicion;
  }

}
