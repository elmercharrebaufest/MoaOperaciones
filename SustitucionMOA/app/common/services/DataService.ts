import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/timeoutWith';
import 'rxjs/add/observable/throw';
import "rxjs/add/observable/defer";
import { Formatter } from './../formatter/Formatter';
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