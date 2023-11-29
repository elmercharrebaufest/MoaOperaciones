import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError as observableThrowError } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { AplicacionCCPP, AplicacionCCPPFiltro, AplicacionCCPPForm, ComboAppContratosCCPPResponse, EnviarCargaMasivaResponse, } from './aplicacion-ccpp.model';
import { ApiResponse } from '../common/models/response';
import { timeoutWith } from 'rxjs/operators';

export interface ListadoRequest {
  fechaInicio: string;
  fechaFin: string;
}

@Injectable()
export class AplicacionCcppService extends BaseService{
  private baseUrl = "/api/AplicacionCartaPorte/";
  getListado({ fechaInicio, fechaFin }: ListadoRequest): Observable<ApiResponse<AplicacionCCPP[], AplicacionCCPPFiltro>> {
    const params = new HttpParams()
      .append('fechaInicio', fechaInicio)
      .append('fechaFin', fechaFin);

    return this.http
      .get<ApiResponse<AplicacionCCPP[], AplicacionCCPPFiltro>>(`${this.baseUrl}/GetListado`, { params });
  }
  eliminarAplicacion(aplicacionId: number): Observable<ApiResponse<boolean>> {
    const params = new HttpParams()
      .append('aplicacionId', aplicacionId.toString())

    return this.http.get<ApiResponse<boolean>>(`${this.baseUrl}/EliminarAplicacion`, { params });
  }
  obtenerComboContratosCcpp(): Observable<ApiResponse<ComboAppContratosCCPPResponse>> {
    return this.http
      .get<ApiResponse<ComboAppContratosCCPPResponse>>(`${this.baseUrl}/ObtenerComboContratosCcpp`);
  }

  guardarAplicacion(aplicacion: AplicacionCCPPForm): Observable<ApiResponse<boolean>> {
    let payload = new FormData();
    payload.append(
      "aplicacionCCPPJSON",
      JSON.stringify(aplicacion)
    );
    return this.http
      .post<ApiResponse<boolean>>
      (`${this.baseUrl}/GuardarAplicacion`, payload);
  }

  enviarCargaMasiva(archivo: File): Observable<ApiResponse<EnviarCargaMasivaResponse>> {
    let payload = new FormData();
    payload.append("archivo", archivo);

    return this.http
      .post(`${this.baseUrl}/CargarMasiva`, payload, { headers: this.headersPost })
      .pipe(timeoutWith(360000, observableThrowError(new Error("Se escedi� el tiempo de espera, por favor int�ntelo m�s tarde"))));
    }
}
