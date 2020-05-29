"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/http");
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/observable/throw");
var LoginService = /** @class */ (function () {
    function LoginService(http) {
        this.http = http;
    }
    LoginService.prototype.login = function (username, pass) {
        var params = new http_1.URLSearchParams();
        params.set('username', username);
        params.set('pass', pass);
        var headers = new http_1.Headers();
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/login/login', { search: params, headers: headers })
            .map(this.extractData);
    };
    LoginService.prototype.registrar = function (numeroProveedor, claveActivacion, username, contrasenia) {
        var params = new http_1.URLSearchParams();
        params.set('numeroProveedor', numeroProveedor);
        params.set('claveActivacion', claveActivacion);
        params.set('username', username);
        params.set('contrasenia', contrasenia);
        var headers = new http_1.Headers();
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/usuario/registrar', { search: params, headers: headers })
            .map(this.extractData);
    };
    LoginService.prototype.recuperarContrasenia = function (username) {
        var params = new http_1.URLSearchParams();
        params.set('username', username);
        return this.http
            .get('/api/usuario/recuperarContrasenia', { search: params })
            .map(this.extractData);
    };
    LoginService.prototype.extractData = function (res) {
        return res.json();
    };
    LoginService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [http_1.Http])
    ], LoginService);
    return LoginService;
}());
exports.LoginService = LoginService;
//# sourceMappingURL=login.service.js.map