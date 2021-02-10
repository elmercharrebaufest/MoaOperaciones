import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { BaseComponent } from '../../common/base-components/base-component';
import { OrdenDeCarga } from '../../common/models/ordenes-de-carga/ordenDeCarga';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { OrdenesDeCargaService } from '../ordenes-de-carga.service';

@Component({
  selector: 'app-ordenes-de-carga.detalle',
  templateUrl: './ordenes-de-carga.detalle.component.html',
  styleUrls: ['./ordenes-de-carga.detalle.component.css']
})
export class OrdenesDeCargaDetalleComponent extends BaseComponent implements OnInit {

    ordenDeCargaId: number = 0;
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    ordenDeCarga: OrdenDeCarga = new OrdenDeCarga();
    mensajeError: string = "";
    
    constructor(protected service: OrdenesDeCargaService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);
    }

  ngOnInit() {
        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.ordenDeCargaId = params["id"];
        });

        this.navService.setSeccionList([]);

        if (this.ordenDeCargaId > 0) {
            this.obtenerOrdenDeCarga();
        }
    }

  obtenerOrdenDeCarga() {
        try {
            this.subscriptionDropDowns = this.service.getOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                    } else if (result.info != undefined) {
                    } else {
                      this.ordenDeCarga = result.data;
                      console.log(this.ordenDeCarga)
                    }
                },
                error => {
                }
            );
        } catch (e) {
        }
    }

    notificarTransporte() {
        try {
            this.subscriptionDropDowns = this.service.getOrdenDeCarga(this.ordenDeCargaId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.ordenDeCargaId = result.data;
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }
}
