
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class LoginService {

    constructor(private http: Http) { }

    public login(username: string, pass: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('username', username);
        params.set('pass', pass);
        let headers = new Headers();
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/login/login', { search: params, headers: headers }).pipe(
            map(this.extractData));
    }

    public validarLoginAzure(): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        let headers = new Headers();
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/login/ValidarLoginAzure', { search: params, headers: headers }).pipe(
                map(this.extractData));
    }


    public registrar(numeroProveedor: string, claveActivacion: string, username: string, contrasenia: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroProveedor', numeroProveedor);
        params.set('claveActivacion', claveActivacion);
        params.set('username', username);
        params.set('contrasenia', contrasenia);
        let headers = new Headers();
        headers.append('Cache-control', 'no-cache');
        headers.append('Cache-control', 'no-store');
        headers.append('Expires', '0');
        headers.append('Pragma', 'no-cache');
        return this.http
            .get('/api/usuario/registrar', { search: params, headers: headers }).pipe(
            map(this.extractData));
    }

    public recuperarContrasenia(username: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('username', username);
        return this.http
            .get('/api/usuario/recuperarContrasenia', { search: params }).pipe(
            map(this.extractData));
    }

    private extractData(res: Response) {
        return res.json();
    }

}