
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, timeoutWith} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';




import "rxjs/add/observable/defer";
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';

@Injectable()
export class CartaPorteService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }  

    getCartasPorteCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/cartaporte/' + method, { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
            
    }

    getDetalle(cartaPorteId: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cartaPorteId', cartaPorteId);
        return this.http
            .get('/api/cartaporte/getDetalle', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
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
            .get('/api/cartaporte/' + method, { search: params, headers: this.headers }).pipe(
            timeoutWith(60000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }

    exportExcelDetalle(cartaPorteId: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cartaPorteId', cartaPorteId);
        return this.http
            .get('/api/cartaporte/downloadDetalle', { search: params, headers: this.headers }).pipe(
            timeoutWith(60000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))),
            map(this.extractData),);
    }

    public exportPDFCalidad(numero_ccpp: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('numeroCCPP', numero_ccpp);
        return this.http
            .get('/api/cartaporte/exportPDFCalidad', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }


    public getFotos(cartaPorteId: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cartaPorteId', cartaPorteId);
        return this.http
            .get('/api/cartaporte/GetFotos', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public getListaFotos(cartaPorteIds: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cartaPorteIds', cartaPorteIds);
        return this.http
            .get('/api/cartaporte/GetListaFotos', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }

    public descargarFotos(cartaPorteIds: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('cartaPorteIds', cartaPorteIds);
       
        return this.http
            .get('/api/cartaporte/DescargarFotos', { search: params, headers: this.headers }).pipe(
            map(this.extractData));
    }
}

@Injectable()
export class CartaPorteDescargaService extends CartaPorteService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getCartasPorteCommon(periodo, fecha_inicio, fecha_fin, 'getDescargas');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadDescargas');
    }
}

@Injectable()
export class CartaPorteAplicacionService extends CartaPorteService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getCartasPorteCommon(periodo, fecha_inicio, fecha_fin, 'getAplicaciones');
    } 

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAplicaciones');
    }
}

@Injectable()
export class CartaPorteFormularioService extends BaseService {

    getFormularioDropdowns(): Observable<any> {
        return this.http
            .get('/api/cartaporte/getFormularioDropdowns', { headers: this.headers }).pipe(
            map(this.extractData));
    }

    getDataCTG(valor: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('valor', valor);
        return this.http
            .get('/api/cartaporte/getDataCTG', { search: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Por favor, intentelo nuevamente"))),
            map(this.extractData),);
    }

    getCompletedPDFTemplate(formulario: any, archivo: any, pageSelected: any): Observable<any> {
        var payload = new FormData();
        payload.append("formularioString", JSON.stringify(formulario));
        payload.append("paginaSeleccionada", pageSelected);
        payload.append("file", archivo);
        return this.http
            .post('/api/cartaporte/getCompletedPDFTemplate', payload, this.headersPost).pipe(
            //.timeoutWith(90000, Observable.throw(new Error("Por favor, intentelo nuevamente")))
            map(this.extractData));
    }

    getTemplate(formulario: any): Observable<any> {
        var payload = new FormData();
        payload.append("formularioString", JSON.stringify(formulario));
        return this.http
            .post('/api/cartaporte/getTemplate', payload, this.headersPost).pipe(
            //.timeoutWith(90000, Observable.throw(new Error("Por favor, intentelo nuevamente")))
            map(this.extractData));
    }
}