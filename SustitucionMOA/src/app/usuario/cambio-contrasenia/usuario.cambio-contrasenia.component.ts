import { Component, ViewChild, OnInit } from '@angular/core';
import { UsuarioService } from './../usuario.service';
import { BaseComponent } from './../../common/base-components/base-component';
import { MensajeComponent } from './../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from './../../common/view-child/spinner/spinner.component';
import { NavService } from './../../common/services/NavService';
import { FloatMsgService } from './../../common/services/FloatMsgService';
import { SecurityService } from './../../common/services/SecurityService';
import { SessionDataService } from './../../common/services/SessionDataService';
import { ModalService } from './../../common/services/ModalService';



@Component({
    selector: 'app-usuario-cambio-contrasenia',
    templateUrl: `usuario.cambio-contrasenia.component.html`,
    providers: [UsuarioService]
})
export class CambioContraseniaComponent extends BaseComponent implements OnInit{

    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    constructor(protected service: UsuarioService, protected navService: NavService, protected sessionDataService: SessionDataService, protected securityService: SecurityService, protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securityService, floatMsgService, modalService);
        this.mensajeComponent = new MensajeComponent();
        this.spinnerComponent = new SpinnerComponent();
    }

    contraseniaActual: string;
    contraseniaNueva: string;
    contraseniaNuevaConfirmacion: string;
    visibleButton: boolean = true;

    setTabs() {
        this.setMenuSeccionTab('usuario', 'Cambio Contraseña');
    }

    ngOnInit() {
        this.setTabs();
        this.navService.setSeccionList([]);
    }

    guardarContrasenia() {
        this.mensajeComponent.setMsgsEmpty();
        this.visibleButton = false;
        this.spinnerComponent.showIt();
        try {
            this.validarContrasenias();
            this.unsubscribe();
            this.subscription = this.service.cambiarContrasenia(this.contraseniaActual, this.contraseniaNueva).subscribe(
                result => {
                    this.visibleButton = true;
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
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

    validarContrasenias() {
        this.contraseniaSinValor(this.contraseniaActual, "Actual");
        this.contraseniaSinValor(this.contraseniaNueva, "Nueva");
        this.contraseniaSinValor(this.contraseniaNuevaConfirmacion, "de Confirmación");
        if (this.contraseniaNueva != this.contraseniaNuevaConfirmacion)
            throw "La contraseña Nueva y de Confirmación deben ser iguales"
    }

    contraseniaSinValor(contrasenia: string, nombreCampo: string) {
        if (contrasenia == undefined || contrasenia == null || contrasenia == "")
            throw "Complete el campo " + nombreCampo;
    }

}