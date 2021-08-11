import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { ContactoMailService } from './contacto-mail.service';
import { FiltroFechaComponent } from './../common/view-child/filtro-fecha/filtro-fecha.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { Seccion } from './../common/models/seccion';
import { ModalService } from './../common/services/ModalService';
import { ReCaptchaComponent } from 'angular2-recaptcha';


declare var $: any;

@Component({
    selector: 'app-contacto-mail',
    templateUrl: `contacto-mail.component.html`,
    providers: [ContactoMailService]

})
export class ContactoMailComponent extends ListBaseComponent {

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

    constructor(protected service: ContactoMailService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.categoriaDropdownComponent = new DropdownComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CONTACTO MAIL"); }

    proveedor: string;
    nombre: string;
    email: string;
    telefono: string;
    categoriaOptions: Array<DropdownOption> = [];
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

    setTabs() {
        this.setMenuSeccionTab("contacto", "Contacto");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([]);
        //this.getData();
    }

    ngAfterViewInit(): void {
        this.getCategorias();
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

    sendContactoMail() {
        ;
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();

        try { this.categoriaSelected.label } catch {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Debe seleccionar una Categoria");
            return false;
        }

        if (this.captchaOk == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Debe completar el Captcha");
            return false;
        }

        try {
            var fecha = this.fechaPagoDTP.nativeElement.value
        } catch { }

        this.unsubscribe();
        try {
            this.subscription = this.service.sendContactoMail(
                this.proveedor,
                this.nombre,
                this.email,
                this.telefono,
                this.categoriaSelected.label,
                this.categoriaSelected.camposAdicionales,
                this.comentario,
                this.contrato,
                this.razonSocial,
                this.cuit,
                this.nombreVendedor,
                this.comprobante,
                fecha,
                this.importe,
                this.impuesto,
                this.inscripcion,
                this.motivo,
                this.file
            ).subscribe(
                (result:any) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.vaciarCampos();
                        this.captcha.reset();
                        this.floatMsgService.setSuccessMsg(result.data);
                    }
                    return false;
                },
                error => {
                    var errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
                    if (error._body.indexOf("length exceeded") >= 0) { errormsj = "El tamaño del archivo supera los 3 MBs permitidos"; }
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setErrorMsg(errormsj);
                }

            );
        } catch (e) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    setCategoria(categoria: any) {

        this.categoriaSelected = this.categoriaOptions.filter(x => x.value == categoria)[0];


        if (this.categoriaSelected != null && this.categoriaSelected != undefined) {
            if (this.categoriaSelected.camposAdicionales === "A") {
                this.camposAdicionales = true;
            } else {
                this.camposAdicionales = false;
                this.vaciarCamposAdicionales();
            }
        }

    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            this.file = fileList[0];
        }
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

    getCategorias() {
        this.unsubscribe();
        try {
            this.subscription = this.service.getCategorias().subscribe(
                (result:any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.categoriaOptions = result.data;
                        this.categoriaOptions.forEach(cat => {
                            if(cat.label == 'ACTUALIZACIONES'){
                                this.categoriaSelected = cat;
                            }
                        });
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

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }


}