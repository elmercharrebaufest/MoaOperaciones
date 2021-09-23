import { Injectable } from '@angular/core';
import { BaseService } from './BaseService';

@Injectable()
export class DataService extends BaseService {

    // SE MOVIO A RYDService
    /*getOptions(tipo: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('tipo', tipo);
        return this.http
            .get('/api/ryd/getElements', {search: params})
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    }*/
    ///////////////////////////////////

}