import { Component, OnInit } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { NavService } from '../../common/services/NavService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SecurityService } from '../../common/services/SecurityService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { EstadoSolicitudService } from './estado-solicitud.service';


@Component({
    selector: 'app-estado-solicitud',
    templateUrl: './app/alta-proveedores/estado-solicitud/estado-solicitud.component.html',
    styleUrls: ['./app/alta-proveedores/estado-solicitud/estado-solicitud.component.css', '../Content/css/bootstrap.min.css']
})
export class EstadoSolicitudComponent extends ListBaseComponent {

    constructor(protected service: EstadoSolicitudService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }
     
    estadoSolicitud: string;
    observaciones: string

    ngOnInit() {
        this.obtenerEstado()
    }


    obtenerEstado() {
        this.subscription = this.service.getEstadoAprobacion().subscribe(
            result => {
                this.estadoSolicitud = result.data.EstadoDescripcion;
                this.observaciones = result.data.Observaciones;
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
    }

}
