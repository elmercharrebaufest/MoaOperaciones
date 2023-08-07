import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { HttpParams } from '@angular/common/http';
import {timeoutWith, map} from 'rxjs/operators';
import { EcheqContrato } from './gestion/echeq-contrato.model';
import { Configuracion } from '../common/models/configuracion';
import { EcheqApertura } from './gestion/echeq.popup/echeqApertura-model';


@Injectable()
export class EcheqService extends BaseService {

    protected GetContratoPendientePago(periodo: string, fecha_inicio: string, fecha_fin: string, method: string) : Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fecha_inicio);
        params = params.append('fechaFin', fecha_fin);
        return this.http
            .get(`/api/echeq/${method}`, { params: params, headers: this.headers });
    }

    public GetData(periodo: string, fecha_inicio: string, fecha_fin: string): Observable<any> {
        return this.GetContratoPendientePago(periodo, fecha_inicio, fecha_fin, 'ObtenerPendientePago');
    }

    public MarcarContrato(contrato: string, pedido: string): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", contrato);
        payload.append("pedido", pedido);

        return this.http
            .post('/api/echeq/MarcarContrato', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public MarcarDocumento(documento: string, pedido: string, contrato: string, ejercicio: string): Observable<any> {
        let payload = new FormData();
        payload.append("documento", documento);
        payload.append("pedido", pedido);
        payload.append("contrato", contrato);
        payload.append("ejercicio", ejercicio);


        return this.http
            .post('/api/echeq/MarcarDocumento', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public DesmarcarContrato(contrato: string, pedido: string): Observable<any> {
        let payload = new FormData();
        payload.append("contrato", contrato);
        payload.append("pedido", pedido);

        return this.http
            .post('/api/echeq/DesmarcarContrato', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public DesmarcarDocumento(documento: string, pedido: string, contrato: string): Observable<any> {
        let payload = new FormData();
        payload.append("documento", documento);
        payload.append("pedido", pedido);
        payload.append("contrato", contrato);

        return this.http
            .post('/api/echeq/DesmarcarDocumento', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public ConfiguracionEcheq(): Observable<Configuracion[]> {
        return this.http
            .get<Configuracion[]>('/api/echeq/ObtenerConfiguracion', { headers: this.headers });
    }

    
    public AgregarApertura(documento: string, pedido: string, contrato: string, listaChequesApertura: EcheqApertura[]): Observable<any> {
        let payload = new FormData();
        payload.append("documento", documento);
        payload.append("pedido", pedido);
        payload.append("contrato", contrato);
        payload.append("aperturaDtos", JSON.stringify(listaChequesApertura));


        return this.http
            .post('/api/echeq/AgregarApertura', payload)
            .pipe(timeoutWith(90000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public GetDatosReporte(periodo: string, fechaInicio: string, fechaFin: string): Observable<any> {
        let params: HttpParams = new HttpParams();
        params = params.append('periodo', periodo);
        params = params.append('fechaInicio', fechaInicio);
        params = params.append('fechaFin', fechaFin);
        return this.http
            .get('/api/echeq/ObtenerDatosReporte', { params: params, headers: this.headers });
    }

}

