import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


@Injectable()
export class LiquidacionService extends BaseService{

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }  

    getLiquidacionesCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('periodo', periodo);
        params.set('fechaInicio', fecha_inicio);
        params.set('fechaFin', fecha_fin);
        return this.http
            .get('/api/liquidacion/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);

    }

    getVinculacion(contrato: string, secuencia: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contrato', contrato);
        params.set('secuencia', secuencia);
        return this.http
            .get('/api/liquidacion/getVinculacion', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
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
            .get('/api/liquidacion/' + method, { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Por favor, restrinja el rango de fechas")))
            .map(this.extractData);
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('documento', documento);
        params.set('ejercicio', ejercicio);
        return this.http
            .get('/api/PDF/downloadDocumentPDF', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

}


@Injectable()
export class LiquidacionAprobadaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getAprobadas');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAprobadas');
    }
}


@Injectable()
export class LiquidacionObservadaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getObservadas');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadObservadas');
    }

}


@Injectable()
export class LiquidacionPagaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getPagas');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadPagas');
    }

}


@Injectable()
export class LiquidacionNGAprobadaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getAprobadasNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadAprobadasNG');
    }

}


@Injectable()
export class LiquidacionNGObservadaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getObservadasNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadObservadasNG');
    }

}


@Injectable()
export class LiquidacionNGPagaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getPagasNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadPagasNG');
    }

}


@Injectable()
export class LiquidacionProformaService extends LiquidacionService {

    getDataProforma(fijacion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/liquidacion/getProforma', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    exportExcelProforma(fijacion: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fijacion', fijacion);
        return this.http
            .get('/api/liquidacion/descargarProforma', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    getFleteProcedencia(contrato: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contrato', contrato);
        return this.http
            .get('/api/liquidacion/getFleteProcedencia', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

}

