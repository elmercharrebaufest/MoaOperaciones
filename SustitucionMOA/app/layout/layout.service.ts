import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';


@Injectable()
export class LayoutService extends BaseService {

    constructor(protected http: Http) {
        super(http);
    }

    public downloadVinculacion(contrato: string, secuencia: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contrato', contrato);
        params.set('secuencia', secuencia);
        return this.http
            .get('/api/liquidacion/downloadVinculacion', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    public downloadProcedencia(contrato: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contrato', contrato);
        return this.http
            .get('/api/liquidacion/descargarFleteProcedencia', { search: params, headers: this.headers })
            .timeoutWith(30000, Observable.throw(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente")))
            .map(this.extractData);
    }

    public goToDataAgro() {
        return this.http
            .get('/api/dataAgro/goToDataAgro')
            .timeoutWith(30000, Observable.throw(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde")))
            .map(this.extractData);
    }
}
