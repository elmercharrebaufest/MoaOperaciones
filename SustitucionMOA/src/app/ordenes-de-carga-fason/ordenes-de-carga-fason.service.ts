import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { timeoutWith } from 'rxjs/operators';
import { BaseService } from '../common/services/BaseService';
import { throwError as observableThrowError, Observable } from 'rxjs';
import { OrdenDeCargaFason } from '../common/models/ordenes-de-carga-fason/ordendecargafason';
import { ListarOrdenDeCargaFasonResponse } from '../common/models/ordenes-de-carga-fason/listarOrdenDeCargaFasonResponse';

@Injectable({
  providedIn: 'root'
})

export class OrdenesDeCargaFasonService extends BaseService {

  public listado(fechaInicio: string, fechaFin: string): Observable<any> {
    let params: HttpParams = new HttpParams()
        .append("fechaInicio", fechaInicio)
        .append("fechaFin", fechaFin);
        console.log({fechaFin, fechaInicio})
    return this.http.get<ListarOrdenDeCargaFasonResponse>('/api/OrdenDeCargaFason/Listar', { params: params })
      .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public getMateriales(): Observable<any> {
    return this.http.get('/api/OrdenDeCarga/Materiales')
      .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public getOrdenDeCargaFason(IdOrdenCargaFason: Number): Observable<any> {
    let params: HttpParams = new HttpParams()
      .append('IdOrdenCargaFason', IdOrdenCargaFason.toString());

    return this.http.get<OrdenDeCargaFason>('/api/OrdenDeCargaFason/GetDetalle', { params: params })
      .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public verificarTransporte(IdOrdenCargaFason: Number): Observable<any> {
    let params: HttpParams = new HttpParams().append('IdOrdenCargaFason', IdOrdenCargaFason.toString());

    return this.http.get('/api/OrdenDeCargaFason/VerificarTransporte', { params: params, headers: this.headers })
      .pipe(timeoutWith(30000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }
}
