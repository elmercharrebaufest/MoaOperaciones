declare let ga: Function;
import { Component, HostListener, Injector, OnDestroy, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from "@angular/router";
import { ServiceLocator } from './common/services/ServiceLocator';
import { SessionDataService } from './common/services/SessionDataService';
import { NavService } from './common/services/NavService';
import { MensajeComponent } from './common/view-child/mensaje/mensaje.component';
import { SpinnerSmallComponent } from './common/view-child/spinner-small/spinner-small.component';
import { Location } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UsuarioLogueado } from './common/models/usuario-logueado';
import { Subscription } from 'rxjs';
import { ConfirmationService } from 'primeng/components/common/api';

@Component({
    selector: 'my-app',
    templateUrl: `app.component.html`
})
export class AppComponent implements OnDestroy {

    @ViewChild(MensajeComponent)
    private mensajeComponent: MensajeComponent;

    @ViewChild(SpinnerSmallComponent)
    private spinnerSmallComponent: SpinnerSmallComponent;

    private validarLoginSub: Subscription;
    private aceptarTyCSub: Subscription;

    constructor(protected sessionDataService: SessionDataService, protected navService: NavService, private injector: Injector, public router: Router, private http: HttpClient, private location: Location, private confirmationService: ConfirmationService) {
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

    aboutScreen: boolean;

    salir() {
        this.navService.navegarSeccion('/compras');
    }

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (event.altKey == true && event.ctrlKey == true && event.key == "b") {
            document.getElementById("aboutScreenBTN").click();
        };
    }

    closeAboutScreen() {
        document.getElementById("aboutScreenBTN").click();
    }

    ngOnInit() {

        this.navService.setSeccionList([]);
        this.navService.setSeccionActive('');
        const path = this.location.path();
        if (path === '/ticket-pesada') {
            this.navService.navegarSeccion("ticket-pesada");
        } else if (path.match(/^\/verLegajoOrdenDeCompra\/\d+\/[a-f0-9-]+$/)) {
            this.navService.navegarSeccion(path);
        }
        else {
            this.validarLoginAzure();
        }
    }

    validarLoginAzure() {
        let headers = new HttpHeaders();
        headers.append('Access-Control-Allow-Origin', '*');
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');

        this.validarLoginSub = this.http
            .get<UsuarioLogueado>('/api/Home/ValidarLoginAzure', { headers: headers }).subscribe(
                (result: any) => {
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
        sessionStorage.setItem("apikey", result.apikey);
        sessionStorage.setItem("cuit", result.cuit)
        sessionStorage.setItem("proveedorId", result.proveedorId)

        this.sessionDataService.setNombre(result.nombre);
        this.sessionDataService.setUsername(result.username);
        this.sessionDataService.setProveedor(result.proveedor);
        this.sessionDataService.setTipoUsuario(result.tipoUsuario);
        this.sessionDataService.setNoticias(result.noticias);
        this.sessionDataService.setPermisos(result.permisos);
        this.sessionDataService.setGranosFlag(result.granosFlag);
        this.sessionDataService.setSeccionesVisitadas(result.seccionesVisitadas);
        this.sessionDataService.setApikey(result.apikey);
        this.sessionDataService.setCuit(result.cuit);
        this.sessionDataService.setProveedorId(result.proveedorId);

        sessionStorage.setItem("granosSelected", result.granosFlag == 'A' ? 'G' : result.granosFlag);

        this.navService.navegarSeccion(result.redirectURL);

        if (result.aceptoTyC != true) {
            document.getElementById("openModalaceptoTyCModal").click();
        }
    }
    aceptarTyC() {
        this.aceptarTyCSub = this.http
            .get<{ error: string, data: boolean }>('/api/Home/AceptarTyC', {})
            .subscribe((result: any) => {
                if (result.error != undefined && result.error != "") {
                    this.mensajeComponent.setErrorMsg(result.error);
                } else {
                    document.getElementById("openModalaceptoTyCModal").click();
                }
            });
    }

    checkTyCChecked(event) {
        this.disabledAgreement = !event.target.checked;
    }

    ngOnDestroy() {
        if (this.validarLoginSub)
            this.validarLoginSub.unsubscribe();

        if (this.aceptarTyCSub)
            this.aceptarTyCSub.unsubscribe();
    }
}