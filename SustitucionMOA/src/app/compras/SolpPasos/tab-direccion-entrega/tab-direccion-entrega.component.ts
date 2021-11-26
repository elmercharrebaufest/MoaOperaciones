import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SelectItem } from 'primeng/api';
import { ListBaseComponent } from '../../../common/base-components/list-base-component';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';
import { Solp } from '../../Solp';

@Component({
  selector: 'tab-direccion-entrega',
  templateUrl: './tab-direccion-entrega.component.html',
  styleUrls: ['../../compras.component.css']
})
export class TabDireccionEntregaComponent extends ListBaseComponent implements OnInit {

  @Input('model')
  protected model: Solp;

  
  constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
    protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
    protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
  }


  centroEntrega: SelectItem[];
  almacenEntrega: SelectItem[];

  camposObligatorios: any[] = [
    { campo: 'nombreEntrega', esObligatorio: false, esFijo: true },
    { campo: 'codigoPostalEntrega', esObligatorio: false, esFijo: true },
    { campo: 'calleEntrega', esObligatorio: true, esFijo: true },
    { campo: 'paisEntrega', esObligatorio: false, esFijo: true },
    { campo: 'numeroEntrega', esObligatorio: true, esFijo: true },
  ];



  ngOnInit() {
  }

  mostrarValidacion(campoAValidar, vacio){
    let camposVacios = this.camposObligatorios.find(x => x.campo == campoAValidar && x.esObligatorio);
    return (camposVacios != null && vacio == "");
  }

}
