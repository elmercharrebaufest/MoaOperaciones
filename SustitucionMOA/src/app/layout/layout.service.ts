
import { BehaviorSubject, throwError as observableThrowError } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CommonResponse } from '../common/models/common-response';

@Injectable()
export class LayoutService extends BaseService {

    private sidebarVisible = new BehaviorSubject(true);
    get $sidebarVisible() {
        return this.sidebarVisible.asObservable();
    }
    public toggleSidebar(value: boolean | null = null) {
        if (value === null) {
            this.sidebarVisible.next(!this.sidebarVisible.value);
        } else {
            this.sidebarVisible.next(value)
        }
    }
    constructor(protected http: HttpClient) {
        super(http);
    }

    public downloadVinculacion(contrato: string, secuencia: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('contrato', contrato);
        params = params.append('secuencia', secuencia);

        return this.http
            .get('/api/liquidacion/downloadVinculacion', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    public seccionVisitada(seccion: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('seccion', seccion);

        return this.http
            .get<CommonResponse>('/api/usuario/SeccionVisitada', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))))
    }

    public downloadProcedencia(contrato: string) {
        let params: HttpParams = new HttpParams();
        params = params.append('contrato', contrato);

        return this.http
            .get('/api/liquidacion/descargarFleteProcedencia', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Tiempo de respuesta agotado, por favor intentar nuevamente"))));
    }

    public goToDataAgro() {
        return this.http
            .get('/api/dataAgro/goToDataAgro')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }
}
