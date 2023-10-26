import { Component, ViewChild, ElementRef } from '@angular/core';
import { NavService } from '../../common/services/NavService';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { SecurityService } from '../../common/services/SecurityService';
import { Seccion } from '../../common/models/seccion';
import { SessionDataService } from '../../common/services/SessionDataService';
import { ModalService } from '../../common/services/ModalService';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { LiquidacionInformarService, LiquidacionService } from '../liquidacion.service';
import { LiquidacionBaseComponent } from '../liquidacion.component';
import { BlockUI, NgBlockUI } from 'ng-block-ui';

@Component({
    selector: 'app-liquidacion-informar',
    templateUrl: `liquidacion.informar.component.html`,
    providers: [{ provide: LiquidacionService, useClass: LiquidacionInformarService }]
})
export class LiquidacionInformarComponent extends LiquidacionBaseComponent{

    @ViewChild('fileInput')
    protected fileInput: ElementRef;

    @ViewChild('recaptchaComponent')
    protected captcha: ReCaptchaComponent;

    @BlockUI() blockUI: NgBlockUI;

    constructor(protected service: LiquidacionInformarService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(service, navService, sessionDataService, securityService, floatMsgService, modalService);
    }

    files: File[] = [];
    captchaOk: any = null;

    ngOnInit(){
        this.setTabs();
        this.checkPermisos();
        this.navService.setSeccionList([new Seccion('/liquidacion/informar', 'liquidacion', 'Informar'),
            new Seccion('/liquidacion/informada', 'liquidacion', 'Informadas'),
            new Seccion('/liquidacion/aprobada', 'liquidacion', 'Aprobadas'),
            new Seccion('/liquidacion/observada', 'liquidacion', 'Observadas'),
            new Seccion('/liquidacion/paga', 'liquidacion', 'Pagas'),
          
            ]);
    }

    setTabs() {
        this.setMenuSeccionTab("liquidacion", "Informar");
    }

    subirPDF() {;
        this.floatMsgService.setMsgsEmpty();
        this.blockUI.start('Informando liquidaciones');


        if (this.files == null || this.files.length <= 0) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg("Ingrese al menos una Liquidación");
            return false;
        }

        if (this.captchaOk == null) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg("Debe completar el Captcha");
            return false;
        }

        this.unsubscribe();
        try {
            this.subscription = this.service.notificarLiquidaciones(this.files).subscribe(
                (result:any) => {
                    this.blockUI.stop();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.floatMsgService.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.floatMsgService.setInfoMsg(result.info);
                    } else {
                        this.vaciarCampos();
                        this.floatMsgService.setSuccessMsg(result.data);
                    }
                    return false;
                },
                error => {
                    var errormsj = "Ha ocurrido un error, por favor intentelo nuevamente";
                    if (error._body.indexOf("length exceeded") >= 0) { errormsj = "El tamaño del archivo supera los 3 MBs permitidos"; }
                    this.blockUI.stop();
                    this.floatMsgService.setErrorMsg(errormsj);
                }

            );
        } catch (e) {
            this.blockUI.stop();
            this.floatMsgService.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    cargarArchivo(event: any) {
        let archivosNuevos = [];
        if(archivosNuevos.length + event.target.files.length > 10){
            this.floatMsgService.setMsgsEmpty();
            this.floatMsgService.setInfoMsg('El límite máximo de archivos es 10. Por favor, revise los archivos e intente nuevamente.');
            this.vaciarCampos();
        }
        else{
            for(let file of event.target.files){
                archivosNuevos.push(file);
            }
            this.files = archivosNuevos;
        }
    }

    handleCorrectCaptcha(event: any) {
        this.captchaOk = event;
    }

    vaciarCampos() {
        this.files = [];
        this.fileInput.nativeElement.value = "";
    }

    public ngOnDestroy() {
        this.floatMsgService.setMsgsEmpty();
        this.unsubscribe();
    }
}