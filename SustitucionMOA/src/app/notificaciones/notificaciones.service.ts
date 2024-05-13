
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { Notificacion } from '../common/models/notificacion';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Adjuntos } from '../common/models/adjuntos';

@Injectable()
export class NotificacionesService extends BaseService {

    constructor(protected http: HttpClient) {
        super(http);
    }

    public getNotificacion(notificacionId: Number): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('notificacionId', notificacionId.toString());

        return this.http
            .get<any[]>('/api/Notificacion/GetNotificacion', { params: params, headers: this.headers })
            .pipe(timeoutWith(600000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public getListado(): Observable<any> {
        return this.http
            .get('/api/Notificacion/GetListado')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public getNotificaciones(): Observable<any> {
        return this.http
            .get('/api/Notificacion/getNotificaciones')
            .pipe(timeoutWith(600000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public grabar(notificacion :Notificacion , adjuntos: Array<Adjuntos>): Observable<any> {

        let payload = new FormData();

        for (let i = 0; i < adjuntos.length; i++) {
            payload.append(`files[${i}]`, adjuntos[i].AdjuntoContenido);
          }

        payload.append(
            "notificacionJson",
            JSON.stringify(notificacion)
        );
        return this.http
            .post('/api/Notificacion/Grabar', payload)
            .pipe(timeoutWith(600000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public eliminar(notificacionId: number): Observable<any> {

        let params = new HttpParams();
        params = params.append('notificacionId', notificacionId.toString());

        return this.http
            .get('/api/Notificacion/Eliminar', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public habilitar(notificacionId: number): Observable<any> {
        
        let params = new HttpParams();
        params = params.append('notificacionId', notificacionId.toString());

        return this.http
            .get('/api/Notificacion/Habilitar', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public deshabilitar(notificacionId: number): Observable<any> {
        
        let params = new HttpParams();
        params = params.append('notificacionId', notificacionId.toString());

        return this.http
            .get('/api/Notificacion/Deshabilitar', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public postNotificacionLeida(notificacionId:number): Observable<any> {
        let params = new HttpParams();
        params = params.append('notificacionId', notificacionId.toString());
        return this.http
            .get('/api/Notificacion/PostNotificacionLeida', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

}
