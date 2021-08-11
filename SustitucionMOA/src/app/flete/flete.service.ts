
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class FleteService extends BaseService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> { return null; }

    public getDataCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);

        return this.http
            .get('/api/flete/' + method, { params: params, headers: this.headers });
    }

    public exportPDF(periodo?: string, fecha_inicio?: string, fecha_fin?: string, proforma?: string): Observable<any> { return null }

    public exportPDFCommon(periodo?: string, fecha_inicio?: string, fecha_fin?: string, proforma?: string, method?: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        params = params.append('proforma', proforma);

        return this.http
            .get('/api/flete/' + method, { params: params, headers: this.headers });
    }

    public validarImporte(importe: string, proforma: string): Observable<any> { return null }

    public guardarDatosProforma(data: any, file: any): Observable<any> {
        var payload = new FormData();
        payload.append("proforma", JSON.stringify(data));
        payload.append("file", file);

        return this.http
            .post('/api/flete/guardarDatosProforma', payload, { headers: this.headersPost });
    }

    exportExcel(periodo?: string, fecha_inicio?: string, fecha_fin?: string, contrato?: string, pago?: string, retencion?: string): Observable<any> {
        return null;
    }

    exportExcelCommon(periodo?: string, fecha_inicio?: string, fecha_fin?: string, method?: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);

        return this.http
            .get('/api/flete/' + method, { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
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
        let params: HttpParams = new HttpParams();
        params = params.append('importe', importe);
        params = params.append('proforma', proforma);

        return this.http
            .get('/api/flete/validarImporte', { params: params, headers: this.headers });
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