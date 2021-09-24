
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class PagoService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    public getDetalle(numero_pago: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroPago', numero_pago);

        return this.http
            .get('/api/pago/getDetalle', { params: params, headers: this.headers });
    }

    getPagosCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);

        return this.http
            .get('/api/pago/' + method, { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    getComprobantes(documento: string, fecha: string, fiscYear: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('documento', documento);
        params = params.append('fecha', fecha);
        params = params.append('fiscalYear', fiscYear);

        return this.http
            .get('/api/pago/getComprobantes', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('documento', documento);
        params = params.append('ejercicio', ejercicio);

        return this.http
            .get('/api/PDF/downloadDocumentPDF', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    exportExcelCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);

        return this.http
            .get('/api/pago/' + method, { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    public exportExcelDetalle(numero_pago: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroPago', numero_pago);
        
        return this.http
            .get('/api/pago/downloadDetalle', { params: params, headers: this.headers });
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