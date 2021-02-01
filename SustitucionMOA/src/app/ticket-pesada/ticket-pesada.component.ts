import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../common/base-components/base-component';
import { ConsultaTicketPesada } from '../common/models/ticket-pesada/consulta-ticket-pesada';
import { FloatMsgService } from '../common/services/FloatMsgService';
import { ModalService } from '../common/services/ModalService';
import { NavService } from '../common/services/NavService';
import { SecurityService } from '../common/services/SecurityService';
import { SessionDataService } from '../common/services/SessionDataService';
import { MensajeComponent } from '../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../common/view-child/spinner/spinner.component';
import { TicketPesadaService } from './ticket-pesada.service';

@Component({
  selector: 'app-ticket-pesada',
  templateUrl: './ticket-pesada.component.html',
  styleUrls: ['./ticket-pesada.component.css']
})
export class TicketPesadaComponent extends BaseComponent implements OnInit {

  TicketPesada: ConsultaTicketPesada = new ConsultaTicketPesada();
  @ViewChild(MensajeComponent)
  protected mensajeComponent: MensajeComponent;

  @ViewChild(SpinnerComponent)
  protected spinnerComponent: SpinnerComponent;
  
  constructor(protected service: TicketPesadaService,
    protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        ) {
        super(navService, securytiService, floatMsgService, modalService);
      
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
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
        .ObtenerTicketPesada(this.TicketPesada)
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
                    var byteArray = new Uint8Array(result.FileContents);
                    var blob = new Blob([byteArray], { type: 'application/zip' });

                    let nombreArchivo = "TicketPesada.zip"

                    if (window.navigator.msSaveOrOpenBlob) {
                        // IE11
                        window.navigator.msSaveOrOpenBlob(blob, nombreArchivo);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        var link = document.createElement("a");
                        document.body.appendChild(link);
                        link.href = url;
                        link.download = nombreArchivo;
                        link.click();
                        setTimeout(function () { window.URL.revokeObjectURL(url); }, 0);
                        return false;
                    }
                }
            },
            (error) => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
  }
}
