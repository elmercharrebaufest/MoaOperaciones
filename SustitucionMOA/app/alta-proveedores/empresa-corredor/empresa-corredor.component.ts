import { Component, OnInit } from '@angular/core';
import { EmpresaGranosService } from '../empresa-granos/empresa-granos.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';

@Component({
  selector: 'app-empresa-corredor',
    templateUrl: './app/alta-proveedores/empresa-corredor/empresa-corredor.component.html',
    styleUrls: ['./app/alta-proveedores/empresa-corredor/empresa-corredor.component.css', '../Content/css/bootstrap.min.css'],
    providers: [EmpresaGranosService]
})
export class EmpresaCorredorComponent extends ListBaseComponent {

    constructor(protected service: EmpresaGranosService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }


    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
    }

}
