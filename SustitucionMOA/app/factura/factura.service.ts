import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class FacturaService extends BaseService {

    public subirPDF(
        archivo: any): Observable<any> {
        var payload = new FormData();
        payload.append("factura", "");
        payload.append("file", archivo);
        return this.http
            .post('/api/factura/subirPDF', payload, this.headersPost)
            .map(this.extractData);
    }   
}