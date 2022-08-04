import { Component, OnInit, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component'
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SelectItem, ConfirmationService } from 'primeng/api';
import { ReporteContratoService } from '../reporte-contrato.service';
import { FiltroFechaComponent } from '../../common/view-child/filtro-fecha/filtro-fecha.component';


@Component({
    selector: 'app-reporte-contrato.listado',
    templateUrl: './reporte-contrato.listado.component.html',
    styleUrls: ['./reporte-contrato.listado.component.css'],
    providers: [
        ReporteContratoService
    ]

})
export class ReporteContratoListado extends ListBaseComponent {

    constructor(protected service: ReporteContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
      
    
    }

  ngOnInit() {
  }

}
