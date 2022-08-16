import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ReporteContratoListado } from '../listado/reporte-contrato.listado.component';
import { ReporteContratoService } from '../reporte-contrato.service';


@Component({
    selector: 'app-detalle',
    templateUrl: './reporte-contrato.detalle.component.html',
    styleUrls: ['./reporte-contrato.detalle.component.css']
})
export class DetalleComponent extends ListBaseComponent implements OnInit {

    constructor(private route: ActivatedRoute, protected service: ReporteContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
      
    }
  
    contratoId: string = "";
    detalle: any[] = null;


    ngOnInit() {

        this.getDetalleContrato();

    }

    getDetalleContrato() {
        debugger;
        this.data = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.route.params.subscribe(params => {
            this.contratoId = params['id'];
            this.unsubscribe();
            this.subscription = this.service.getDetalleContrato2(this.contratoId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.detalle = result.data;
                        console.log(this.detalle);

                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );

        });
    }

}
