import { DatePipe } from '@angular/common';
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
declare var $: any;

@Component({
    selector: 'app-alta-notificaciones',
    templateUrl: './alta-notificaciones.component.html',
    styleUrls: ['./alta-notificaciones.component.css'],
    providers: [NotificacionesService, DatePipe]
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

    fecha_inicio: string;
    fecha_fin: string;
    horaInicio: number;
    mensajeError: string = "";

    allRoles: boolean = false;
    allTipos: boolean = false;

    constructor(protected service: NotificacionesService,
        protected usuarioService: UsuarioService, protected navService: NavService,
        private route: ActivatedRoute,
        protected sessionDataService: SessionDataService, protected securytiService: SecurityService,
        protected floatMsgService: FloatMsgService, protected modalService: ModalService,
        public datepipe: DatePipe) {
        super(navService, securytiService, floatMsgService, modalService);
    }

    ngOnInit() {

        this.route.params.forEach((params: Params) => {
            if (params["id"] > 0) this.notificacionId = params["id"];
        });

        this.navService.setSeccionList([]);

        this.getRolesOptions();

        if (this.notificacionId > 0) {
            this.obtenerNotificacion();
        }
    }

    ngAfterViewInit(): void {

        $(document).ready(function () {
            $(".form_datetime1").datetimepicker({
                format: 'dd/mm/yyyy',
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

            $(".form_datetime2").datetimepicker({
                format: 'dd/mm/yyyy',
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

    validar() {
        if (this.notificacion.Nombre.length < 3) {
            this.mensajeError = "Ingrese el nombre.";
            return false;
        }

        if (this.fecha_inicio.length == 0) {
            this.mensajeError = "Ingrese la fecha de inicio.";
            return false;
        }

        if (this.fecha_fin.length == 0) {
            this.mensajeError = "Ingrese la fecha de fin.";
            return false;
        }

        if (this.notificacion.LinkAdjunto != undefined) {
            if (this.notificacion.LinkAdjunto.length > 0) {
                if (!this.validarURL()) {
                    this.mensajeError = "La dirección del link adjunto es inválida.";
                    return false;
                }
            }
        }

        if (this.notificacion.Mensaje.length == 0) {
            this.mensajeError = "Ingrese el mensaje.";
            return false;
        }

        if (this.roles.filter(x => x.checked).length == 0) {
            this.mensajeError = "Seleccione algún rol.";
            return false;
        }


        return true;
    }

    validarURL() {
        var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
            '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
            '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
            '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
            '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
            '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
        return !!pattern.test(this.notificacion.LinkAdjunto);
    }


    obtenerNotificacion() {
        try {
            this.subscriptionDropDowns = this.service.getNotificacion(this.notificacionId).subscribe(
                (result: any) => {
                    if (result.logout == true) {
                        this.sessionDataService.logout();
                    } else if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.info != undefined) {
                        this.mensajeComponent.setInfoMsg(result.info);
                    } else {
                        this.notificacion = result.data;

                        this.notificacion.FiltroRoles.forEach(element => {
                            this.roles.find(x => x.Id == element.toString()).checked = true
                        });

                        this.allRoles = this.roles.filter(x => x.checked).length == this.roles.length;

                        this.fecha_inicio = this.notificacion.FechaInicio.toString();
                        this.horaInicio = this.notificacion.HoraInicio;
                        this.fecha_fin = this.notificacion.FechaFin.toString();

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

    getRolesOptions() {
        try {
            this.subscriptionDropDowns = this.usuarioService.getRoles().subscribe(
                (result: any) => {
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


    checkAllRoles() {
        setTimeout(() => {
            this.roles.forEach(element => {
                element.checked = this.allRoles;
            })
        }, 0)
    }


    submit() {

        this.fecha_inicio = (<HTMLInputElement>document.querySelectorAll('[fechaInicioInput]')[0]).value;
        this.fecha_fin = (<HTMLInputElement>document.querySelectorAll('[fechaFinInput]')[0]).value;

        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();

        this.notificacion.FiltroRoles = this.roles.filter(x => x.checked);

        var dateParts = this.fecha_inicio.split("/");


        this.notificacion.FechaInicio = new Date(+dateParts[2], +dateParts[1] - 1, +dateParts[0], this.horaInicio >= 3 ? this.horaInicio - 3 : 23 - this.horaInicio);

        dateParts = this.fecha_fin.split("/");

        this.notificacion.FechaFin = new Date(+dateParts[2], +dateParts[1] - 1, +dateParts[0]);

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


    redirigirAListado() {

        document
            .getElementById("botonCerrarModal")
            .click();
        this.navService.navegarSeccion(
            "/notificaciones"
        );
    }
}
