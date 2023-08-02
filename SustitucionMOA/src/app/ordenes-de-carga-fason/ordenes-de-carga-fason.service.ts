import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { timeoutWith } from 'rxjs/operators';
import { throwError as observableThrowError, Observable } from 'rxjs';
import { OrdenDeCargaFasonDto } from '../common/models/ordenes-de-carga-fason/ordenDeCargaFasonDto';
import { ListarOrdenDeCargaFasonResponse } from '../common/models/ordenes-de-carga-fason/listarOrdenDeCargaFasonResponse';
import { ApiResponse } from '../common/models/response';
import { DestinoFason } from './orden-carga-fason-utils';
import { Material } from '../common/models/material';
import { ValidarIntermediarioFleteResponse } from '../common/models/ordenes-de-carga/ValidarIntermediarioFleteResponse';
import { OrdenesBaseService } from '../common/base-components/ordenes-base-component';
import { Planta } from '../common/models/ordenes-de-carga/planta';
import { Domicilio } from '../common/models/ordenes-de-carga/domicilio';

@Injectable({
  providedIn: 'root'
})

export class OrdenesDeCargaFasonService extends OrdenesBaseService {

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
  public enviarMailGestionarAltaCuit(cuit: string, razonSocial: string, esIntermediarioFlete: boolean): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("cuit", cuit)
      .append("razonSocial", razonSocial)
      .append("esIntermediarioFlete", esIntermediarioFlete.toString());

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/GestionarAltaCuit',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
  }
  public validarIntermediarioFlete(cuit: string): Observable<ApiResponse<ValidarIntermediarioFleteResponse>> {
    let params: HttpParams = new HttpParams()
      .append("cuit", cuit);

    return this.http
      .get<ApiResponse<ValidarIntermediarioFleteResponse>>(
        '/api/OrdenDeCargaFason/ValidarIntermediarioFlete',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public obtenerPlantasDestino(destinoCuit: string): Observable<ApiResponse<Planta[]>> {
    let params: HttpParams = new HttpParams()
      .append("destinoCuit", destinoCuit);

    return this.http
      .get<ApiResponse<Planta[]>>(
        '/api/OrdenDeCargaFason/ObtenerPlantasDestino',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public obtenerDomiciliosDestino(destinoCuit: string): Observable<ApiResponse<Domicilio[]>> {
    let params: HttpParams = new HttpParams()
      .append("destinoCuit", destinoCuit);

    return this.http
      .get<ApiResponse<Domicilio[]>>(
        '/api/OrdenDeCargaFason/ObtenerDomiciliosDestino',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
}
