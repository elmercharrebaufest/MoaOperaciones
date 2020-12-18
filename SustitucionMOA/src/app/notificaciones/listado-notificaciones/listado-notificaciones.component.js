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
import { Component, ViewChild } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { MensajeComponent } from '../../common/view-child/mensaje/mensaje.component';
import { SpinnerComponent } from '../../common/view-child/spinner/spinner.component';
import { NotificacionesService } from '../notificaciones.service';
var ListadoNotificacionesComponent = /** @class */ (function (_super) {
    __extends(ListadoNotificacionesComponent, _super);
    function ListadoNotificacionesComponent(service, navService, sessionDataService, securytiService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securytiService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securytiService = securytiService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.orderedByColumn = "Nombre";
        _this.orderDirection = 1;
        _this.itemsPerPage = 20;
        return _this;
    }
    ListadoNotificacionesComponent.prototype.ngOnInit = function () {
        this.navService.setSeccionList([]);
        this.getListado();
    };
    ListadoNotificacionesComponent.prototype.getListado = function () {
        var _this = this;
        this.mensajeComponent.setMsgsEmpty();
        this.spinnerComponent.showIt();
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getListado().subscribe(function (result) {
                _this.spinnerComponent.hideIt();
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
                    _this.data = result.data;
                }
            }, function (error) {
                _this.spinnerComponent.hideIt();
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    ListadoNotificacionesComponent.prototype.habilitar = function (notificacionId) {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.habilitar(notificacionId).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
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
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getListado();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    ListadoNotificacionesComponent.prototype.deshabilitar = function (notificacionId) {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.deshabilitar(notificacionId).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
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
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getListado();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    ListadoNotificacionesComponent.prototype.eliminar = function (notificacionId) {
        var _this = this;
        this.spinnerComponent.showIt();
        this.mensajeComponent.setMsgsEmpty();
        try {
            this.service.eliminar(notificacionId).subscribe(function (result) {
                _this.spinnerComponent.hideIt();
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
                    _this.mensajeComponent.setSuccessMsg(result.data);
                    _this.getListado();
                }
            }, function (error) {
                _this.mensajeComponent.setErrorMsg(error.message);
            });
        }
        catch (e) {
            this.spinnerComponent.hideIt();
            this.mensajeComponent.setErrorMsg(e);
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], ListadoNotificacionesComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerComponent),
        __metadata("design:type", SpinnerComponent)
    ], ListadoNotificacionesComponent.prototype, "spinnerComponent", void 0);
    ListadoNotificacionesComponent = __decorate([
        Component({
            selector: 'app-listado-notificaciones',
            templateUrl: './listado-notificaciones.component.html',
            styleUrls: ['./listado-notificaciones.component.css'],
            providers: [NotificacionesService]
        }),
        __metadata("design:paramtypes", [NotificacionesService, NavService,
            SessionDataService, SecurityService,
            FloatMsgService, ModalService])
    ], ListadoNotificacionesComponent);
    return ListadoNotificacionesComponent;
}(BaseComponent));
export { ListadoNotificacionesComponent };
//# sourceMappingURL=listado-notificaciones.component.js.map