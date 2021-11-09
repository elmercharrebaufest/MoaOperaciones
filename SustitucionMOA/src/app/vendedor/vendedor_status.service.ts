
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class VendedorStatusService extends BaseService {

    public getVendedorStatus(cuit: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cuit', cuit);
        
        return this.http
            .get('/api/vendedor/getVendedorStatus', { params: params, headers: this.headers });
    }

    public getVariosVendedoresStatus(cuitStr: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cuitStr', cuitStr);

        return this.http
            .get('/api/vendedor/GetVariosVendedoresStatus', { params: params, headers: this.headers });
    }
}