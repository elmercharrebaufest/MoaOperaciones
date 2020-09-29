import { Injectable } from '@angular/core';
import { URLSearchParams } from '@angular/http';
import { Observable, throwError as observableThrowError } from 'rxjs';
import { map, timeoutWith } from 'rxjs/operators';
import { BaseService } from './../common/services/BaseService';

@Injectable()
export class DatoFiscalService extends BaseService{

    public getDatosFiscales(vendedor: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('vendedor', vendedor);
        return this.http
            .get('/api/vendedor/getDatoFiscales', { search: params, headers: this.headers })
            .pipe( map(this.extractData));
    }

    public getVendedores(fecha_inicio : string, fecha_fin : string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/vendedor/getVendedores', { search: params })
            .pipe(
                timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
                map(this.extractData)
            );
    }


    public getVendedoresPendientes(): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        return this.http
            .get('/api/vendedor/getVendedoresPendientes', { search: params })
            .pipe(
                timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
                map(this.extractData)
            );
    }

    public agregarVendedor(nuevoVendedorRazonSocial: string, nuevoVendedorCUIT: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cuit', nuevoVendedorCUIT);
        params.set('razonSocial', nuevoVendedorRazonSocial);
        return this.http
            .get('/api/vendedor/agregarVendedor', { search: params })
            .pipe(
                timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
                map(this.extractData)
            );
    }

}