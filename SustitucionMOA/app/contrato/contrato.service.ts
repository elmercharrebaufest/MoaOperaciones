
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {timeoutWith, map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class ContratoService extends BaseService {

    public getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    public getDetalle(numero_contrato: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/getDetalle', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getDetalleFijacion(numero_contrato: string, fijacion: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/getDetalleFijacion', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public downloadBoletoFisico(numero_contrato: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadBoletoFisico', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public exportExcelDetalle(numero_contrato: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/downloadDetalle', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public exportPDFCalidad(numero_contrato: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        return this.http
            .get('/api/contrato/exportPDFCalidad', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public exportExcelDetalleFijacion(numero_contrato: string, fijacion: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroContrato', numero_contrato);
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/contrato/downloadDetalleFijacion', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    protected getContratosCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    protected getContratosNoCumplidosCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string, contratos: Array<string>) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }


    public exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    public exportExcelCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/contrato/' + method, { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
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

