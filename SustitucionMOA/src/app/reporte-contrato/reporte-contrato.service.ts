
import { throwError as observableThrowError, Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { ReporteContrato, DetalleReporteContrato, FiltrosReporteContrato } from './ReporteContrato.model';
import { OrdenDeCarga } from '../common/models/ordenes-de-carga/ordenDeCarga';
import { ApiResponse } from '../common/models/response';



@Injectable({
    providedIn: 'root'
})
export class ReporteContratoService extends BaseService {
    private filters = new BehaviorSubject<FiltrosReporteContrato>({} as FiltrosReporteContrato);
    private _contratoSeleccionado = new BehaviorSubject<string>('0');
    
    setContratoSeleccionado(value: string) {
        this._contratoSeleccionado.next(value);
    }
    getContratoSeleccionado() {
        return this._contratoSeleccionado.asObservable()
    }

    get fechaInicio() {
        return this.filters.value.fechaInicio
    }
    get fechaFin() {
        return this.filters.value.fechaFin
    }

    setFechas(fechaInicio: string, fechaFin: string){
        this.filters.next({
            ...this.filters.value,
            fechaFin,
            fechaInicio
        })
    }

    public getListado(fechaInicio, fechaFin, mostrarPendientes): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('fechaInicio', fechaInicio)
            .append('fechaFin', fechaFin)
            .append('mostrarPendientes', mostrarPendientes);


        return this.http
            .get('/api/ReporteContrato/GetContratos', { params: params }).pipe(
                timeoutWith(300000, observableThrowError(new Error("Por favor, restrinja el rango de fechas"))));

    }


    public getTotalFormatter(KilosEntregados: string, KilosTotales: string, KilosPendienteEntrega: string): Observable<any> {
        let params: HttpParams = new HttpParams()
            .append('KilosEntregados', KilosEntregados)
            .append('KilosTotales', KilosTotales)
            .append('KilosPendienteEntrega', KilosPendienteEntrega);


        return this.http
            .get('/api/ReporteContrato/getTotalFormatter', { params: params })
            .pipe(timeoutWith(300000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor intentelo mas tarde"))));
    }

    public obtenerContratosFiltro(fechaInicio, fechaFin, mostrarPendientes, data: ReporteContrato[]) {

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
            .append('contrato', contrato)
            .append('fechaInicio', this.fechaInicio)
            .append('fechaFin', this.fechaFin);


        return this.http
            .get('/api/ReporteContrato/ObtenerDetalleContrato', { params: params });

    }

    public getOrdenDeCarga(det: DetalleReporteContrato): Observable<ApiResponse<OrdenDeCarga>> {
        let params: HttpParams = new HttpParams()
            .append('nroEntrega', det.Entrega);
        return this.http
            .get<ApiResponse<OrdenDeCarga>>('/api/ReporteContrato/ObtenerOrdenDeCarga', { params: params });
    }
}

