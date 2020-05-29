import { Component, ViewChild, OnDestroy, Renderer } from '@angular/core';
import { Router } from "@angular/router";
import { LoginService } from './../login.service';
import { LoginCommonComponent } from './../login.component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { SessionDataService } from './../../common/services/SessionDataService';
import { SecurityService } from './../../common/services/SecurityService';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { ModalService } from './../../common/services/ModalService';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Component({
    selector: 'cambio-contrasenia',
    templateUrl: `./app/login/registro/login.registro.component.html?v=${new Date().getTime()}`,
    providers: [LoginService]
})
export class RegistroUsuarioComponent extends LoginCommonComponent implements OnDestroy {

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: LoginService, protected renderer: Renderer, protected sessionDataService: SessionDataService, protected router: Router, protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(sessionDataService, router, renderer, navService, securityService, floatMsgService, modalService);
        this.renderer.setElementClass(document.body, 'loginBody', true);

        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    numeroProveedor: string;
    claveActivacion: string;
    username: string;
    contrasenia: string;
    contraseniaConfirmacion: string;
    visibleButton: boolean = true;

    registrar() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        this.unsubscribe();
        try {
            this.validarContrasenias();
            this.subscription = this.service.registrar(this.numeroProveedor, this.claveActivacion, this.username, this.contrasenia).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.loginUser(result);
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

    validarContrasenias() {
        this.contraseniaSinValor(this.contrasenia, "Ingresada");
        this.contraseniaSinValor(this.contraseniaConfirmacion, "de Confirmación");
        if (this.contrasenia != this.contraseniaConfirmacion)
            throw "La contraseña Ingresada y de Confirmación deben ser iguales"
    }

    contraseniaSinValor(contrasenia: string, nombreCampo: string) {
        if (contrasenia == undefined || contrasenia == null || contrasenia == "")
            throw "Complete el campo " + nombreCampo;
    }

    ngOnDestroy() {
        this.renderer.setElementClass(document.body, 'loginBody', false);
    }

}