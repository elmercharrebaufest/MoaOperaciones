import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Usuario } from './usuario'
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class UsuarioService extends BaseService {

    public cambiarContrasenia(contraseniaActual: string, contraseniaNueva: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contraseniaActual', contraseniaActual);
        params.set('contraseniaNueva', contraseniaNueva);
        return this.http
            .get('/api/usuario/cambiarContrasenia', { search: params, headers: this.headers  })
            .map(this.extractData);
    }

    public alta(usuario: Usuario): Observable<any> {
        let body = JSON.stringify(usuario);
        return this.http
            .post('/api/usuario/alta', body, this.headersPost)
            .map(this.extractData);
    }

    public getPerfiles(): Observable<any> {
        return this.http
            .get('/api/usuario/getPerfiles', { headers: this.headers })
            .map(this.extractData);
    }

    public getUsuarios(): Observable<any> {
        return this.http
            .get('/api/usuario/getUsuarios', { headers: this.headers })
            .map(this.extractData);
    }

    public desbloquearUsuario(usuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/desbloquear', { search: params, headers: this.headers })
            .map(this.extractData);
    }

    public deshabilitarUsuario(usuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/deshabilitar', { search: params, headers: this.headers })
            .map(this.extractData);
    }

    public habilitarUsuario(usuario: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('usuario', usuario);
        return this.http
            .get('/api/usuario/habilitar', { search: params, headers: this.headers })
            .map(this.extractData);
    }
   
    public seleccionarVendedor(vendedor: string, descripcion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('vendedor', vendedor);
        params.set('descripcion', descripcion);
        return this.http
            .get('/api/usuario/seleccionarVendedor', { search: params, headers: this.headers })
            .map(this.extractData);
    }
}