
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { OrdenDeCarga } from '../common/models/ordenes-de-carga/ordenDeCarga';

@Injectable({
  providedIn: 'root'
})
export class OrdenesDeCargaService extends BaseService{
 constructor(protected http: Http) {
        super(http);
    }

    public getOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get('/api/OrdenDeCarga/GetOrdenDeCarga', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public getListado(): Observable<any> {
        return this.http
            .get('/api/OrdenDeCarga/GetListado')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public grabar(ordenDeCarga :OrdenDeCarga): Observable<any> {
        let payload = new FormData();
        console.log(ordenDeCarga)
        payload.append(
            "ordenDeCargaJson",
            JSON.stringify(ordenDeCarga)
        );

        return this.http
            .post('/api/OrdenDeCarga/Agregar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

}
