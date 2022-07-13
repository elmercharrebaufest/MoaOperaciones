import { Component, OnInit, ViewChild, Renderer, ElementRef, AfterViewInit } from '@angular/core';
import { LiquidacionService, LiquidacionInformadaService } from './../liquidacion.service';
import { LiquidacionBaseComponent } from './../liquidacion.component';
import { SessionDataService } from './../../common/services/SessionDataService';
//import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
//import { ListBaseComponent } from './../common/base-components/list-base-component'
//import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
//import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SecurityService } from './../../common/services/SecurityService';
import { Seccion } from '../../common/models/seccion';
import { LiquidacionInformada } from '../../common/models/liquidacionInformada';

@Component({
    selector: 'app-liquidacion-informada',
    templateUrl: `liquidacion.informada.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionInformadaService }]
})
export class LiquidacionInformadaComponent extends LiquidacionBaseComponent {

    @ViewChild('myCalendar', undefined) private calendar: any;

    constructor(protected service: LiquidacionInformadaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected elementRef: ElementRef, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroCOE: string = "";
    filteredfechas: LiquidacionInformada[];
    liquidacionInformada: LiquidacionInformada[] = new Array<LiquidacionInformada>();
    fechaInicio: any;
    fechaFin: any;
    es: any;
    rangeDates: Date[];
    data = null;
    tipoFiltroFecha = 1;

    ngOnInit() {       
        this.setTabs();
        this.checkPermisos();
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
        if (this.securityService.tienePermiso("INFORMAR LIQUIDACION")) {
            this.navService.setSeccionList([
                new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'),
                new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'),
                new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas'),
                new Seccion('/liquidacion/informada', 'liquidacion', 'Informadas'),
                new Seccion('/liquidacion/informar', 'liquidacion', 'Informar')
            ]);
        }
        else {
            this.navService.setSeccionList([
                new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'),
                new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'),
                new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas'),
                new Seccion('/liquidacion/informada', 'liquidacion', 'Informadas'),
            ]);
        }

        this.getData();
    }
    
    returnToTodaysDate() {
        this.filteredfechas = this.data.liquidaciones;
        if (this.filteredfechas.length > 0) {
            this.mensajeComponent.setMsgsEmpty();
        }
    }

    onSelect(event: any) {     

        if (this.rangeDates[0] && this.rangeDates[1] == null) {
            let d = new Date(Date.parse(event));
            this.fechaInicio = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;

        } else {
            let d = new Date(Date.parse(event));
            this.fechaFin = `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
            if (this.rangeDates[1]) { // If second date is selected
                this.calendar.overlayVisible = false;
            }
            this.getData();
        }
    }

    getData() {        
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        this.vaciarFiltros();
        try {
            this.unsubscribe();
            this.subscription = this.service.getData(null, null, null).subscribe(
                (result: any) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {                        
                        this.data = { liquidaciones: result.data };
                        this.filteredfechas = this.data.liquidaciones;
                        if (this.fechaInicio != null && this.fechaFin != null) {
                            this.actualizarFiltroFecha();
                        }

                    }
                },
                error => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    actualizarFiltroFecha() {       
        var fechaDesde = this.fechaInicio;
        var fechaHasta = this.fechaFin + " 23:59:59";

        this.filteredfechas =
            this.data.liquidaciones
                .filter(x =>
                    new Date(Date.parse(this.tipoFiltroFecha == 1 ? (x.FechaInformada) : x.FechaComprobante)) >= new Date(fechaDesde) &&
                    new Date(Date.parse(this.tipoFiltroFecha == 1 ? (x.FechaInformada) : x.FechaComprobante)) <= new Date(fechaHasta)
                )

        if (this.filteredfechas.length == 0) {
            this.mensajeComponent.setInfoMsg("No se encontraron liquidaciones")
        }
        else {
            this.mensajeComponent.setMsgsEmpty();
        }


    }

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Informadas");
    }

    vaciarFiltros() {
        this.filtroCOE = "";
    }
}