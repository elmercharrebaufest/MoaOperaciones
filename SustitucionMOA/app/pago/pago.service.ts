
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class PagoService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    public getDetalle(numero_pago: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroPago', numero_pago);
        return this.http
            .get('/api/pago/getDetalle', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    getPagosCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/pago/' + method, { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))))
            .map(this.extractData);

    }

    getComprobantes(documento: string, fecha: string, fiscYear: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('documento', documento);
        params.set('fecha', fecha);
        params.set('fiscalYear', fiscYear);
        return this.http
            .get('/api/pago/getComprobantes', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .map(this.extractData);
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
            .map(this.extractData);
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    exportExcelCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/pago/' + method, { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))))
            .map(this.extractData);
    }

    public exportExcelDetalle(numero_pago: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroPago', numero_pago);
        return this.http
            .get('/api/pago/downloadDetalle', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

}


@Injectable()
export class PagoEmitidoService extends PagoService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getPagosCommon(periodo, fecha_inicio, fecha_fin, 'getEmitidos');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadEmitidos');
    }

}

@Injectable()
export class PagoEmitidoNGService extends PagoService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getPagosCommon(periodo, fecha_inicio, fecha_fin, 'getEmitidosNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadEmitidosNG');
    }

}