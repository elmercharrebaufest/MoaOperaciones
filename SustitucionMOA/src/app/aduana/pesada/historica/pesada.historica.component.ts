import { Component, OnInit, ViewChild } from '@angular/core';
import { AduanaService } from './../../aduana.service';
import { FiltroFechaComponent } from './../../../common/view-child/filtro-fecha/filtro-fecha.component';
import { MensajeComponent } from './../../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../../common/view-child/spinner/spinner.component';
import { AduanaBaseComponent } from './../../aduana.component';


import { Formatter } from './../../../common/formatter/Formatter';
import { NavService } from './../../../common/services/NavService';
import { FloatMsgService } from './../../../common/services/FloatMsgService';
import { SessionDataService } from './../../../common/services/SessionDataService';
import { SecurityService } from './../../../common/services/SecurityService';
import { ModalService } from './../../../common/services/ModalService';
declare var $: any;

@Component({
    selector: 'app-aduana-pesada-historica',
    templateUrl: `pesada.historica.component.html`,
    providers: [AduanaService]
})

export class PesadaHistoricaComponent extends AduanaBaseComponent {

    constructor(protected navService: NavService, protected service: AduanaService, protected securityService: SecurityService, protected sessionDataService: SessionDataService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, service, securityService, floatMsgService, modalService);
    }

    titulo = "";
    itemsPerPage = 20;
    pesadasList: any = null;
    fechaInicio = new Date().toLocaleDateString('en-GB') + " 00:00";
    fechaFin = new Date().toLocaleDateString('en-GB') + " 23:59";

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    private spinnerComponent: SpinnerComponent;

    checkPermisos() { this.securityService.tienePermisoRedirect("CONSULTAR PESADAS"); }

    ngAfterViewInit(): void {
        $('.form_datetime_Inicio').datetimepicker({
            format: 'dd/mm/yyyy hh:ii',
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            defaultDate: 0
        });


        $('.form_datetime_Fin').datetimepicker({
            format: 'dd/mm/yyyy hh:ii',
            language: 'es',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            forceParse: 0,
            showMeridian: 1,
            defaultDate: 0
        });

    }

    setTabs() {
        this.setMenuSeccionTab("aduana", "Pesada Histórica");
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
        this.subscription = this.service.getPesada("1029", this.fechaInicio, this.fechaFin).subscribe(
            (result:any) => {
                this.spinnerComponent.hideIt();
                if (result.logout == true) {
                    this.sessionDataService.logout();
                } else if (result.error != undefined && result.error != "") {
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

    getDataEvent(fecha_inicio: string, fecha_fin: string) {
        this.fechaInicio = Formatter.parseFecha(fecha_inicio);
        this.fechaFin = Formatter.parseFecha(fecha_fin);
        this.getData();
        return false;
    }

    isVisible() {
        return this.pesadasList != null;
    }

    showModalTableResponsive(pesada: any) {
        this.modalService.openModalTableResponsive("Pesada", [
            { etiqueta: "Fecha y Hora Inicio", valor: pesada.fechaIncio },
            { etiqueta: "Balanza", valor: pesada.balanza },
            { etiqueta: "Total Embarcado [Kg]", valor: pesada.totalEmbarcado },
            { etiqueta: "Commodity", valor: pesada.commodity },
            { etiqueta: "Bodega", valor: pesada.bodega },
            { etiqueta: "Destino", valor: pesada.destino },
            { etiqueta: "Exportador", valor: pesada.exportador },
            { etiqueta: "Vapor", valor: pesada.vapor },
            { etiqueta: "Peso Programado [Kg]", valor: pesada.pesoProgramado }
        ]);
        return false;
    }
}