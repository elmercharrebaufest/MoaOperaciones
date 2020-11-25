import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, Params } from "@angular/router";
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
    notificacionId: number = 0;

    notificacion: Notificacion = new Notificacion();
    
    
    constructor(protected service: NotificacionesService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.notificacionId = params["id"];
        });

        this.navService.setSeccionList([]);

        this.getRolesOptions();

        this.initTipoUsuarios();
        

        if (this.notificacionId > 0)
            this.obtenerNotificacion();
    }

    validar() {
        return true;
    }

    async obtenerNotificacion() {
        try {
            this.subscriptionDropDowns = this.service.getNotificacion(this.notificacionId).subscribe(
                result => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.notificacion = result.data;

                        console.log(this.notificacion);
                        this.notificacion.FiltroRoles.forEach(element => {
                            this.roles.find(x => x.Id == element.toString()).checked = true
                        });

                        
                        this.notificacion.FiltroTipoUsuario.forEach(element => {
                            this.tipoUsuarioArray.find(x => x.Id == element.toString()).checked = true
                        });
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

    async getRolesOptions() {
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

    async initTipoUsuarios() {
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

        this.notificacion.FiltroRoles = this.roles.filter(x => x.checked).map(r => parseInt(r.Id));
        this.notificacion.FiltroTipoUsuario = this.tipoUsuarioArray.filter(x => x.checked).map(r => parseInt(r.Id));

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
