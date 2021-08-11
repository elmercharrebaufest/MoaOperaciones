
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

@Injectable()
export class LoginService {

    constructor(private http: HttpClient) { }

    public login(username: string, pass: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('username', username);
        params = params.append('pass', pass);

        let headers = new HttpHeaders();
        headers = headers.append('Cache-control', 'no-cache');
        headers = headers.append('Cache-control', 'no-store');
        headers = headers.append('Expires', '0');
        headers = headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/login/login', { params: params, headers: headers });
    }

    public validarLoginAzure(): Observable<any> {
        let headers = new HttpHeaders();

        headers = headers.append('Cache-control', 'no-cache');
        headers = headers.append('Cache-control', 'no-store');
        headers = headers.append('Expires', '0');
        headers = headers.append('Pragma', 'no-cache');
        
        return this.http
            .get('/api/login/ValidarLoginAzure', { headers: headers });
    }


    public registrar(numeroProveedor: string, claveActivacion: string, username: string, contrasenia: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroProveedor', numeroProveedor);
        params = params.append('claveActivacion', claveActivacion);
        params = params.append('username', username);
        params = params.append('contrasenia', contrasenia);

        let headers = new HttpHeaders();
        headers = headers.append('Cache-control', 'no-cache');
        headers = headers.append('Cache-control', 'no-store');
        headers = headers.append('Expires', '0');
        headers = headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/usuario/registrar', { params: params, headers: headers });
    }

    public recuperarContrasenia(username: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('username', username);

        return this.http
            .get('/api/usuario/recuperarContrasenia', { params: params });
    }
}