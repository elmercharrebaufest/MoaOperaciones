
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Notificacion } from '../common/models/notificacion';

@Injectable()
export class NotificacionesService extends BaseService {

    constructor(protected http: Http) {
        super(http);
    }

    public getListado(): Observable<any> {
        return this.http
            .get('/api/Notificacion/GetListado')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public getListadoUsuario(): Observable<any> {
        return this.http
            .get('/api/Notificacion/GetListadoUsuario')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public grabar(notificacion :Notificacion): Observable<any> {
        let payload = new FormData();
        payload.append(
            "notificacionJson",
            JSON.stringify(notificacion)
        );

        return this.http
            .post('/api/Notificacion/Grabar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public eliminar(notificacionId: number): Observable<any> {
        return this.http
            .get('/api/Notificacion/Eliminar')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }


    public habilitar(notificacionId: number): Observable<any> {
        return this.http
            .get('/api/Notificacion/Habilitar')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public deshabilitar(notificacionId: number): Observable<any> {
        return this.http
            .get('/api/Notificacion/Deshabilitar')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

}
