import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { SessionDataService } from '../common/services/SessionDataService';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { ListadoNovedadesService } from './listado-novedades.service'
import { NotificacionesService } from '../notificaciones/notificaciones.service';
import { ModalNotificacionesComponent } from '../notificaciones/modal-notificaciones/modal-notificaciones.component';

@Component({

    selector: 'app-listado-novedades',
    templateUrl: './listado-novedades.component.html',
    styleUrls: ['./listado-novedades.component.css'],
    providers: [ListadoNovedadesService, NotificacionesService]

})
export class ListadoNovedadesComponent extends BaseComponent implements OnInit {

    path: string[] = [];
    order: number = 1;


    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    data: any;

    orderedByColumn: string = "Nombre";
    orderDirection: number = 1;
    itemsPerPage = 5;
    mostarModal: boolean = false;

    @ViewChild("myModal") modal: ModalNotificacionesComponent;

    protected modalNotificacionesComponent: ModalNotificacionesComponent;
    notificacionModal: any;
    // notificacion: any;
    notificacionLeida: boolean[] = [];
    adjuntoModal = [];


    constructor(protected service: ListadoNovedadesService, protected notificacionesService: NotificacionesService, protected navService: NavService,
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
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListadoCompletoNovedades().subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = result.data;
                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
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

    getNotificacion(notificacionId: number) {
        try {
          this.unsubscribe();
            this.subscription = this.notificacionesService.getNotificacion(notificacionId).subscribe(
            (result:any) => {
              this.notificacionModal = result.data;
            },
            error => {
            }
          );
        } catch (e) {
          return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
      }

    openModalNovedad(notificacionId: number, notificacion: any, adjunto: any) {
        this.notificacionModal = null;
      
        try {
          this.notificacionesService.postNotificacionLeida(notificacionId).subscribe(
            (result: any) => {
              if (result.logout == true) {
                this.sessionDataService.logout();
              } else if (result.error != undefined && result.error != "") {
              } else if (result.info != undefined) {
              } else {
                this.getNotificacion(notificacionId);
                this.mostarModal = true;
                setTimeout(() => {
                  if (this.notificacionModal != null) {
                    if (this.notificacionModal.Leida == 1) {
                      this.notificacionModal[this.notificacionModal.Id] = true;
                    } else {
                      this.notificacionLeida[this.notificacionModal.Id] = false;
                    }
        
                    this.notificacionModal.ArchivosAdjuntos.forEach((adjunto: any) => { 
                      if(adjunto.AdjuntoTipo != 'previsualizacion') {
                        this.adjuntoModal.push(adjunto);
                      }
                    })  
                    // this.notificacionModal = this.da;
                    this.modal.mostrarModal();
                  }    
                }, 800);
              }
              
            },
            error => {
            }
          );
        } catch (e) {
          return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
      }
}
