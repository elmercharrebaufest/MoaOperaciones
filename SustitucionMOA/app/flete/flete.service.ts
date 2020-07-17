
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class FleteService extends BaseService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> { return null; }

    public getDataCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/flete/' + method, { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public exportPDF(periodo?: string, fecha_inicio?: string, fecha_fin?: string, proforma?: string): Observable<any> { return null }

    public exportPDFCommon(periodo?: string, fecha_inicio?: string, fecha_fin?: string, proforma?: string, method?: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        params.set('proforma', proforma);
        return this.http
            .get('/api/flete/' + method, { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public validarImporte(importe: string, proforma: string): Observable<any> { return null }

    public guardarDatosProforma(data: any, file: any): Observable<any> {
        var payload = new FormData();
        payload.append("proforma", JSON.stringify(data));
        payload.append("file", file);
        return this.http
            .post('/api/flete/guardarDatosProforma', payload, this.headersPost).pipe(
            map(this.extractData));
    }

    exportExcel(periodo?: string, fecha_inicio?: string, fecha_fin?: string, contrato?: string, pago?: string, retencion?: string): Observable<any> {
        return null;
    }

    exportExcelCommon(periodo?: string, fecha_inicio?: string, fecha_fin?: string, method?: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/flete/' + method, { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }

}




@Injectable()
export class FletePendienteService extends FleteService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getDataCommon(periodo, fecha_inicio, fecha_fin, "getViajesPendientes");
    }

    public exportExcel(periodo?: string, fecha_inicio?: string, fecha_fin?: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, "downloadViajesPendientes")
    }

}

@Injectable()
export class FleteAFacturarService extends FleteService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getDataCommon(periodo, fecha_inicio, fecha_fin, "getViajesAFacturar");
    }

    public exportPDF(periodo?: string, fecha_inicio?: string, fecha_fin?: string, proforma?: string): Observable<any> {
        return this.exportPDFCommon(periodo, fecha_inicio, fecha_fin, proforma, "exportarPDFAFacturar")
    }

    public validarImporte(importe: string, proforma: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('importe', importe);
        params.set('proforma', proforma);
        return this.http
            .get('/api/flete/validarImporte', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public exportExcel(periodo?: string, fecha_inicio?: string, fecha_fin?: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, "downloadViajesAFacturar")
    }

}

@Injectable()
export class FleteFacturadoService extends FleteService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getDataCommon(periodo, fecha_inicio, fecha_fin, "getViajesFacturados");
    }

    public exportPDF(periodo?: string, fecha_inicio?: string, fecha_fin?: string, proforma?: string): Observable<any> {
        return this.exportPDFCommon(periodo, fecha_inicio, fecha_fin, proforma, "exportarPDFFacturado")
    }

    public exportExcel(periodo?: string, fecha_inicio?: string, fecha_fin?: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, "downloadViajesFacturados")
    }
}