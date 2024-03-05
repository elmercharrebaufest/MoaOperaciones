import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PeticionDeOfertaDto } from '../../../modelos/peticion-de-oferta-model';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { FloatMsgService } from '../../../common/services/FloatMsgService';
import { ModalService } from '../../../common/services/ModalService';
import { NavService } from '../../../common/services/NavService';
import { SecurityService } from '../../../common/services/SecurityService';
import { SessionDataService } from '../../../common/services/SessionDataService';
import { ComprasService } from '../../compras.service';

@Component({
    selector: 'app-cerrar-cotizacion',
    templateUrl: './cerrar-cotizacion.component.html',
    styleUrls: ['./cerrar-cotizacion.component.css']
})
export class CerrarCotizacionComponent implements OnInit {

    @Input() displayCerrarCotizacion: boolean;
    @Input() public peticion: PeticionDeOfertaDto;
    @Output() cerrarModalCotizacionEmitter = new EventEmitter();
    @Output() onCloseModalEmitter = new EventEmitter();
    @Input() visualizarAlertCotizacion: boolean;

    observacion: any;
    displayOkCerrarCotizacion: boolean;
    @BlockUI() blockUI: NgBlockUI;
    error: string = "";
    aviso: string = "";

    visualizarAlert = false;

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }

    ngOnInit() {
    }

    onCerrarModalCotizar() {
        this.visualizarAlert = false;
        this.onCloseModalEmitter.next();
    }

    salir() {
        this.visualizarAlert = false;
        this.cerrarModalCotizacionEmitter.next();
        this.displayOkCerrarCotizacion = false;
    }

    validar() {
        if (this.observacion == "" || this.observacion == undefined) {
            this.error = "El campo Observaciones es obligatorio";
            return this.visualizarAlert = true;
        }
        return this.visualizarAlert = false;
    }

    cerrarCotizacion(peticionId, observacion) {
        this.validar();
        if (!this.visualizarAlert) {
            this.blockUI.start('Cargando...');
            try {
                this.service.cerrarCotizacion(peticionId, observacion)
                    .subscribe(
                        (result) => {
                            if (result.logout === true) {
                                this.sessionDataService.logout();
                            } else if (result.error !== undefined && result.error !== "") {
                                this.error = result.error;
                                this.visualizarAlert = true;
                            } else if (result.info !== undefined) {
                                this.error = result.info;
                                this.visualizarAlert = true;
                            } else {
                                this.peticion = result.data;
                                this.blockUI.stop();
                                this.onCerrarModalCotizar();
                                this.displayOkCerrarCotizacion = true;
                            }
                        },
                        (error) => {
                            this.error = error.message;
                            this.visualizarAlert = true;
                            this.blockUI.stop();
                        }
                    );
            } catch (e) {
                //this.floatMsgService.setErrorMsg(e);
                this.error = e;
                this.visualizarAlert = true;
                this.blockUI.stop();
                return false; //<-- Prevent Refresh
            }
            return false; //<-- Prevent Refresh

        }
    }

}
