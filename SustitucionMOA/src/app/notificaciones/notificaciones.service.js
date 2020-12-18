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
import { throwError as observableThrowError } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
var NotificacionesService = /** @class */ (function (_super) {
    __extends(NotificacionesService, _super);
    function NotificacionesService(http) {
        var _this = _super.call(this, http) || this;
        _this.http = http;
        return _this;
    }
    NotificacionesService.prototype.getNotificacion = function (notificacionId) {
        var params = new URLSearchParams();
        params.set('notificacionId', notificacionId.toString());
        return this.http
            .get('/api/Notificacion/GetNotificacion', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService.prototype.getListado = function () {
        return this.http
            .get('/api/Notificacion/GetListado')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService.prototype.getNotificaciones = function () {
        return this.http
            .get('/api/Notificacion/getNotificaciones')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService.prototype.grabar = function (notificacion) {
        var payload = new FormData();
        console.log(notificacion);
        payload.append("notificacionJson", JSON.stringify(notificacion));
        return this.http
            .post('/api/Notificacion/Grabar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService.prototype.eliminar = function (notificacionId) {
        var params = new URLSearchParams();
        params.set('notificacionId', notificacionId.toString());
        return this.http
            .get('/api/Notificacion/Eliminar', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService.prototype.habilitar = function (notificacionId) {
        var params = new URLSearchParams();
        params.set('notificacionId', notificacionId.toString());
        return this.http
            .get('/api/Notificacion/Habilitar', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService.prototype.deshabilitar = function (notificacionId) {
        var params = new URLSearchParams();
        params.set('notificacionId', notificacionId.toString());
        return this.http
            .get('/api/Notificacion/Deshabilitar', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    };
    NotificacionesService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [Http])
    ], NotificacionesService);
    return NotificacionesService;
}(BaseService));
export { NotificacionesService };
//# sourceMappingURL=notificaciones.service.js.map