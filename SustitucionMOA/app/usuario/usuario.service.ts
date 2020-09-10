
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Usuario } from './usuario'



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class UsuarioService extends BaseService {

    guardarRolesUsuario(usuarioSeleccionado: any, idRoles: any ) {
        let params: URLSearchParams = new URLSearchParams();

        var idUsuario = usuarioSeleccionado.Id;

        params.set('idRoles', idRoles);
        params.set('idUsuario', idUsuario);
        return this.http
            .get('/api/usuario/GuardarRoles', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    public cambiarContrasenia(contraseniaActual: string, contraseniaNueva: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contraseniaActual', contraseniaActual);
        params.set('contraseniaNueva', contraseniaNueva);
        return this.http
            .get('/api/usuario/cambiarContrasenia', { search: params, headers: this.headers  }).pipe(
            map(this.extractData));
    }

    public alta(usuario: Usuario): Observable<any> {
        let body = JSON.stringify(usuario);
        return this.http
            .post('/api/usuario/alta', body, this.headersPost).pipe(
            map(this.extractData));
    }

    public getPerfiles(): Observable<any> {
        return this.http
            .get('/api/usuario/getPerfiles', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getUsuarios(): Observable<any> {
        return this.http
            .get('/api/usuario/getUsuarios', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    public desbloquearUsuario(usuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/desbloquear', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public deshabilitarUsuario(mailUsuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('mailUsuario', mailUsuario);
        return this.http
            .get('/api/usuario/deshabilitar', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public habilitarUsuario(mailUsuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('mailUsuario', mailUsuario);
        return this.http
            .get('/api/usuario/habilitar', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }
   
    public seleccionarVendedor(vendedor: string, descripcion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('vendedor', vendedor);
        params.set('descripcion', descripcion);
        return this.http
            .get('/api/usuario/seleccionarVendedor', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getRoles(): Observable<any> {
        return this.http
            .get('/api/usuario/getRoles', { headers: this.headers }).pipe(
                map(this.extractData));
    }

}