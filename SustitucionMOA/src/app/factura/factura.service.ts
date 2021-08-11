
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './../common/services/BaseService';

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