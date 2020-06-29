import { Component, ViewChild, OnInit, Renderer, OnDestroy } from '@angular/core';
import { Router } from "@angular/router";
import { LoginService } from './login.service';
import { MensajeComponent } from './../common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from './../common/view-child/spinner-small/spinner-small.component';
import { SessionDataService } from './../common/services/SessionDataService';
import { SecurityService } from './../common/services/SecurityService';
import { NavService } from './../common/services/NavService';
import { FloatMsgService } from './../common/services/FloatMsgService';
import { BaseComponent } from './../common/base-components/base-component';
import { ModalService } from './../common/services/ModalService';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';


export class LoginCommonComponent extends BaseComponent {

    constructor(protected sessionDataService: SessionDataService, protected router: Router, protected renderer: Renderer, protected navService: NavService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.renderer.setElementClass(document.body, 'wrapper', false);
    }

    loginUser(result: any) {
        sessionStorage.setItem("username", result.username);
        sessionStorage.setItem("nombre", result.nombre);
        sessionStorage.setItem("proveedor", result.proveedor);
        sessionStorage.setItem("granosFlag", result.granosFlag);
        sessionStorage.setItem("tipoUsuario", result.tipoUsuario);
        sessionStorage.setItem("noticias", JSON.stringify(result.noticias));
        sessionStorage.setItem("permisos", JSON.stringify(result.permisos));
        this.sessionDataService.setNombre(result.nombre);
        this.sessionDataService.setUsername(result.username);
        this.sessionDataService.setProveedor(result.proveedor);
        this.sessionDataService.setTipoUsuario(result.tipoUsuario);
        this.sessionDataService.setNoticias(result.noticias);
        this.sessionDataService.setPermisos(result.permisos);
        this.sessionDataService.setGranosFlag(result.granosFlag);

        if (result.tipoUsuario == "ADMP" || result.tipoUsuario == "ADNA" || result.tipoUsuario == "RYDD") {
            this.navService.navegarSeccion('/aduana/pesada-online');
        } else if (result.tipoUsuario == "CLIE") {
            this.navService.navegarSeccion('/cuenta-corriente/simple');
        } else {
            if (result.granosFlag == "A") {
                sessionStorage.setItem("granosSelected", "G");
                this.navService.navegarSeccion('/home');
            } else {
                sessionStorage.setItem("granosSelected", result.granosFlag);
                if (result.granosFlag == "G") {
                    this.navService.navegarSeccion('/home');
                } else {
                    this.navService.navegarSeccion('/home-ngs');
                }
            }
        }
        
    }
}

@Component({
    selector: 'app-login',
    templateUrl: `./app/login/login.component.html?v=${new Date().getTime()}`,
    providers: [LoginService]
})
export class LoginComponent extends LoginCommonComponent implements OnInit, OnDestroy {

    constructor(private service: LoginService, protected sessionDataService: SessionDataService, protected navService: NavService, protected router: Router, protected renderer: Renderer, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(sessionDataService, router, renderer, navService, securityService, floatMsgService, modalService);
        this.renderer.setElementClass(document.body, 'loginBody', true);

        this.mensajeComponent = new MensajeComponent();
        this.spinnerSmallComponent = new SpinnerSmallComponent();
    }

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');
    }

    titulo = "";
    loginResponse: any;
    username = "";
    pass = "";
    loginButtonEnable = true;
    captchaOk : any = null;

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    private spinnerSmallComponent: SpinnerSmallComponent;

    login() {
        this.mensajeComponent.setMsgsEmpty();
        this.loginButtonEnable = false;
        this.spinnerSmallComponent.showIt();
        this.unsubscribe();
        this.subscription = this.service.login(this.username, this.pass).subscribe(
            result => {
                this.spinnerSmallComponent.hideIt();
                this.loginButtonEnable = true;
                if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else if (result.info != undefined) {
                    this.mensajeComponent.setInfoMsg(result.info);
                } else {
                    //this.loginUser(result);
                    this.redirect(result);
                }
                return false;
            },
            error => {
                this.spinnerSmallComponent.hideIt();
                this.mensajeComponent.setErrorMsg(error.message);
            }
        );
        return false;
    }

    enterPressedLogin(event: any) {
        if (event.keyCode == 13 && this.loginButtonEnable) {
            this.login();
        }
    }

    ngOnDestroy() {
        this.renderer.setElementClass(document.body, 'loginBody', false);
    }

    private redirect(result: any) {

        if (result.tipoUsuario == "DATAAGROLOGIN") {
            if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else if (result.url == undefined || result.url == "") {
                this.mensajeComponent.setErrorMsg("No se pudo obtener la URL destino");
            } else {
                location.href = result.url;
            }
        } else {
            this.loginUser(result);
        }
    }
}


