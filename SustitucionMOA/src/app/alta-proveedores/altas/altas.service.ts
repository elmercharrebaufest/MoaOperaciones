
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Empresa } from './Empresa'
import { HistorialAprobaciones } from './Empresa'



import { Formatter } from './../../common/formatter/Formatter';
import { BaseService } from './../../common/services/BaseService';
import { environment } from '../../../environments/environment';

@Injectable()
export class AltaEmpresaService extends BaseService {

    public getEmpresas(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEmpresas', { headers: this.headers }).pipe(
            map(this.extractData));
    }


    public setEstadoAprobacion(empresaId: number, estadoId: number, observacion: string, observacionesProveedor: string,estadoSIPER: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('empresaId', empresaId.toString());
        params.set('estado', estadoId.toString());
        params.set('observacion', observacion);
        params.set('observacionParaElProveedor', observacionesProveedor);
        params.set('estadoSIPER', estadoSIPER);
        return this.http
            .get('/api/AltaEmpresa/setEstadoAprobacion', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getEstados(): Observable<any> {
        return this.http
            .get('/api/AltaEmpresa/getEstados', { headers: this.headers }).pipe(
                map(this.extractData));
    }

}