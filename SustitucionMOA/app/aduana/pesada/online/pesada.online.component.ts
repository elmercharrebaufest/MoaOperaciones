import { Component, OnInit, ViewChild } from '@angular/core';
import { AduanaService } from './../../aduana.service';
import { FiltroFechaComponent } from './../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { AduanaBaseComponent } from './../../aduana.component';
import { Formatter } from './../../../common/formatter/Formatter';
import { ModalService } from './../../../common/services/ModalService';


declare var $: any;

@Component({
    selector: 'app-aduana-pesada-online',
    templateUrl: `./app/aduana/pesada/online/pesada.online.component.html?v=${new Date().getTime()}`,
    providers: [AduanaService]
})

export class PesadaOnlineComponent extends AduanaBaseComponent {

    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, service, securityService, floatMsgService, modalService);
    }

    titulo = "";
    fecha = new Date().toLocaleDateString('en-GB');
    horaInicio = "00:00";
    horaFin = "23:59";
    itemsPerPage = 20;
    pesadasList: any = null;

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    private spinnerComponent: SpinnerComponent;

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR PESADAS"); }

    ngAfterViewInit(): void {
        $('.form_datetime_Inicio').datetimepicker({
            format: 'hh:ii',
            language: 'es',
            autoclose: 1,
            startView: 1,
            forceParse: 0,
            minuteStep: 1
        });

        $('.form_datetime_Fin').datetimepicker({
            format: 'hh:ii',
            language: 'es',
            autoclose: 1,
            startView: 1,
            forceParse: 0,
            minuteStep: 1
        });
    }

    setTabs() {
        this.setMenuSeccionTab("aduana", "Pesada Online");
    }

    ngOnInit() {
        super.ngOnInit();
        this.getData();
    }

    getData() {
        this.pesadasList = null;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.getPesada("1029", this.fecha + " " + this.horaInicio, this.fecha + " " + this.horaFin).subscribe(
            result => {
                this.spinnerComponent.hideIt()
                if (result.logout == true) {
                    this.sessionDataService.logout();
                }else if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    this.pesadasList = result.pesadas;
                }
            },
            error => {
                this.spinnerComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;
    }

    getDataEvent(hora_inicio: string, hora_fin: string) {
        this.horaInicio = Formatter.parseHora(hora_inicio);
        this.horaFin = Formatter.parseHora(hora_fin);
        this.getData();
        return false;
    }

    isVisible() {
        return this.pesadasList != null;
    }

    horaCambio(event: any) {
        this.horaFin = event;
    }
}