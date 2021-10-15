
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { OrdenDeCarga } from '../common/models/ordenes-de-carga/ordenDeCarga';
import { CorredorContrato } from '../common/models/ordenes-de-carga/corredorContrato';

@Injectable({
    providedIn: 'root'
})
export class OrdenesDeCargaService extends BaseService {

    public getOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get<OrdenDeCarga>('/api/OrdenDeCarga/Get', { params: params })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public getEditarOrdenDeCarga(ordenDeCargaId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenDeCargaId', ordenDeCargaId.toString());

        return this.http
            .get<OrdenDeCarga>('/api/OrdenDeCarga/GetEditar', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public getListado(fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin);

        return this.http
            .get<OrdenDeCarga[]>('/api/OrdenDeCarga/GetListado', { params: params })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
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
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
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
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public anular(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/AnularOrden', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public notificarTransporte(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/NotificarTransporte', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerContratos(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerContratos', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    // public obtenerContratosYCorredores(ordenId: Number): Observable<any> {
    //     let params: HttpParams = new HttpParams()
            //.append('ordenId', ordenId.toString());

    //     return this.http
    //         .get('/api/OrdenDeCarga/ObtenerContratosYCorredores', { params: params, headers: this.headers })
    //         .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))))
    //         ;
    // }


    public obtenerCorredores(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/ObtenerCorredores', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
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
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public seleccionarCorredor(ordenId: Number, corredor: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString())
            .append('corredor', corredor);

        return this.http
            .get('/api/OrdenDeCarga/SeleccionarCorredor', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public verificarSituacionCrediticia(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/VerificarSituacionCrediticia', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public verificarTransporte(ordenId: Number): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('ordenId', ordenId.toString());

        return this.http
            .get('/api/OrdenDeCarga/VerificarTransporte', { params: params, headers: this.headers })
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }


    public getMateriales(): Observable<any> {
        return this.http
            .get('/api/OrdenDeCarga/Materiales')
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

}
