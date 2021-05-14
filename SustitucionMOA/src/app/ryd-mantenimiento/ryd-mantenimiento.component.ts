import { Component, OnInit, ViewChild } from '@angular/core';
import { RYDMantenimientoService } from './ryd-mantenimiento.service';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { SessionDataService } from './../common/services/SessionDataService';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { BaseComponent } from './../common/base-components/base-component';
import { Seccion } from './../common/models/seccion';
import { Balanza, BalanzaBusqueda, BalanzaAplicar } from './ryd-mantenimiento';
import { Subscription } from 'rxjs';
import { ModalService } from './../common/services/ModalService';



@Component({
    selector: 'app-ryd-mantenimiento',
    template: ``,
    providers: [RYDMantenimientoService]
})
export class RYDMantenimientoBaseComponent extends BaseComponent implements OnInit {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: RYDMantenimientoService, protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    data: any;
    visibleButton: boolean = true;

    checkPermisos() { }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/ryd-mantenimiento/balanzas', 'ryd-mantenimiento', 'Balanzas'), new Seccion('/ryd-mantenimiento/commodities', 'ryd-mantenimiento', 'Commodities'), new Seccion('/ryd-mantenimiento/exportadores', 'ryd-mantenimiento', 'Exportadores')]);
        //super.ngOnInit();
        this.getDataInputs();
    }

    getDataInputs() { }
   
    initRequestBotones() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
    }

    endRequestBotones() {
        this.visibleButton = true;
        this.spinnerComponent.hideIt();
    }
}