import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { Notificacion } from '../../common/models/notificacion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { NotificacionesService } from '../notificaciones.service';


@Component({
  selector: 'app-carousel-notificaciones',
  templateUrl: './carousel-notificaciones.component.html',
  styleUrls: ['./carousel-notificaciones.component.css']
})
export class CarouselNotificacionesComponent extends BaseComponent implements OnInit {

  data: any;
  notificacionActual: Notificacion;
  indiceNotificacion: number = 0;
  totalNotificaciones: number = 0;
  mostrarNotificaciones: boolean = false;
  mostrarBotonSiguiente: boolean = false;
  mostrarBotonAnterior: boolean = false;
  mostrarVerMas: boolean = false;


  constructor(protected service: NotificacionesService, protected navService: NavService,
              protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
    protected floatMsgService: FloatMsgService, protected modalService: ModalService)
    {
        super(navService, securytiService, floatMsgService, modalService);
    }

  ngOnInit(): void {
      this.navService.setSeccionList([]);
      this.getNotificaciones();
    }
  
    getNotificaciones() {
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getNotificaciones().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                      this.data = result.data;
                      this.totalNotificaciones = this.data.length
                      this.notificacionActual = this.data[this.indiceNotificacion];

                      console.log(this.notificacionActual.Mensaje);
                      if (this.data.length > 0)
                        this.mostrarNotificaciones = true;
                      this.actualizarBotones()
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

  siguienteNotificacion() {
    this.notificacionActual = this.data[++this.indiceNotificacion];
    this.actualizarBotones()
  }

  notificacionAnterior() {
    this.notificacionActual = this.data[--this.indiceNotificacion];
    this.actualizarBotones()
  }

  cerrarNotificaciones() {
    this.mostrarNotificaciones = false
  }

  actualizarBotones() {
    this.mostrarBotonSiguiente = (this.indiceNotificacion + 1) != this.totalNotificaciones;
    
    this.mostrarBotonAnterior = this.indiceNotificacion != 0;
    
    this.mostrarVerMas = (this.notificacionActual.LinkAdjunto || '') != '';

  }
}
