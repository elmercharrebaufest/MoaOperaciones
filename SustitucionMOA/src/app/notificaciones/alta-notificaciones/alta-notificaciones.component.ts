import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { Notificacion } from '../../common/models/notificacion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { NotificacionesService } from '../notificaciones.service';

@Component({
  selector: 'app-alta-notificaciones',
  templateUrl: './alta-notificaciones.component.html',
  styleUrls: ['./alta-notificaciones.component.css'],
  providers: [NotificacionesService]
})
export class AltaNotificacionesComponent extends BaseComponent implements OnInit {
  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  constructor(protected service: NotificacionesService, protected navService: NavService,
            protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
  protected floatMsgService: FloatMsgService, protected modalService: ModalService)
  {
      super(navService, securytiService, floatMsgService, modalService);
  }

  notificacion: Notificacion = new Notificacion();


  ngOnInit() {
      this.navService.setSeccionList([]);

  }

}
