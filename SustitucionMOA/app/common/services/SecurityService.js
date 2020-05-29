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
require("rxjs/add/operator/map");
require("rxjs/add/operator/catch");
require("rxjs/add/operator/timeoutWith");
require("rxjs/add/observable/throw");
require("rxjs/add/observable/defer");
var SecurityService = /** @class */ (function () {
    function SecurityService(router) {
        this.router = router;
    }
    SecurityService.prototype.tienePermiso = function (permiso) {
        return (JSON.parse(sessionStorage.getItem("permisos")).indexOf(permiso) != -1);
    };
    SecurityService.prototype.tienePermisoRedirect = function (permiso) {
        if (JSON.parse(sessionStorage.getItem("permisos")).indexOf(permiso) == -1) {
            this.router.navigate(['no-autorizado']);
        }
    };
    SecurityService.prototype.esGranosRedirect = function () {
        if (sessionStorage.getItem("granosFlag") == "N") {
            this.router.navigate(['/home-ngs']);
            return false;
        }
        return true;
    };
    SecurityService.prototype.esNoGranosRedirect = function () {
        if (sessionStorage.getItem("granosFlag") == "G") {
            this.router.navigate(['/home']);
            return false;
        }
        return true;
    };
    SecurityService = __decorate([
        core_1.Injectable(),
        __metadata("design:paramtypes", [router_1.Router])
    ], SecurityService);
    return SecurityService;
}());
exports.SecurityService = SecurityService;
//# sourceMappingURL=SecurityService.js.map