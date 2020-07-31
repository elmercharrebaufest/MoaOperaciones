
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';



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
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))) )
            .pipe(map(this.extractData));
    }

    public downloadProcedencia(contrato: string) {
        let params: URLSearchParams = new URLSearchParams();
        params.set('contrato', contrato);
        return this.http
            .get('/api/liquidacion/descargarFleteProcedencia', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))) )
            .pipe(map(this.extractData));
    }

    public goToDataAgro() {
        return this.http
            .get('/api/dataAgro/goToDataAgro')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }
}
