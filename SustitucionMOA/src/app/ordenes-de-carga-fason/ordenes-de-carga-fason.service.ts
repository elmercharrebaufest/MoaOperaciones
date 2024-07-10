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
import { ValidarCuitExisteScatoResponse } from '../common/models/ordenes-de-carga-common/ValidarCuitExisteScatoResponse';
import { Proveedor } from '../common/models/proveedor';
import { ValidarCamionResponse } from '../common/models/ordenes-de-carga/ValidarCamionResponse';

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

  public verificarTransporte(IdOrdenCargaFason: Number): Observable<ApiResponse<OrdenDeCargaFasonDto>> {
    let params: HttpParams = new HttpParams()
      .append('IdOrdenCargaFason', IdOrdenCargaFason.toString());

    return this.http
      .get<ApiResponse<OrdenDeCargaFasonDto>>('/api/OrdenDeCargaFason/VerificarTransporte', { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public obtenerCorredores(): Observable<any> {
    return this.http
      .get('/api/OrdenDeCargaFason/ObtenerCorredores')
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
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
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
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

  public validarExisteCuitScato(cuit: string): Observable<ApiResponse<ValidarCuitExisteScatoResponse>> {
    let params: HttpParams = new HttpParams();
    params = params.append("cuit", cuit);

    return this.http
      .get
      <ApiResponse<ValidarCuitExisteScatoResponse>>
      ('/api/OrdenDeCargaFason/ValidarCuitExisteScato', { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public validarSisaCuit(cuitDestinatario: string, cuitDestino: string, codigoMaterial: string): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("cuitDestinatario", cuitDestinatario)
      .append("cuitDestino", cuitDestino)
      .append("codigoMaterial", codigoMaterial)

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/ValidarSisaCuit',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public validarCuitRuca(cuit: string): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("cuit", cuit);

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/ValidarCuitRuca',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public solicitarAnulacion(ordenId: number) {
    return this.http
      .post<ApiResponse<OrdenDeCargaFasonDto>>(
        '/api/OrdenDeCargaFason/SolicitarAnulacion', { ordenId }
      )
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public actualizarSolicitudAnulacion(ordenId: number, aprobado: boolean) {
    return this.http
      .post<ApiResponse<OrdenDeCargaFasonDto>>(
        '/api/OrdenDeCargaFason/ActualizarSolicitudAnulacion', { ordenId, aprobado }
      )
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public actualizarSolicitudEdicion(ordenId: number, aprobado: boolean) {
    return this.http
      .post<ApiResponse<OrdenDeCargaFasonDto>>(
        '/api/OrdenDeCargaFason/ActualizarSolicitudEdicion', { ordenId, aprobado }
      )
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public anularOrden(ordenId: number) {
    return this.http
      .post<ApiResponse<OrdenDeCargaFasonDto>>(
        '/api/OrdenDeCargaFason/AnularOrden', { ordenId }
      )
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public validarCuilChofer(cuil: string): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("cuilChofer", cuil);

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/ValidarCuilChofer',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public validarCuitTransporte(cuil: string): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("cuitTransporte", cuil);

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/ValidarCuitTransporte',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public obtenerProveedor(idProveedor: Number): Observable<ApiResponse<Proveedor>> {
    let params: HttpParams = new HttpParams()
      .append("idProveedor", idProveedor.toString());

    return this.http
      .get<ApiResponse<Proveedor>>('/api/OrdenDeCargaFason/ObtenerProveedor', { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public getCuilsChofer(ordenDeCargaFason: OrdenDeCargaFasonDto): Observable<any> {
    let payload = new FormData();
    payload.append(
      "ordenDeCargaFasonJson",
      JSON.stringify(ordenDeCargaFason)
    );
    return this.http
      .post('/api/OrdenDeCargaFason/ObtenerCuilsChofer', payload);
  }

  public getCuitsTransporte(ordenDeCargaFason: OrdenDeCargaFasonDto): Observable<any> {
    let payload = new FormData();
    payload.append(
      "ordenDeCargaFasonJson",
      JSON.stringify(ordenDeCargaFason)
    );
    return this.http
      .post('/api/OrdenDeCargaFason/ObtenerCuitsTransporte', payload);
  }
  public verificarCNRT(chasis: string, acoplado: string): Observable<ApiResponse<ValidarCamionResponse>> {
    let params: HttpParams = new HttpParams()
      .append("patenteChasis", chasis)
      .append("patenteAcoplado", acoplado);

    return this.http
      .get<ApiResponse<ValidarCamionResponse>>(
        '/api/OrdenDeCargaFason/ValidarCamion',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
  public getPatentes(ordenDeCargaFason: OrdenDeCargaFasonDto): Observable<any> {
    let payload = new FormData();
    // console.log(ordenDeCarga)
    payload.append(
      "ordenDeCargaFasonJson",
      JSON.stringify(ordenDeCargaFason)
    );
    return this.http
      .post('/api/OrdenDeCargaFason/ObtenerPatentes', payload);
  }
  public EnviarMailAltaCuitTerceros(gestionaFlete: boolean, gestionaDestino: boolean, gestionaDestinatario: boolean,
    ordenId: string): Observable<ApiResponse<boolean>> {
    const payload = {
      gestionaFlete,
      gestionaDestino,
      gestionaDestinatario,
      ordenId
    }
    return this.http
      .post<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/EnviarMailAltaCuitTerceros',
        payload)
  }
  public verificarCuitsTerceros(ordenId: number): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("ordenId", ordenId.toString());

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/VerificarCuitsTerceros',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }

  public validarOrdenActivaScato(ordenId: number): Observable<ApiResponse<boolean>> {
    let params: HttpParams = new HttpParams()
      .append("ordenId", ordenId.toString());

    return this.http
      .get<ApiResponse<boolean>>(
        '/api/OrdenDeCargaFason/ValidarOrdenActivaScato',
        { params: params, headers: this.headers })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde"))));
  }
}
