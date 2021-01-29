
import { throwError as observableThrowError, Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, Headers } from '@angular/http';
import { BaseService } from './../common/services/BaseService';
import { timeoutWith, map } from 'rxjs/operators';
import { ConsultaTicketPesda } from '../common/models/ticket-pesada/consulta-ticket-pesada';

@Injectable()
export class TicketPesadaesService extends BaseService {

    constructor(protected http: Http) {
        super(http);
    }

    public buscarTicket(ticketPesada: ConsultaTicketPesda): Observable<any> {
        let payload = new FormData();
        payload.append(
            "ticketPesadaJson",
            JSON.stringify(ticketPesada)
        );

        return this.http
            .post('/api/TicketPesada/Obtener', payload)
            .pipe(timeoutWith(30000, observableThrowError(new Error("Se exedio el tiempo de espera, por favor intentelo mas tarde"))))
            .pipe(map(this.extractData));
    }
}
