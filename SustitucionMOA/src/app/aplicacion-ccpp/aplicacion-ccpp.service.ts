import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../common/services/BaseService';
import { AplicacionCCPP, AplicacionCCPPFiltro } from './aplicacion-ccpp.model';

export interface ListadoRequest {
  fechaInicio: string;
  fechaFin: string;
}
export interface ApiResponse <T,U>{
  data?:T,
  info?:string;
  error?:string;
  filtros:U;
  logout?:boolean;
}
@Injectable()
export class AplicacionCcppService extends BaseService{
  private baseUrl = "/api/AplicacionCartaPorte/";
  getListado({fechaInicio, fechaFin}: ListadoRequest): Observable<ApiResponse<AplicacionCCPP[],AplicacionCCPPFiltro>>{
    const params =  new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin);
            
    return this.http.get<ApiResponse<AplicacionCCPP[],AplicacionCCPPFiltro>>(`${this.baseUrl}/GetListado`,{params});
  }
}
