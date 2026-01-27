
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { ConsultaTicketPesada, ConsultaTicketPesadaSubproductos } from '../common/models/ticket-pesada/consulta-ticket-pesada';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable()
export class TicketPesadaService extends BaseService {

    constructor(protected http: HttpClient) {
        super(http);
    }

    public ObtenerTicketPesada(ticketPesada: ConsultaTicketPesada): Observable<any> {
        let payload = new FormData();
        payload.append(
            "ticketPesadaJson",
            JSON.stringify(ticketPesada)
        );

        return this.http
            .post('/api/TicketPesada/Obtener', payload)
            .pipe(timeoutWith(60000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public ObtenerTicketPesadaSubproductos(ticketPesadaSubProductos: ConsultaTicketPesadaSubproductos): Observable<any> {
        let payload = new FormData();
        payload.append(
            "ticketPesadaNoGranosJson",
            JSON.stringify(ticketPesadaSubProductos)
        );

        return this.http
            .post('/api/TicketPesada/ObtenerNoGranos', payload)
            .pipe(timeoutWith(60000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

    public ListarDatosTicketPesada(fechaInicio: Date, fechaEgreso: Date, cuitProveedor: string, cuitTransportista: string,
        ctg: string, patente: string, cuitIntermediarioFlete: string, esAdmin: boolean): Observable<any> {
        const params = new HttpParams()
  .set('fechaInicio', fechaInicio.toISOString())
  .set('fechaEgreso', fechaEgreso.toISOString())
  .set('cuitProveedor', cuitProveedor)
  .set('cuitTransportista', cuitTransportista)
  .set('ctg', ctg)
  .set('patente', patente)
  .set('cuitIntermediarioFlete', cuitIntermediarioFlete)
  .set('esAdmin', esAdmin.toString());

        return this.http
            .get<any[]>('/api/TicketPesada/ListarDatosTicketPesada', { params: params, headers: this.headers })
            .pipe(timeoutWith(60000, observableThrowError(new Error("Se excedió el tiempo de espera, por favor inténtelo más tarde "))));
    }

   
}
