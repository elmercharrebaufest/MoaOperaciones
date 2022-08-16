
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable, EventEmitter, Output } from '@angular/core';
import { HttpHeaders, HttpParams } from '@angular/common/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';



@Injectable({
  providedIn: 'root'
})
export class ReporteContratoService extends BaseService {
   


    public getListado(fechaInicio, fechaFin, mostrarPendientes): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin)
            .append('mostrarPendientes', mostrarPendientes);
            
           
        return this.http
            .get('/api/ReporteContrato/GetContratos', { params: params });
          
    }


    public getTotalFormatter(KilosEntregados: string, KilosTotales: string, KilosPendienteEntrega :string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('KilosEntregados', KilosEntregados)
            .append('KilosTotales', KilosTotales)
            .append('KilosPendienteEntrega', KilosPendienteEntrega);


        return this.http
            .get('/api/ReporteContrato/getTotalFormatter', { params: params })
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerContratosFiltro(fechaInicio, fechaFin, mostrarPendientes, data: string[]) {
            
        var payload = new FormData();
        let dataContrato = JSON.stringify({

            Resultados: data
        });

        payload.append('dataContrato', dataContrato);

        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin)         
            .append('mostrarPendientes', mostrarPendientes);
            

        return this.http
            .post('/api/ReporteContrato/ObtenerContratosFiltro', payload, { params: params });

    }

    public getDetalleContrato2(contrato): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('contrato', contrato);


        return this.http
            .get('/api/ReporteContrato/ObtenerDetalleContrato', { params: params });

    }

    
  }

