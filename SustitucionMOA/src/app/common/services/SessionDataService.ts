import { Injectable } from '@angular/core';
import { Router } from "@angular/router";
import { Subject } from 'rxjs';
import { Http, Response, URLSearchParams } from '@angular/http';

@Injectable()
export class SessionDataService {

    constructor(protected http: Http, private router: Router) { }

    public username = new Subject<string>();
    public nombre = new Subject<string>();
    public proveedor = new Subject<string>();
    public granosFlag = new Subject<string>();
    public granosSelected = new Subject<string>();
    public tipoUsuario = new Subject<string>();
    public permisos = new Subject<any>();
    public noticias = new Subject<any>();
    public seccionesVisitadas = new Subject<string>();
    public apikey = new Subject<string>();

    username$ = this.username.asObservable();
    nombre$ = this.nombre.asObservable();
    proveedor$ = this.proveedor.asObservable();
    granosFlag$ = this.granosFlag.asObservable();
    granosSelected$ = this.granosSelected.asObservable();
    tipoUsuario$ = this.tipoUsuario.asObservable();
    permisos$ = this.permisos.asObservable();
    noticias$ = this.noticias.asObservable();
    seccionesVisitadas$ = this.seccionesVisitadas.asObservable();
    apikey$ = this.apikey.asObservable();

    setUsername(value: string) {
        this.username.next(value);
    }

    setNombre(value: string) {
        this.nombre.next(value);
    }

    setSeccionesVisitadas(value: string) {
        this.seccionesVisitadas.next(value);
    }

    setProveedor(value: string) {
        this.proveedor.next(value);
    }

    setGranosFlag(value: string) {
        this.granosFlag.next(value);
    }

    setGranosSelected(value: string) {
        this.granosSelected.next(value);
    }

    setTipoUsuario(value: string) {
        this.tipoUsuario.next(value);
    }

    setPermisos(value: any) {
        this.permisos.next(value);
    }

    setNoticias(value: any) {
        this.noticias.next(value);
    }

    setApikey(value: string) {
        this.apikey.next(value);
    }

    logout() {
        this.setUsername("");
        this.setNombre("");
        this.setProveedor("");
        this.setGranosFlag("");
        this.setGranosSelected("");
        this.setTipoUsuario("");
        this.setPermisos(null);
        this.setNoticias(null);
        this.setSeccionesVisitadas("");
        this.setApikey("");
        sessionStorage.clear();

        let logoutURL = window.location.origin;

        logoutURL = logoutURL.replace("/web/", "");

        logoutURL += '/SignOut';
        window.location.href = logoutURL;
    }

    editarCuenta() {
        let logoutURL = window.location.origin + '/EditarCuenta';

        window.location.href = logoutURL;
    }
}