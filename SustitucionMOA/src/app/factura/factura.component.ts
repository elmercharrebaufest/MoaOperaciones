import { Component, OnInit, ViewChild, OnDestroy, ElementRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FacturaService } from './factura.service';
import { SpinnerComponent } from './../common/view-child/spinner/spinner.component';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { ListBaseComponent } from './../common/base-components/list-base-component'
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { SecurityService } from './../common/services/SecurityService';
import { Seccion } from './../common/models/seccion';
import { BaseComponent } from './../common/base-components/base-component';
import { SessionDataService } from './../common/services/SessionDataService';
import { ModalService } from './../common/services/ModalService';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { DropdownComponent, DropdownOption } from './../common/view-child/dropdown/dropdown.component';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { ValidationResult } from '../common/models/validationResult';


declare var $: any;


@Component({
    selector: 'app-factura',
    templateUrl: `factura.component.html`,
    providers: [FacturaService]
})
export class FacturaComponent extends ListBaseComponent {
    @BlockUI() blockUI: NgBlockUI;

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    public spinnerSmallComponent: SpinnerSmallComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    @ViewChild('fileInput')
    protected fileInput: ElementRef;

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;
    resultados: ValidationResult[];

    constructor(protected service: FacturaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CARGAR FACT PROV"); }

    modalServiceSusbcription: any;
    file: FileList;
    captchaOk: any = null;


    setTabs() {
        this.setMenuSeccionTab("factura", "Factura");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        var secciones = [];
        secciones.push(new Seccion('/factura', 'factura', 'Factura'));
        this.navService.setSeccionList(secciones);
    }

    subirPDF() {
        ;
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();


        if (this.file == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Ingrese un Archivo");
            return false;
        }

        //if (this.captchaOk == null) {
        //    this.spinnerSmallComponent.hideIt();
        //    this.floatMsgService.setErrorMsg("Debe completar el Captcha");
        //    return false;
        //}

        this.unsubscribe();
        try {
            this.resultados = [];
            this.blockUI.start('Scanneandno documentos...');
            this.subscription = this.service.subirPDF(this.file).subscribe(
                (result: any) => {
                    this.spinnerSmallComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.vaciarCampos();
                        this.resultados = result.data as ValidationResult[];
                        console.log(this.resultados);
                    }
                    this.blockUI.stop();
                    return false;
                },
                error => {
                    var errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
                    if (error._body.indexOf("length exceeded") >= 0) { errormsj = "El tamaño del archivo supera los 3 MBs permitidos"; }
                    this.spinnerSmallComponent.hideIt();
                    this.floatMsgService.setErrorMsg(errormsj);
                    this.blockUI.stop();
                }

            );
        } catch (e) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg(e);
            this.blockUI.stop();
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    }

    cargarArchivo(event: any) {
        let fileList: FileList = event.target.files;
        this.file = fileList;
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    vaciarCampos() {

        this.file = null;
        this.fileInput.nativeElement.value = "";
    }

    public ngOnDestroy() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        if (this.modalServiceSusbcription != undefined) {
            this.modalServiceSusbcription.unsubscribe();
        }
    }
}