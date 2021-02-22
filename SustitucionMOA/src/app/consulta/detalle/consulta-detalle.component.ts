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
import { Comentario, Categoria, EstadoConsulta, Subcategoria } from '../consulta';

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
    categoriaSelected: any;
    estados: EstadoConsulta[];
    categorias: Categoria[];
    subcategorias: Subcategoria[];
    
    categoriasList: any;
    subcategoriasList: Subcategoria[];

    estadoId: number;
    categoriaId: number;
    prueba: any;

    consulta: any;
    comentariosList: any;
    estadoConsulta: number;
    consultaId: string;
    file: any;
    fecha: any;
    hora: any;
    username = sessionStorage.getItem("userName");
    detalle: string = "";

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    setTabs() {
        this.setMenuSeccionTab("consulta", "detalle");
    }

    ngOnInit(){
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')]);
        this.jqueryOnInit();
        this.getConsultaId();
        this.getDetalleConsulta();
        this.getCombos();
    }

    getConsultaId(){
        const queryString = window.location.href;
        this.consultaId = queryString.split('=')[1];
    }
    
    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
    }

    actualizarCombos() {           
        this.spinnerSmallComponent.showIt();
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        
        this.categoriaId = $("#categoriaSelect").children("option:selected").val();
        this.estadoId = $("#estadoSelect").children("option:selected").val();
        this.subscription = this.service.actualizarCombos(this.consultaId, this.estadoId, this.categoriaId).subscribe(
            result => {
                this.spinnerComponent.hideIt();
                this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                    else{
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.spinnerModal.hideIt();
                }
            );
    }

    postComentario(){
        let comentario: Comentario = {consulta_Id: this.consultaId, Detalle: this.detalle, Fecha: new Date()};
        this.subscription = this.service.agregarComentario(this.consultaId, comentario).subscribe(
            result => {
                this.getDetalleConsulta();
                this.spinnerModal.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                },
                error => {
                    this.spinnerModal.hideIt();
                }
            );
        this.detalle = "";
    }

    getCombos() {
        try {
            this.subscription = this.service.getCombos().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.categorias = result.categorias;
                        this.estados = result.estados;
                        this.subcategorias = result.subcategorias;
                    }
                },
                error => {
                    this.floatMsgService.setErrorMsg(error.message);
                }

                );
        } catch (e) {
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    getDetalleConsulta(){
        this.subscription = this.service.getDetalleConsulta(this.consultaId).subscribe(
            result => {
                this.consulta = result;
                this.consulta.Comentarios.forEach(x => {
                    x.Fecha = new Date (this.getDateFromAspNetFormat(x.Fecha));
                    x.Fecha = this.convertDate(x.Fecha);
                });
                this.consulta.FechaCreacion = new Date (this.getDateFromAspNetFormat(this.consulta.FechaCreacion));
                this.consulta.FechaCreacion = this.convertDate(this.consulta.FechaCreacion).substring(0, 10);
                this.spinnerModal.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                },
                error => {
                    this.spinnerModal.hideIt();
                }
            );
        }


    jqueryOnInit(){
        $(".adjuntarArchivo").click(function () {
            $(".adjuntarArchivo1").click();
        });
        $('.enviarComentario').click(function(e){
            e.preventDefault()
        });
    }
}
