import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class VendedorStatusService extends BaseService {

    public getVendedorStatus(cuit: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cuit', cuit);
        return this.http
            .get('/api/vendedor/getVendedorStatus', { search: params, headers: this.headers })
            .map(this.extractData);
    }
}