
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';

import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class CuentaCorrienteService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('contrato', contrato);
        params = params.append('pago', pago);
        params = params.append('retencion', retencion);

        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientes', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));

    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string = "", pago: string = "", retencion: string = "" ): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('contrato', contrato);
        params = params.append('pago', pago);
        params = params.append('retencion', retencion);

        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientes', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    descargarDocumentoPDF(documento: string, ejercicio: string ): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('documento', documento);
        params = params.append('ejercicio', ejercicio);

        return this.http
            .get('/api/PDF/downloadDocumentPDF', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

}

@Injectable()
export class CuentaCorrienteAgrupadaService extends CuentaCorrienteService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string ): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('contrato', contrato);
        params = params.append('pago', pago);
        params = params.append('retencion', retencion);

        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientesAgrupadas', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string = "", pago: string = "", retencion: string = ""): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('contrato', contrato);
        params = params.append('pago', pago);
        params = params.append('retencion', retencion);

        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientesAgrupadas', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }
}

@Injectable()
export class CuentaCorrientePartidasAbiertasService extends CuentaCorrienteService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string, pago: string, retencion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('contrato', contrato);
        params = params.append('pago', pago);
        params = params.append('retencion', retencion);

        return this.http
            .get('/api/CuentaCorriente/getCuentasCorrientesAgrupadas', { params: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));

    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string, contrato: string = "", pago: string = "", retencion: string = ""): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('contrato', contrato);
        params = params.append('pago', pago);
        params = params.append('retencion', retencion);

        return this.http
            .get('/api/CuentaCorriente/downloadCuentasCorrientesPartidasAbiertas', { params: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

}
