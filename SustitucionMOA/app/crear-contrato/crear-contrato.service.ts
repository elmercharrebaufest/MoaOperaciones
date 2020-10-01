
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class CrearContratoService extends BaseService {

    //public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return null;
    //}

    obteneDatosContrato(): Observable<any> {
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

    searchLocalidad(term): Observable<any> {
        this.headers = new Headers();
        this.headers.append('Content-Type', 'application/json');
        this.headers.append('Accept', 'q=0.8;application/json;q=0.9')
        this.headers.append('Cache-control', 'no-cache');
        this.headers.append('Cache-control', 'no-store');
        this.headers.append('Expires', '0');
        this.headers.append('Pragma', 'no-cache');

        let params: URLSearchParams = new URLSearchParams();
        params.set('localidad', term);

        return this.http.get('/api/AltaEmpresaGranos/GetLocalidadCombo', { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }
    
}

@Injectable()
export class CrearContratoAPrecioService extends CrearContratoService {

    //public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAnulaciones', new Array<string>());
    //}

    //public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAnulaciones');
    //}
}

@Injectable()
export class CrearContratoAFijarService extends CrearContratoService {

    //public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return this.getContratosCommon(periodo, fecha_inicio, fecha_fin, 'getVigentes');
    //}

    //public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadVigentes');
    //}
}

@Injectable()
export class CrearContratoFijacionService extends CrearContratoService {

    //public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getFijaciones', new Array <string>());
    //}

    //public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
    //    return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadFijaciones');
    //}

}

