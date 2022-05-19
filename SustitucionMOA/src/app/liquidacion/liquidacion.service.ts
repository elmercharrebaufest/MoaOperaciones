
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class LiquidacionService extends BaseService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return null;
    }

    getLiquidacionesCommon(periodo: string, fecha_inicio: string, fecha_fin: string, method: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);

        return this.http
            .get('/api/liquidacion/' + method, { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));
    }

    getVinculacion(contrato: string, secuencia: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('contrato', contrato);
        params = params.append('secuencia', secuencia);

        return this.http
            .get('/api/liquidacion/getVinculacion', { params: params, headers: this.headers })
            .pipe(
                timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            );
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
            .get('/api/liquidacion/' + method, { params: params, headers: this.headers })
            .pipe(
                timeoutWith(30000, observableThrowError(new Error("Por favor, restrinja el rango de fechas")))
            );
    }

    descargarDocumentoPDF(documento: string, ejercicio: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('documento', documento);
        params = params.append('ejercicio', ejercicio);

        return this.http
            .get('/api/PDF/downloadDocumentPDF', { params: params, headers: this.headers })
            .pipe(
                timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            );
    }

    descargarComprobanteNGPDF(CodigoProveedorSAP: string, FechaDocumento: string, NumeroLegalDocumento: string): Observable<any> {
        
        var dateString = FechaDocumento.substr(6);
        var currentTime = new Date(parseInt(dateString ));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var date = year + "-" + month + "-" + day;
        

        
        let params: HttpParams = new HttpParams();
        params = params.append('CodigoProveedorSAP', CodigoProveedorSAP);
        params = params.append('FechaDocumento', date);
        params = params.append('NumeroLegalDocumento', NumeroLegalDocumento);

        

        return this.http
            .get('/api/liquidacion/descargaComprobantesNG', { params: params, headers: this.headers }).pipe(
            timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
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
export class LiquidacionNGPagaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getPagasNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadPagasNG');
    }
}

@Injectable()
export class LiquidacionNGRegistradoService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getRegistradosNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadRegistradosNG');
    }
}

@Injectable()
export class LiquidacionNGPendienteRegistroService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getPendienteRegistroNG');
    }

    exportExcel(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.exportExcelCommon(periodo, fecha_inicio, fecha_fin, 'downloadPendienteRegistroNG');
    }
}

@Injectable()
export class LiquidacionProformaService extends LiquidacionService {

    getDataProforma(fijacion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('fijacion', fijacion);

        return this.http
            .get('/api/liquidacion/getProforma', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    exportExcelProforma(fijacion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('fijacion', fijacion);

        return this.http
            .get('/api/liquidacion/descargarProforma', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    descargarProformaFinal(fijacion: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('fijacion', fijacion);

        return this.http
            .get('/api/liquidacion/descargarProformaFinal', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    getFleteProcedencia(contrato: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('contrato', contrato);

        return this.http
            .get('/api/liquidacion/getFleteProcedencia', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }
}

@Injectable()
export class LiquidacionInformarService extends LiquidacionService{
    notificarLiquidaciones(
        archivos: File[]): Observable<any> {
        var payload = new FormData();

        for (let i = 0; i < archivos.length; i++) {
            let fileToUpload = archivos[i];
            payload.append("file", fileToUpload, fileToUpload.name);
        }

        return this.http
            .post('/api/liquidacion/notificar', payload, { headers: this.headersPost });
    }   
}

@Injectable()
export class LiquidacionInformadaService extends LiquidacionService {

    getData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.getLiquidacionesCommon(periodo, fecha_inicio, fecha_fin, 'getInformadas');
    }

}