import { Component, OnInit } from '@angular/core';
import { CartaPorteService } from '../../carta-porte/carta-porte2.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';

@Component({
  selector: 'app-ordenes-de-carga.listado',
  templateUrl: './ordenes-de-carga.listado.component.html',
  styleUrls: ['./ordenes-de-carga.listado.component.css']
})
export class OrdenesDeCargaListado extends ListBaseComponent {

     constructor(protected service: OrdenesDeCargaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

   ngOnInit() {
      this.setTabs();
      this.checkPermisos();
    }

}
