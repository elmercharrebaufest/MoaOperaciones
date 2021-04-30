import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { Seccion } from './../../common/models/seccion';
import { ModalService } from './../../common/services/ModalService';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { element } from '@angular/core/src/render3/instructions';
import { e } from '@angular/core/src/render3';
import { SolpService } from './../solp.service';
import { Card } from './../../common/models/card'


declare var $: any;

@Component({
    selector: 'dashboard',
    templateUrl: `dashboard.component.html`,
    providers: [SolpService]

})
export class DashboardComponent extends ListBaseComponent {

    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    cards = [ 
        { nombre: "Con documento de pliego", path: "/compras/solp" },
        { nombre: "Con documentos requerimientos", path: "" },
        { nombre: "Sin documento", path: "" },
        { nombre: "Emergencia", path: "" },
        { nombre: "Adicional", path: ""}
    ]


    goToSeccion(path: string) {
        $("#mySidenav").css({ 'right': '-270px' });
        $("#myMenuClose").css({ 'display': 'none' });
        $("#myMenuOpen").css({ 'display': 'block' });
        $("#coverAll").fadeOut();
        this.navService.navegarSeccion(path);
        return false;
    }


    ngOnInit() {
        this.navService.setSeccionList([]);

    }

    
}