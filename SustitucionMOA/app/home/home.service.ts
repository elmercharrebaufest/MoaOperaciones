import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


@Injectable()
export class HomeService extends BaseService {

    constructor(protected http: Http) {
        super(http);
    }

    public getHomeInfo(fecha_inicio: string, fecha_fin: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/home/getHomeInfo', { search: params, headers: this.headers }).map(this.extractData)
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")));
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }
}

@Injectable()
export class HomeNGService extends HomeService {

    public getHomeInfo(fecha_inicio: string, fecha_fin: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/home/getHomeNGInfo', { search: params, headers: this.headers }).map(this.extractData)
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")));
    }
}