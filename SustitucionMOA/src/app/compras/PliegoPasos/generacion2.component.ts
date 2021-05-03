import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FiltroFechaComponent } from './../../common/view-child/filtro-fecha/filtro-fecha.component';
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
import { SolpService } from './../solp.service'
import { Solp } from './../Solp';


declare var $: any;

@Component({
    selector: 'generacion2',
    templateUrl: `generacion2.component.html`,
    styleUrls: ['../compras.component.css'],
})
export class Generacion2Component extends ListBaseComponent {

    @Input("model") 
    protected model:Solp;


    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    
    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild('dtp_fecha_pago')
    protected fechaPagoDTP: ElementRef;

  

    constructor(protected service: SolpService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();
        this.categoriaDropdownComponent = new DropdownComponent();
    }


    

    // checkPermisos() { this.securityService.tienePermisoRedirect("GENERACION 1"); }

    // nombreDeObra: string = "";
    // fiscalContrato: string = "";
    // telefono: string = "";
    // mail: String = sessionStorage.getItem("username");

    // solpGeneracion1: SolpGeneracion1 = new SolpGeneracion1();

    solpPaso2Result: any;
    fechaEntrega: any;
    horaEntrega: any;
    

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
        this.setMenuSeccionTab("Generacion2", "Generacion2");
    }

    ngOnInit() {
        this.setTabs();
        // this.model.mail = sessionStorage.getItem("username");
        // this.checkPermisos();
        //this.getData();
    }

    ngAfterViewInit(): void {
        $(document).on("mouseover", '.form_datetime', function () {
        $(".form_datetime").datetimepicker({
        format: 'yyyy-mm-dd',
        language: 'es',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        forceParse: 0,
        showMeridian: 1,
        pickTime: false,
        minView: 2,
        maxView: 4
        });
        });
        }

}
