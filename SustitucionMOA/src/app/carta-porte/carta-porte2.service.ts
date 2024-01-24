
import { throwError as observableThrowError, Observable } from 'rxjs';

import { map, timeoutWith } from 'rxjs/operators';
import { Injectable } from '@angular/core';

import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import { CommonResponse } from '../common/models/common-response';
import { ApiResponse } from '../common/models/response';
import { CalidadCCPP } from '../common/models/cartaPorte';

@Injectable()
export class CartaPorteService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    getCartasPorteCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);

        return this.http
            .get('/api/cartaporte/' + method, { params: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));

    }

    getDetalle(cartaPorteId: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorteId', cartaPorteId);

        return this.http
            .get('/api/cartaporte/getDetalle', { params: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
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
            .get('/api/cartaporte/' + method, { params: params, headers: this.headers }).pipe(
                timeoutWith(60000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    exportExcelDetalle(cartaPorteId: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorteId', cartaPorteId);

        return this.http
            .get('/api/cartaporte/downloadDetalle', { params: params, headers: this.headers }).pipe(
                timeoutWith(60000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    public exportPDFCalidad(numero_ccpp: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('numeroCCPP', numero_ccpp);

        return this.http
            .get<CommonResponse>('/api/cartaporte/exportPDFCalidad', { params: params, headers: this.headers });
    }

    public getFotos(cartaPorteId: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorteId', cartaPorteId);

        return this.http
            .get('/api/cartaporte/GetFotos', { params: params, headers: this.headers });
    }

    public getListaFotos(cartaPorteIds: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorteIds', cartaPorteIds);

        return this.http
            .get('/api/cartaporte/GetListaFotos', { params: params, headers: this.headers });
    }

    public descargarFotos(cartaPorteIds: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorteIds', cartaPorteIds);

        return this.http
            .get('/api/cartaporte/DescargarFotos', { params: params, headers: this.headers });
    }

    public detalleCalidades(ccpp: number | string): Observable<ApiResponse<CalidadCCPP[]>> {
        let params: HttpParams = new HttpParams();
        params = params.append('cartaPorte', ccpp.toString());

        return this.http
            .get<ApiResponse<CalidadCCPP[]>>('/api/cartaporte/DetalleCalidades', { params: params, headers: this.headers });

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
            .get('/api/cartaporte/getFormularioDropdowns', { headers: this.headers });
    }

    getDataCTG(valor: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('valor', valor);

        return this.http
            .get('/api/cartaporte/getDataCTG', { params: params, headers: this.headers }).pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, intentelo nuevamente"))));
    }

    getCompletedPDFTemplate(formulario: any, archivo: any, pageSelected: any): Observable<any> {
        var payload = new FormData();
        payload.append("formularioString", JSON.stringify(formulario));
        payload.append("paginaSeleccionada", pageSelected);
        payload.append("file", archivo);

        return this.http
            .post('/api/cartaporte/getCompletedPDFTemplate', payload, { headers: this.headersPost });
    }

    getTemplate(formulario: any): Observable<any> {
        var payload = new FormData();
        payload.append("formularioString", JSON.stringify(formulario));

        return this.http
            .post('/api/cartaporte/getTemplate', payload, { headers: this.headersPost });
    }
}