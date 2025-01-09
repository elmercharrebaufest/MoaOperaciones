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
import { FileUpload } from 'primeng/fileupload';


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

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;
    resultados: ValidationResult[];

    @ViewChild('fileUpload') fileUpload: FileUpload;
    constructor(protected service: FacturaService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService, protected route: ActivatedRoute, protected router: Router) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    checkPermisos() { this.securityService.tienePermisoRedirect("CARGAR FACT PROV"); }

    modalServiceSusbcription: any;
    captchaOk: any = null;
    archivos = new Array<File>()

    setTabs() {
        this.setMenuSeccionTab("factura", "Factura");
    }

    ngOnInit() {
        this.setTabs();
        this.checkPermisos();
        let secciones = [];
        secciones.push(new Seccion('/factura', 'factura', 'Factura'));
        this.navService.setSeccionList(secciones);
    }

    subirPDF() {
        ;
        this.floatMsgService.setMsgsEmpty();
        this.spinnerSmallComponent.showIt();


        if (this.archivos == null) {
            this.spinnerSmallComponent.hideIt();
            this.floatMsgService.setErrorMsg("Ingrese un Archivo");
            return false;
        }

        this.unsubscribe();
        try {
            this.resultados = [];
            this.blockUI.start('Analizando documentos...');
            this.subscription = this.service.subirPDF(this.archivos).subscribe(
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
                    let errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
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
    mensajeIrAOC(mensaje: ValidationResult) {
        return !mensaje.IsValid && mensaje.ValidataionType == "OrdenCompraValidationCommand" && mensaje.Value != "" && mensaje.Value.length > 0;        
    }

    irACertificaciones(nroOC: string) {
        this.navService.navegarSeccionParam('/compras/dashboardCertificacionDeServiciosProveedores', nroOC);
        return false; 
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    vaciarCampos() {
        this.fileUpload.clear();
        this.archivos = new Array<File>();
    }

    public ngOnDestroy() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
        if (this.modalServiceSusbcription != undefined) {
            this.modalServiceSusbcription.unsubscribe();
        }
    }

    uploadHandler(filesUpload: any): boolean {

        for (let file of this.archivos) {
            if (!this.isValidFileType(file)) {
                this.floatMsgService.setErrorMsg(`El archivo ${file.name} no es válido. Solo se permiten archivos PDF, JPG, JPEG y PNG.`);
                this.eliminarAdjuntoNuevo(file); 
                return true;
            }
        }

        let archivoWeb = filesUpload["files"].reduce((sum, file) => sum + file.size, 0);
        this.archivos = filesUpload["files"];
        if (archivoWeb > 10000000) {
            this.floatMsgService.setErrorMsg("El archivo adjuntado no debe superar los 10Mb");
            if (this.archivos.length > 0) {
                this.eliminarAdjuntoNuevo(this.archivos[this.archivos.length - 1])
            }
            return true;
        }
    }

    isValidFileType(file: File): boolean {
        const validTypes = ['.pdf', '.jpg', '.jpeg', '.png'];
        const fileExtension = file.name.split('.').pop().toLowerCase();
        return validTypes.includes(`.${fileExtension}`);
    }

    eliminarAdjuntoNuevo(archivo): void {
        let indice = this.archivos.indexOf(archivo)
        this.archivos.splice(indice, 1)
    }
}