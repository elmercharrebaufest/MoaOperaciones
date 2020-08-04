
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';




import "rxjs/add/observable/defer";
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class RYDService extends BaseService {


    getInputsCargaPesadas(): Observable<any> {
        return this.http
            .get('/api/ryd/getDataInputsCargaPesadas', { headers: this.headers }).pipe(
            timeoutWith(1200000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }

    getFiltros(method: string): Observable<any> {
        return this.http
            .get('/api/ryd/getFiltros' + method, { headers: this.headers }).pipe(
            timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }


    postRegistrarPesadas(balanza: string, fecha: string, bodega: string, commodity: string, destino: string, exportador: string, vapor: string, pesoProgramado: any, pesoAcumulado: any, numeroPesada: any, fechaPesada: string, pesoTara: any, pesoBruto: any ): Observable<any> {

        let params: URLSearchParams = new URLSearchParams();
        params.set('balanza', balanza);
        params.set('fecha', fecha);
        params.set('bodega', bodega);
        params.set('commodity', commodity);
        params.set('destino', destino);
        params.set('exportador', exportador);
        params.set('vapor', vapor);
        params.set('pesoProgramado', pesoProgramado);
        params.set('pesoAcumulado', pesoAcumulado);
        params.set('numeroPesada', numeroPesada);
        params.set('fechaPesada', fechaPesada);
        params.set('pesoTara', pesoTara);
        params.set('pesoBruto', pesoBruto);

        return this.http
            .post('/api/ryd/registrarPesada', params, this.headersPost).pipe(
            timeoutWith(1200000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }

    postFinalizarCargaPesadas(balanza: string, fecha: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('balanza', balanza);
        params.set('fecha', fecha);
        return this.http
            .post('/api/ryd/finalizarCargaPesadas', params, this.headersPost).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }

    verificarBalanzaEnProceso(balanza: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('balanza', balanza);
        return this.http
            .post('/api/ryd/verificarBalanza', params, this.headersPost).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }

}

@Injectable()
export class RYDInformeService extends RYDService {

    getFiltros(): Observable<any> {
        return super.getFiltros("Informe");
    }

    getInforme(balanza: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('balanza', balanza);
        return this.http
            .get('/api/ryd/getInforme', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }

    exportExcel(balanza: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('balanza', balanza);
        return this.http
            .get('/api/ryd/downloadInforme', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }
}

@Injectable()
export class RYDListadoPesadasService extends RYDService {

    getListado(commodity : string, exportador : string, fechaInicio : string, fechaFin : string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('commodity', commodity);
        params.set('exportador', exportador);
        params.set('fechaInicio', fechaInicio);
        params.set('fechaFin', fechaFin);
        return this.http
            .get('/api/ryd/getListadoPesadas', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }

    getFiltros(): Observable<any> {
        return super.getFiltros("ListadoPesadas");
    }

    exportExcelListadoPesada(commodity: string, exportador: string, fechaInicio: string, fechaFin: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('commodity', commodity);
        params.set('exportador', exportador);
        params.set('fechaInicio', fechaInicio);
        params.set('fechaFin', fechaFin);
        return this.http
            .get('/api/ryd/downloadListadoPesada', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }
}


