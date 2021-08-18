import { HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from './../common/services/BaseService';

@Injectable()
export class GestionCM05Service extends BaseService {

    public listarCabeceras(): Observable<any> {
        return this.http
            .get('/api/GestionImpuestos/ListarCabeceras', { headers: this.headers });
    }

    public listarDetalles(idCabecera): Observable<any> {
        return this.http
            .get('/api/GestionImpuestos/ListarDetalles?idCabecera=' + idCabecera, { headers: this.headers });
    }

    public editarRow(rowdata){
        let detalleJson = JSON.stringify(rowdata);
        var payload = new FormData();

        payload.append('detalleJson', detalleJson);

        return this.http
            .post('/api/GestionImpuestos/EditarIngresosBrutosCoeficienteUnificadoDetalle', payload, {headers: this.headers});
    }

    public autorizarCabecera(idCabecera): Observable<any> {
        return this.http
            .get('/api/GestionImpuestos/AutorizarCabecera?idCabecera=' + idCabecera, { headers: this.headers });
    }

    public DescargarArchivoFormularioCM05(idCabecera: number): Observable<any> {
        let headers = new HttpHeaders();
        headers = headers.append("Content-Type", "application/json");
        headers = headers.append("Accept", "q=0.8;application/json;q=0.9");
        headers = headers.append("Cache-control", "no-cache");
        headers = headers.append("Cache-control", "no-store");
        headers = headers.append("Expires", "0");
        headers = headers.append("Pragma", "no-cache");
        let params: HttpParams = new HttpParams();
        params.set("idCabecera", idCabecera.toString());
        return this.http
            .get('/api/GestionImpuestos/DescargarFormularioCM05?idCabecera=' + idCabecera, { headers: this.headers });
    }

}