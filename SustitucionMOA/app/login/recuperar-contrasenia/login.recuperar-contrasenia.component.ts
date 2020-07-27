import { Component, ViewChild, OnDestroy, Renderer } from '@angular/core';
import { LoginService } from './../login.service';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { BaseComponent } from './../../common/base-components/base-component';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-login-recuerar-contrasenia',
    templateUrl: `./app/login/recuperar-contrasenia/login.recuperar-contrasenia.component.html?v=${new Date().getTime()}`,
    providers: [LoginService]
})
export class RecuperarContraseniaComponent extends BaseComponent implements OnDestroy {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: LoginService, private renderer: Renderer, protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.renderer.setElementClass(document.body, 'loginBody', true);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    username: string;
    visibleButton: boolean = true;

    recuperarContrasenia() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.subscription = this.service.recuperarContrasenia(this.username).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setSuccessMsg(result.data);
                    }
                },
                error => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }

            );
        } catch (e) {
            this.visibleButton = true;
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }

        return false; //<-- Prevent Refresh
    }

    ngOnDestroy() {
        this.renderer.setElementClass(document.body, 'loginBody', false);
    }

}