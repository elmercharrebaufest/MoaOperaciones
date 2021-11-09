
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable()
export class HomeService extends BaseService {

    constructor(protected http: HttpClient) {
        super(http);
    }

    public getHomeInfo(fecha_inicio: string, fecha_fin: string): Observable<any> {
        let params: HttpParams = new HttpParams()
        .append('fechaInicio', fecha_inicio)
        .append('fechaFin', fecha_fin);
        
        return this.http
            .get('/api/home/getHomeInfo', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),);
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('documento', documento);
        params = params.append('ejercicio', ejercicio);
        
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }
}

@Injectable()
export class HomeNGService extends HomeService {

    public getHomeInfo(fecha_inicio: string, fecha_fin: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fecha_inicio)
            .append('fechaFin', fecha_fin);
        
        return this.http
            .get('/api/home/getHomeNGInfo', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }
}