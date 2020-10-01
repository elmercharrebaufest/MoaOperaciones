var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
import { Component, Injector, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from "@angular/router";
import { ServiceLocator } from './common/services/ServiceLocator';
import { SessionDataService } from './common/services/SessionDataService';
import { NavService } from './common/services/NavService';
import { MensajeComponent } from './common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from './common/view-child/spinner-small/spinner-small.component';
import { map } from 'rxjs/operators';
import { Http, URLSearchParams, Headers } from '@angular/http';
var AppComponent = /** @class */ (function () {
    function AppComponent(sessionDataService, navService, injector, router, http) {
        this.sessionDataService = sessionDataService;
        this.navService = navService;
        this.injector = injector;
        this.router = router;
        this.http = http;
        ServiceLocator.injector = this.injector;
        this.router.events.subscribe(function (event) {
            if (event instanceof NavigationEnd) {
                try {
                    ga('set', 'page', event.urlAfterRedirects);
                    ga('send', 'pageview');
                }
                catch (e) { }
            }
        });
    }
    AppComponent.prototype.ngOnInit = function () {
        this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');
        this.validarLoginAzure();
    };
    AppComponent.prototype.extractData = function (res) {
        return res.json();
    };
    AppComponent.prototype.validarLoginAzure = function () {
        var _this = this;
        var params = new URLSearchParams();
        var headers = new Headers();
        headers.append('Access-Control-Allow-Origin', '*');
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');
        var observable = this.http
            .get('/api/Home/ValidarLoginAzure', { search: params, headers: headers })
            .pipe(map(this.extractData));
        observable.subscribe(function (result) {
            if (result.tipoUsuario == "DATAAGROLOGIN") {
                if (result.error != undefined && result.error != "") {
                    _this.mensajeComponent.setErrorMsg(result.error);
                }
                else if (result.url == undefined || result.url == "") {
                    _this.mensajeComponent.setErrorMsg("No se pudo obtener la URL destino");
                }
                else {
                    location.href = result.url;
                }
            }
            else {
                if (result.error != undefined && result.error != "") {
                    alert(result.error);
                    location.href = "/";
                }
                else {
                    _this.loginUser(result);
                }
            }
        });
    };
    AppComponent.prototype.loginUser = function (result) {
        sessionStorage.setItem("username", result.username);
        sessionStorage.setItem("nombre", result.nombre);
        sessionStorage.setItem("proveedor", result.proveedor);
        sessionStorage.setItem("granosFlag", result.granosFlag);
        sessionStorage.setItem("tipoUsuario", result.tipoUsuario);
        sessionStorage.setItem("noticias", JSON.stringify(result.noticias));
        sessionStorage.setItem("permisos", JSON.stringify(result.permisos));
        this.sessionDataService.setNombre(result.nombre);
        this.sessionDataService.setUsername(result.username);
        this.sessionDataService.setProveedor(result.proveedor);
        this.sessionDataService.setTipoUsuario(result.tipoUsuario);
        this.sessionDataService.setNoticias(result.noticias);
        this.sessionDataService.setPermisos(result.permisos);
        this.sessionDataService.setGranosFlag(result.granosFlag);
        sessionStorage.setItem("granosSelected", result.granosFlag);
        this.navService.navegarSeccion(result.redirectURL);
        //La URL a donde direccionamos ahora la traemos del controller. Esto es para no tener que estan pasando tantas variables que no nos interesan acá
        //if (result.esNuevoUsuario) {
        //    if (result.granosFlag == "A") {
        //        sessionStorage.setItem("granosSelected", "G");
        //        this.navService.navegarSeccion('/dato-fiscal/documentacion');
        //    } else {
        //        sessionStorage.setItem("granosSelected", result.granosFlag);
        //        if (result.granosFlag == "G") {
        //            this.navService.navegarSeccion('/alta-empresa-granos');
        //        } else {
        //            this.navService.navegarSeccion('/dato-fiscal/documentacion');
        //        }
        //    }
        //} else {
        //    if (result.tipoUsuario == "ADMP" || result.tipoUsuario == "ADNA" || result.tipoUsuario == "RYDD") {
        //        this.navService.navegarSeccion('/aduana/pesada-online');
        //    } else if (result.tipoUsuario == "CLIE") {
        //        this.navService.navegarSeccion('/cuenta-corriente/simple');
        //    } else {
        //        if (result.granosFlag == "A") {
        //            sessionStorage.setItem("granosSelected", "G");
        //            this.navService.navegarSeccion('/home');
        //        } else {
        //            sessionStorage.setItem("granosSelected", result.granosFlag);
        //            if (result.granosFlag == "G") {
        //                this.navService.navegarSeccion('/home');
        //            } else {
        //                this.navService.navegarSeccion('/home-ngs');
        //            }
        //        }
        //    }
        //}
    };
    __decorate([
        ViewChild(MensajeComponent),
        __metadata("design:type", MensajeComponent)
    ], AppComponent.prototype, "mensajeComponent", void 0);
    __decorate([
        ViewChild(SpinnerSmallComponent),
        __metadata("design:type", SpinnerSmallComponent)
    ], AppComponent.prototype, "spinnerSmallComponent", void 0);
    AppComponent = __decorate([
        Component({
            selector: 'my-app',
            templateUrl: "app.component.html"
        }),
        __metadata("design:paramtypes", [SessionDataService, NavService, Injector, Router, Http])
    ], AppComponent);
    return AppComponent;
}());
export { AppComponent };
//# sourceMappingURL=app.component.js.map