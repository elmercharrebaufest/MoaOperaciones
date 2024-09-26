import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { ComprasService } from '../../../compras.service';
import { NavService } from '../../../../common/services/NavService';
import { SecurityService } from '../../../../common/services/SecurityService';
import { FloatMsgService } from '../../../../common/services/FloatMsgService';
import { ModalService } from '../../../../common/services/ModalService';
import { SessionDataService } from '../../../../common/services/SessionDataService';
import { MensajeComponent } from '../../../../common/view-child/mensaje/mensaje.component';
import { finalize } from 'rxjs/operators';
import { ApiResponse } from '../../../../common/models/response';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
    selector: 'app-actualizar-fecha-vigencia',
    templateUrl: './actualizar-fecha-vigencia.component.html',
    styleUrls: ['./actualizar-fecha-vigencia.component.css']
})
export class ActualizarFechaVigenciaComponent implements OnInit {
    @BlockUI() blockUI: NgBlockUI;

    @Input()
    public posicionesSinVigencia: any[] = [];

    @Input('locale') es: any;

    nuevaFechaVigencia = null;
    @ViewChild("mensajeActualizarFechaVigenciaComponent")
    mensajeActualizarFechaVigenciaComponent: MensajeComponent;

    @Output() cerrarActualizarFechaVigenciaEmitter = new EventEmitter();

    hoy = new Date();

    constructor(protected service: ComprasService, protected navService: NavService, protected sessionDataService: SessionDataService,
        protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        protected route: ActivatedRoute, protected router: Router, private confirmationService: ConfirmationService, private formBuilder: FormBuilder) {
    }


    ngOnInit() {
        this.es = {
            firstDayOfWeek: 1,
            dayNames: ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"],
            dayNamesShort: ["dom", "lun", "mar", "mié", "jue", "vie", "sáb"],
            dayNamesMin: ["D", "L", "M", "X", "J", "V", "S"],
            monthNames: ["enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"],
            monthNamesShort: ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic"],
            today: 'Hoy',
            clear: 'Borrar'
        }

    }


    cerrarActualizarSinVigenciaModal() {
        this.nuevaFechaVigencia = null;
        this.cerrarActualizarFechaVigenciaEmitter.next();
    }

    actualizarFechaVigenciaRegistroInfo() {
        this.blockUI.start('Actualizando fechas de vigencia ...');
        this.service.actualizarFechaVigenciaRegistroInfo(
            this.nuevaFechaVigencia,
            this.posicionesSinVigencia
                .map(({ CotizacionPosicion_Id, SolpPosicion_Id }) => ({ CotizacionPosicion_Id, SolpPosicion_Id }))
        )
            .pipe(
                finalize(() => this.blockUI.stop())
            )
            .subscribe(res => {
                const result = this.manejarApiResponse(res, this.sessionDataService, this.mensajeActualizarFechaVigenciaComponent)
                if (result) {
                    this.cerrarActualizarSinVigenciaModal();
                    this.floatMsgService.setSuccessMsg("La fecha de vigencia se actualizó correctamente.");
                }
            })

    }

    manejarApiResponse<T>({ logout, error, info, data }: ApiResponse<T>, sessionDataService: SessionDataService, mensajeComponente: MensajeComponent) {
        if (logout) {
            sessionDataService.logout();
            return null;
        }
        if (error) {
            const renderFunc = mensajeComponente.setErrorMsg;
            renderFunc.bind(mensajeComponente)(error)
            return null;
        }
        if (info) {
            const renderFunc = mensajeComponente.setInfoMsg;
            renderFunc.bind(mensajeComponente)(info)
            return null;

        }
        return data;
    }
}
