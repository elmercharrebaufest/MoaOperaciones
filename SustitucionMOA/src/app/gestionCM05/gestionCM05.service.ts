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
            .post('/api/GestionImpuestos/EditarIngresosBrutosCoeficienteUnificadoDetalle', payload, this.headers).map(this.extractData);
    }

    public autorizarCabecera(idCabecera): Observable<any> {
        var payload = new FormData();
        payload.append('idCabecera', idCabecera.toString());

        return this.http
            .post('/api/GestionImpuestos/AutorizarCabecera', payload, this.headers)
            .map(this.extractData);
    }

    public DescargarArchivoFormularioCM05(idCabecera: number): Observable<any> {
        this.headers = new Headers();
        this.headers.append("Content-Type", "application/json");
        this.headers.append("Accept", "q=0.8;application/json;q=0.9");
        this.headers.append("Cache-control", "no-cache");
        this.headers.append("Cache-control", "no-store");
        this.headers.append("Expires", "0");
        this.headers.append("Pragma", "no-cache");
        let params: URLSearchParams = new URLSearchParams();
        params.set("idCabecera", idCabecera.toString());
        return this.http
            .get('/api/GestionImpuestos/DescargarFormularioCM05?idCabecera=' + idCabecera, { headers: this.headers })
            .pipe(map(this.extractData));
    }

    public editarCabecera(cabecera) {
        let cabeceraJson = JSON.stringify(cabecera);
        var payload = new FormData();

        payload.append('cabeceraJson', cabeceraJson);

        return this.http
            .post('/api/GestionImpuestos/EditarIngresosBrutosCoeficienteUnificado', payload, this.headers).map(this.extractData);
    }
}