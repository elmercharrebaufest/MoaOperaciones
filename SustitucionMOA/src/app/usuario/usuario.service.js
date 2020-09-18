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
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
var UsuarioService = /** @class */ (function (_super) {
    __extends(UsuarioService, _super);
    function UsuarioService() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    UsuarioService.prototype.cambiarContrasenia = function (contraseniaActual, contraseniaNueva) {
        var params = new URLSearchParams();
        params.set('contraseniaActual', contraseniaActual);
        params.set('contraseniaNueva', contraseniaNueva);
        return this.http
            .get('/api/usuario/cambiarContrasenia', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService.prototype.alta = function (usuario) {
        var body = JSON.stringify(usuario);
        return this.http
            .post('/api/usuario/alta', body, this.headersPost).pipe(map(this.extractData));
    };
    UsuarioService.prototype.getPerfiles = function () {
        return this.http
            .get('/api/usuario/getPerfiles', { headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService.prototype.getUsuarios = function () {
        return this.http
            .get('/api/usuario/getUsuarios', { headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService.prototype.desbloquearUsuario = function (usuario) {
        var params = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/desbloquear', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService.prototype.deshabilitarUsuario = function (usuario) {
        var params = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/deshabilitar', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService.prototype.habilitarUsuario = function (usuario) {
        var params = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/habilitar', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService.prototype.seleccionarVendedor = function (vendedor, descripcion) {
        var params = new URLSearchParams();
        params.set('vendedor', vendedor);
        params.set('descripcion', descripcion);
        return this.http
            .get('/api/usuario/seleccionarVendedor', { search: params, headers: this.headers }).pipe(map(this.extractData));
    };
    UsuarioService = __decorate([
        Injectable()
    ], UsuarioService);
    return UsuarioService;
}(BaseService));
export { UsuarioService };
//# sourceMappingURL=usuario.service.js.map