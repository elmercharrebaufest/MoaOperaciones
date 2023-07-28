import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { timeoutWith } from 'rxjs/operators';
import { BaseService } from '../common/services/BaseService';
import { throwError as observableThrowError, Observable } from 'rxjs';
import { OrdenDeCargaFasonDto } from '../common/models/ordenes-de-carga-fason/ordenDeCargaFasonDto';
import { ListarOrdenDeCargaFasonResponse } from '../common/models/ordenes-de-carga-fason/listarOrdenDeCargaFasonResponse';
import { NumberFormat } from 'xlsx/types';
import { ApiResponse } from '../common/models/response';
import { DestinoFason } from './orden-carga-fason-utils';
import { Material } from '../common/models/material';

@Injectable({
  providedIn: 'root'
})

export class OrdenesDeCargaFasonService extends BaseService {

  public listado(fechaInicio: string, fechaFin: string): Observable<any> {
    let params: HttpParams = new HttpParams()
      .append("fechaInicio", fechaInicio)
      .append("fechaFin", fechaFin);
    return this.http.get<ListarOrdenDeCargaFasonResponse>('/api/OrdenDeCargaFason/Listar', { params: params })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public agregar(ordenDeCargaFason: OrdenDeCargaFasonDto): Observable<any> {
    let payload = new FormData();
    payload.append(
      "ordenDeCargaFasonJson",
      JSON.stringify(ordenDeCargaFason)
    );

    return this.http
      .post('/api/OrdenDeCargaFason/Agregar', payload)
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public editar(ordenDeCargaFason: OrdenDeCargaFasonDto): Observable<any> {
    let payload = new FormData();
    payload.append(
      "ordenDeCargaJson",
      JSON.stringify(ordenDeCargaFason)
    );

    return this.http
      .post('/api/OrdenDeCargaFason/Editar', payload)
      //.post('/api/OrdenDeCargaFason/Editar', payload)
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public getMateriales(): Observable<ApiResponse<Array<Material>>> {
    return this.http.get<ApiResponse<Array<Material>>>('/api/OrdenDeCargaFason/Materiales')
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public getOrdenDeCargaFason(IdOrdenCargaFason: Number): Observable<any> {
    let params: HttpParams = new HttpParams()
      .append('IdOrdenCargaFason', IdOrdenCargaFason.toString());

    return this.http.get<OrdenDeCargaFasonDto>('/api/OrdenDeCargaFason/GetDetalle', { params: params })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public getDestino(clienteId: string): Observable<ApiResponse<Array<DestinoFason>>> {
    let params: HttpParams = new HttpParams()
      .append("clienteId", clienteId.toString())
    return this.http.get<ApiResponse<Array<DestinoFason>>>('/api/OrdenDeCargaFason/ObtenerDestinos', { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public getClientes(codigoCorredor: string): Observable<any> {
    let params: HttpParams = new HttpParams();
    params = params.append("codigoCorredor", codigoCorredor);

    return this.http
      .get('/api/OrdenDeCargaFason/ObtenerClientes', { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public verificarTransporte(IdOrdenCargaFason: Number): Observable<any> {
    let params: HttpParams = new HttpParams()
      .append('IdOrdenCargaFason', IdOrdenCargaFason.toString());

    return this.http.get('/api/OrdenDeCargaFason/VerificarTransporte', { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

  public obtenerCorredores(): Observable<any> {
    return this.http
      .get('/api/OrdenDeCargaFason/ObtenerCorredores')
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }

}
