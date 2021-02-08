import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { element } from '@angular/core/src/render3/instructions';
import { Seccion } from '../../common/models/seccion';
import { ConsultaService } from '../consulta.service';
import { BaseComponent } from '../../common/base-components/base-component';

declare var $: any;

@Component({
    selector: 'consulta-detalle',
    templateUrl: `consulta-detalle.component.html`,
    providers: [{ provide: ConsultaService, useClass: ConsultaService }]

})
export class DetalleConsultaComponent extends BaseComponent {

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

    constructor(private route: ActivatedRoute, protected service: ConsultaService, protected navService: NavService,
        protected securityService: SecurityService,
        protected sessionDataService: SessionDataService,
        protected floatMsgService: FloatMsgService,
        protected modalService: ModalService) {
            super(navService, securityService, floatMsgService, modalService);
            this.mensajeComponent = new MensajeComponent();
            this.spinnerComponent = new SpinnerComponent();
    }

    ComentariosList = [
        {id: 0, nombre: "Martin", comentario: "Comentario 1, lorem ipsum.\nDol sit a ver", fecha: "11/10/2021", hora: "15:30"},
        {id: 1, nombre: "juan", comentario: "Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo.", fecha: "11/10/2021", hora: "15:30"},
        {id: 2, nombre: "Martin", comentario: "Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo.", fecha: "13/10/2021", hora: "15:30"},
        {id: 3, nombre: "juan", comentario: "Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo. Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo. Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo. Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo.Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo. Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo. \n Comentario 2, lorem ipsum dol sit a ver esto es un texto mas largo, mas largo, mas largo.", fecha: "14/10/2021", hora: "15:30"},
        {id: 4, nombre: "Martin", comentario: "ESTAMOS EN LA B.", fecha: "14/10/2021", hora: "15:30"},
        {id: 5, nombre: "Martin", comentario: "ok como estas ?", fecha: "14/10/2021", hora: "15:30"}
    ];

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    setTabs() {
        this.setMenuSeccionTab("consulta", "detalle");
    }

    ngOnInit(){
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);

        // this.route.params.forEach((params: Params) => {
        //     let idConsulta = params['idConsulta'];
        //     this.unsubscribe();
        //     this.subscription = this.service.getConsultaDetalle(idConsulta).subscribe(
        //         result => {
        //             if (result.logout == true) {
        //                 this.sessionDataService.logout();
        //             } else if (result.error != undefined && result.error != "") {
        //                 this.mensajeComponent.setErrorMsg(result.error);
        //             } else if (result.info != undefined) {
        //                 this.mensajeComponent.setInfoMsg(result.info);
        //             } else {
        //                 this.ComentariosList = [];

        //                 result.Comentarios.forEach(x => {
        //                     this.ComentariosList.push(
        //                         {
        //                             id: x.Id,
        //                             nombre: 'Martin',
        //                             comentario: x.Detalle,
        //                             fecha: '14/01/2021',
        //                             hora: '15:30'
        //                         }
        //                     );
        //                 });
        //             }
        //         },
        //         error => {
        //             this.mensajeComponent.setErrorMsg(error.message);
        //         }
        //     );
        // });
    }
}