import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MisConsultasService } from './mis-consultas.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { ModalService } from './../common/services/ModalService';
import { element } from '@angular/core/src/render3/instructions';


declare var $: any;

@Component({
    selector: 'mis-consultas',
    templateUrl: `mis-consultas.component.html`,
    providers: [MisConsultasService]

})
export class MisConsultasComponent extends ListBaseComponent {

    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild("spinnerModal")
    protected spinnerModal: SpinnerSmallComponent;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: MisConsultasService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    proveedor: string;
    nombre: string;
    email: string;
    telefono: string;
    categoriaOptions: Array<DropdownOption> = [];
    inscripcion: string;
    motivo: string;
    file: any;
    camposAdicionales: boolean = false;
    fechaPagoDP: any;
    visibleButton: boolean = true;
    categoriaSelected: any;
    captchaOk: any = null;

    setTabs() {
        this.setMenuSeccionTab("mis-consultas", "mis-consultas");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
    }
    
}