import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { Formatter } from './../common/formatter/Formatter';
import { BaseService } from './../common/services/BaseService';


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
            .timeoutWith(30000, Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    }

    protected setComprobantePesificacion(contrato: string, fijacion: string, cantidad: number) {
        let payload = new FormData();
        let data = { contrato: contrato, fijacion: fijacion, cantidad: cantidad };
        payload.append("contrato", JSON.stringify(data));
        return this.http
            .post('/api/pesificacion/setComprobante', payload)
            .timeoutWith(30000, Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    }

    protected setComprobantesPesificacion(file: any): Observable<any> {
        let payload = new FormData();
        payload.append("file", file);
        return this.http
            .post('/api/pesificacion/setComprobantes', payload, this.headersPost)
            .timeoutWith(30000, Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    }
}