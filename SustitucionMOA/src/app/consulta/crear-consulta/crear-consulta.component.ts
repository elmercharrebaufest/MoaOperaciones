import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConsultaService } from '../consulta.service';
import { ListBaseComponent } from './../../common/base-components/list-base-component';
import { Seccion } from './../../common/models/seccion';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import { NavService } from './../../common/services/NavService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { DropdownComponent } from './../../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../../common/view-child/spinner-small/spinner-small.component';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { SelectItem } from 'primeng/components/common/selectitem';
import { Causa, Comentario, Categoria, Subcategoria, Consulta } from '../consulta';
import { InformeComercialComponent } from '../../alta-proveedores/informe-comercial/informe-comercial.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

declare var $: any;

@Component({
    selector: 'crear-consulta',
    templateUrl: `crear-consulta.component.html`,
    providers: [{ provide: ConsultaService, useClass: ConsultaService }]

})
export class CrearConsultaComponent extends ListBaseComponent {

    @BlockUI() blockUI: NgBlockUI;

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

    mensajeError: string;
    esCorredor: boolean = sessionStorage.getItem("tipoUsuario") === "CORR";
    proveedor: string;
    nombre: string = sessionStorage.getItem("nombre");
    email: string;
    telefono: string;
    categoriaCount = 0;
    subcategoriaCount = 0;

    proveedorId: number;
    tieneSubcategorias: boolean = true;

    consulta: any;

    categorias: Categoria[];
    subcategorias: Subcategoria[];
    causas: Causa[];

    codigoCorredor: any;
    codigoProveedor: any;
    razonSocialProveedor: string = "";
    razonSocialCorredor: string;

    subcategoria: Subcategoria = {
        Id: null
    };
    subcategoriasList: Subcategoria[];
    categoria: Categoria;
    causa: Causa;
    categoriaCode: any;
    subcategoriaCode: any;

    proveedorSelected: any;
    listaArchivos: Array<File> = new Array<File>();

    Detalle: any;
    nuevoComentario: any;
    comprobanteExtra: any;

    caratula: any;
    material: string;
    comentario: string;
    contrato: any;
    razonSocial: string;
    cuit: string;
    nombreVendedor: string;
    comprobante: any;
    fechaPago: string;
    fecha: Date;
    importe: any;
    impuesto: any;
    bolsaEmisoraOblea: string;
    files: FileList = null;

    fechaPagoDP: any;
    visibleButton: boolean = true;
    categoriaSelected: any;
    captchaOk: any = null;
    asunto: string;
    cliente: any;

    fechaFactura: string;


    prueba: any;
    setTabs() {
        this.setMenuSeccionTab("consulta", "crear-consulta");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/consulta/crear-consulta', 'crear-consulta', 'Nueva Consulta'), new Seccion('/consulta/mis-consultas', 'consulta', 'Mis Consultas')]);
        //this.getData();
        this.getCombos();

        if (this.esCorredor) {
            this.codigoCorredor = sessionStorage.getItem("proveedor");
        }
        else {
            this.codigoProveedor = sessionStorage.getItem("proveedor");
        }
        $(".adjuntarArchivo").click(function () {
            $(".adjuntarArchivo1").click();
        });
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
        let file;

        if (fileList.length > 0) {
            this.files = fileList;
            for (let i = 0; i < fileList.length; i++) {
                file = fileList[i];
                this.listaArchivos.push(file);
            }
        }

        let $formInput = $('input[type=file]');
        $formInput.val(null);
    }

    vaciarCamposAdicionales() {
        this.contrato = '';
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
                        this.proveedorId = result.proveedorID
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


    onProveedorSeleccionado(proveedor: any) {
        this.proveedorSelected = proveedor;
        this.proveedorId = proveedor.proveedorId;
    }

    validarConsulta() {
        this.mensajeComponent.setMsgsEmpty();
        if (this.codigoProveedor == "" || !this.codigoProveedor) {
            this.floatMsgService.setErrorMsg("El campo proveedor esta vacio.");
            return true;
        }
        if (this.categoriaCount == 0) {
            this.floatMsgService.setErrorMsg("Por favor seleccione una categoria.");
            return true;
        }
        if (this.tieneSubcategorias && this.subcategoriaCount == 0) {
            this.floatMsgService.setErrorMsg("Por favor seleccione una subcategoria.");
            return true;
        }
        if (this.asunto == "" || !this.asunto) {
            this.floatMsgService.setErrorMsg("El campo Asunto esta vacio.");
            return true;
        }
        if (this.nombre == "" || !this.nombre) {
            this.floatMsgService.setErrorMsg("El campo Nombre esta vacio.");
            return true;
        }
        if (this.nuevoComentario == "" || !this.nuevoComentario) {
            this.floatMsgService.setErrorMsg("El campo Comentario esta vacio.");
            return true;
        }
        if ((this.esCorredor && this.codigoCorredor == "") || (this.esCorredor && !this.codigoCorredor)) {
            this.floatMsgService.setErrorMsg("El campo Corredor esta vacio.");
            return true;
        }
        if (this.categoriaCode == 'REI' && this.subcategoriaCode == 'RET') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;

            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo N° Salida de pago esta vacio.");
                return true;
            }
            if (this.contrato == "" || !this.contrato) {
                this.floatMsgService.setErrorMsg("El campo Contrato esta vacio.");
                return true;
            }
            if (this.impuesto == "" || !this.impuesto) {
                this.floatMsgService.setErrorMsg("El campo Impuesto retenido esta vacio.");
                return true;
            }
            if (this.importe == "" || !this.importe) {
                this.floatMsgService.setErrorMsg("El campo Importe retención esta vacio.");
                return true;
            }
            if (this.fechaPago == "" || !this.fechaPago) {
                this.floatMsgService.setErrorMsg("Falta seleccionar el campo fecha");
                return true;
            }
        }
        if (this.categoriaCode == 'REI' && this.subcategoriaCode == 'PER') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
            if (this.fechaPago == "" || !this.fechaPago) {
                this.floatMsgService.setErrorMsg("Falta seleccionar el campo fecha");
                return true;
            }
            if (this.comprobanteExtra == "" || !this.comprobanteExtra) {
                this.floatMsgService.setErrorMsg("El campo Cliente de pago esta vacio.");
                return true;
            }
            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo N° de factura esta vacio.");
                return true;
            }
            if (this.impuesto == "" || !this.impuesto) {
                this.floatMsgService.setErrorMsg("El campo Impuesto retenido esta vacio.");
                return true;
            }
        }
        if ((this.categoriaCode == 'BOL' && this.subcategoriaCode == 'CON') || (this.categoriaCode == 'BOL' && this.subcategoriaCode == 'REG')) {
            if (this.contrato == "" || !this.contrato) {
                this.floatMsgService.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'BOL' && this.subcategoriaCode == 'OPC') {
            this.fechaPago = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
            if (this.contrato == "" || !this.contrato) {
                this.floatMsgService.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
            if (this.bolsaEmisoraOblea == "" || !this.bolsaEmisoraOblea) {
                this.floatMsgService.setErrorMsg("El campo bolsa Emisora de Oblea esta vacio.");
                return true;
            }
            if (this.files == null || this.files.length < 2) {
                this.floatMsgService.setErrorMsg("Falta adjuntar liquidación y la oblea emitida por bolsa");
                return true;
            }
            if (this.fechaPago == "" || !this.fechaPago) {
                this.floatMsgService.setErrorMsg("Falta seleccionar el campo fecha");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'IMP') {
            if (this.impuesto == "" || !this.impuesto) {
                this.floatMsgService.setErrorMsg("El campo Impuesto esta vacio.");
                return true;
            }
            if (this.files == null || this.files.length < 1) {
                this.floatMsgService.setErrorMsg("Falta adjuntar constancia");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'INF') {
            if (this.files == null || this.files.length < 1) {
                this.floatMsgService.setErrorMsg("Falta adjuntar Informe Comercial");
                return true;
            }
        }
        if (this.categoriaCode == 'ACT' && this.subcategoriaCode == 'CAP') {
            if (this.files == null || this.files.length < 1) {
                this.floatMsgService.setErrorMsg("Falta adjuntar Carta presentacón");
                return true;
            }
        }
        if ((this.categoriaCode == 'PAR' && this.subcategoriaCode == 'NROR') || (this.categoriaCode == 'FIN' && this.subcategoriaCode)) {
            if (this.contrato == "" || !this.contrato) {
                this.floatMsgService.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo N° COE esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'CAL') {
            if ((this.contrato == "" || !this.contrato) && (this.comprobante == "" || !this.comprobante)) {
                this.floatMsgService.setErrorMsg("Debe completar Campo N° de contrato o CCPP.");
                return true;
            }
        }
        if (this.categoriaCode == 'COM') {
            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo N° de Factura esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'APP') {
            if (this.comprobanteExtra == "" || !this.comprobanteExtra) {
                this.floatMsgService.setErrorMsg("El campo Material esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'PES') {
            if (this.contrato == "" || !this.contrato) {
                this.floatMsgService.setErrorMsg("El campo N° de contrato esta vacio.");
                return true;
            }
        }
        if ((this.categoriaCode == 'PROVG' && this.subcategoriaCode == 'VENC') || (this.categoriaCode == 'PROVG' && this.subcategoriaCode == 'POTR')) {
            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo N° de Factura esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'MATBA' && this.subcategoriaCode == 'CAL') {
            if ((this.comprobante == "" || !this.comprobante) && (this.comprobanteExtra == "" || !this.comprobanteExtra)) {
                this.floatMsgService.setErrorMsg("Debe completar Campo carátula o CCPP.");
                return true;
            }
        }
        if (this.categoriaCode == 'FLET' && this.subcategoriaCode == 'PDF') {
            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo N° de Proforma esta vacio.");
                return true;
            }
        }
        if (this.categoriaCode == 'FLET' && this.subcategoriaCode == 'CCP') {
            if (this.comprobanteExtra == "" || !this.comprobanteExtra) {
                this.floatMsgService.setErrorMsg("El campo N° de Proforma esta vacio.");
                return true;
            }
            if (this.comprobante == "" || !this.comprobante) {
                this.floatMsgService.setErrorMsg("El campo CCPP esta vacio.");
                return true;
            }
        }
    }

    postConsulta() {
        this.blockUI.start('Generando Consulta');
        this.spinnerComponent.showIt();

        if (this.esCorredor) {
            this.codigoProveedor = this.proveedorSelected.idVendedor;
            this.razonSocialProveedor = this.proveedorSelected.descVendedor;
            this.razonSocialCorredor = this.nombre;
        }
        else {
            this.razonSocialProveedor = this.nombre;
        }

        if (this.validarConsulta()) {
            this.spinnerComponent.hideIt();
            this.blockUI.stop();
            return;
        }

        if(this.fechaPago != '' && this.fechaPago != null){
        var dateParts = this.fechaPago.split("-");
        this.fecha = new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2]);
        } 
    
        this.Detalle = {Consulta_Id: 0, Fecha: this.fecha, ComprobanteNo: this.comprobante, OtroComprobanteNo: this.comprobanteExtra, 
            ContratoNo: this.contrato, Importe: this.importe, Impuesto: this.impuesto, BolsaEmisoraOblea: this.bolsaEmisoraOblea
        }

        this.consulta = {
            CodigoCorredor: this.codigoCorredor, RazonSocialCorredor: this.razonSocialCorredor,
            CodigoProveedor: this.codigoProveedor, RazonSocialProveedor: this.razonSocialProveedor,
            Categoria_Id: this.categoria.Id, Detalle: this.Detalle,
            SubCategoria_Id: this.subcategoria.Id, Asunto: this.asunto
        }

        let comentario: Comentario = {consulta_Id: 0, Detalle: this.nuevoComentario, Fecha: new Date()};

        try {
            this.subscription = this.service.AgregarConsulta(this.consulta, comentario, this.listaArchivos).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                        this.blockUI.stop();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                        this.blockUI.stop();
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                        this.blockUI.stop();
                    } else {
                        this.spinnerComponent.hideIt();
                        this.blockUI.stop();
                        this.goToSeccion('/consulta/mis-consultas');
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

    setSubcategorias(categoria){
        this.categoriaCount = 1;
        this.subcategoriaCode = null;
        this.categoria = categoria;
        this.categoriaCode = categoria.Code;
        this.subcategoriasList = [];

        this.Avisos(this.categoriaCode);

        this.subcategorias.forEach(x => {
            if(x.CategoriaId == categoria.Id){
                if(x.Code == "INF" && this.esCorredor){
                }
                else if(x.Code == "CAP" && !this.esCorredor){
                }
                else
                {
                    this.subcategoriasList.push(x);
                }
            }
        });

        if (this.subcategoriasList.length == 0) {
            this.tieneSubcategorias = false;
        }
        else {
            this.tieneSubcategorias = true;
        }
    }

    borrarArchivo(i: number)
    {
        this.listaArchivos.splice(i, 1);
    }

    Avisos(categoriaCode) {
        this.mensajeComponent.setMsgsEmpty();
        if (categoriaCode == "PROVG") {
            this.mensajeComponent.setInfoMsg("Texto a definir");
            return true;
        }
        if (categoriaCode == "BOL" && this.subcategoriaCode == "OPC") {
            this.mensajeComponent.setInfoMsg("Recuerde Adjuntar liquidación y la oblea emitida por bolsa");
            return true;
        }
        if (categoriaCode == "ACT" && this.subcategoriaCode == "IMP") {
            this.mensajeComponent.setInfoMsg("Recuerde Adjuntar Constancia");
            return true;
        }
        this.mensajeComponent.setMsgsEmpty();
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    setCodeSubcategoria(subcategoria) {
        this.subcategoriaCount = 1;
        this.subcategoriaCode = subcategoria.Code;
        this.Avisos(this.categoriaCode)
    }


}