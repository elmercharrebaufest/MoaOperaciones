
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';

@Injectable()
export class AduanaService extends BaseService {

    /*public exportExcelCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params })
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    }*/

    public getPesada(centro: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('centro', centro);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/aduana/getPesada', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }
    
    public getPesadaDetalle(centro: string, nroOrden: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('centro', centro);
        params.set('nroOrden', nroOrden);
        return this.http
            .get('/api/aduana/getPesadaDetalle', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getImagenCamaraConsolidacion(url: string, nombre: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('url', url);
        params.set('nombre', nombre);
        return this.http
            .get('/api/aduana/obtenerImagenCamaraConsolidacion', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }
}

