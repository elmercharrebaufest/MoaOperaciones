
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';



@Injectable({
  providedIn: 'root'
})
export class ReporteContratoService extends BaseService {

    public getListado(fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin);
           
        return this.http
            .get('/api/ReporteContrato/GetContratos', { params: params });
          
    }


    public getTotalFormatter(KilosEntregados: string, KilosTotales: string, KilosPendienteEntrega :string): Observable<any> {
        debugger;
        let params: HttpParams = new HttpParams()
            .append('KilosEntregados', KilosEntregados)
            .append('KilosTotales', KilosTotales)
            .append('KilosPendienteEntrega', KilosPendienteEntrega);


        return this.http
            .get('/api/ReporteContrato/getTotalFormatter', { params: params })
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

  }

