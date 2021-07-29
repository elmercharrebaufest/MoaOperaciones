import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseService } from './../common/services/BaseService';

@Injectable()
export class GestionCM05Service extends BaseService {

    public listarCabeceras(): Observable<any> {
        return this.http
            .get('/api/GestionImpuestos/ListarCabeceras', { headers: this.headers })
            .pipe(map(this.extractData));
    }

    public listarDetalles(idCabecera): Observable<any> {
        return this.http
            .get('/api/GestionImpuestos/ListarDetalles?idCabecera=' + idCabecera, { headers: this.headers })
            .pipe(map(this.extractData));
    }

    public editarRow(rowdata){
        let detalleJson = JSON.stringify(rowdata);
        var payload = new FormData();

        payload.append('detalleJson', detalleJson);

        return this.http
            .post('/api/GestionImpuestos/Editar', payload, this.headers).map(this.extractData);
    }

    public autorizarCabecera(idCabecera)
    {
        return this.http
            .get('/api/GestionImpuestos/AutorizarCabecera?idCabecera=' + idCabecera, { headers: this.headers })
            .pipe(map(this.extractData));
    }

    public autorizarCabecera2(idCabecera): Observable<any> {
        let params: URLSearchParams = new URLSearchParams();
        params.set('idCabecera', idCabecera.toString());
        return this.http
            .get(`/api/GestionImpuestos/AutorizarCabecera`, { search: params, headers: this.headers })
            .pipe(map(this.extractData));
    }
    //public recordarComentario(consultaId: any): Observable<any> {
    //    let params: URLSearchParams = new URLSearchParams();
    //    params.set('consultaId', consultaId.toString());
    //    return this.http
    //        .get(`/api/Consulta/RecordarComentario`, { search: params, headers: this.headers }).pipe(
    //            map(this.extractData));
    //}
}