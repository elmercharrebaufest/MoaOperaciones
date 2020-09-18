
import {throwError as observableThrowError,  Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';



import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';

@Injectable()
export class PesificacionService extends BaseService {

    public getData() {
        return this.getFechaPesificacion();
    }

    public setData(contrato: string, fijacion: string, cantidad: number) {
        return this.setComprobantePesificacion(contrato, fijacion, cantidad);
    }

    public setMassiveData(file: any){
        return this.setComprobantesPesificacion(file);
    }

    protected getFechaPesificacion() {
        return this.http
            .get('/api/pesificacion/getFechaPesificacion')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    protected setComprobantePesificacion(contrato: string, fijacion: string, cantidad: number) {
        let payload = new FormData();
        let data = { contrato: contrato, fijacion: fijacion, cantidad: cantidad };
        payload.append("contrato", JSON.stringify(data));
        return this.http
            .post('/api/pesificacion/setComprobante', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    protected setComprobantesPesificacion(file: any): Observable<any> {
        let payload = new FormData();
        payload.append("file", file);
        return this.http
            .post('/api/pesificacion/setComprobantes', payload, this.headersPost)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }
}