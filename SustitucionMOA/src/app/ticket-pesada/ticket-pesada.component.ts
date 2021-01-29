import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { ConsultaTicketPesda } from '../common/models/ticket-pesada/consulta-ticket-pesada';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { SessionDataService } from '../common/services/SessionDataService';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { TicketPesadaesService } from './ticket-pesada.service';

@Component({
  selector: 'app-ticket-pesada',
  templateUrl: './ticket-pesada.component.html',
  styleUrls: ['./ticket-pesada.component.css']
})
export class TicketPesadaComponen extends BaseComponent implements OnInit {

  TicketPesada: ConsultaTicketPesda;
  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;
  
  constructor(protected service: TicketPesadaesService,
    protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        ) {
        super(navService, securytiService, floatMsgService, modalService);
  }

  ngOnInit() {
  }

  validar() {
    return false;
  }

  enviar() {
    if (this.validar()) {
        this.spinnerComponent.hideIt();
        return;
    }
    this.mensajeComponent.setMsgsEmpty();
    this.spinnerComponent.showIt();
    this.unsubscribe();
  
    this.subscription = this.service
        .getTicketPesada(this.TicketPesada)
        .subscribe(
            (result) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (
                    result.error != undefined &&
                    result.error != ""
                ) {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    console.log("it works!")
                }
            },
            (error) => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
  }
}
