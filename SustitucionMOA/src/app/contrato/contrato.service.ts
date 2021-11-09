
import {throwError as observableThrowError,  Observable } from 'rxjs';
import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
 
@Injectable()
export class ContratoService extends BaseService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    public getDetalle(numero_contrato: string) {
        let params: HttpParams = new HttpParams();
        params = params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/getDetalle', { params: params, headers: this.headers });
    }

    public getDetalleFijacion(numero_contrato: string, fijacion: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroContrato', numero_contrato);
        params = params.append('fijacion', fijacion);
        return this.http
            .get('/api/contrato/getDetalleFijacion', { params: params, headers: this.headers });
    }

    public downloadBoletoFisico(numero_contrato: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadBoletoFisico', { params: params, headers: this.headers });
    }

    public exportExcelDetalle(numero_contrato: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadDetalle', { params: params, headers: this.headers });
    }

    public exportPDFCalidad(numero_contrato: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/exportPDFCalidad', { params: params, headers: this.headers });
    }

    public exportExcelDetalleFijacion(numero_contrato: string, fijacion: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroContrato', numero_contrato);
        params = params.append('fijacion', fijacion);
        return this.http
            .get('/api/contrato/downloadDetalleFijacion', { params: params, headers: this.headers });
    }

    protected getContratosCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { params: params, headers: this.headers });
    }

    protected getContratosNoCumplidosCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string, contratos: Array<string>) {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { params: params, headers: this.headers });
    }


    public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    public exportExcelCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }
}

@Injectable()
export class ContratoVigenteService extends ContratoService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getContratosCommon(periodo, fecha_inicio, fecha_fin, 'getVigentes');
    }

    public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadVigentes');
    }
}

@Injectable()
export class ContratoFijacionService extends ContratoService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getFijaciones', new Array <string>());
    }

    public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadFijaciones');
    }

}

@Injectable()
export class ContratoAnulacionService extends ContratoService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAnulaciones', new Array<string>());
    }

    public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAnulaciones');
    }
}


@Injectable()
export class ContratoAmpliacionService extends ContratoService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getContratosNoCumplidosCommon(periodo, fecha_inicio, fecha_fin, 'getAmpliaciones', new Array<string>());
    }

    public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAmpliaciones');
    }

}

