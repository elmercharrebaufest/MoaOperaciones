var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Injectable } from '@angular/core';
import { Router } from "@angular/router";
import { Subject } from 'rxjs';
var NavService = /** @class */ (function () {
    function NavService(router) {
        this.router = router;
        this.seccionList = new Subject();
        this.seccionActive = new Subject();
        this.menuActive = new Subject();
        this.seccionList$ = this.seccionList.asObservable();
        this.seccionActive$ = this.seccionActive.asObservable();
        this.menuActive$ = this.menuActive.asObservable();
        this.menuActiveValue = "";
        this.seccionActiveValue = "";
    }
    NavService.prototype.navegarSeccion = function (path) {
        this.router.navigate([path]);
    };
    NavService.prototype.navegarSeccionParam = function (path, param) {
        this.router.navigate([path, param]);
    };
    NavService.prototype.navegarSeccionParamDos = function (path, param1, param2) {
        this.router.navigate([path, param1, param2]);
    };
    NavService.prototype.navegarSeccionParamTres = function (path, param1, param2, param3) {
        this.router.navigate([path, param1, param2, param3]);
    };
    NavService.prototype.setMenuSeccionTab = function (menu, seccion) {
        this.setMenuActive(menu);
        this.setSeccionActive(seccion);
    };
    NavService.prototype.setSeccionList = function (seccionList) {
        this.seccionList.next(seccionList);
    };
    NavService.prototype.setSeccionActive = function (value) {
        this.seccionActive.next(value);
        this.seccionActiveValue = value;
    };
    NavService.prototype.setMenuActive = function (value) {
        this.menuActive.next(value);
        this.menuActiveValue = value;
    };
    NavService = __decorate([
        Injectable(),
        __metadata("design:paramtypes", [Router])
    ], NavService);
    return NavService;
}());
export { NavService };
//# sourceMappingURL=NavService.js.map