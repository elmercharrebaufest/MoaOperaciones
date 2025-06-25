
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './../common/services/BaseService';
import { GrupoCertificaciones } from './factura.model';

@Injectable()
export class FacturaService extends BaseService {

    public subirPDF(
        archivos: Array<File>): Observable<any> {
        let payload = new FormData();
        payload.append("factura", "");
        for (const element of archivos) {
            payload.append("files", element);
        }
        return this.http
            .post('/api/factura/subirPDF', payload, {headers: this.headersPost});
    }

    public registrarCertificaciones(gruposCertificaciones: GrupoCertificaciones[], archivos: Array<File>): Observable<any> {
        let payload = new FormData();
        payload.append("gruposCertificaciones", JSON.stringify(gruposCertificaciones));
        for (const element of archivos) {
            payload.append("files", element);
        }
        return this.http.post('/api/factura/RegistrarCertificacion', payload, {headers: this.headersPost});
    }

    public guardarFacturaDiferenciaTipoDeCambio(archivoFactura: File): Observable<any> {
        let payload = new FormData();
        payload.append("archivoFactura", archivoFactura);
        return this.http.post('/api/factura/GuardarFacturaDiferenciaTasaDeCambio', payload, {headers: this.headersPost});
    }

    public descargarDocumentoAdjunto(archivoId:string): Observable<any>
    {
        let payload = new FormData();
        payload.append("archivoId", archivoId);
        return this.http.post(`/api/factura/DescargarDocumentoAdjunto`,payload,{headers: this.headersPost});
    }
}