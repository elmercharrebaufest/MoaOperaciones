
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';
 
@Injectable()
export class ReporteService extends BaseService {

    public getDatosContratos(numero_contrato: string, fijacion: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/CrearContrato/getContratos', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }
    public getDatosCombos() {
        
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        return this.http
            .get('/api/CrearContrato/ObteneDatosContrato', { headers: this.headers })
            .pipe(map(this.extractData));
    }
    public getDatosCupos(numero_contrato: string, fijacion: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/getDetalleFijacion', { search: params, headers: this.headers }).pipe(
                map(this.extractData));
    }
}

@Injectable()
export class ReporteContratoService extends ReporteService {

    
}


@Injectable()
export class ReporteCupoService extends ReporteService {


}
