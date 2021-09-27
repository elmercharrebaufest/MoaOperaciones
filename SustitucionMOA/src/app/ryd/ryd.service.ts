
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';

import "rxjs/add/observable/defer";
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class RYDService extends BaseService {

    getInputsCargaPesadas(): Observable<any> {
        return this.http
            .get('/api/ryd/getDataInputsCargaPesadas', { headers: this.headers }).pipe(
            timeoutWith(1200000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    getFiltros(method: string): Observable<any> {
        return this.http
            .get('/api/ryd/getFiltros' + method, { headers: this.headers }).pipe(
            timeoutWith(300000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }


    postRegistrarPesadas(balanza: string, fecha: string, bodega: string, commodity: string, destino: string, exportador: string, vapor: string, pesoProgramado: any, pesoAcumulado: any, numeroPesada: any, fechaPesada: string, pesoTara: any, pesoBruto: any ): Observable<any> {

        let params: HttpParams = new HttpParams();
        params = params.append('balanza', balanza);
        params = params.append('fecha', fecha);
        params = params.append('bodega', bodega);
        params = params.append('commodity', commodity);
        params = params.append('destino', destino);
        params = params.append('exportador', exportador);
        params = params.append('vapor', vapor);
        params = params.append('pesoProgramado', pesoProgramado);
        params = params.append('pesoAcumulado', pesoAcumulado);
        params = params.append('numeroPesada', numeroPesada);
        params = params.append('fechaPesada', fechaPesada);
        params = params.append('pesoTara', pesoTara);
        params = params.append('pesoBruto', pesoBruto);

        return this.http
            .post('/api/ryd/registrarPesada', params,  {headers : this.headersPost}).pipe(
            timeoutWith(1200000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    postFinalizarCargaPesadas(balanza: string, fecha: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('balanza', balanza);
        params = params.append('fecha', fecha);

        return this.http
            .post('/api/ryd/finalizarCargaPesadas', params, {headers : this.headersPost}).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    verificarBalanzaEnProceso(balanza: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('balanza', balanza);
        return this.http
            .post('/api/ryd/verificarBalanza', params, {headers : this.headersPost}).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }
}

@Injectable()
export class RYDInformeService extends RYDService {

    getFiltros(): Observable<any> {
        return super.getFiltros("Informe");
    }

    getInforme(balanza: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('balanza', balanza);
        return this.http
            .get('/api/ryd/getInforme', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    exportExcel(balanza: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('balanza', balanza);
        return this.http
            .get('/api/ryd/downloadInforme', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }
}

@Injectable()
export class RYDListadoPesadasService extends RYDService {

    getListado(commodity : string, exportador : string, fechaInicio : string, fechaFin : string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('commodity', commodity);
        params = params.append('exportador', exportador);
        params = params.append('fechaInicio', fechaInicio);
        params = params.append('fechaFin', fechaFin);

        return this.http
            .get('/api/ryd/getListadoPesadas', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    getFiltros(): Observable<any> {
        return super.getFiltros("ListadoPesadas");
    }

    exportExcelListadoPesada(commodity: string, exportador: string, fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('commodity', commodity);
        params = params.append('exportador', exportador);
        params = params.append('fechaInicio', fechaInicio);
        params = params.append('fechaFin', fechaFin);
        
        return this.http
            .get('/api/ryd/downloadListadoPesada', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }
}
