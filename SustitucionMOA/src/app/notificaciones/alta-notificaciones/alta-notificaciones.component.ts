import { Component, OnInit, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { Notificacion } from '../../common/models/notificacion';
import { Rol } from '../../common/models/rol';
import { TipoUsuario } from '../../common/models/tipoUsuario';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { NotificacionesService } from '../notificaciones.service';

@Component({
    selector: 'app-alta-notificaciones',
    templateUrl: './alta-notificaciones.component.html',
    styleUrls: ['./alta-notificaciones.component.css'],
    providers: [NotificacionesService]
})
export class AltaNotificacionesComponent extends BaseComponent implements OnInit {
    @ViewChild(MensajeComponent)
    protected mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerComponent)
    protected spinnerComponent: SpinnerComponent;

    rolesUsuarioSeleccionado: Array<Rol> = [];
    tipoUsuarioArray: Array<TipoUsuario> = [];
    roles: Array<Rol> = [];

    notificacion: Notificacion = new Notificacion();
    
    
    constructor(protected service: NotificacionesService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {
        this.navService.setSeccionList([]);

        this.getRolesOptions();

        this.initTipoUsuarios();
        
    }

    validar() {
        return true;
    }

    getRolesOptions() {
        try {
            this.subscriptionDropDowns = this.usuarioService.getRoles().subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.roles = result.data.roles;
                    }
                },
                error => {
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
        } catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    }

    initTipoUsuarios() {
        this.tipoUsuarioArray.push(
            new TipoUsuario ({ Id: "2", Nombre: "Granos", checked: false, }),
            new TipoUsuario ({ Id: "3", Nombre: "No Granos", checked: false, }),
            new TipoUsuario ({ Id: "4", Nombre: "Corredor", checked: false, }),
            new TipoUsuario ({ Id: "5", Nombre: "Cliente", checked: false, }),
        );
    }

    submit() {
        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
    
        this.subscription = this.service
            .grabar(this.notificacion)
            .subscribe(
                (result) => {
                    this.spinnerComponent.hideIt();
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (
                        result.error != undefined &&
                        result.error != ""
                    ) {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.mensajeComponent.setMsgsEmpty();
                        document
                            .getElementById("openModalNotificacion")
                            .click();
                    }
                },
                (error) => {
                    this.spinnerComponent.hideIt();
                    this.mensajeComponent.setErrorMsg(error.message);
                }
            );
    }
}
