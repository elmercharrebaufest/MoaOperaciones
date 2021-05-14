declare let ga: Function;
import { Component, Injector, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from "@angular/router";
import { ServiceLocator } from './common/services/ServiceLocator';
import { SessionDataService } from './common/services/SessionDataService';
import { NavService } from './common/services/NavService';
import { MensajeComponent } from './common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from './common/view-child/spinner-small/spinner-small.component';
import { map } from 'rxjs/operators';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { environment } from '../environments/environment';
import { Location } from '@angular/common';

@Component({
    selector: 'my-app',
    templateUrl: `app.component.html`
})
export class AppComponent {

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    private spinnerSmallComponent: SpinnerSmallComponent;

    constructor(protected sessionDataService: SessionDataService, protected navService: NavService, private injector: Injector, public router: Router, private http: Http, private location: Location) {
        ServiceLocator.injector = this.injector;
        this.router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                try {
                    ga('set', 'page', event.urlAfterRedirects);
                    ga('send', 'pageview');
                } catch (e) { }
            }
        });
    }

    disabledAgreement: boolean = true;

    ngOnInit() {
        this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');

        if (this.location.path() === '/ticket-pesada') {
            this.navService.navegarSeccion("ticket-pesada");
        }
        else {
            this.validarLoginAzure();
        }
    }

    private extractData(res: Response) {
        return res.json();
    }

    validarLoginAzure() {
        let params: URLSearchParams = new URLSearchParams();
        let headers = new Headers();
        headers.append('Access-Control-Allow-Origin', '*');
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');

        let observable = this.http
            .get('/api/Home/ValidarLoginAzure', { search: params, headers: headers })
            .pipe(map(this.extractData));

        observable.subscribe(
            result => {
                if (result.tipoUsuario == "DATAAGROLOGIN") {
                    if (result.error != undefined && result.error != "") {
                        this.mensajeComponent.setErrorMsg(result.error);
                    } else if (result.url == undefined || result.url == "") {
                        this.mensajeComponent.setErrorMsg("No se pudo obtener la URL destino");
                    } else {
                        window.location.href = result.url;
                    }
                } else {
                    if (result.error != undefined && result.error != "") {
                        alert(result.error);
                        window.location.href = window.location.origin + '/SignOut';
                    }
                    else {
                        this.loginUser(result);
                    }
                }
            }
        );
    }

    loginUser(result: any) {
        sessionStorage.setItem("username", result.username);
        sessionStorage.setItem("nombre", result.nombre);
        sessionStorage.setItem("proveedor", result.proveedor);
        sessionStorage.setItem("granosFlag", result.granosFlag);
        sessionStorage.setItem("tipoUsuario", result.tipoUsuario);
        sessionStorage.setItem("noticias", JSON.stringify(result.noticias));
        sessionStorage.setItem("permisos", JSON.stringify(result.permisos));
        sessionStorage.setItem("seccionesVisitadas", result.seccionesVisitadas);
        this.sessionDataService.setNombre(result.nombre);
        this.sessionDataService.setUsername(result.username);
        this.sessionDataService.setProveedor(result.proveedor);
        this.sessionDataService.setTipoUsuario(result.tipoUsuario);
        this.sessionDataService.setNoticias(result.noticias);
        this.sessionDataService.setPermisos(result.permisos);
        this.sessionDataService.setGranosFlag(result.granosFlag);
        this.sessionDataService.setSeccionesVisitadas(result.seccionesVisitadas);

        sessionStorage.setItem("granosSelected", result.granosFlag == 'A' ? 'G' : result.granosFlag);

        //this.navService.navegarSeccion("notificaciones/alta");
        //this.navService.navegarSeccion(result.redirectURL);

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
        if (result.aceptoTyC != true) {
            document.getElementById("openModalaceptoTyCModal").click();

        }
    }
    aceptarTyC() {
        let observable = this.http
            .get('/api/Home/AceptarTyC', {})
            .pipe(map(this.extractData));

        observable.subscribe(result => {
            if (result.error != undefined && result.error != "") {
                this.mensajeComponent.setErrorMsg(result.error);
            } else {
                document.getElementById("openModalaceptoTyCModal").click();
            }
        })
    }

    checkTyCChecked(event) {
        this.disabledAgreement = !event.target.checked;
    }
}