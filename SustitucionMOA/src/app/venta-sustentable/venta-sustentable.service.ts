import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CampoProveedor } from './sustentable';

@Injectable()
export class VentaSustentableService extends BaseService {



    getCamposProveedores(){
        let params: URLSearchParams = new URLSearchParams();
        return this.http
             .get('/api/CampoSustentable/CamposProveedores', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }

    campoProveedorAgregar(campoProveedor: CampoProveedor, file: File){
        let body = JSON.stringify(campoProveedor);
        return this.http
            .post('/api/CampoSustentable/CampoProveedorAgregar', {campoProveedor, body}, this.headersPost).pipe(
                map(this.extractData));
    }

    searchLocalidad(term): Observable<any> {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");

        let params: URLSearchParams = new URLSearchParams();
        params.set("localidad", term);

        return this.http
            .get("/api/AltaEmpresaGranos/GetLocalidadCombo", {
                search: params,
                headers: this.headers,
            })
            .pipe(map(this.extractData));
    }
}