import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { AplicacionCCPP, AplicacionCCPPFiltro } from './aplicacion-ccpp.model';
import { ApiResponse } from '../common/models/response';

export interface ListadoRequest {
  fechaInicio: string;
  fechaFin: string;
}
@Injectable()
export class AplicacionCcppService extends BaseService {
  private baseUrl = "/api/AplicacionCartaPorte/";
  getListado({ fechaInicio, fechaFin }: ListadoRequest): Observable<ApiResponse<AplicacionCCPP[], AplicacionCCPPFiltro>> {
    const params = new HttpParams()
      .append('fechaInicio', fechaInicio)
      .append('fechaFin', fechaFin);

    return this.http.get<ApiResponse<AplicacionCCPP[], AplicacionCCPPFiltro>>(`${this.baseUrl}/GetListado`, { params });
  }
  eliminarAplicacion(aplicacionId: number): Observable<ApiResponse<boolean>> {
    const params = new HttpParams()
      .append('aplicacionId', aplicacionId.toString())

    return this.http.get<ApiResponse<boolean>>(`${this.baseUrl}/EliminarAplicacion`, { params });
  }
}
