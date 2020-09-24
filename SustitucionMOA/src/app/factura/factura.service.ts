
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';


@Injectable()
export class FacturaService extends BaseService {

    public subirPDF(
        archivo: any): Observable<any> {
        var payload = new FormData();
        payload.append("factura", "");
        payload.append("file", archivo);
        return this.http
            .post('/api/factura/subirPDF', payload, this.headersPost).pipe(
            map(this.extractData));
    }   
}