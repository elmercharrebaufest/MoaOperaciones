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
import { Comentario, Categoria, Subcategoria } from '../consulta';
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

    proveedor: string;
    nombre: string;
    email: string;
    telefono: string;
    proveedorId = 24760;

    categorias: Categoria[];
    subcategorias: Subcategoria[];

    subcategoria: Subcategoria;
    subcategoriasList: Subcategoria[];
    categoria: Categoria;
    categoriaCode: any;
    subcategoriaCode: any;

    comentario: string;
    contrato: string;
    razonSocial: string;
    cuit: string;
    nombreVendedor: string;
    comprobante: string;
    fechaPago: string;
    importe: string;
    impuesto: string;
    inscripcion: string;
    motivo: string;
    file: any;
    camposAdicionales: boolean = false;
    fechaPagoDP: any;
    visibleButton: boolean = true;
    categoriaSelected: any;
    captchaOk: any = null;
    asunto: string;

    setTabs() {
        this.setMenuSeccionTab("consulta", "crear-consulta");
    }

    ngOnInit() {      
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')]);
        //this.getData();
        this.getCombos();
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

        this.obtenerSubcategoria();
    }

    obtenerSubcategoria(){
        $("#categoriaSelect").change(function() {
            var id = $("#categoriaSelect").children("option:selected").val();

            this.categoriaOptions = this.getCategorias();
            //this.categoriaSelected = this.categoriaOptions.filter(x => x.Id == id)[0];
            this.categoriaOptions.forEach(x => {
                if(x.Id == id){
                    this.categoriaSelected = x;
                }
            });

            if (this.categoriaSelected != null) {
                if (this.categoriaSelected.camposAdicionales === "A"){
                    this.camposAdicionales = true;
                } else {
                    this.camposAdicionales = false;
                    this.vaciarCamposAdicionales();
                }
            }
        });
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
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
        this.inscripcion = '';
        this.motivo = '';
    }

    vaciarCampos() {
        this.camposAdicionales = false;
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
                        /*
                        this.categoriasList = [];
                        this.categorias.forEach(x => this.categoriasList.push({ label: x.Nombre, value: x.Id})); */
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