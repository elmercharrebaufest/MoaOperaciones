import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, throwError as observableThrowError, throwError, BehaviorSubject } from 'rxjs';
import { timeoutWith } from 'rxjs/operators';
import { BaseService } from '../common/services/BaseService';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class ComunicacionesService extends BaseService {

    constructor(protected http: HttpClient) {
        super(http);
    }

    public getComunicaciones(idProveedor: string, start_date: string, end_date: string) {
        let params = new HttpParams();
        params = params.append('vendedor', idProveedor);
        params = params.append('fechaInicio', start_date);
        params = params.append('fechaFin', end_date);


        return this.http
            .get('/api/Comunicacion/GetAllByProveedor', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public postComunicacionLeida(idComunicacion: number[]): Observable<any> {
        const objectComunicacion = { ItemId: idComunicacion };
        return this.http
            .post('/api/Comunicacion/PostComunicacionLeida', objectComunicacion, {
                headers: this.headers
            })
            .pipe(timeoutWith(30000, throwError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public postComunicacionNoLeida(idComunicacion: number[]): Observable<any> {
        const objectComunicacion = { ItemId: idComunicacion };
        return this.http
            .post('/api/Comunicacion/PostComunicacionNoLeida', objectComunicacion, {
                headers: this.headers
            })
            .pipe(timeoutWith(30000, throwError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))
            ));
    }

}