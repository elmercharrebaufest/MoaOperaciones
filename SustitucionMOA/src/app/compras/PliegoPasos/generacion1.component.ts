import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { SolpService } from './../solp.service'
import { Solp } from './../Solp';

declare var $: any;

@Component({
    selector: 'generacion1',
    templateUrl: `generacion1.component.html`,
})
export class Generacion1Component extends ListBaseComponent {

    @Input('model') 
    protected model:Solp;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild('dtp_fecha_pago')
    protected fechaPagoDTP: ElementRef;

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    // checkPermisos() { this.securityService.tienePermisoRedirect("GENERACION 1"); }

    nombreDeObra: string = "";
    // fiscalContrato: string = "";
    // telefono: string = "";
    // mail: String = sessionStorage.getItem("username");

    // solpGeneracion1: SolpGeneracion1 = new SolpGeneracion1();

    solpPaso1Result: any;
    fechaEntrega: any;
    horaEntrega: any = "10:00";

    
    

    // postSolp1() {
    //     debugger
    //     this.unsubscribe();
            //this.parsearFecha()
    //     try {
    //         this.subscription = this.service.solp(
    //             this.nombreDeObra, 
    //             this.fiscalContrato, 
    //             this.telefono, 
    //             this.mail
    //             this.fechaDeEntregaDeOfertasFecha,
    //             this.fechaDeEntregaDeOfertasHora
    //             ).subscribe(
    //             result => {
    //                 if (result.logout == true) {
    //                     this.sessionDataService.logout();
    //                 } else if (result.error != undefined && result.error != "") {
    //                     this.floatMsgService.setErrorMsg(result.error);
    //                 } else if (result.info != undefined) {
    //                     this.floatMsgService.setInfoMsg(result.info);
    //                 } else { 
    //                     this.solpPaso1Result = result;
    //                 }
    //             },
    //             error => {
    //                 this.floatMsgService.setErrorMsg(error.message);
    //             }
 
    //         );
    //     } catch (e) {
    //         this.floatMsgService.setErrorMsg(e);
    //         return false; //<-- Prevent Refresh
    //     }
    //     return false; //<-- Prevent Refresh
    // }

    parsearFecha () {
        this.fechaEntrega = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
    if(this.fechaEntrega != '' && this.fechaEntrega != null && this.horaEntrega != '' && this.horaEntrega != null){
        var dateParts = this.fechaEntrega.split("-");
        this.model.fechaDeEntregaDeOfertasFecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2], this.horaEntrega);
        }
        console.log(this.model.fechaDeEntregaDeOfertasFecha, "No funciona");

    }


    

    setTabs() {
        this.setMenuSeccionTab("Generacion1", "Generacion1");
    }

    ngOnInit() {
        this.setTabs();
        if(!this.model.mail)
            this.model.mail = sessionStorage.getItem("username");
        // this.checkPermisos();
        //this.getData();
    }


}
