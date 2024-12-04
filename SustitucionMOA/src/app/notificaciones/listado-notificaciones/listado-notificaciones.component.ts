import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { NotificacionesService } from '../notificaciones.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
    selector: 'app-listado-notificaciones',
    templateUrl: './listado-notificaciones.component.html',
    styleUrls: ['./listado-notificaciones.component.css'],
    providers: [NotificacionesService]

})
export class ListadoNotificacionesComponent extends BaseComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    path: string[] = [];
    order: number = 1;


    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    data: any;

    orderedByColumn: string = "Nombre";
    orderDirection: number = 1;
    itemsPerPage = 20;


    constructor(protected service: NotificacionesService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit(): void {
        this.navService.setSeccionList([]);
        this.getListado();
    }

    getListado() {
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.blockUI.start('Cargando...');
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListado().subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data.filter(item => item.Borrada !== true);
                    }
                    this.blockUI.stop();

                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }


    habilitar(notificacionId: number) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('Grabando...');
        try {
            this.service.habilitar(notificacionId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getListado()
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    deshabilitar(notificacionId: number) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('Grabando...');
        try {
            this.service.deshabilitar(notificacionId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.getListado()
                    }
                    this.blockUI.stop();
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    eliminar(notificacionId: number) {
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        this.blockUI.start('Grabando...');
        try {
            this.service.eliminar(notificacionId).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.getListado()
                        this.mensajeComponent.setSuccessMsg(result.data);
                        this.blockUI.stop();
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                    this.blockUI.stop();
                }
            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    orderColumnBy(column: string) {
        if (column === this.orderedByColumn) {
            this.orderDirection = -this.orderDirection;
        } else {
            this.orderDirection = 1;
            this.orderedByColumn = column;
        }
    }
}
