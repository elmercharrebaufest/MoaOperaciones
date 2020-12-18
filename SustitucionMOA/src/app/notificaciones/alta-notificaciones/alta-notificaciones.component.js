var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { DatePipe } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { BaseComponent } from '../../common/base-components/base-component';
import { Notificacion } from '../../common/models/notificacion';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { UsuarioService } from '../../usuario/usuario.service';
import { NotificacionesService } from '../notificaciones.service';
var AltaNotificacionesComponent = /** @class */ (function (_super) {
    __extends(AltaNotificacionesComponent, _super);
    function AltaNotificacionesComponent(service, usuarioService, navService, route, sessionDataService, securytiService, floatMsgService, modalService, datepipe) {
        var _this = _super.call(this, navService, securytiService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.usuarioService = usuarioService;
        _this.navService = navService;
        _this.route = route;
        _this.sessionDataService = sessionDataService;
        _this.securytiService = securytiService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.datepipe = datepipe;
        _this.rolesUsuarioSeleccionado = [];
        _this.tipoUsuarioArray = [];
        _this.roles = [];
        _this.notificacionId = 0;
        _this.notificacion = new Notificacion();
        _this.mensajeError = "";
        _this.allRoles = false;
        _this.allTipos = false;
        return _this;
    }
    AltaNotificacionesComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.route.params.forEach(function (params) {
            if (params["id"] > 0)
                _this.notificacionId = params["id"];
        });
        this.navService.setSeccionList([]);
        this.getRolesOptions();
        if (this.notificacionId > 0) {
            this.obtenerNotificacion();
        }
    };
    AltaNotificacionesComponent.prototype.ngAfterViewInit = function () {
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
    };
    AltaNotificacionesComponent.prototype.validar = function () {
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
        if (this.roles.filter(function (x) { return x.checked; }).length == 0) {
            this.mensajeError = "Seleccione algún rol.";
            return false;
        }
        return true;
    };
    AltaNotificacionesComponent.prototype.validarURL = function () {
        var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
            '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
            '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
            '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
            '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
            '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
        return !!pattern.test(this.notificacion.LinkAdjunto);
    };
    AltaNotificacionesComponent.prototype.obtenerNotificacion = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.service.getNotificacion(this.notificacionId).subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.notificacion = result.data;
                    _this.notificacion.FiltroRoles.forEach(function (element) {
                        _this.roles.find(function (x) { return x.Id == element.toString(); }).checked = true;
                    });
                    _this.allRoles = _this.roles.filter(function (x) { return x.checked; }).length == _this.roles.length;
                    _this.fecha_inicio = _this.notificacion.FechaInicio.toString();
                    _this.horaInicio = _this.notificacion.HoraInicio;
                    _this.fecha_fin = _this.notificacion.FechaFin.toString();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    AltaNotificacionesComponent.prototype.getRolesOptions = function () {
        var _this = this;
        try {
            this.subscriptionDropDowns = this.usuarioService.getRoles().subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.info != undefined) {
                    _this.mensajeComponent.setInfoMsg(result.info);
                }
                else {
                    _this.roles = result.data.roles;
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.mensajeComponent.setErrorMsg(e);
        }
    };
    AltaNotificacionesComponent.prototype.checkAllRoles = function () {
        var _this = this;
        setTimeout(function () {
            _this.roles.forEach(function (element) {
                element.checked = _this.allRoles;
            });
        }, 0);
    };
    AltaNotificacionesComponent.prototype.submit = function () {
        var _this = this;
        this.fecha_inicio = document.querySelectorAll('[fechaInicioInput]')[0].value;
        this.fecha_fin = document.querySelectorAll('[fechaFinInput]')[0].value;
        if (!this.validar()) {
            this.spinnerComponent.hideIt();
            return;
        }
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.unsubscribe();
        this.notificacion.FiltroRoles = this.roles.filter(function (x) { return x.checked; });
        var dateParts = this.fecha_inicio.split("/");
        this.notificacion.FechaInicio = new Date(+dateParts[2], +dateParts[1] - 1, +dateParts[0], this.horaInicio >= 3 ? this.horaInicio - 3 : 23 - this.horaInicio);
        dateParts = this.fecha_fin.split("/");
        this.notificacion.FechaFin = new Date(+dateParts[2], +dateParts[1] - 1, +dateParts[0]);
        this.subscription = this.service
            .grabar(this.notificacion)
            .subscribe(function (result) {
            _this.spinnerComponent.hideIt();
            if (result.logout == true) {
                _this.sessionDataService.logout();
            }
            else if (result.error != undefined &&
                result.error != "") {
                _this.mensajeComponent.setErrorMsg(result.error);
            }
            else if (result.info != undefined) {
                _this.mensajeComponent.setInfoMsg(result.info);
            }
            else {
                _this.mensajeComponent.setMsgsEmpty();
                document
                    .getElementById("openModalNotificacion")
                    .click();
            }
        }, function (error) {
            _this.spinnerComponent.hideIt();
            _this.mensajeComponent.setErrorMsg(error.message);
        });
    };
    AltaNotificacionesComponent.prototype.redirigirAListado = function () {
        document
            .getElementById("botonCerrarModal")
            .click();
        this.navService.navegarSeccion("/notificaciones");
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], AltaNotificacionesComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], AltaNotificacionesComponent.prototype, "spinnerComponent", void 0);
    AltaNotificacionesComponent = __decorate([
        Component({
            selector: 'app-alta-notificaciones',
            templateUrl: './alta-notificaciones.component.html',
            styleUrls: ['./alta-notificaciones.component.css'],
            providers: [NotificacionesService, DatePipe]
        }),
        __metadata("design:paramtypes", [NotificacionesService,
            UsuarioService, NavService,
            ActivatedRoute,
            SessionDataService, SecurityService,
            FloatMsgService, ModalService,
            DatePipe])
    ], AltaNotificacionesComponent);
    return AltaNotificacionesComponent;
}(BaseComponent));
export { AltaNotificacionesComponent };
//# sourceMappingURL=alta-notificaciones.component.js.map