import { Component, OnDestroy, OnInit } from '@angular/core';
import { ListBaseComponent } from '../../common/base-components/list-base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { SelectItem, ConfirmationService } from 'primeng/api';
import { ReporteContratoService } from '../reporte-contrato.service';



@Component({
    selector: 'app-reporte-contrato.listado',
    templateUrl: './reporte-contrato.listado.component.html',
    styleUrls: ['./reporte-contrato.listado.component.css'],
    //providers: [
    //    ReporteContratoService
    //]

})
export class ReporteContratoListado extends ListBaseComponent implements OnDestroy {

    constructor(protected service: ReporteContratoService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);     
    }

    detalle: any[];
    cabecera: [];

    ngOnInit() {

        this.getListado();
         this.navService.setSeccionList([]);
       
    }


    getListado() {
        this.detalle = null;
        this.mensajeComponent.setMsgsEmpty();

        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getListado().subscribe(
            (result: any) => {
                
                this.mensajeComponent.setMsgsEmpty();

                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.cabecera = result.data.Resultados;
                    console.log(this.cabecera);
                    result.data.Resultados.Detalles.forEach(x => {
                        console.log(result.data.Resultados.Detalles);
                    });

                }
                
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }

        );
        return false;
    }

    isVisible() {
        return this.cabecera && this.cabecera.length != 0;
    }

}
