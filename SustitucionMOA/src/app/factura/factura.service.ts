
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './../common/services/BaseService';

@Injectable()
export class FacturaService extends BaseService {

    public subirPDF(
        archivos: FileList): Observable<any> {
        var payload = new FormData();
        payload.append("factura", "");
        for (let i = 0; i < archivos.length; i++) {
            payload.append("files", archivos[i]);
        }
        return this.http
            .post('/api/factura/subirPDF', payload, {headers: this.headersPost});
    }   
}