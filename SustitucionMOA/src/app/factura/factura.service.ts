
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BaseService } from './../common/services/BaseService';

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

    public registrarCertificaciones(certificaciones:any, archivos: Array<File>): Observable<any> {
        let payload = new FormData();
        payload.append("certificaciones", JSON.stringify(certificaciones));
        for (const element of archivos) {
            payload.append("files", element);
        }
        return this.http.post('/api/factura/RegistrarCertificacion', payload, {headers: this.headersPost});
    }

    public verificarSiExisteRegistro(NRO_Certificacion:string): Observable<any>
    {
        let payload = new FormData();
        payload.append("NRO_Certificacion", NRO_Certificacion);
        return this.http.post(`/api/factura/VerificarSiExisteRegistro`,payload ,{headers: this.headersPost});   
    }

    public descargarDocumentoAdjunto(NRO_Certificacion:string): Observable<any>
    {
        let payload = new FormData();
        payload.append("NRO_Certificacion", NRO_Certificacion);
        return this.http.post(`/api/factura/DescargarDocumentoAdjunto`,payload,{headers: this.headersPost});
    }
}