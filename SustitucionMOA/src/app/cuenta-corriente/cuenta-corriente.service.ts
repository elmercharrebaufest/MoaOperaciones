
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { environment } from '../../environments/environment';

@Injectable()
export class CuentaCorrienteService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientes', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);

    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string ): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientes', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }

    descargarDocumentoPDF(documento: string, ejercicio: string ): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))),
            map(this.extractData),);
    }

}

@Injectable()
export class CuentaCorrienteAgrupadaService extends CuentaCorrienteService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string ): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientesAgrupadas', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);

    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string ): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientesAgrupadas', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }

}


@Injectable()
export class CuentaCorrientePartidasAbiertasService extends CuentaCorrienteService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientesAgrupadas', { search: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
                map(this.extractData));

    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('contrato', contrato);
        params.set('pago', pago);
        params.set('retencion', retencion);
        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientesPartidasAbiertas', { search: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
                map(this.extractData));
    }

}
