import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable()
export class VentaSustentableService extends BaseService {



    getCamposProveedores(){
        let params: URLSearchParams = new URLSearchParams();
        return this.http
             .get('/api/CampoSustentable/CamposProveedores', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }
}