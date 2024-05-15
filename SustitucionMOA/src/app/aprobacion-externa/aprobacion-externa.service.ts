import { Injectable } from '@angular/core';
import { BaseService } from '../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import { CommonResponse } from '../common/models/common-response';
import { throwError as observableThrowError } from 'rxjs';
import { timeoutWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
@Injectable()
export class AprobacionExternaService extends BaseService {

    public getESData(nroESLocal): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('nroESLocal', nroESLocal)

        return this.http
            .get('/api/AprobacionExterna/Index', { params: params, headers: this.headers })
            .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

}
