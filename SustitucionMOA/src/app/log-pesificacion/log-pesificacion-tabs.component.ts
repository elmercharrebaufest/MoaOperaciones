import { Component } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { ListBaseComponent } from '../common/base-components/list-base-component';
import { Seccion } from '../common/models/seccion';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { SessionDataService } from '../common/services/SessionDataService';

@Component({
    selector: 'app-log-pesificacion-tabs',
    template: ``,
})
export class LogPesificacionTab extends BaseComponent {

    constructor(protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
    }


    checkPermisos() { this.securityService.tienePermisoRedirect("GUARDADO Y CONSULTA DE LOG PESIFICACIONES"); }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList(
            [
                new Seccion('logPesificacion/listado', 'Pesificacion', 'Pesificaciones Manuales'),
                new Seccion('logPesificacion/listadoAutomaticas', 'Pesificacion', 'Pesificaciones Automaticas'),
            ]
        );
        
        this.navService.navegarSeccion("logPesificacion/listado");
    }

}