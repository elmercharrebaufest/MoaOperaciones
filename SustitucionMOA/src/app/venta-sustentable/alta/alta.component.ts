import { Component, OnInit, ViewChild} from '@angular/core';
import { VentaSustentableService } from './../venta-sustentable.service'
import { BaseComponent } from './../../common/base-components/base-component';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';


@Component({
  selector: 'app-alta',
  templateUrl: './alta.component.html',
  providers: [VentaSustentableService]
})
export class AltaComponent  extends BaseComponent implements OnInit {

  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;

  constructor(protected service: VentaSustentableService, protected navService: NavService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
    super(navService, securityService, floatMsgService, modalService);
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
 }

  ngOnInit() {
    this.navService.setSeccionList([new Seccion('sustentable/Alta/', 'Alta', 'Dar de Alta'), new Seccion('/sustentable/listado-campos', 'sustentable', 'Listado Campos')]);
  }

}
