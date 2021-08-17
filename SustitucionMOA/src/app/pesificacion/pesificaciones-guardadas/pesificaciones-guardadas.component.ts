import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Pesificacion } from '../../common/models/pesificacion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from '../../common/view-child/spinner-small/spinner-small.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { PesificacionBaseComponent } from '../pesificacion-base.component';
import { PesificacionService } from '../pesificacion.service';
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-pesificaciones-guardadas',
  templateUrl: './pesificaciones-guardadas.component.html',
  styleUrls: ['./pesificaciones-guardadas.component.css'],
  providers: [PesificacionService]

})
export class PesificacionesGuardadasComponent extends PesificacionBaseComponent implements OnInit {

  constructor(protected service: PesificacionService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
    super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    this.mensajeComponent = new MensajeComponent();
    this.spinnerComponent = new SpinnerComponent();
    this.spinnerSmallComponent = new SpinnerSmallComponent();
  }

  filtroContrato: string = "";
  pesificaciones: Pesificacion[]

  ngOnInit() {
    super.ngOnInit();
    this.setMenuSeccionTab("pesificacion", "Listado");

    // this.pesificaciones = [
    //   { Contrato: "100", Fijacion: "100", FechaCarga: new Date('Jul 12 2021'), FechaPesificacion: new Date('Jul 12 2021'), Kilos: 100, Precio: 200, TipoCambio: 50 },
    //   { Contrato: "200", Fijacion: "200", FechaCarga: new Date('Jul 12 2021'), FechaPesificacion: new Date('Jul 12 2021'), Kilos: 200, Precio: 200, TipoCambio: 60 },
    // ];

    this.getPesificaciones()

  }


  getPesificaciones() {
    this.mensajeComponent.setMsgsEmpty();
    this.unsubscribe();
    this.subscription = this.service.getPesificacionesSap().subscribe(
      result => {
        if (result.logout == true) {
          this.sessionDataService.logout();
        } else if (result.error != undefined && result.error != "") {
          this.mensajeComponent.setErrorMsg(result.error);
        } else if (result.info != undefined) {
          this.mensajeComponent.setInfoMsg(result.info);
        } else {
          // this.data = result;

          this.pesificaciones = result.data;
        }
      },
      error => {
        this.mensajeComponent.setErrorMsg(error.message);
      }
    );

    return false;
  }

}
