
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Empresa } from './empresa'



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class AltaEmpresaService extends BaseService {

    //public cambiarContrasenia(contraseniaActual: string, contraseniaNueva: string): Observable<any> {
    //    let params: URLSearchParams = new URLSearchParams();
    //    params.set('contraseniaActual', contraseniaActual);
    //    params.set('contraseniaNueva', contraseniaNueva);
    //    return this.http
    //        .get('/api/usuario/cambiarContrasenia', { search: params, headers: this.headers  }).pipe(
    //        map(this.extractData));
    //}

    //public alta(usuario: Usuario): Observable<any> {
    //    let body = JSON.stringify(usuario);
    //    return this.http
    //        .post('/api/usuario/alta', body, this.headersPost).pipe(
    //        map(this.extractData));
    //}

    //public getPerfiles(): Observable<any> {
    //    return this.http
    //        .get('/api/usuario/getPerfiles', { headers: this.headers }).pipe(
    //        map(this.extractData));
    //}

    public getEmpresas(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEmpresas', { headers: this.headers }).pipe(
            map(this.extractData));
    }


    public setEstadoAprobacion(empresaId: number, estadoId: number, observacion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('empresaId', empresaId.toString());
        params.set('estado', estadoId.toString());
        params.set('observacion', observacion);
        return this.http
            .get('/api/usuario/deshabilitar', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

   

}