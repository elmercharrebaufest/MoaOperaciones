
import { Component } from '@angular/core';
import { ListBaseComponent } from './../common/base-components/list-base-component';
import { Seccion } from './../common/models/seccion';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { NavService } from './../common/services/NavService';
import { SecurityService } from './../common/services/SecurityService';
import { SessionDataService } from './../common/services/SessionDataService';
import { AplicacionCcppService } from './aplicacion-ccpp.service';
import { SeccionAplicacionCCPP, EstadoAplicacionCCPP } from './aplicacion-ccpp.model';

@Component({
    selector: 'app-aplicacion-ccpp-base',
    template: ``,
})
export class AplicacionCcppBaseComponent extends ListBaseComponent {

    constructor(protected service: AplicacionCcppService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    public crearSecciones() {
        this.navService.setSeccionList(
            [
                new Seccion(`/${SeccionAplicacionCCPP}`, SeccionAplicacionCCPP, 'Carga manual'),
                new Seccion(`/${SeccionAplicacionCCPP}/masiva`, SeccionAplicacionCCPP, 'Carga masiva'),
                new Seccion(`/${SeccionAplicacionCCPP}/listado`, SeccionAplicacionCCPP, 'Estado de cargas'),
            ]
        );
    }
    estadosAplicacionCCPP = EstadoAplicacionCCPP
}