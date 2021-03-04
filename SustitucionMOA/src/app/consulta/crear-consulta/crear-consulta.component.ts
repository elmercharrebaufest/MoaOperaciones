import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ConsultaService } from '../consulta.service';
import { FiltroFechaComponent } from './../../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../../common/base-components/list-base-component'
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { Seccion } from './../../common/models/Seccion';
import { ModalService } from './../../common/services/ModalService';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { SelectItem } from 'primeng/components/common/selectitem';
import { Causa, Comentario, Categoria, Subcategoria, Consulta } from '../consulta';
import { InformeComercialComponent } from '../../alta-proveedores/informe-comercial/informe-comercial.component';

declare var $: any;

@Component({
    selector: 'crear-consulta',
    templateUrl: `crear-consulta.component.html`,
    providers: [{ provide: ConsultaService, useClass: ConsultaService }]

})
export class CrearConsultaComponent extends ListBaseComponent {

    @ViewChild('dropdown_categoria')
    protected categoriaDropdownComponent: DropdownComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild('fileInput')
    protected fileInput: ElementRef;

    @ViewChild('dtp_fecha_pago')
    protected fechaPagoDTP: ElementRef;

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;

    constructor(protected service: ConsultaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }
    

    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    proveedor: string;
    nombre: string = sessionStorage.getItem("nombre");
    email: string;
    telefono: string;

    proveedorId: number;
    
    consulta: any;

    categorias: Categoria[];
    subcategorias: Subcategoria[];
    causas: Causa[];

    codigoCorredor: any;
    codigoProveedor: any;
    razonSocialProveedor: string;
    razonSocialCorredor: string;

    subcategoria: Subcategoria;
    subcategoriasList: Subcategoria[];
    categoria: Categoria;
    causa: Causa;
    categoriaCode: any;
    subcategoriaCode: any;


    Detalle: any;
    nuevoComentario: any;

    comentario: string;
    contrato: string;
    razonSocial: string;
    cuit: string;
    nombreVendedor: string;
    comprobante: string;
    fechaPago: string;
    importe: string;
    impuesto: string;
    file: any;

    files: FileList;

    fechaPagoDP: any;
    visibleButton: boolean = true;
    categoriaSelected: any;
    captchaOk: any = null;
    asunto: string;
    cliente: any;

    fechaFactura: string;

    setTabs() {
        this.setMenuSeccionTab("consulta", "crear-consulta");
    }

    ngOnInit() {      
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')]);
        //this.getData();
        this.getCombos();

        if(this.esCorredor){
            this.codigoCorredor = sessionStorage.getItem("proveedor");
        }
        else{
            this.proveedor = sessionStorage.getItem("proveedor");
        }
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

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.files = fileList;
        }
    }

    vaciarCamposAdicionales() {
        this.contrato= '';
        this.razonSocial = '';
        this.cuit = '';
        this.nombreVendedor = '';
        this.comprobante = '';
        this.fechaPago = '';
        this.importe = '';
        this.impuesto = '';
    }

    vaciarCampos() {
        this.vaciarCamposAdicionales();
        this.proveedor = "";
        this.nombre = "";
        this.email = "";
        this.telefono = "";
        this.categoriaDropdownComponent.setSelectItem("");
        this.comentario = "";
        this.file = null;
        this.fileInput.nativeElement.value = "";
    }

    getCombos() {
        this.unsubscribe();
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
                        this.subcategorias = result.subcategorias;
                        this.causas = result.causas;
                        this.proveedorId = result.proveedorId;
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

    postConsulta(){
        this.spinnerComponent.showIt();

        if(this.proveedor[0] == 'c'){
            this.codigoCorredor = this.proveedor;
            this.codigoProveedor = null;
            this.razonSocialCorredor = this.nombre;
            this.razonSocialProveedor = null;
        }
        else{
            this.codigoProveedor = this.proveedor;
            this.codigoCorredor = null;
            this.razonSocialCorredor = null;
            this.razonSocialProveedor = this.nombre;
        }

        this.Detalle = {Consulta_Id: null, Fecha: this.fechaPago, ComprobanteNo: this.comprobante, ContratoNo: this.contrato,
            Importe: this.importe, Impuesto: this.impuesto, BolsaEmisoraOblea: null, CausaConsulta_Id: 1
        }

        this.consulta = {CodigoCorredor: this.codigoCorredor, RazonSocialCorredor: this.razonSocialCorredor, 
            CodigoProveedor: this.codigoProveedor, RazonSocialProveedor: this.razonSocialProveedor, 
            Categoria_Id: this.categoria.Id, Detalle: this.Detalle,
            SubCategoria_Id: this.subcategoria.Id, Asunto: this.asunto
        }

        try {
            this.subscription = this.service.AgregarConsulta(this.consulta).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.postComentario(result.Id, this.nuevoComentario);
                        this.spinnerComponent.hideIt();
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
    }

    postComentario(consultaId, detalle){
        let comentario: Comentario = {consulta_Id: consultaId, Detalle: detalle, Fecha: new Date()};
        this.subscription = this.service.agregarComentario(consultaId, comentario).subscribe(
            result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    }
                    else{      
                        this.postArchivos(result.Id, consultaId);
                        this.files = null;
                    }
                },
            );
    }

    postArchivos(comentarioId, consultaId){
        if(this.files){
            debugger;
            this.subscription = this.service.adjuntar(this.files, consultaId, comentarioId).subscribe(
                result => {
                        if (result.logout == true) {
                            this.sessionDataService.logout();
                        } else if (result.error != undefined && result.error != "") {
                            this.mensajeComponent.setErrorMsg(result.error);
                        } else if (result.info != undefined) {
                            this.mensajeComponent.setInfoMsg(result.info);
                        }
                    },
                );
        }
    }

    setSubcategorias(categoria){
        this.categoria = categoria;
        this.categoriaCode = categoria.Code;
        this.subcategoriasList = [];

        this.subcategorias.forEach(x => {
            if(x.CategoriaId == categoria.Id){
                this.subcategoriasList.push(x);
            }
        });
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    setCodeSubcategoria(subcategoria){
        this.subcategoriaCode = subcategoria.Code;
    }


}