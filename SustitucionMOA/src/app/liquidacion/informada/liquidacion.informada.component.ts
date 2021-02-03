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

@Component({
    selector: 'app-liquidacion-informada',
    templateUrl: `liquidacion.informada.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionInformadaService }]
})
export class LiquidacionInformadaComponent extends LiquidacionBaseComponent {

    constructor(protected service: LiquidacionInformadaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected elementRef: ElementRef, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    filtroCOE: string = "";

    ngOnInit(){
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'), new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'), new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas'), new Seccion('/liquidacion/informada', 'liquidacion', 'Informadas'), new Seccion('/liquidacion/informar', 'liquidacion', 'Informar')]);
        this.getData();
    }

    getData(){
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        this.vaciarFiltros();
        try {
            this.unsubscribe();
            this.subscription = this.service.getData(null, null, null).subscribe(
                result => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.data = { liquidaciones: result.data };
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

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Informadas");
    }

    vaciarFiltros(){
        this.filtroCOE = "";
    }
}