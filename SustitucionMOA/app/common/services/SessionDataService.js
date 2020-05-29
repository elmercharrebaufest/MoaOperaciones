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
var router_1 = require("@angular/router");
var Subject_1 = require("rxjs/Subject");
var http_1 = require("@angular/http");
var SessionDataService = /** @class */ (function () {
    function SessionDataService(http, router) {
        this.http = http;
        this.router = router;
        this.username = new Subject_1.Subject();
        this.nombre = new Subject_1.Subject();
        this.proveedor = new Subject_1.Subject();
        this.granosFlag = new Subject_1.Subject();
        this.granosSelected = new Subject_1.Subject();
        this.tipoUsuario = new Subject_1.Subject();
        this.permisos = new Subject_1.Subject();
        this.noticias = new Subject_1.Subject();
        this.username$ = this.username.asObservable();
        this.nombre$ = this.nombre.asObservable();
        this.proveedor$ = this.proveedor.asObservable();
        this.granosFlag$ = this.granosFlag.asObservable();
        this.granosSelected$ = this.granosSelected.asObservable();
        this.tipoUsuario$ = this.tipoUsuario.asObservable();
        this.permisos$ = this.permisos.asObservable();
        this.noticias$ = this.noticias.asObservable();
    }
    SessionDataService.prototype.setUsername = function (value) {
        this.username.next(value);
    };
    SessionDataService.prototype.setNombre = function (value) {
        this.nombre.next(value);
    };
    SessionDataService.prototype.setProveedor = function (value) {
        this.proveedor.next(value);
    };
    SessionDataService.prototype.setGranosFlag = function (value) {
        this.granosFlag.next(value);
    };
    SessionDataService.prototype.setGranosSelected = function (value) {
        this.granosSelected.next(value);
    };
    SessionDataService.prototype.setTipoUsuario = function (value) {
        this.tipoUsuario.next(value);
    };
    SessionDataService.prototype.setPermisos = function (value) {
        this.permisos.next(value);
    };
    SessionDataService.prototype.setNoticias = function (value) {
        this.noticias.next(value);
    };
    SessionDataService.prototype.logout = function () {
        this.http.get('/api/login/logout').subscribe(function (result) {
        });
        this.setUsername("");
        this.setNombre("");
        this.setProveedor("");
        this.setGranosFlag("");
        this.setGranosSelected("");
        this.setTipoUsuario("");
        this.setPermisos(null);
        this.setNoticias(null);
        sessionStorage.clear();
        this.router.navigate(['/login']);
    };
    SessionDataService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [http_1.Http, router_1.Router])
    ], SessionDataService);
    return SessionDataService;
}());
exports.SessionDataService = SessionDataService;
//# sourceMappingURL=SessionDataService.js.map