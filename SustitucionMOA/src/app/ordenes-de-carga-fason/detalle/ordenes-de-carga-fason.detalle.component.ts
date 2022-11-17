import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { CorredorContrato } from '../../common/models/ordenes-de-carga/corredorContrato';
import { EstadoOrdenDeCarga } from '../../common/models/ordenes-de-carga/estadoOrdenDeCarga';
import { OrdenDeCargaFason } from '../../common/models/ordenes-de-carga-fason/ordendecargafason';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { NgBlockUI, BlockUI } from 'ng-block-ui';
import { ConfirmationService } from 'primeng/api';
import { OrdenesDeCargaFasonService } from '../ordenes-de-carga-fason.service';
import { ListBaseComponent } from '../../common/base-components/list-base-component';



@Component({
    selector: 'app-detalle',
    templateUrl: './ordenes-de-carga-fason.detalle.component.html',
    styleUrls: ['./ordenes-de-carga-fason.detalle.component.css', '../listado/ordenes-de-carga-fason.listado.component.css']
})
export class OrdenesDeCargaFasonDetalleComponent extends ListBaseComponent implements OnInit {

    @BlockUI() blockUI: NgBlockUI;

    ordenDeCargaFason: OrdenDeCargaFason = new OrdenDeCargaFason();
  /*  ordenDeCargaFason: [];*/
    IdordenDeCargaFason: number = 0;
    mostrarBotonVerificarTransporte: boolean = false;
    mensajeError: string = "";

    esTercero: boolean = this.isAuthorized('VER ORDENES DE CARGA FASON');
    esAdmin: boolean = this.isAuthorized('VER ORDENES DE CARGA FASON ADMIN');

    constructor(private route: ActivatedRoute, protected service: OrdenesDeCargaFasonService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe,
        private confirmationService: ConfirmationService) {
        super(service, navService, sessionDataService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.IdordenDeCargaFason = params["id"];
        });

        this.navService.setSeccionList([]);
        this.obtenerOrdenDeCargaFason();

    }

    verificarBotones() {

        if (this.esAdmin) {

            if (!this.ordenDeCargaFason.TransporteExiste) {
                console.log(this.ordenDeCargaFason.TransporteExiste);
                this.mostrarBotonVerificarTransporte = true;
            }
        }


    }


    obtenerOrdenDeCargaFason() {
        try {
            this.unsubscribe();
            this.subscriptionDropDowns = this.service.getOrdenDeCargaFason(this.IdordenDeCargaFason).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                        this.ordenDeCargaFason = result.data.Response;
                        this.ordenDeCargaFason.Cantidad = 30000;
                        this.verificarBotones();

                    }
                },
                error => {

                }
            );
        } catch (e) {
        }
    }
    
    verificarTransporte() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.blockUI.start('Procesando...');
        try {
            this.subscriptionDropDowns = this.service.verificarTransporte(this.IdordenDeCargaFason).subscribe(
                result => {
                    this.blockUI.stop();
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        if (result.data != "Orden de carga actualizada correctamente") {
                            this.mensajeComponent.setInfoMsg(result.data);
                        } else {
                            this.mensajeComponent.setSuccessMsg(result.data);
                        }
                        this.mostrarBotonVerificarTransporte = false;
                        this.obtenerOrdenDeCargaFason();

                    }
                },
                error => {
                    this.blockUI.stop();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

}
