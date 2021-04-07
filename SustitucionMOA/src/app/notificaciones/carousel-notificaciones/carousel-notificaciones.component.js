var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
import { Component } from '@angular/core';
import { BaseComponent } from '../../common/base-components/base-component';
import { FloatMsgService } from '../../common/services/FloatMsgService';
import { ModalService } from '../../common/services/ModalService';
import { NavService } from '../../common/services/NavService';
import { SecurityService } from '../../common/services/SecurityService';
import { SessionDataService } from '../../common/services/SessionDataService';
import { NotificacionesService } from '../notificaciones.service';
var CarouselNotificacionesComponent = /** @class */ (function (_super) {
    __extends(CarouselNotificacionesComponent, _super);
    function CarouselNotificacionesComponent(service, navService, sessionDataService, securytiService, floatMsgService, modalService) {
        var _this = _super.call(this, navService, securytiService, floatMsgService, modalService) || this;
        _this.service = service;
        _this.navService = navService;
        _this.sessionDataService = sessionDataService;
        _this.securytiService = securytiService;
        _this.floatMsgService = floatMsgService;
        _this.modalService = modalService;
        _this.indiceNotificacion = 0;
        _this.totalNotificaciones = 0;
        _this.mostrarNotificaciones = false;
        _this.mostrarBotonSiguiente = false;
        _this.mostrarBotonAnterior = false;
        _this.mostrarVerMas = false;
        return _this;
    }
    CarouselNotificacionesComponent.prototype.ngOnInit = function () {
        this.navService.setSeccionList([]);
        this.getNotificaciones();
    };
    CarouselNotificacionesComponent.prototype.getNotificaciones = function () {
        var _this = this;
        this.data = null;
        try {
            this.unsubscribe();
            this.subscription = this.service.getNotificaciones().subscribe(function (result) {
                if (result.logout == true) {
                    _this.sessionDataService.logout();
                }
                else if (result.error != undefined && result.error != "") {
                }
                else if (result.info != undefined) {
                }
                else {
                    _this.data = result.data;
                    _this.totalNotificaciones = _this.data.length;
                    _this.notificacionActual = _this.data[_this.indiceNotificacion];
                    if (_this.data.length > 0)
                        _this.mostrarNotificaciones = true;
                    _this.actualizarBotones();
                }
            }, function (error) {
            });
        }
        catch (e) {
            return false; //<-- Prevent Refresh
        }
        return false; //<-- Prevent Refresh
    };
    CarouselNotificacionesComponent.prototype.siguienteNotificacion = function () {
        this.notificacionActual = this.data[++this.indiceNotificacion];
        this.actualizarBotones();
    };
    CarouselNotificacionesComponent.prototype.notificacionAnterior = function () {
        this.notificacionActual = this.data[--this.indiceNotificacion];
        this.actualizarBotones();
    };
    CarouselNotificacionesComponent.prototype.cerrarNotificaciones = function () {
        this.mostrarNotificaciones = false;
    };
    CarouselNotificacionesComponent.prototype.actualizarBotones = function () {
        this.mostrarBotonSiguiente = (this.indiceNotificacion + 1) != this.totalNotificaciones;
        this.mostrarBotonAnterior = this.indiceNotificacion != 0;
        this.mostrarVerMas = (this.notificacionActual.LinkAdjunto || '') != '';
    };
    CarouselNotificacionesComponent = __decorate([
        Component({
            selector: 'app-carousel-notificaciones',
            templateUrl: './carousel-notificaciones.component.html',
            styleUrls: ['./carousel-notificaciones.component.css']
        }),
        __metadata("design:paramtypes", [NotificacionesService, NavService,
            SessionDataService, SecurityService,
            FloatMsgService, ModalService])
    ], CarouselNotificacionesComponent);
    return CarouselNotificacionesComponent;
}(BaseComponent));
export { CarouselNotificacionesComponent };
//# sourceMappingURL=carousel-notificaciones.component.js.map