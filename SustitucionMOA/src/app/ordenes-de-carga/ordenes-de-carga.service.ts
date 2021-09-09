
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { OrdenDeCarga } from '../common/models/ordenes-de-carga/ordenDeCarga';
import { CorredorContrato } from '../common/models/ordenes-de-carga/corredorContrato';

@Injectable({
    providedIn: 'root'
})
export class OrdenesDeCargaService extends BaseService {
    constructor(protected http: Http) {
        super(http);
    }

    public getOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get('/api/OrdenDeCarga/Get', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public getEditarOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get('/api/OrdenDeCarga/GetEditar', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public getListado(fechaInicio: string, fechaFin: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaInicio', fechaInicio.toString());
        params.set('fechaFin', fechaFin.toString());

        return this.http
            .get('/api/OrdenDeCarga/GetListado', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public agregar(ordenDeCarga: OrdenDeCarga): Observable<any> {
        let payload = new FormData();
        console.log(ordenDeCarga)
        payload.append(
            "ordenDeCargaJson",
            JSON.stringify(ordenDeCarga)
        );
        console.log("payload:", payload)

        return this.http
            .post('/api/OrdenDeCarga/Agregar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public editar(ordenDeCarga: OrdenDeCarga): Observable<any> {
        let payload = new FormData();
        console.log(ordenDeCarga)
        payload.append(
            "ordenDeCargaJson",
            JSON.stringify(ordenDeCarga)
        );
        console.log("payload:", payload)

        return this.http
            .post('/api/OrdenDeCarga/Editar', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public anular(ordenId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/AnularOrden', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public notificarTransporte(ordenId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/NotificarTransporte', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public obtenerContratos(ordenId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerContratos', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    // public obtenerContratosYCorredores(ordenId: Number): Observable<any> {
    //     let params: URLSearchParams = new URLSearchParams();
    //     params.set('ordenId', ordenId.toString());

    //     return this.http
    //         .get('/api/OrdenDeCarga/ObtenerContratosYCorredores', { search: params, headers: this.headers })
    //         .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
    //         .pipe(map(this.extractData));
    // }


    public obtenerCorredores(ordenId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerCorredores', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public seleccionarContrato(ordenId: Number, contrato: string): Observable<any> {
        let payload = new FormData();
        payload.append(
            "contratoSAP",
            contrato
        );
        payload.append(
            "ordenId",
            ordenId.toString()
        );

        return this.http
            .post('/api/OrdenDeCarga/SeleccionarContrato', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public seleccionarCorredor(ordenId: Number, corredor: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());
        params.set('corredor', corredor);

        return this.http
            .get('/api/OrdenDeCarga/SeleccionarCorredor', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public verificarSituacionCrediticia(ordenId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/VerificarSituacionCrediticia', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public verificarTransporte(ordenId: Number): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/VerificarTransporte', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }


    public getMateriales(): Observable<any> {
        return this.http
            .get('/api/OrdenDeCarga/Materiales')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }

    public getListadoFiltradoOrdenCarga(fechaInicio: string, fechaFin: string): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('fechaInicio', fechaInicio.toString());
        params.set('fechaFin', fechaFin.toString());

        return this.http
            .get('/api/OrdenDeCarga/GetListadoFiltradoOrdenCarga', { search: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }


}
