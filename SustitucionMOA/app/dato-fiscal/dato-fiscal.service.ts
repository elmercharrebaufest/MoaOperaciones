import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class DatoFiscalService extends BaseService{


    public getDatosFiscales(vendedor: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('vendedor', vendedor);
        return this.http
            .get('/api/vendedor/getDatoFiscales', { search: params, headers: this.headers })
            .map(this.extractData);
    }

    public getVendedores(fecha_inicio : string, fecha_fin : string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/vendedor/getVendedores', { search: params })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    //Se vuelve a la solucion de tener la documentacion dentro del proyecto.
    /*public getDocumento(nombre: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('nombre', nombre);
        return this.http
            .get('/api/usuario/getDocumento', { search: params })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }*/

}